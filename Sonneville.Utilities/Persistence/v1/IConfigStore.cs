namespace Sonneville.Utilities.Persistence.v1
{
    public interface IConfigStore<T> where T : class, new()
    {
        T Get();

        void Save(T config);

        T Load();

        void DeleteAll();
    }
}