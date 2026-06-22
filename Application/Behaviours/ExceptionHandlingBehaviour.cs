using MediatR;
using FluentValidation;

namespace Application.Common.Behaviours;

public class ExceptionHandlingBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        try
        {
            return await next();
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .Select(e => new { e.PropertyName, e.ErrorMessage });

            throw; // let middleware handle it
        }
    }
}