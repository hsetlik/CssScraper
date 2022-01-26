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
        All
    }
    public static class SelectorExpressions
    {
        public static Regex IdExp { get { return new Regex(@"(?<=#)([\S]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);}}
        public static Regex ClassExp { get { return new Regex(@"(?<=^\.)[^\{]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);}}
        public static Regex ElementWithClassExp { get { return new Regex(@"(\S)+\.(\S+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);}}
        public static Regex ElementListExp { get { return new Regex(@"([\s\S]+)\,([\S\ ][^\{]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);}}
        public static Regex ElementExp { get { return new Regex(@"^([^\.\s]*+)(?!\.)", RegexOptions.Compiled | RegexOptions.IgnoreCase);}}
        public static Regex AllExp { get { return new Regex(@"\*", RegexOptions.Compiled | RegexOptions.IgnoreCase);}}
    }
    public class CssSelector
    {
        public SelectorType SelectorType { get {return SelectorTypeFor(Value);}}
        public string Value { get; set; }
        public CssSelector()
        {
        }

        public CssSelector(string input)
        {
            Value = SelectorTextFor(input);
        }
        private static SelectorType SelectorTypeFor(string rawValue)
        {
            if (SelectorExpressions.IdExp.Matches(rawValue).Any())
                return SelectorType.Id;
            else if (SelectorExpressions.ClassExp.Matches(rawValue).Any())
                return SelectorType.Class;
            else if (SelectorExpressions.ElementExp.Matches(rawValue).Any())
                return SelectorType.Element;
            else if (SelectorExpressions.ElementWithClassExp.Matches(rawValue).Any())
                return SelectorType.ElementWithClass;
            else if (SelectorExpressions.ElementListExp.Matches(rawValue).Any())
                return SelectorType.ElementList;
            else if (SelectorExpressions.AllExp.Matches(rawValue).Any())
                return SelectorType.All;
            return SelectorType.All;
        }

        private static string SelectorTextFor(string rawValue)
        {
            if (SelectorExpressions.IdExp.Matches(rawValue).Any())
                return SelectorExpressions.IdExp.Match(rawValue).Value;
            else if (SelectorExpressions.ClassExp.Matches(rawValue).Any())
                return SelectorExpressions.ClassExp.Match(rawValue).Value;
            else if (SelectorExpressions.ElementExp.Matches(rawValue).Any())
                return SelectorExpressions.ElementExp.Match(rawValue).Value;
            else if (SelectorExpressions.ElementWithClassExp.Matches(rawValue).Any())
                return SelectorExpressions.ElementExp.Match(rawValue).Value;
            else if (SelectorExpressions.ElementListExp.Matches(rawValue).Any())
                return SelectorExpressions.ElementListExp.Match(rawValue).Value;
            else if (SelectorExpressions.AllExp.Matches(rawValue).Any())
                return SelectorExpressions.AllExp.Match(rawValue).Value;
            return $"Input {rawValue} does not match any selector type!";
        }
        
    }
}