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
        public static Regex PropertyNameExp { get { return new Regex(@"(\S+)(?=\:)", RegexOptions.Compiled | RegexOptions.IgnoreCase);}}
        public static Regex PropertyValueExp { get { return new Regex(@"(?<=\:\s)(\S[^\;]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);}}
    }
    public class CssProperty
    {
        private string _value = "";
        public string Name { get; set; }
        public string Value { get {return _value;} }
        public string CssValue {get { return $"{Name}: {Value};\n";}}
        public CssProperty()
        {
        }

        public CssProperty(string inputStr)
        {
            Name = PropertyExpressions.PropertyNameExp.Match(inputStr).Value;
            _value = PropertyExpressions.PropertyValueExp.Match(inputStr).Value;
        }

        //set methods for various types
        public void Set(string input)
        {
            _value = input;
        }

        private string CssUnitString(CssLengthUnit unit)
        {
            if (unit == CssLengthUnit.percent)
                return @"%";
            if (unit == CssLengthUnit.inch)
                return "in";
            return unit.ToString();
        }
        public void Set(CssLengthUnit unit, int input)
        {
            _value = input.ToString() + CssUnitString(unit);
        }
        public void Set(CssLengthUnit unit, double input)
        {
            _value = input.ToString() + CssUnitString(unit);
        }
    }

}