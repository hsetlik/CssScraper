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
    public class CssSelector
    {
        public SelectorType SelectorType { get {return SelectorTypeFor(Value);}}
        public string Value { get; set; }
        public CssSelector()
        {
        }

        public CssSelector(string input, bool log=false)
        {
            //Value = input;
            Value = SelectorTextFor(input, log);
            //Console.WriteLine($"Selector value is: {Value}");
        }
        private static SelectorType SelectorTypeFor(string rawValue)
        {
            if (SelectorExpressions.IdExp.IsMatch(rawValue))
                return SelectorType.Id;
            else if (SelectorExpressions.ClassExp.IsMatch(rawValue))
                return SelectorType.Class;
            else if (SelectorExpressions.ElementExp.IsMatch(rawValue))
                return SelectorType.Element;
            else if (SelectorExpressions.AtRuleExp.IsMatch(rawValue))
                return SelectorType.AtRule;
            else if (SelectorExpressions.ElementWithClassExp.IsMatch(rawValue))
                return SelectorType.ElementWithClass;
            else if (SelectorExpressions.ElementListExp.IsMatch(rawValue))
                return SelectorType.ElementList;
            else if (SelectorExpressions.AllExp.IsMatch(rawValue))
                return SelectorType.All;
            return SelectorType.All;
        }

        private static string SelectorTextFor(string rawValue, bool log=false)
        {
            var selType = SelectorTypeFor(rawValue);
            if (log)
            {
                Console.WriteLine($"Selector type for string: {rawValue} is {selType.ToString()}");
            }
            switch (selType)
            {
                case SelectorType.Id:
                    return SelectorExpressions.IdExp.Match(rawValue).Value;
                case SelectorType.Class:
                    return SelectorExpressions.ClassExp.Match(rawValue).Value;
                case SelectorType.Element:
                    return SelectorExpressions.ElementExp.Match(rawValue).Value;
                case SelectorType.ElementWithClass:
                    return SelectorExpressions.ElementWithClassExp.Match(rawValue).Value;               
                case SelectorType.ElementList:
                    return SelectorExpressions.ElementListExp.Match(rawValue).Value;              
                case SelectorType.AtRule:
                    return SelectorExpressions.AtRuleExp.Match(rawValue).Value;
                case SelectorType.All:
                    return SelectorExpressions.AllExp.Match(rawValue).Value;
                default:
                    Console.WriteLine($"No selector type for: {rawValue}");
                    return "null";
            }
        }
        
    }
}