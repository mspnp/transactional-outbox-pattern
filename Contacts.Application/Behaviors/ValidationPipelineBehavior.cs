using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Contacts.Application.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest> _validator;

    public ValidationPipelineBehavior(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _validator.ValidateAndThrow(request);
        return next();
    }
}