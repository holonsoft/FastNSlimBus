﻿using holonsoft.NoQBus.Abstractions.Contracts;
using System;

namespace holonsoft.NoQBus.Abstractions.Models
{

	public abstract record ResponseBase : MessageBase, IResponse
	{
#if NET5_0_OR_GREATER
		public Guid CorrospondingRequestMessageId { get; init; }
#else
		public Guid CorrospondingRequestMessageId { get; set; }
#endif


		public ResponseBase()
		{
		}

		public ResponseBase(IMessage cloneFromMessage) : base(cloneFromMessage)
		{
			CorrospondingRequestMessageId = cloneFromMessage.MessageId;
		}
	}

	public abstract record ResponseBase<TRequest> : ResponseBase, IResponse<TRequest> where TRequest : IRequest
	{
		public ResponseBase()
		{
		}

		public ResponseBase(TRequest cloneFromRequest) : base(cloneFromRequest)
		{
		}
	}
}
