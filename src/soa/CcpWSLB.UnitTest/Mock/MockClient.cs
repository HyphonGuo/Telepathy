﻿using System;
using System.ServiceModel.Channels;

namespace Microsoft.Hpc.ServiceBroker.UnitTest.Mock
{
    using Microsoft.Telepathy.ServiceBroker.BackEnd;

    internal class MockClient : IService
    {
        public Action Action
        {
            get;
            set;
        }

        public Message Response
        {
            get;
            set;
        }

        public Message ProcessMessage(Message request)
        {
            return null;
        }

        public IAsyncResult BeginProcessMessage(Message request, AsyncCallback callback, object asyncState)
        {
            return null;
        }

        public Message EndProcessMessage(IAsyncResult ar)
        {
            if (this.Action != null)
            {
                this.Action();
            }

            return Response;
        }
    }
}
