namespace Sonneville.Utilities.Persistence.v2
{
    public interface IDataStore
    {
        T Get<T>() where T : class, new();

        void Save<T>(T config);

        T Load<T>() where T : class, new();

        void DeleteAll();
    }
}