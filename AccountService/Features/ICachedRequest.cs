using MediatR;

namespace AccountService.Features;

/// <summary>
/// Marker interface to mark commands which results should be cached
/// </summary>
public interface ICachedRequest : IBaseRequest
{
    string CacheKey { get; }
    TimeSpan? AbsoluteExpirationRelativeToNow { get; }
}