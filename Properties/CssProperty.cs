using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CssScraper
{
    public class CssProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string CssValue {get { return $"{Name}: {Value}";}}
    }

}