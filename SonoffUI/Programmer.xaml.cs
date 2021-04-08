using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Diagnostics;

namespace SonoffUI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Programmer : System.Windows.Controls.UserControl
    {
        public ConfigManager configManager = new ConfigManager(Constants.CONFIG_FILE);

        bool isInitialized = false;

        public Programmer()
        {
            configManager.readConfig();

            InitializeComponent();

            isInitialized = true;
        }

        public void onUpdateValue(object sender, EventArgs args)
        {
            if (isInitialized)
            {
                this.configManager.updateConfig(Constants.OLD_FW_CHECKBOX_XML_NAME, this.OldFirmwareCheckBox.IsChecked.ToString());

                configManager.updateConfig(Constants.EXEC_FILE_XML_NAME, this.ExecutableTextBox.Text);
                configManager.updateConfig(Constants.FW_FILE_XML_NAME, this.FirmwareTextBox.Text);
                configManager.updateConfig(Constants.OLD_FW_FILE_XML_NAME, this.OldFirmwareTextBox.Text);
            }
        }
        // auto selects first com port seen if there is one or more
        public void autoSelect()
        {
            this.ComPortComboBox.Items.Clear();
            foreach (string s in SerialPort.GetPortNames())
            {
                this.ComPortComboBox.Items.Add(s);
            }
            if (!this.ComPortComboBox.Items.IsEmpty)
            {
                this.ComPortComboBox.SelectedIndex = 0;
            }
        }

        // gets list of com ports detected and adds them to combo box
        private void onComPortSelect(object sender, EventArgs e)
        {
            this.ComPortComboBox.Items.Clear();
            foreach (string s in SerialPort.GetPortNames())
            {
                this.ComPortComboBox.Items.Add(s);
            }
        }

        private void onInitialized(object sender, EventArgs e)
        {
            autoSelect();

            this.ExecutableTextBox.Text = configManager.getInitialValues(Constants.EXEC_FILE_XML_NAME, Constants.DEFAULT_EXEC_FILEPATH);
            this.FirmwareTextBox.Text = configManager.getInitialValues(Constants.FW_FILE_XML_NAME, Constants.DEFAULT_FIRMWARE_FILEPATH);
            this.OldFirmwareTextBox.Text = configManager.getInitialValues(Constants.OLD_FW_FILE_XML_NAME, "");
            this.OldFirmwareCheckBox.IsChecked = Boolean.Parse(configManager.getInitialValues(Constants.OLD_FW_CHECKBOX_XML_NAME, "False"));
        }

        private void onBrowseExecutable(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Binary Files|*.bin";
            if (openFileDialog.ShowDialog() == true)
                this.ExecutableTextBox.Text = openFileDialog.FileName;
        }

        private void onBrowseFirmware(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Binary Files|*.bin";
            if (openFileDialog.ShowDialog() == true)
                this.FirmwareTextBox.Text = openFileDialog.FileName;
        }

        private void onBrowseOldFirmware(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog diag = new FolderBrowserDialog();
            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.OldFirmwareTextBox.Text = diag.SelectedPath;
            }
        }

        private void onProgram(object sender, RoutedEventArgs e)
        {
            Process process = new Process();

            string exec = "python3";

            bool shouldSave = (bool)this.OldFirmwareCheckBox.IsChecked;
            string oldBinFile = this.OldFirmwareTextBox.Text;

            string execFile = this.ExecutableTextBox.Text;
            string binFile = this.FirmwareTextBox.Text;

            string port;
            try
            {
                port = this.ComPortComboBox.SelectedItem.ToString();
            }
            catch
            {
                Console.WriteLine("SELECT COM PORT");
            }

            port = "COM5";

            process.StartInfo.FileName = exec;
            process.StartInfo.Arguments = 
                (shouldSave ? string.Join(" ", new string[] {execFile, "--port", port, "read_flash 0x00000 0x100000", System.IO.Path.Combine(oldBinFile, DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss")) + ".bin", "&& " }) : "" ) +
                string.Join(" ", new string[] { exec, execFile, "--port", port, "erase_flash", "&& " }) + 
                string.Join(" ", new string[] { exec, execFile, "--port", port, "write_flash -fs 1MB -fm dout 0x0", binFile });

            Console.WriteLine(process.StartInfo.Arguments);

            process.Start();

            process.WaitForExit(30000);
        }
    }
}
