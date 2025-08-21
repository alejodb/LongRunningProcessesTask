using System.Diagnostics;

namespace LongRunningProcesses.Domain;

public class ProcessState
{
  public required string ProcessId { get; set; }
  public int ProgressPosition { get; set; }
  public bool Canceled { get; set; } = false;
}
