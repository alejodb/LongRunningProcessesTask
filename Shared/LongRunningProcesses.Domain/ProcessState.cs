using System.Diagnostics;

namespace LongRunningProcesses.Domain;

public enum ProcessStatus
{
  Pending,
  Processing,
  Canceled,
  Failed
}

public class ProcessState
{
  public required string ProcessId { get; set; }
  public required ProcessStatus Status { get; set; }
  public int ProgressPosition { get; set; }
}
