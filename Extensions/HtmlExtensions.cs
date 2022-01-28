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
        public static Stylesheet SheetFromStyleNode(HtmlNode node)
        {
            if (node.Name != "style")
            {
                Console.WriteLine($"Invalid node name: {node.Name}");
                return null;
            }
            return new Stylesheet(node.InnerText);
        }
        
        public static List<Stylesheet> GetStylesheets(this Uri uri)
        {
            return GetStylesheets(uri.AbsoluteUri);
        }
        private static List<Stylesheet> GetStylesheets(string url)
        {
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
                //Console.WriteLine($"Stylesheet URL: {urlRoot+ rel}");
            }
            //Console.WriteLine($"Root Begins with: {rootLong}");
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
            return output;
        }

        public static IEnumerable<HtmlNode> SelectChildren(this HtmlNode node, CssSelector selector)
        {
            if (selector.SelectorType == SelectorType.ElementList)
            {
                var pattern = @"([^\s\,]+)";
                var matches = Regex.Matches(selector.Value, pattern, RegexOptions.Compiled);
                return matches.SelectMany(m => node.CssSelect(m.Value));
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
                if (!node.Attributes.Any(atb => atb.Name == prop.Name))
                {
                    Console.WriteLine($"Adding inline property {prop.Name} with value {prop.Value}");
                }
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

        public static HtmlDocument LoadPageWithInlineStyles(this Uri uri)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var d = new HttpDownloader(uri.AbsoluteUri, null, null);
            var doc = new HtmlDocument();
            doc.LoadHtml(d.GetPage());
            var sheets = uri.GetStylesheets();
            doc = doc.WithInlineStylesheets(sheets);
            watch.Stop();
            Console.WriteLine($"Loading page at: {uri.AbsoluteUri} with inline style took {watch.ElapsedMilliseconds} ms");
            return doc;
        }

        public static IEnumerable<Stylesheet> GetNodeStyles(this HtmlDocument doc)
        {
            return doc.DocumentNode.CssSelect("style").Select(n => new Stylesheet(n.InnerText));
        }

        public static Stylesheet GetMergedNodeStyles(this HtmlDocument doc)
        {
            var nodeStyles = doc.GetNodeStyles().ToList();
            if (nodeStyles.Count < 1)
            {
                Console.WriteLine("No style nodes in document");
                return null;
                
            }
            var output = nodeStyles[0];
            nodeStyles.RemoveAt(0);
            foreach (var style in nodeStyles)
            {
                output.Append(style);
            }
            return output;
        }
    }
}