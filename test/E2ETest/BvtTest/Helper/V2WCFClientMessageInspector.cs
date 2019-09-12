﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.Xml;
using AITestLib.Helper.Trace;

namespace AITestLib.Helper
{
    public class V2WCFClientMessageInspector : IClientMessageInspector
    {
        private int sessionId;

        public V2WCFClientMessageInspector(int sessionId)
        {
            this.sessionId = sessionId;
        }

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            Guid id;
            reply.Headers.RelatesTo.TryGetGuid(out id);
            TraceLogger.LogResponseRecived(sessionId, id.ToString());
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
        {
            Guid id = Guid.NewGuid();
            request.Headers.MessageId = new UniqueId(id);
            TraceLogger.LogSendRequest(sessionId, id.ToString());
            return null;
        }
    }
}