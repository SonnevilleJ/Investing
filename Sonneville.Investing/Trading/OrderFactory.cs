using System;
using Sonneville.Investing.Domain;

namespace Sonneville.Investing.Trading
{
    public class OrderFactory
    {
        public Order Create(string ticker, TransactionType transactionType, DateTime creationDate, decimal shares, decimal pricePerShare)
        {
            return new Order
            {
                Ticker = ticker,
                TransactionType = transactionType,
                CreationDate = creationDate,
                Shares = shares,
                PricePerShare = pricePerShare,
            };
        }
    }
}