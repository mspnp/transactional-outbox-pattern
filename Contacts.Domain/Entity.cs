using System;
using Newtonsoft.Json;

namespace Contacts.Domain
{
    public abstract class Entity : IEntity
    {
        [JsonProperty] public Guid Id { get; protected init; }
    }
}