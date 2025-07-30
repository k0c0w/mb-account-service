using AccountService.Features.Utils;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace AccountService.PipelineBehaviours;

public class CachingBehavior<TRequest, TResponse>(
    IMemoryCache cache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (request is not ICachedRequest cachedRequest)
        {
            return await next(ct);
        }

        var cacheKey = cachedRequest.CacheKey;
        if (cache.TryGetValue(cacheKey, out TResponse? cachedResponse))
        {
            logger.LogInformation("Cache hit for key {CacheKey}", cacheKey);
            if (cachedResponse != null) return cachedResponse;
        }

        logger.LogInformation("Cache miss for key {CacheKey}", cacheKey);
        var response = await next(ct);

        cache.Set(cacheKey, response, cachedRequest.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(5));

        return response;
    }
}
