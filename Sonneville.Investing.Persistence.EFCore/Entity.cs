namespace Sonneville.Investing.Persistence.EFCore
{
    public abstract class Entity<TKey>
    {
        public TKey DatabaseId { get; set; }
    }
}
