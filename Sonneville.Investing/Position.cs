using System;

namespace Sonneville.Investing
{
    public class Position
    {
        public string Ticker { get; set; }

        public decimal Shares { get; set; }

        public decimal PerSharePrice { get; set; }

        public DateTime DateTime { get; set; }

        public decimal Value => Shares*PerSharePrice;
    }
}