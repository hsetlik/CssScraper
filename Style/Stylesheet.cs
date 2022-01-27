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
        public static class StylesheetRegex
        {
            public static Regex SelectorBodyExp = new Regex(@"(?<=\{)([\s\S]+)(?=\})");
        }
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
            
            var selectorStrings = sheetString.Split(@"}").Select(str => str + @"}").ToList();
            
            watch.Stop();
            Console.WriteLine($"Splitting string with size {sheetString.Length * sizeof(char)} kb took {watch.ElapsedMilliseconds} ms");
            watch.Restart();
            StyleMap = selectorStrings.Where(str => CssSelector.IsValidSelector(str)).ToDictionary(str => new CssSelector(str), str => PropsForFullString(str)); 
            watch.Stop();
            Console.WriteLine($"Parsing into {selectorStrings.Count} selectors took {watch.ElapsedMilliseconds} ms-- using LINQ");
            /*
            if (selectorStrings.Count < 20)
            {
                foreach(var kvp in StyleMap)
                {
                    Console.WriteLine($"Selector: {kvp.Key.Value}");
                    foreach(var prop in kvp.Value)
                    {
                        Console.WriteLine($"    {prop.CssValue}");
                    }
                }
            }
            */
        }
        private static List<CssProperty> PropsForFullString(string input)
        {
            var output = new List<CssProperty>();
            string propsBody = StylesheetRegex.SelectorBodyExp.Match(input).Value;
            var props = propsBody.Split(@";").Select(str => str + @";").ToList();
            Parallel.ForEach(props, prop =>
            {
                output.Add(new CssProperty(prop));
            });
            return output;
        }

        private static List<Task<KeyValuePair<CssSelector, List<CssProperty>>>> GetStyleMapTasks(IEnumerable<string> input)
        {
            return input.Where(str => CssSelector.IsValidSelector(str)).Select(str => 
            {
                return Task.Run(() => 
                {
                    var selector = new CssSelector(str, input.Count() < 20);
                    return new KeyValuePair<CssSelector, List<CssProperty>>(selector, PropsForFullString(str));
                });
            }).ToList();
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