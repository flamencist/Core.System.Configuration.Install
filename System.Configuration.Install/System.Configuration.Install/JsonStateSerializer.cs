using System.Collections;
using Newtonsoft.Json;

namespace System.Configuration.Install
{
    internal interface IStateSerializer
    {
        string Serialize(IDictionary state);
        IDictionary Deserialize(string serialized);
    }

    internal class JsonStateSerializer : IStateSerializer
    {
        public string Serialize(IDictionary state)
        {
            return JsonConvert.SerializeObject(state);
        }

        public IDictionary Deserialize(string serialized)
        {
            return JsonConvert.DeserializeObject<IDictionary>(serialized);
        }
    }
}