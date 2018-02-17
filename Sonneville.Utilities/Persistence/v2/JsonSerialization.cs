using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sonneville.Utilities.Persistence.v2
{
    public static class JsonSerialization
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new SettingsReaderContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            TypeNameHandling = TypeNameHandling.Auto,
        };

        private class SettingsReaderContractResolver : DefaultContractResolver
        {
            private const BindingFlags Flags = BindingFlags.Public |
                                               BindingFlags.Instance;

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                return type.GetProperties(Flags)
                    .Select(propertyInfo => CreateProperty(propertyInfo, memberSerialization))
                    .Union(type.GetFields(Flags).Select(field => CreateProperty(field, memberSerialization)))
                    .Select(jsonProperty =>
                    {
                        jsonProperty.Writable = true;
                        jsonProperty.Readable = true;
                        return jsonProperty;
                    })
                    .ToList();
            }
        }
    }
}