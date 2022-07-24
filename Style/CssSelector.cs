using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CssScraper.Style
{
    public enum SelectorType
    {
        Id,
        Class,
        Element,
        ElementWithClass,
        ElementList,
        AtRule,
        All
    }
    /*
    public static class SelectorExpressions
    {
        public static Regex IdExp = new Regex(@"^(#[^\{]+)");
        public static Regex ClassExp = new Regex(@"^(\.[^\{]+)");
        public static Regex ElementWithClassExp = new Regex(@"^[\w\.]+(?=\.)[^\{]+");
        public static Regex ElementListExp = new Regex(@"([\s\S]+)\,([\S\ ][^\{]+)");
        public static Regex ElementExp = new Regex(@"^(?!#)(?!@)[^\,\.*]+(?=\{)");
        public static Regex AllExp = new Regex(@"\*");
        public static Regex AtRuleExp = new Regex(@"(@[^\n]+)(?=\{)");
    }
    */
    public class CssSelector
    {
        public static string SelectorPattern = @"^[^\{]+";
        public static IEnumerable<(SelectorType type, string pattern)> SelectorPatterns = new List<(SelectorType type, string pattern)>
        {
            (SelectorType.Id, @"^(#[^\{]+)"),
            (SelectorType.Class, @"^(\.[^\{]+)"),
            (SelectorType.ElementWithClass, @"^[\w\.]+(?=\.)[^\{]+"), //good
            (SelectorType.Element, @"^(?!#)(?!@)[^,\.*#]+(?=\{)"), //good
            (SelectorType.AtRule, @"(@[^\n\{]+)(?=\{)"), //good
            (SelectorType.ElementList, @"^(?!@+)[^\n]+\,([^\n]+)(?=\{)"), //good
            (SelectorType.All, @"\*") //good
        };
        public SelectorType SelectorType { get {return SelectorTypeFor(Value + @"{");}}
        public string Value { get; private set; }
        public CssSelector()
        {
        }

        public CssSelector(string input, bool log=false)
        {
            //Value = input;

            Value = Regex.Match(input, SelectorPattern, RegexOptions.Compiled).Value;

        }
        private static SelectorType SelectorTypeFor(string rawValue)
        {
            return SelectorPatterns.FirstOrDefault(kvp => Regex.IsMatch(rawValue, kvp.pattern, RegexOptions.Compiled)).type;
        }

        public static bool IsValidSelector(string rawValue)
        {
           return SelectorPatterns.Any(kvp => Regex.IsMatch(rawValue, kvp.pattern, RegexOptions.Compiled)); 
        }

        private static string SelectorTextFor(string rawValue, bool log=false)
        {
            var pair = SelectorPatterns.FirstOrDefault(kvp => Regex.IsMatch(rawValue, kvp.pattern, RegexOptions.Compiled));
            var output = Regex.Match(rawValue, pair.pattern, RegexOptions.Compiled).Value;
            if (log)
            {
                var str = (rawValue.Length > 20) ? rawValue.Substring(0, 20) + "..." : rawValue;
                Console.WriteLine($"String {str}\nUsing pattern: {pair.pattern}");
                Console.WriteLine($"Output string is: {output} \n");
            }
            return output;
        }
        
    }
}