using System;
namespace AresLib
{
  public interface IAresDevice
  {
    Guid Id { get; }
    CommandIssueResult IssueCommand(AresCommand command);
  }
}
