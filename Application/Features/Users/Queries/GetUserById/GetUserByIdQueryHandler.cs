using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
namespace Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler
    : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly IMemoryCache _cache; // ← inject cache

    public GetUserByIdQueryHandler(IUserRepository userRepository, IMemoryCache cache)
    {
        _userRepository = userRepository;
        _cache = cache;
    }

    public async Task<UserDto?> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        // Step 1 — build unique cache key
        var cacheKey = $"user-{request.Id}";

        // Step 2 — check cache first
        if (_cache.TryGetValue(cacheKey, out UserDto? cachedUser))
        {
            Console.WriteLine($"Cache HIT — returning from cache for user {request.Id}");
            return cachedUser; // ← return immediately, no DB call
        }

        Console.WriteLine($"Cache MISS — fetching from DB for user {request.Id}");

        // Step 3 — not in cache, go to DB
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null) return null;


        var userDto = new UserDto(user.Id, user.Name, user.Email, user.CreatedAt);
        // Step 4 — store in cache with expiry
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5), // expires after 5 mins
            SlidingExpiration = TimeSpan.FromMinutes(2)                // reset timer if accessed
        };

        _cache.Set(cacheKey, userDto, cacheOptions);
        // Map entity to DTO — never expose Domain entity directly
        return new UserDto(user.Id, user.Name, user.Email, user.CreatedAt);
    }
}