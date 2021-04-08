using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoffUI
{
    class Constants
    {
        // needs to be a file that lists configs
        public const string CONFIG_FILE = @"../../Config.xml";

        // defaults
        // based on RobotDev computer. feel free to change them.
        public const string DEFAULT_EXEC_FILEPATH = @"C:\Users\RobotDev\Downloads\esptool\esptool.py";
        public const string DEFAULT_FIRMWARE_FILEPATH = @"C:\Users\RobotDev\Downloads\tasmota.bin";


        // base XML file nodes
        public const string ROOT_XML_NAME = "SonoffConfig";


        // when writing into XML file
        public const string EXEC_FILE_XML_NAME = "ExecPath";
        public const string FW_FILE_XML_NAME = "FwPath";
        public const string OLD_FW_FILE_XML_NAME = "OldFwPath";
        public const string OLD_FW_CHECKBOX_XML_NAME = "OldFWCB";

    }
}
