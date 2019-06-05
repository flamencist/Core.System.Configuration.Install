using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace System.Configuration.Install.Tests.System.Configuration.Install
{
    public class StateSerializerTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        public void Serialize_Deserialize(Hashtable expected)
        {
            var stateSerializer = new JsonStateSerializer();
            var serialized = stateSerializer.Serialize(expected);
            var actual = stateSerializer.Deserialize(serialized);
            foreach (DictionaryEntry entry in actual)
            {
                Assert.True(expected.ContainsKey(entry.Key));
                Assert.Equal(expected[entry.Key],actual[entry.Key]);
            }
        }

        public static IEnumerable<object[]> Data() =>
            new List<object[]>
            {
                new object[]{new Hashtable() },
                new object[]{new Hashtable
                {
                    [nameof(Int64)] = 1l,
                    [nameof(Double)] = 1.1d
                } },
            };
    }
}