using System.Linq;
using Ares.Core.Messages;
using Microsoft.AspNetCore.Authorization;

namespace AresCoreServices;

public class AuthorizeRoles : AuthorizeAttribute
{
  public AuthorizeRoles(params AresUserType[] roles)
  {
    Roles = roles.Select(type => type.ToString()).Aggregate((type1, type2) => $"{type1}, {type2}");
  }
}
