using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SabreTools.Core
{
    // https://github.com/dotnet/runtime/issues/728
    public class BaseFirstContractResolver : DefaultContractResolver
    {
        public BaseFirstContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy();
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization)
                .OrderBy(p => BaseTypesAndSelf(p.DeclaringType).Count()).ToList();

            IEnumerable<Type> BaseTypesAndSelf(Type t)
            {
                while (t != null)
                {
                    yield return t;
                    t = t.BaseType;
                }
            }
        }
    }
}
