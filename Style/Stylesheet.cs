using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CssScraper.Style
{
    using SelectorStyleMap = Dictionary<CssSelector, List<CssProperty>>;
    public class Stylesheet
    {
        public SelectorStyleMap StyleMap = new SelectorStyleMap();
        public Stylesheet()
        {

        }

        //Parse the style out of the file's body text
        public Stylesheet(string sheetString)
        {
            var selectorStrings = sheetString.Split(@"\}").ToList();
            foreach(var sel in selectorStrings)
            {
                var selector = new CssSelector(sel);
                StyleMap[selector] = new List<CssProperty>();
                var propStrings = sel.Substring(sel.Length).Split(@";").ToList();
                foreach(var str in propStrings)
                {
                    StyleMap[selector].Add(new CssProperty(str));
                }
            }
        }
        //Get the style object straight from a web address
        public Stylesheet(Uri uri)
        {

        }

        public List<CssProperty> GetPropertiesOf(string selector)
        {
            return StyleMap.FirstOrDefault(kvp => kvp.Key.Value == selector).Value;
        }

        public string GetCssString()
        {
            string output = "";
            foreach(var kvp in StyleMap)
            {
                output += StyleMap.SelectorStyleString(kvp.Key.Value) + "\n";
            }
            return output;
        }
    }
}