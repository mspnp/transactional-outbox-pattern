using System;

namespace Contacts.Domain
{
     public interface IEntity
    {
        Guid Id { get; }
    }
}