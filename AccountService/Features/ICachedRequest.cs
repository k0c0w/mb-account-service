
namespace AccountService.Features;

public interface ICachedRequest
{
    string CacheKey { get; }
    TimeSpan? AbsoluteExpirationRelativeToNow { get; }
}