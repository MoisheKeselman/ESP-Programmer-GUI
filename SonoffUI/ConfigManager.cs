using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SonoffUI
{
    public class ConfigManager
    {

        public static XElement configEl = new XElement("default");

        public string configFilepath;
        public XElement configElInst;

        public ConfigManager(string filepath)
        {
            this.configFilepath = filepath;
        }

        public bool readConfig()
        {
            /**
             * returns true if config exists, false if it does not
            */

            // if there is no config file, make blank one. needs to be done before initializing because other components need the file.
            if (!File.Exists(this.configFilepath))
            {
                this.configElInst = new XElement(Constants.ROOT_XML_NAME);
                return false;
            }
            else
            {
                try
                {
                    this.configElInst = XElement.Load(this.configFilepath);
                    return true;
                }
                catch
                {
                    // malformed xml
                    Console.WriteLine("ERROR: MALFORMED XML");

                    this.configElInst = new XElement(Constants.ROOT_XML_NAME);

                    return false;
                }
            }
        }

        public string getInitialValues(string xmlName, string defaultValue)
        {
            /**
             * returns the value that should be used
            */

            if (this.configElInst.Element(xmlName) != null)
            {
                return this.configElInst.Element(xmlName).Value;
            }
            else
            {
                this.configElInst.Add(new XElement(xmlName));
                this.configElInst.Element(xmlName).SetValue(defaultValue);

                return defaultValue;
            }
        }

        public void updateConfig(string xmlName, XElement newEl)
        {
            try
            {
                this.configElInst.Element(xmlName).ReplaceWith(newEl);
            }
            catch (NullReferenceException e)
            {
                this.configElInst.Add(newEl);
            }

            saveConfigToFile();
        }

        public void updateConfig(string xmlName, string newVal)
        {
            try
            {
                this.configElInst.Element(xmlName).Value = newVal;
            }
            catch (NullReferenceException e)
            {
                XElement newEl = new XElement(xmlName);
                newEl.Value = newVal;
                this.configElInst.Add(newEl);
            }

            saveConfigToFile();
        }

        public void saveConfigToFile()
        {
            try
            {
                this.configElInst.Save(this.configFilepath);
            }
            catch (Exception e)
            {
                Console.WriteLine(configFilepath);
                Console.WriteLine(e);
            }
        }
    }
}
