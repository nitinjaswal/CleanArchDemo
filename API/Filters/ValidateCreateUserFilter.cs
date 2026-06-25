using Application.Features.Users.Commands.CreateUser;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

/// <summary>
/// Action filter for validating CreateUserCommand before it reaches the handler.
/// Works in conjunction with the MediatR ValidationBehaviour pipeline.
/// </summary>
public class ValidateCreateUserFilter : IActionFilter
{
    private readonly IValidator<CreateUserCommand> _validator;
    private readonly ILogger<ValidateCreateUserFilter> _logger;

    public ValidateCreateUserFilter(
        IValidator<CreateUserCommand> validator,
        ILogger<ValidateCreateUserFilter> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ActionArguments.TryGetValue("command", out var commandObj) ||
            commandObj is not CreateUserCommand command)
        {
            return;
        }

        _logger.LogInformation(
            "Validating CreateUserCommand: Name={Name}, Email={Email}",
            command.Name, command.Email);

        var validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning(
                "CreateUserCommand validation failed with {ErrorCount} errors",
                validationResult.Errors.Count);

            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            context.Result = new BadRequestObjectResult(new
            {
                errors,
                message = "Validation failed"
            });
        }
        else
        {
            _logger.LogInformation("CreateUserCommand validation passed");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Optional: Log after action execution
    }
}