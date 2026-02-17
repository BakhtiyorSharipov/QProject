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
        await cacheService.HashRemoveAsync(CacheKeys.AllQueuesHashKey, cancellationToken);
        await cacheService.HashRemoveAsync(CacheKeys.CustomerQueuesHashKey(customerId), cancellationToken);
        await cacheService.RemoveAsync(CacheKeys.QueueId(queueId), cancellationToken);
        await cacheService.RemoveAsync(CacheKeys.EmployeeId(employeeId), cancellationToken);
    }
}