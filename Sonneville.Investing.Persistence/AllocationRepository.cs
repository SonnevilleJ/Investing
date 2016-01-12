using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Persistence
{
    public interface IAllocationRepository
    {
        void Save(string username, AccountAllocation allocation);

        AccountAllocation Get(string username);

        bool Exists(string username);

        void Delete(string username);
    }

    public class AllocationRepository : IAllocationRepository
    {
        private readonly string _repositoryRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public void Save(string username, AccountAllocation allocations)
        {
            var dictionary = allocations.ToDictionary().ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToDictionary());
            var json = JsonConvert.SerializeObject(dictionary, Formatting.Indented);

            var persistenceStorePath = GetPersistenceStorePath(username);
            File.WriteAllText(persistenceStorePath, json);
        }

        public AccountAllocation Get(string username)
        {
            if (!Exists(username))
            {
                throw new KeyNotFoundException($"Couldn't find persisted value for username {username}!");
            }

            var json = File.ReadAllText(GetPersistenceStorePath(username));

            var deserializedObject = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, decimal>>>(json);
            var accountDictionary = deserializedObject.ToDictionary(
                kvp => kvp.Key,
                kvp => PositionAllocation.FromDictionary(kvp.Value));
            return AccountAllocation.FromDictionary(accountDictionary);
        }

        public bool Exists(string username)
        {
            var persistenceStorePath = GetPersistenceStorePath(username);
            return File.Exists(persistenceStorePath);
        }

        public void Delete(string username)
        {
            var persistenceStorePath = GetPersistenceStorePath(username);
            if (File.Exists(persistenceStorePath))
            {
                File.Delete(persistenceStorePath);
            }
        }

        private string GetPersistenceStorePath(string username)
        {
            return Path.Combine(_repositoryRoot, $"{username}.json");
        }
    }
}