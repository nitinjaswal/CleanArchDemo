namespace Application.Common.Interfaces;

// Marker interface — any Query that implements this will be cached automatically
// Commands should NEVER implement this — only Queries
public interface ICacheable
{
    string CacheKey { get; }
    TimeSpan? AbsoluteExpiration { get; }
    TimeSpan? SlidingExpiration { get; }
}