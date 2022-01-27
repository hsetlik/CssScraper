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
            return input.Select(str => 
            {
                return Task.Run(() => 
                {
                    var selector = new CssSelector(str, input.Count() < 20);
                    return new KeyValuePair<CssSelector, List<CssProperty>>(selector, PropsForFullString(str));
                });
            }).ToList();
        }

        //Parse the style out of the file's body text
        public Stylesheet(string sheetString)
        {
            //Console.WriteLine($"Sheet value: {sheetString}");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var selectorStrings = sheetString.Split(@"}").Select(str => str + @"}").ToList();
            var mapTasks = GetStyleMapTasks(selectorStrings).ToArray();
            var pairs = Task.WhenAll<KeyValuePair<CssSelector, List<CssProperty>>>(mapTasks).Result.ToList();
            foreach(var pair in pairs)
            {
                StyleMap[pair.Key] = pair.Value;
            }
            watch.Stop();
            Console.WriteLine($"Parsing into {selectorStrings.Count} parts took {watch.ElapsedMilliseconds} ms");
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