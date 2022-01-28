using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CssScraper.Style
{
    public enum CssLengthUnit
    {
        cm,
        mm,
        inch,
        px,
        pt,
        pc,
        em,
        ex,
        rem,
        ch,
        vw,
        vh,
        vmin,
        vmax,
        percent
    }
    public class CssProperty
    {
        public static string PropNamePattern = @"([^\n]+)(?=\:)"; 
        public static string PropValuePattern = @"(?<=\:)([^\n]+)(?=\;)";
        public string Name { get; set; }
        public string Value { get; set;  }
        public string CssValue {get { return $"{Name}: {Value};\n";}}
        public CssProperty()
        {
        }

        public CssProperty(string inputStr)
        {
            Name = Regex.Match(inputStr, PropNamePattern, RegexOptions.Compiled).Value;
            Value = Regex.Match(inputStr, PropValuePattern, RegexOptions.Compiled).Value;
        }

        

        private string CssUnitString(CssLengthUnit unit)
        {
            if (unit == CssLengthUnit.percent)
                return @"%";
            if (unit == CssLengthUnit.inch)
                return "in";
            return unit.ToString();
        }
    }

}