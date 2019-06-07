using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xunit;

namespace System.Configuration.Install.Tests.System.Configuration.Install
{
    public class StateSerializerTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        public void Serialize_Deserialize_JsonConvert(Hashtable expected)
        {
            var stateSerializer = new JsonStateSerializer();
            var serialized = stateSerializer.Serialize(expected);
            var actual = stateSerializer.Deserialize<Hashtable>(serialized);
            foreach (DictionaryEntry entry in actual)
            {
                Assert.True(expected.ContainsKey(entry.Key));
                Assert.Equal(expected[entry.Key],actual[entry.Key]);
            }
        }
        
        [Theory]
        [MemberData(nameof(Data))]
        public void Serialize_Deserialize_JsonDataContract(Hashtable expected)
        {
            var stateSerializer = new XmlStateSerializer();
            var serialized = stateSerializer.Serialize(expected);
            var actual = stateSerializer.Deserialize<Hashtable>(serialized);
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
                    [nameof(Int64)] = 1L,
                    [nameof(Double)] = 1.1d
                } },
                new object[]{new Hashtable
                {
                    ["empty"] = string.Empty,
                    ["NULL"] = (string)null,
                    [nameof(String)] = "string"
                } },
                new object[]{new Hashtable
                {
                    [nameof(TestData)] = new TestData{A="test",B=1},
                    ["nested_states"] = new []
                    {
                        new Hashtable
                        {
                            ["empty"] = string.Empty,
                            ["NULL"] = (string)null,
                        }, 
                    }
                } },
            };


    }
    
    public class TestData
    {
        public string A { get; set; }
        public int B { get; set; }
        public override bool Equals(object obj)
        {
            if (base.Equals(obj)) return true;
            var that = obj as TestData;
            if (that == null) return false;
            return A.Equals(that.A) && B.Equals(that.B);
        }
    }
}