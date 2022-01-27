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
    public static class PropertyExpressions
    {
        public static Regex PropertyNameExp { get { return new Regex(@"([^\n]+)(?=\:)", RegexOptions.Compiled | RegexOptions.IgnoreCase);}}
        public static Regex PropertyValueExp { get { return new Regex(@"(?<=\:)([^\n]+)(?=\;)", RegexOptions.Compiled | RegexOptions.IgnoreCase);}}
    }
    public class CssProperty
    {
        public string Name { get; set; }
        public string Value { get; set;  }
        public string CssValue {get { return $"{Name}: {Value};\n";}}
        public CssProperty()
        {
        }

        public CssProperty(string inputStr)
        {
            Name = PropertyExpressions.PropertyNameExp.Match(inputStr).Value;
            Value = PropertyExpressions.PropertyValueExp.Match(inputStr).Value;
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