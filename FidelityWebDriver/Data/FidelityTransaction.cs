using System;
using System.Reflection;
using System.Text;

namespace Sonneville.FidelityWebDriver.Data
{
    public interface IFidelityTransaction : IEquatable<IFidelityTransaction>
    {
        DateTime? RunDate { get; }

        string AccountName { get; }

        string AccountNumber { get; }

        string Action { get; }

        TransactionType Type { get; }

        string Symbol { get; }

        string SecurityDescription { get; }

        string SecurityType { get; }

        decimal? Quantity { get; }

        decimal? Price { get; }

        decimal? Commission { get; }

        decimal? Fees { get; }

        decimal? AccruedInterest { get; }

        decimal? Amount { get; }

        DateTime? SettlementDate { get; }
    }

    public class FidelityTransaction : IFidelityTransaction
    {
        public DateTime? RunDate { get; set; }

        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        public string Action { get; set; }

        public TransactionType Type { get; set; }

        public string Symbol { get; set; }

        public string SecurityDescription { get; set; }

        public string SecurityType { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? Price { get; set; }

        public decimal? Commission { get; set; }

        public decimal? Fees { get; set; }

        public decimal? AccruedInterest { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? SettlementDate { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var memberInfo in typeof(FidelityTransaction).GetProperties())
            {
                var propertyInfo = (PropertyInfo) memberInfo;
                stringBuilder.AppendLine($"{propertyInfo.Name} {propertyInfo.GetValue(this, null)}");
            }
            return stringBuilder.ToString();
        }

        public bool Equals(IFidelityTransaction other)
        {
            return other != null &&
                   RunDate.Equals(other.RunDate) &&
                   string.Equals(AccountName, other.AccountName, StringComparison.InvariantCulture) &&
                   string.Equals(AccountNumber, other.AccountNumber, StringComparison.InvariantCulture) &&
                   string.Equals(Action, other.Action, StringComparison.InvariantCulture) && Type == other.Type &&
                   string.Equals(Symbol, other.Symbol, StringComparison.InvariantCulture) &&
                   string.Equals(SecurityDescription, other.SecurityDescription, StringComparison.InvariantCulture) &&
                   string.Equals(SecurityType, other.SecurityType, StringComparison.InvariantCulture) &&
                   Quantity == other.Quantity && Price == other.Price && Commission == other.Commission &&
                   Fees == other.Fees && AccruedInterest == other.AccruedInterest && Amount == other.Amount &&
                   SettlementDate.Equals(other.SettlementDate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FidelityTransaction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = RunDate.GetHashCode();
                hashCode = (hashCode * 397) ^ (AccountName != null
                               ? StringComparer.InvariantCulture.GetHashCode(AccountName)
                               : 0);
                hashCode = (hashCode * 397) ^ (AccountNumber != null
                               ? StringComparer.InvariantCulture.GetHashCode(AccountNumber)
                               : 0);
                hashCode = (hashCode * 397) ^ (Action != null
                               ? StringComparer.InvariantCulture.GetHashCode(Action)
                               : 0);
                hashCode = (hashCode * 397) ^ (int) Type;
                hashCode = (hashCode * 397) ^ (Symbol != null
                               ? StringComparer.InvariantCulture.GetHashCode(Symbol)
                               : 0);
                hashCode = (hashCode * 397) ^ (SecurityDescription != null
                               ? StringComparer.InvariantCulture.GetHashCode(SecurityDescription)
                               : 0);
                hashCode = (hashCode * 397) ^ (SecurityType != null
                               ? StringComparer.InvariantCulture.GetHashCode(SecurityType)
                               : 0);
                hashCode = (hashCode * 397) ^ Quantity.GetHashCode();
                hashCode = (hashCode * 397) ^ Price.GetHashCode();
                hashCode = (hashCode * 397) ^ Commission.GetHashCode();
                hashCode = (hashCode * 397) ^ Fees.GetHashCode();
                hashCode = (hashCode * 397) ^ AccruedInterest.GetHashCode();
                hashCode = (hashCode * 397) ^ Amount.GetHashCode();
                hashCode = (hashCode * 397) ^ SettlementDate.GetHashCode();
                return hashCode;
            }
        }
    }
}