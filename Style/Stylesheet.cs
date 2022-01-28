using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace CssScraper.Style
{
    using SelectorStyleMap = Dictionary<CssSelector, List<CssProperty>>;
    public class Stylesheet
    {
        public static string SelectorBodyPattern = @"(?<=\{)([\s\S]+)(?=})";
        public SelectorStyleMap StyleMap = new SelectorStyleMap();

        public Stylesheet()
        {

        }
        // Append the content of another stylesheet onto this one. Note that selectors with the same name will be replaced
        public void Append(Stylesheet sheet)
        {
            foreach(var kvp in sheet.StyleMap)
            {
                this.StyleMap[kvp.Key] = kvp.Value;
            }
        }
        //Parse the style out of the file's body text
        public Stylesheet(string sheetString)
        {
            //Console.WriteLine($"Sheet value: {sheetString}");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var selectorStrings = sheetString.Split(@"}").Select(str => str + @"}");
            bool shouldLog = selectorStrings.Count() <= 20;
            StyleMap = selectorStrings.Where(str => CssSelector.IsValidSelector(str)).ToDictionary(str => new CssSelector(str, shouldLog), str => PropsForFullString(str)); 
            watch.Stop();
            Console.WriteLine($"Parsing stylesheet into {StyleMap.Count} selectors took {watch.ElapsedMilliseconds} ms-- using LINQ");
            if (shouldLog)
            {
                foreach(var sel in StyleMap.Keys)
                {
                    var selStr = (sel.Value.Length < 20) ? sel.Value : sel.Value.Substring(0, 20) + "...";
                    Console.WriteLine($"Selector: {selStr} Type: {sel.SelectorType}");
                }
            }
        }
        private static List<CssProperty> PropsForFullString(string input)
        {
            string propsBody = Regex.Match(input, SelectorBodyPattern, RegexOptions.Compiled).Value;
            var props = propsBody.Split(@";").Select(str => str + @";");
            return props.Select(p => new CssProperty(p)).ToList();
        }

        private static string GetSheetString(string url)
        {
            var d = new HttpDownloader(url, null, null);
            var output = d.GetPage();
            Console.WriteLine($"Sheet at URL {url} has size {(output.Length * sizeof(char)) / 1000} kb");
            return output;
        }
        //Get the style object straight from a web address
        public Stylesheet(Uri uri) : this(GetSheetString(uri.AbsoluteUri))
        {
            
        }

        public List<CssProperty> GetPropertiesOf(string selector)
        {
            return StyleMap.FirstOrDefault(kvp => kvp.Key.Value == selector).Value;
        }

        public string CssString { get {return GetCssString();}}
        private string GetCssString()
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