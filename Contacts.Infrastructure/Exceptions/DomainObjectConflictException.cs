using System;

namespace Contacts.Infrastructure.Exceptions
{
    public class DomainObjectConflictException : Exception
    {
        public DomainObjectConflictException()
        {
        }

        public DomainObjectConflictException(string message) : base(message)
        {
        }

        public DomainObjectConflictException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}