using MediatR;

namespace Application.Features.Users.Commands.CreateUser;

// This is the REQUEST — what data comes IN
public record CreateUserCommand(string Name, string Email)
    : IRequest<CreateUserResponse>;

// This is the RESPONSE — what data goes OUT
public record CreateUserResponse(int Id, string Name, string Email);