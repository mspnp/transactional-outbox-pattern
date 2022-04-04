using Contacts.Domain;
using Newtonsoft.Json;

namespace Contacts.Infrastructure.Context;

public class DataObject<T> : IDataObject<T> where T : Entity
{
    public DataObject(string id, string partitionKey, string objectType, T entity,
        string eTag,
        int ttl,
        EntityState state = EntityState.Unmodified
    )
    {
        Id = id;
        PartitionKey = partitionKey;
        Type = objectType;
        Data = entity;
        Ttl = ttl;
        Etag = eTag;
        State = state;
    }

    [JsonProperty] public string Id { get; private set; }
    [JsonProperty] public string PartitionKey { get; private set; }
    [JsonProperty] public string Type { get; private set; }
    public T Data { get; set; }
    [JsonProperty("_etag")] public string Etag { get; set; }
    public int Ttl { get; }

    [JsonIgnore] public EntityState State { get; set; }
}