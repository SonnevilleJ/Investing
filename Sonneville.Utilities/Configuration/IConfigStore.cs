namespace Sonneville.Utilities.Configuration
{
    public interface IConfigStore
    {
        T Get<T>() where T : class, new();

        void Save<T>(T config);

        T Load<T>() where T : class, new();

        void DeleteAll();
    }
}