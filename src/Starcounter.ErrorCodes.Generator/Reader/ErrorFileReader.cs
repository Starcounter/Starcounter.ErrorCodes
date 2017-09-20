using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Starcounter.ErrorCodes.Generator {
    public static class ErrorFileReader {
        private static readonly Regex MultipleWhitespace = new Regex(@"\s+");

        public static ErrorFile ReadErrorCodes(string errorCodeFilePath) {
            XmlDocument document;
            XmlReaderSettings settings;
            List<ErrorCode> allCodes;

            settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = false;
            settings.DtdProcessing = DtdProcessing.Ignore;

            using (var xmlReader = XmlReader.Create(new FileStream(errorCodeFilePath, FileMode.Open, FileAccess.Read), settings)) {
                document = new XmlDocument();
                document.Load(xmlReader);
                document.Normalize();
                
                allCodes = new List<ErrorCode>();
                foreach (XmlNode fnode in document.GetElementsByTagName("facility")) {
                    Facility facility = NodeToFacility(fnode);
                    foreach (XmlNode cnode in fnode.ChildNodes) {
                        if (!(cnode is XmlElement)) {
                            continue;
                        }
                        allCodes.Add(NodeToErrorCode(cnode, facility));
                    }
                }
            }
            return new ErrorFile(errorCodeFilePath, allCodes);
        }

        private static ErrorCode NodeToErrorCode(XmlNode cnode, Facility facility) {
            XmlElement e;
            string name;
            ushort code;
            List<string> remparams;
            XmlNode msgnode;
            XmlNode remnode;

            e = (XmlElement)cnode;
            name = e.GetAttribute("name");
            
            if (!ushort.TryParse(e.GetAttribute("hex"), NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out code)) {
                throw new FormatException();
            }

            remparams = new List<string>();
            msgnode = null;
            remnode = null;

            foreach (XmlNode childNode in cnode.ChildNodes) {
                if (childNode.Name == "message") {
                    msgnode = childNode;
                } else if (childNode.Name == "remarks") {
                    remnode = childNode;
                }
            }

            if (remnode != null) {
                foreach (XmlNode pnode in remnode.ChildNodes) {
                    remparams.Add(TrimSpacesAndLineBreaks(pnode.InnerText));
                }
            }

            return new ErrorCode(
                facility,
                name,
                code,
                (Severity)Enum.Parse(typeof(Severity),
                e.GetAttribute("severity")),
                TrimSpacesAndLineBreaks(msgnode.InnerText),
                remparams
            );
        }

        static Facility NodeToFacility(XmlNode fnode) {
            XmlElement e;
            string name;
            uint code;

            e = (XmlElement)fnode;
            name = e.GetAttribute("name");
            
            if (!uint.TryParse(e.GetAttribute("hex"), NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out code)) {
                throw new FormatException();
            }

            return new Facility(name, code);
        }

        private static string TrimSpacesAndLineBreaks(string s) {
            return MultipleWhitespace.Replace(s, " ").Trim(' ', '\r', '\n');
        }
    }
}
