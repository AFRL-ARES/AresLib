using Microsoft.EntityFrameworkCore;

namespace Ares.Core;

public static class DbContextExtensions
{
  public static void UpdateProperties(this DbContext context, object target, object source)
  {
    foreach (var propertyEntry in context.Entry(target).Properties)
    {
      var property = propertyEntry.Metadata;
      // Skip shadow and key properties
      if (property.IsShadowProperty() || (propertyEntry.EntityEntry.IsKeySet && property.IsKey())) continue;

      propertyEntry.CurrentValue = property.GetGetter().GetClrValue(source);
    }
  }

  public static void UpdateNavigations(this DbContext context, object target, object source)
  {
    foreach (var navObj in context.Entry(source).Navigations)
    foreach (var navExist in context.Entry(target).Navigations)
      if (navObj.Metadata.Name == navExist.Metadata.Name)
        navExist.CurrentValue = navObj.CurrentValue;
  }
}
