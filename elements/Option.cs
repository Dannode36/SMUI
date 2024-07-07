using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUI.elements
{
    public record struct Option
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public Option() { }
        public Option(string value)
        {
            Label = value;
            Value = value;
        }
        public Option(string label, string value)
        {
            Label = label;
            Value = value;
        }
    }
}
