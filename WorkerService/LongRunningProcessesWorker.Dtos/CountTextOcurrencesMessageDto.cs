using System;

namespace LongRunningProcessesWorker.Dtos;

public record CountTextOcurrencesMessageDto(string ProcessId, string Text, string ConnectionId);
