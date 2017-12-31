namespace Sonneville.FidelityWebDriver.Data
{
    public interface IPosition
    {
        string Ticker { get; }

        string Description { get; }

        bool IsCore { get; }

        bool IsMargin { get; }

        decimal LastPrice { get; }

        decimal TotalGainDollar { get; }

        decimal TotalGainPercent { get; }

        decimal CurrentValue { get; }

        decimal Quantity { get; }

        decimal CostBasisPerShare { get; }

        decimal CostBasisTotal { get; }
    }

    public class Position : IPosition
    {
        public Position()
        {
        }

        public Position(string ticker, string description, bool isCore, bool isMargin, decimal lastPrice,
            decimal totalGainDollar, decimal totalGainPercent, decimal currentValue, decimal quantity,
            decimal costBasisPerShare, decimal costBasisTotal)
        {
            Ticker = ticker;
            Description = description;
            IsCore = isCore;
            IsMargin = isMargin;
            LastPrice = lastPrice;
            TotalGainDollar = totalGainDollar;
            TotalGainPercent = totalGainPercent;
            CurrentValue = currentValue;
            Quantity = quantity;
            CostBasisPerShare = costBasisPerShare;
            CostBasisTotal = costBasisTotal;
        }

        public string Ticker { get; set; }

        public string Description { get; set; }

        public bool IsCore { get; set; }

        public bool IsMargin { get; set; }

        public decimal LastPrice { get; set; }

        public decimal TotalGainDollar { get; set; }

        public decimal TotalGainPercent { get; set; }

        public decimal CurrentValue { get; set; }

        public decimal Quantity { get; set; }

        public decimal CostBasisPerShare { get; set; }

        public decimal CostBasisTotal { get; set; }
    }
}