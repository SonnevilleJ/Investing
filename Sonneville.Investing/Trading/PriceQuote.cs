using System;

namespace Sonneville.Investing.Trading
{
    public class PriceQuote
    {
        public string Ticker { get; set; }

        public decimal PerSharePrice { get; set; }

        public DateTime DateTime { get; set; }
    }
}