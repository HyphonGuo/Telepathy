﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Telepathy.ServiceBroker.Persistences.AzureQueuePersist
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Telepathy.ServiceBroker.BrokerQueue;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Table;

    internal class AzureQueueResponseFetcher : AzureQueueMessageFetcher
    {
        private readonly CloudTable responseTable;

        private long lastIndex;

        public AzureQueueResponseFetcher(
            CloudTable responseTable,
            long messageCount,
            IFormatter messageFormatter,
            CloudBlobContainer blobContainer)
            : base(
                messageCount,
                messageFormatter,
                DefaultPrefetchCacheCapacity,
                Environment.ProcessorCount > DefaultMaxOutstandingFetchCount
                    ? Environment.ProcessorCount
                    : DefaultMaxOutstandingFetchCount,
                blobContainer)
        {
            this.responseTable = responseTable;
            this.prefetchTimer.Elapsed += (sender, args) =>
                {
                    Debug.WriteLine("[AzureQueueResponseFetch] .prefetchTimer raised.");
                    this.PeekMessageAsync().GetAwaiter().GetResult();
                    if (!this.isDisposedField)
                    {
                        this.prefetchTimer.Enabled = true;
                    }
                };
            this.prefetchTimer.Enabled = true;
        }

        private async Task PeekMessageAsync()
        {
            if (this.isDisposedField)
            {
                this.RevertFetchCount();
                return;
            }

            var exceptions = new List<Exception>();
            var index = 0;
            List<ResponseEntity> responseList = null;
            while (true)
            {
                Exception exception = null;
                if (this.pendingFetchCount < 1)
                {
                    break;
                }

                while (this.pendingFetchCount > 0)
                {
                    byte[] messageBody = null;
                    try
                    {
                        if (responseList == null || responseList.Count <= index)
                        {
                            responseList = await AzureStorageTool.GetBatchEntityAsync(
                                               this.responseTable,
                                               this.lastIndex);
                            index = 0;
                            BrokerTracing.TraceVerbose(
                                "[AzureQueueResponseFetch] .PeekMessageAsync: get batch entity count={0}",
                                responseList.Count);
                        }

                        if (responseList.Count > index)
                        {
                            messageBody = responseList[index].Message;
                            if (long.TryParse(responseList[index].RowKey, out var tempIndex)
                                && tempIndex > this.lastIndex)
                            {
                                this.lastIndex = tempIndex;
                            }

                            index++;
                        }
                    }
                    catch (Exception e)
                    {
                        BrokerTracing.TraceError(
                            "[AzureQueueResponseFetch] .PeekMessageAsync: peek batch messages failed, Exception:{0}",
                            e.ToString());
                        exceptions.Add(e);
                    }

                    if (messageBody != null && messageBody.Length > 0)
                    {
                        BrokerQueueItem brokerQueueItem = null;

                        // Deserialize message to BrokerQueueItem
                        try
                        {
                            brokerQueueItem = (BrokerQueueItem)this.formatter.Deserialize(
                                await AzureStorageTool.GetMsgBody(this.blobContainer, messageBody));
                            brokerQueueItem.PersistAsyncToken.AsyncToken =
                                brokerQueueItem.Message.Headers.RelatesTo.ToString();
                            BrokerTracing.TraceVerbose(
                                "[AzureQueueResponseFetch] .PeekMessage: deserialize header={0} property={1}",
                                brokerQueueItem.Message.Headers.RelatesTo,
                                brokerQueueItem.Message.Properties);
                        }
                        catch (Exception e)
                        {
                            BrokerTracing.TraceError(
                                "[AzureQueueResponseFetch] .PeekMessage: deserialize message failed, Exception:{0}",
                                e.ToString());
                            exception = e;
                            exceptions.Add(e);
                        }

                        this.HandleMessageResult(new MessageResult(brokerQueueItem, exception));
                    }

                    Interlocked.Decrement(ref this.pendingFetchCount);
                }

                this.CheckAndGetMoreMessages();
            }

            foreach (var exception in exceptions)
            {
                this.HandleMessageResult(new MessageResult(null, exception));
            }
        }
    }
}