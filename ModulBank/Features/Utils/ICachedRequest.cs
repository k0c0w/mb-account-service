
namespace ModulBank.Features.Utils;

public interface ICachedRequest
{
    string CacheKey { get; }
    TimeSpan? AbsoluteExpirationRelativeToNow { get; }
}