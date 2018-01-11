namespace Sonneville.Utilities.Configuration
{
    public interface IConfigStore
    {
        void Save<T>(T config);

        T Load<T>() where T : class, new();

        void DeleteAll();
    }
}