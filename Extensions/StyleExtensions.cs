using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CssScraper.Style
{
    using SelectorStyleMap = Dictionary<CssSelector, List<CssProperty>>;
    public static class StyleExtensions
    {
        public static string SelectorStyleString(this SelectorStyleMap map, string selector)
        {
            if (!map.Any(kvp => kvp.Key.Value == selector))
                return "null";
            var selPair = map.FirstOrDefault(kvp => kvp.Key.Value == selector);
            string output = selPair.Key.Value;
            output += @" {" + '\n';
            
            output += @"}";
            return output;
        }
        
    }
}