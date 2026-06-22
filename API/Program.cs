using Application;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var error = context.Features.Get<IExceptionHandlerFeature>();

if (error?.Error is ValidationException validationException)
{
    // Validation error → return 400
    context.Response.StatusCode = 400;
    context.Response.ContentType = "application/json";

    var errors = validationException.Errors
        .Select(e => new
        {
            field = e.PropertyName,
            message = e.ErrorMessage
        });

    await context.Response.WriteAsJsonAsync(new { errors });
}
else
{
    // Any other error → return 500
    context.Response.StatusCode = 500;
    context.Response.ContentType = "application/json";

    await context.Response.WriteAsJsonAsync(new
    {
        error = "An unexpected error occurred"
    });
}
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();