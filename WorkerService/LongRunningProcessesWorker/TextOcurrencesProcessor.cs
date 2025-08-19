using System;
using System.Text;
using LongRunningProcesses.Dtos.Queues;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Caching.Distributed;

namespace LongRunningProcessesWorker;

public class TextOcurrencesProcessor(ILogger<TextOcurrencesProcessor> logger, IDistributedCache redisCache, IConfiguration configuration)
{
  private readonly HubConnection hubConnection = new HubConnectionBuilder()
        .WithUrl($"{configuration["services:apiservice:http:0"]}/chatHub")
        .Build();

  public async Task ProcessMessage(CountTextOcurrencesMessageDto message)
  {
    await redisCache.SetStringAsync($"process:status:{message.ProcessId}", "processing");
    await hubConnection.StartAsync();

    try
    {
      await GenerateAndSendSignedTextOcurrences(message);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, $"Error processing message for process {message.ProcessId}");
      await redisCache.SetStringAsync($"process:status:{message.ProcessId}", "failed, the process will be retried");
      throw;
    }
    finally
    {
      await hubConnection.DisposeAsync();
    }
  }

  private async Task GenerateAndSendSignedTextOcurrences(CountTextOcurrencesMessageDto message)
  {
    var signedTextOcurrences = GenerateSignedTextOcurrencesString(message.Text);
    logger.LogInformation($"Generated signed text occurrences for process {message.ProcessId}: {signedTextOcurrences}");
    var randomizer = new Random();
    var currentCharacter = await redisCache.GetStringAsync($"process:currentCharacter:{message.ProcessId}");

    if (!string.IsNullOrEmpty(currentCharacter))
    {
      logger.LogInformation($"Resuming process {message.ProcessId} from character position {currentCharacter}");
    }
    else
    {
      logger.LogInformation($"Starting new process {message.ProcessId} for text: {message.Text}");
    }

    var position = !string.IsNullOrEmpty(currentCharacter) ? int.Parse(currentCharacter) : 0;

    while (position < signedTextOcurrences.Length)
    {
      var character = signedTextOcurrences[position];
      var delay = randomizer.Next(5000);
      await Task.Delay(delay);

      // Uncomment this line for testing exceptions and retries
      /*if (position == 5)
      {
        throw new InvalidOperationException($"Simulated error at position {position} for process {message.ProcessId}");
      }*/

      var status = await redisCache.GetStringAsync($"process:status:{message.ProcessId}");
      if (status != "canceled")
      {
        position++;
        await hubConnection.InvokeAsync("SendResponseMessage", message.ConnectionId, character.ToString());
        await redisCache.SetStringAsync($"process:currentCharacter:{message.ProcessId}", $"{position}");
        logger.LogInformation($"Processed character '{character}' at position {position} for process {message.ProcessId}");
      }
      else
      {
        logger.LogInformation($"Process {message.ProcessId} was canceled.");
        await hubConnection.InvokeAsync("SendEndMessage", message.ConnectionId, $"Long-running process CANCELED successfully.");
        return;
      }
    }
    await hubConnection.InvokeAsync("SendEndMessage", message.ConnectionId, $"Long-running process COMPLETED successfully.");

    await redisCache.RemoveAsync($"process:currentCharacter:{message.ProcessId}");
    await redisCache.RemoveAsync($"process:status:{message.ProcessId}");
  }

  private string GenerateSignedTextOcurrencesString(string text)
  {
    var base64EncodedText = GenerateBase64EncodedText(text);
    var textOcurrences = GenerateTextOcurrencesString(text);
    var signedTextOcurrences = $"{textOcurrences}/{base64EncodedText}";

    return signedTextOcurrences;
  }

  private string GenerateTextOcurrencesString(string text)
  {
    return FormatCharacterCount(CountTextCharactersOcurrences(text));
  }

  private IDictionary<char, int> CountTextCharactersOcurrences(string text)
  {
    var charactersCount = new Dictionary<char, int>();

    foreach (var character in text)
    {
      if (charactersCount.ContainsKey(character))
      {
        charactersCount[character]++;
      }
      else
      {
        charactersCount[character] = 1;
      }
    }

    return charactersCount.OrderBy(keyValuePair => keyValuePair.Key)
                          .ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);
  }

  private string FormatCharacterCount(IDictionary<char, int> characterCounts)
  {
    return string.Join("", characterCounts.Select(keyValuePair => $"{keyValuePair.Key}{keyValuePair.Value}"));
  }

  private string GenerateBase64EncodedText(string text)
  {
    return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
  }
}
