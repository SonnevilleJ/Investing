using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Sonneville.Investing.Persistence
{
    public class AllocationRepository
    {
        private readonly string _repositoryRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public void Save(string username, Dictionary<string, Dictionary<string, decimal>> allocation)
        {
            var json = JsonConvert.SerializeObject(allocation, Formatting.Indented);

            var persistenceStorePath = GetPersistenceStorePath(username);
            File.WriteAllText(persistenceStorePath, json);
        }

        public Dictionary<string, Dictionary<string, decimal>> Get(string username)
        {
            if (!Exists(username))
            {
                throw new KeyNotFoundException($"Couldn't find persisted value for username {username}!");
            }

            var json = File.ReadAllText(GetPersistenceStorePath(username));

            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, decimal>>>(json);
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