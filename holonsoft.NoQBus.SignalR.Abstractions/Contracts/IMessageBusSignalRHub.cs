﻿using holonsoft.NoQBus.Abstractions.Models;

namespace holonsoft.NoQBus.SignalR.Abstractions.Contracts
{
  public interface IMessageBusSignalRHub
  {
    public Task<SinkTransportDataResponse> ProcessMessage(SinkTransportDataRequest request);
    public Task<bool> ReceiveResponse(SinkTransportDataResponse response);
  }
}
