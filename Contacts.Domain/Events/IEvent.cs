using System;
using MediatR;

namespace Contacts.Domain.Events
{
    public interface IEvent : INotification
    {
        public Guid Id { get; }
        public string Action { get; }
    }
}