using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CssScraper.Style
{
    public static class PropertyExpressions
    {
        public static Regex PropertyNameExp { get { return new Regex(@"(\S+)(?=\:)", RegexOptions.Compiled | RegexOptions.IgnoreCase);}}
        public static Regex PropertyValueExp { get { return new Regex(@"(?<=\:\s)(\S[^\;]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);}}
    }
    public class CssProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string CssValue {get { return $"{Name}: {Value};\n";}}
        public CssProperty()
        {
        }

        public CssProperty(string inputStr)
        {
            Name = PropertyExpressions.PropertyNameExp.Match(inputStr).Value;
            Value = PropertyExpressions.PropertyValueExp.Match(inputStr).Value;
        }
    }

}