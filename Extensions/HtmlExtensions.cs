using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CssScraper.Style;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace CssScraper.Extensions
{
    public static class HtmlExtensions
    {
        public static List<Stylesheet> GetStylesheets(string url)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var downloader = new HttpDownloader(url, null, null);
            var pageString = downloader.GetPage();
            var document = new HtmlDocument();
            document.LoadHtml(pageString);
            var urlRoot = new Uri(url).DnsSafeHost;
            var rootLong = urlRoot.Substring(0, 8);
            var rootShort = urlRoot.Substring(0, 7);
            var stylesheetNodes = document.DocumentNode.Descendants().Where(d => d.Attributes.Any(a => a.Value == "stylesheet")).ToList();
            var sheetUrls = new List<string>();
            foreach(var sheet in stylesheetNodes)
            {
                var rel = sheet.GetAttributeValue("href", "no href");
                rel = Regex.Replace(rel, @"amp;", "");
                Console.WriteLine($"Stylesheet URL: {urlRoot+ rel}");
            }
            Console.WriteLine($"Root Begins with: {rootLong}");
            if (!(rootShort == @"http://" || rootLong == @"https://"))
            {
                string shorter = url.Substring(0, 7);
                string longer = url.Substring(0, 8);
                string prefix = (shorter == @"http://") ? shorter : longer;
                urlRoot = prefix + urlRoot;
            }
            var output = new List<Stylesheet>();
            foreach(var sheetUrl in sheetUrls)
            {
                var d = new HttpDownloader(sheetUrl, null, null);
                output.Add(new Stylesheet(d.GetPage()));
            }
            watch.Stop();
            Console.WriteLine($"Getting {sheetUrls.Count} stylesheets took {watch.ElapsedMilliseconds} ms");
            return output;
        }

        public static IEnumerable<HtmlNode> SelectChildren(this HtmlNode node, CssSelector selector)
        {
            if (selector.SelectorType == SelectorType.ElementList)
            {
                var exp = new Regex(@"([^\s\,]+)");
                var matches = exp.Matches(selector.Value).ToList();
                var output = new List<HtmlNode>();
                foreach(var match in matches)
                {
                    output.AddRange(node.CssSelect(match.Value));
                }
                return output;
            }
            return node.CssSelect(selector.Value);
        }
        public static HtmlNode WithInlineStylesheet(this HtmlNode node, Stylesheet sheet)
        {
            foreach(var style in sheet.StyleMap)
            {
                var children = node.SelectChildren(style.Key).ToList();
                foreach(var child in children)
                {
                    var newChild = child.WithProperties(style.Value);
                    node.ReplaceChild(newChild, child);
                }
            }
            return node;
        }
        
        public static HtmlNode WithProperties(this HtmlNode node, IEnumerable<CssProperty> props)
        {
            foreach(var prop in props)
            {
                node.SetAttributeValue(prop.Name, prop.Value);
            }
            return node;
        }
        public static HtmlDocument WithInlineStylesheets(this HtmlDocument doc, IEnumerable<Stylesheet> sheets)
        {
            var node = doc.DocumentNode;
            foreach(var sheet in sheets)
            {
                node = node.WithInlineStylesheet(sheet);
            }
            return node.OwnerDocument;
        }
    }
}