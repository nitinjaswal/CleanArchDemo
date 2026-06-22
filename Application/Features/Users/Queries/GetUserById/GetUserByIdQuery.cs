using MediatR;

namespace Application.Features.Users.Queries.GetUserById;

// Query = READ only, never modifies data
public record GetUserByIdQuery(int Id) : IRequest<UserDto?>;

public record UserDto(int Id, string Name, string Email, DateTime CreatedAt);