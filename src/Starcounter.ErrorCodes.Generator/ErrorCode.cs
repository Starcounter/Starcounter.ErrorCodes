using System;
using System.Collections.Generic;

namespace Starcounter.ErrorCodes.Generator {
    public sealed class ErrorCode {
        public Facility Facility { get; private set;}
        public string Name { get; private set; }
        public ushort Code { get; private set; }
        public Severity Severity { get; private set; }
        public string Description { get; private set; }
        public IList<string> RemarkParagraphs { get; private set; }

        public string ConstantName {
            get { return Name; }
        }

        public string ConstantNameToUpper {
            get { return Name.ToUpper(); }
        }

        public uint CodeWithFacility {
            get {
                return (Facility.Code * 1000) + Code;
            }
        }

        public string FormattedCodeWithDescription {
            get {
                string formattedDesc = Description;
                formattedDesc = formattedDesc.Replace("\"", "\\\"");
                return string.Format("{0} (SCERR{1}): {2}", Name, CodeWithFacility, formattedDesc);
            }
        }

        internal ErrorCode(
                Facility facility,
                string name,
                ushort code,
                Severity severity,
                string description,
                IEnumerable<string> remarkParagraphs) {
            if (code > 999)
                throw new ArgumentOutOfRangeException("code", code, "Not a valid value (allowed range is 0-999): 0x" + code.ToString("X"));

            this.Facility = facility;
            this.Name = name;
            this.Code = code;
            this.Severity = severity;
            this.Description = description;
            this.RemarkParagraphs = new List<string>(remarkParagraphs).AsReadOnly();
        }
    }
}