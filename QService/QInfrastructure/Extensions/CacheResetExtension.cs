using QApplication.Caching;

namespace QInfrastructure.Extensions;

public static class CacheResetExtensions
{
    public static async Task ResetCacheAsync(
        this ICacheService cacheService,
        int queueId,
        int customerId,
        int employeeId,
        CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.HashRemoveAsync(CacheKeys.AllQueuesHashKey, cancellationToken),
            cacheService.HashRemoveAsync(CacheKeys.CustomerQueuesHashKey(customerId), cancellationToken),
            cacheService.RemoveAsync(CacheKeys.QueueId(queueId), cancellationToken),
            cacheService.RemoveAsync(CacheKeys.EmployeeId(employeeId), cancellationToken));
    }
}

