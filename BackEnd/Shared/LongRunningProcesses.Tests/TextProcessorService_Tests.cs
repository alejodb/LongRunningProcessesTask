using Moq;
using LongRunningProcesses.Application.Services;
using LongRunningProcesses.Dtos.Apis;
using Microsoft.Extensions.Logging;
using LongRunningProcesses.Dtos.Queues;

namespace LongRunningProcesses.Tests;

public class TextProcessorService_Tests
{
    [Fact]
    public async Task IsMessagePublished_WhenCalled_CountTextOcurrences()
    {
        var messagePublisherMock = new MessagesPublisherMock();
        var textProcessorServiceLogger = new Mock<ILogger<TextProcessorService>>();
        var textProcessorService = new TextProcessorService(textProcessorServiceLogger.Object, messagePublisherMock);
        var countTextOcurrencesRequestDto = new CountTextOcurrencesRequestDto("Hello World!", "Connection1");

        var countTextOcurrencesResponseDto = await textProcessorService.CountTextOcurrences(countTextOcurrencesRequestDto);

        Assert.NotNull(countTextOcurrencesResponseDto);
        Assert.IsType<CountTextOcurrencesResponseDto>(countTextOcurrencesResponseDto);

        Assert.Single(messagePublisherMock.PublishedMessages);
        Assert.IsType<CountTextOcurrencesMessageDto>(messagePublisherMock.PublishedMessages.First());
        Assert.Equal(countTextOcurrencesRequestDto.ConnectionId, ((CountTextOcurrencesMessageDto)messagePublisherMock.PublishedMessages.First()).ConnectionId);
        Assert.Equal(countTextOcurrencesRequestDto.Text, ((CountTextOcurrencesMessageDto)messagePublisherMock.PublishedMessages.First()).Text);
        Assert.Equal(countTextOcurrencesResponseDto.ProcessId, ((CountTextOcurrencesMessageDto)messagePublisherMock.PublishedMessages.First()).ProcessId);
    }

    [Fact]
    public async Task IsMessagePublished_WhenCalled_CancelCountTextOcurrencesProcess()
    {
        var messagePublisherMock = new MessagesPublisherMock();
        var textProcessorServiceLogger = new Mock<ILogger<TextProcessorService>>();
        var textProcessorService = new TextProcessorService(textProcessorServiceLogger.Object, messagePublisherMock);
        var processId = Guid.NewGuid().ToString("N");

        await textProcessorService.CancelCountTextOcurrencesProcess(processId);

        Assert.Single(messagePublisherMock.PublishedMessages);
        Assert.IsType<CancelProcessMessageDto>(messagePublisherMock.PublishedMessages.First());
        Assert.Equal(processId, ((CancelProcessMessageDto)messagePublisherMock.PublishedMessages.First()).ProcessId);
    }
}
