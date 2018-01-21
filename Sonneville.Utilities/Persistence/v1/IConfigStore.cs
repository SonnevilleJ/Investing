namespace Sonneville.Utilities.Persistence.v1
{
    public interface IConfigStore
    {
        T Get<T>() where T : class, new();

        void Save<T>(T config);

        T Load<T>() where T : class, new();

        void DeleteAll();
    }
}