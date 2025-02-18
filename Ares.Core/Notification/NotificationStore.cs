using System.Collections;
using Ares.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.Notification;

class NotificationStore : INotificationStore
{
  readonly IDbContextFactory<CoreDatabaseContext> _dbContextFactory;
  public NotificationStore(IDbContextFactory<CoreDatabaseContext> dbContextFactory)
  {
    _dbContextFactory = dbContextFactory;
  }

  public async Task<AresNotification[]> GetAllNotifications()
  {
    using var context = _dbContextFactory.CreateDbContext();
    var notifications = await context.Notifications.ToArrayAsync();
    return notifications;
  }

  public IEnumerator<AresNotification> GetEnumerator()
  {
    throw new NotImplementedException();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
