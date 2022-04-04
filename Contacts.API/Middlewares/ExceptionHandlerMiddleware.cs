using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Contacts.Infrastructure.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Contacts.API.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            switch (error)
            {
                case OperationCanceledException e:
                    context.Response.StatusCode = 499; // Client Closed Request - non-standard used by nginx
                    break;
                case ValidationException e:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var vpd = new ValidationProblemDetails
                    {
                        Title = "Validation Error",
                        Detail = "Validation error occured.",
                        Status = (int)HttpStatusCode.BadRequest,
                        Instance = context.Request.Path,
                        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                        Extensions =
                        {
                            ["TraceId"] = Activity.Current?.Id ?? context?.TraceIdentifier
                        }
                    };

                    foreach (var failure in e.Errors)
                    {
                        vpd.Errors.Add(Char.ToLower(failure.PropertyName[0]) + failure.PropertyName.Substring(1),
                            new[] { failure.ErrorMessage });
                    }
                        
                    context.Response.ContentType = "application/problem+json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(vpd));
                    break;
                case DomainObjectNotModifiedException e:
                    context.Response.StatusCode = (int)HttpStatusCode.NotModified;
                    break;
                case DomainObjectNotFoundException e:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    var pdNf = new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = "Object has not been found.",
                        Status = (int)HttpStatusCode.NotFound,
                        Instance = context.Request.Path,
                        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                        Extensions =
                        {
                            ["TraceId"] = Activity.Current?.Id ?? context?.TraceIdentifier
                        }
                    };
                    context.Response.ContentType = "application/problem+json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(pdNf));
                    break;
                case DomainObjectPreconditionFailedException e:
                    context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    var pdPf = new ProblemDetails
                    {
                        Title = "Mid-Air Collision",
                        Detail = "Object has been modified since it has been read.",
                        Status = (int)HttpStatusCode.PreconditionFailed,
                        Instance = context.Request.Path,
                        Type = "https://datatracker.ietf.org/doc/html/rfc7232#section-4.2",
                        Extensions =
                        {
                            ["TraceId"] = Activity.Current?.Id ?? context?.TraceIdentifier
                        }
                    };
                    context.Response.ContentType = "application/problem+json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(pdPf));
                    break;
                case DomainObjectConflictException e:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    var pdC = new ProblemDetails
                    {
                        Title = "Conflict",
                        Detail = "Conflict in the data store occured.",
                        Status = (int)HttpStatusCode.Conflict,
                        Instance = context.Request.Path,
                        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
                        Extensions =
                        {
                            ["TraceId"] = Activity.Current?.Id ?? context?.TraceIdentifier
                        }
                    };
                    context.Response.ContentType = "application/problem+json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(pdC));
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
        }
    }
}