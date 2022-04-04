using System;

namespace Contacts.Infrastructure.Exceptions;

public class DomainObjectTooManyRequestsException : Exception
{
    public DomainObjectTooManyRequestsException()
    {
    }

    public DomainObjectTooManyRequestsException(string message) : base(message)
    {
    }

    public DomainObjectTooManyRequestsException(string message, Exception inner) : base(message, inner)
    {
    }
}