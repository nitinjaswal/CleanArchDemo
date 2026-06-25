using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(int Id)
    : IRequest<UserDto?>, ICacheable  // ← implement ICacheable
{
    // Each query defines its own cache key and expiry
    public string CacheKey => $"user-{Id}";
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromMinutes(5);
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(2);
}

public record UserDto(int Id, string Name, string Email, DateTime CreatedAt);