using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using Newtonsoft.Json;


namespace System.Configuration.Install
{
    internal interface IStateSerializer
    {
        string Serialize<T>(T state) where T:class,IDictionary ;
        T Deserialize<T>(string serialized) where T:class,IDictionary;
    }

    internal class JsonStateSerializer : IStateSerializer
    {
        public string Serialize<T>(T state)where  T:class,IDictionary 
        {
            return JsonConvert.SerializeObject(state);
        }

        public T Deserialize<T>(string serialized)where  T:class,IDictionary 
        {
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }

    internal class XmlStateDataContractResolver : DataContractResolver
    {
	    public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
	    {
		    
		    var type = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType,null);
		    if (type == null)
		    {
			    var fullName = string.Concat(typeNamespace.Substring(typeNamespace.LastIndexOf('/')  + 1 ),".",typeName);
			    type = AppDomain.CurrentDomain.GetAssemblies().Select(_ => _.GetType(fullName))
				    .FirstOrDefault(_ => _ != null);
		    }

		    return type;
	    }

	    public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver,
		    out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
	    {
		    return knownTypeResolver.TryResolveType(type, declaredType, null, out typeName, out typeNamespace);  
	    }
    }

    internal class XmlStateSerializer : IStateSerializer
    {
	    public string Serialize<T>(T state) where  T:class,IDictionary 
	    {
		    var stream = new MemoryStream();
		    try
		    {
			    var types = state.Keys.Cast<object>().Where(_=>_!=null).Select(_ => _.GetType())
				    .Concat(state.Values.Cast<object>().Where(_ =>_!=null).Select(_ => _.GetType()))
				    .Concat(new []{typeof(Hashtable),typeof(Array)})
				    .Distinct();
			    
			    var netDataContractSerializer = new DataContractSerializer(typeof(T),new DataContractSerializerSettings
			    {
				    KnownTypes = types,
				    DataContractResolver = new XmlStateDataContractResolver()
			    });
			    
			    netDataContractSerializer.WriteObject(stream, state);
			    stream.Position = 0;
			    using (var reader = new StreamReader(stream))
			    {
				    return reader.ReadToEnd();
			    }
		    }
		    finally
		    {
			    stream.Close();
		    }
	    }

	    public T Deserialize<T>(string serialized) where T:class,IDictionary 
	    {
		    var fileStream = GenerateStreamFromString(serialized);
		    try
		    {
				    var netDataContractSerializer = new DataContractSerializer(typeof(T),new DataContractSerializerSettings
				    {
					    KnownTypes = new List<Type>{typeof(Hashtable)},
					    DataContractResolver = new XmlStateDataContractResolver(),
					    
				    });
				    return (T)netDataContractSerializer.ReadObject(fileStream);

		    }
		    finally
		    {
			    fileStream?.Close();
		    }
	    }
	    
	    private static Stream GenerateStreamFromString(string s)
	    {
		    var stream = new MemoryStream();
		    var writer = new StreamWriter(stream);
		    writer.Write(s);
		    writer.Flush();
		    stream.Position = 0;
		    return stream;
	    }
    }
}