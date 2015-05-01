using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Local.Classes.Helpers
{
    public class Currencies
    {
        public static readonly Dictionary<string, string> CurrencyMap = new Dictionary<string, string>
        {
            { "0", "None Selected" },
            { "1", "USD" },
            { "2", "EURO" },
            { "3", "CHF" },
            { "4", "GBP" },
            { "5", "SGD" },
            { "6", "INR" },
            { "7", "BRL" },
            { "8", "SAR" },
            { "9", "Days" },
            { "10", "%" },
            { "11", "RMB" }
        };
    }
}