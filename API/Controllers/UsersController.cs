using API.Filters;
using Application.Features.Users.Commands.CreateUser;
using Application.Features.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    // Controller only knows about MediatR — nothing else!
    // No repository, no DbContext, no business logic here
    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST api/users
    [HttpPost]
    [ServiceFilter(typeof(ValidateCreateUserFilter))]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // GET api/users/1
    [HttpGet("{id}")]
    
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetUserByIdQuery(id), cancellationToken);

        if (result is null) return NotFound();

        return Ok(result);
    }
}