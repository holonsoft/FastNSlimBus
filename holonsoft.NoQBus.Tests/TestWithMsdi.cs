﻿using FluentAssertions;
using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.SignalR.Client;
using holonsoft.NoQBus.SignalR.Host;
using holonsoft.NoQBus.Tests.TestDtoClasses;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace holonsoft.NoQBus.Tests;

public class TestWithMsdi
{
  [Fact]
  public async void TestSimpleLocalMessage()
  {
    ServiceCollection serviceCollection = new();
    serviceCollection.AddNoQMessageBus();
    var serviceProvider = serviceCollection.BuildServiceProvider();
    await serviceProvider.StartLocalNoQMessageBus();

    var messageBus = serviceProvider.GetRequiredService<IMessageBus>();

    const string testString = "Test4711";
    static Task<TestResponse> ReceiveTestRequest(TestRequest request) => Task.FromResult(new TestResponse(request, testString));

    await messageBus.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

    TestRequest sendRequest = new();

    var receivedResponse = await messageBus.GetResponses<TestResponse>(sendRequest);

    receivedResponse.Should().HaveCount(1);

    receivedResponse[0].CorrespondingRequestMessageId.Should().Be(sendRequest.MessageId);
    receivedResponse[0].TestString.Should().Be(testString);
  }

  [Fact]
  public async void TestMessageSendFromServerToClient()
  {
    CancellationTokenSource cts = new();
    try
    {
      ServiceCollection serviceCollectionServer = new();
      serviceCollectionServer.AddNoQMessageBus();
      serviceCollectionServer.AddNoQSignalRHost();
      await using var serviceProviderServer = serviceCollectionServer.BuildServiceProvider();

      await serviceProviderServer.StartNoQSignalRHost(cancellationToken: cts.Token);

      var messageBusServer = serviceProviderServer.GetRequiredService<IMessageBus>();

      ServiceCollection serviceCollectionClient = new();
      serviceCollectionClient.AddNoQMessageBus();
      serviceCollectionClient.AddNoQSignalRClient();
      await using var serviceProviderClient = serviceCollectionClient.BuildServiceProvider();

      await serviceProviderClient.StartNoQSignalRClient(cancellationToken: cts.Token);

      var messageBusClient = serviceProviderClient.GetRequiredService<IMessageBus>();

      const string testString = "Test4711";
      static Task<TestResponse> ReceiveTestRequest(TestRequest request) => Task.FromResult(new TestResponse(request, testString));

      await messageBusClient.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

      TestRequest sendRequest = new();

      var receivedResponse = await messageBusServer.GetResponses<TestResponse>(sendRequest);

      receivedResponse.Should().HaveCount(1);

      receivedResponse[0].CorrespondingRequestMessageId.Should().Be(sendRequest.MessageId);
      receivedResponse[0].TestString.Should().Be(testString);
    }
    finally
    {
      cts.Cancel();
    }
  }
}
