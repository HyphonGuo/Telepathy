//------------------------------------------------------------------------------
// <copyright file="IBrokerLauncher.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      Interface for Broker Launcher
// </summary>
//------------------------------------------------------------------------------

namespace Microsoft.Hpc.Scheduler.Session.Internal
{
    using System.ServiceModel;
    using System;
    using Microsoft.Hpc.Scheduler.Session.Interface;

    /// <summary>
    /// Interface for Broker Launcher
    /// </summary>
    [ServiceContract(Name = "IBrokerLauncher", Namespace = "http://hpc.microsoft.com/brokerlauncher/")]
    internal interface IBrokerLauncher
    {
        /// <summary>
        /// Create a new broker
        /// </summary>
        /// <param name="info">session start info</param>
        /// <param name="sessionid">the session id which is also service job id</param>
        /// <returns>The session Info</returns>
        [OperationContract]
        [FaultContract(typeof(SessionFault), Action = SessionFault.Action)]
        BrokerInitializationResult Create(SessionStartInfoContract info, int sessionId);

        /// <summary>
        /// Async Create a new broker
        /// </summary>
        /// <param name="info">session start info</param>
        /// <param name="sessionid">the session id which is also service job id</param>
        /// <returns>The session Info</returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCreate(SessionStartInfoContract info, int sessionId, AsyncCallback callback, object state);

        /// <summary>
        /// End the async creating
        /// </summary>
        /// <param name="ar">The async result</param>
        /// <returns>the sessioninfo</returns>
        BrokerInitializationResult EndCreate(IAsyncResult ar);

        /// <summary>
        /// Pings specified broker
        /// </summary>
        /// <param name="sessionID">sessionID of broker to ping</param>
        [OperationContract]
        bool PingBroker(int sessionID);

        /// <summary>
        /// Pings specified broker
        /// </summary>
        /// <param name="sessionID">sessionID of broker to ping</param>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPingBroker(int sessionID, AsyncCallback callback, object state);

        /// <summary>
        /// Pings specified broker
        /// </summary>
        /// <param name="sessionID">sessionID of broker to ping</param>
        bool EndPingBroker(IAsyncResult result);

        /// <summary>
        /// Pings specified broker
        /// </summary>
        /// <param name="sessionID">sessionID of broker to ping</param>
        [OperationContract]
        String PingBroker2(int sessionID);

        /// <summary>
        /// Pings specified broker
        /// </summary>
        /// <param name="sessionID">sessionID of broker to ping</param>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPingBroker2(int sessionID, AsyncCallback callback, object state);

        /// <summary>
        /// Pings specified broker
        /// </summary>
        /// <param name="sessionID">sessionID of broker to ping</param>
        String EndPingBroker2(IAsyncResult result);

        /// <summary>
        /// Create a new broker which working under Durable manner.
        /// </summary>
        /// <param name="info">session start info</param>
        /// <param name="sessionid">the session id which is also service job id</param>
        /// <returns>The created session info</returns>
        [OperationContract]
        [FaultContract(typeof(SessionFault), Action = SessionFault.Action)]
        BrokerInitializationResult CreateDurable(SessionStartInfoContract info, int sessionId);

        /// <summary>
        /// Create a new broker which working under Durable manner.
        /// </summary>
        /// <param name="info">session start info</param>
        /// <param name="sessionid">the session id which is also service job id</param>
        /// <param name="callback">The async callback</param>
        /// <param name="state">the async state</param>
        /// <returns>Async result</returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCreateDurable(SessionStartInfoContract info, int sessionId, AsyncCallback callback, object state);

        /// <summary>
        /// End the async operation of CreateDurable
        /// </summary>
        /// <param name="ar">the async result</param>
        /// <returns>The session Info</returns>
        BrokerInitializationResult EndCreateDurable(IAsyncResult ar);

        /// <summary>
        /// Attach to an exisiting session
        /// </summary>
        /// <param name="sessionId">The session Identity</param>
        /// <returns>the Broker Launcher EPR</returns>
        [OperationContract]
        [FaultContract(typeof(SessionFault), Action = SessionFault.Action)]
        BrokerInitializationResult Attach(int sessionId);

        /// <summary>
        /// Attach to an exisiting session
        /// </summary>
        /// <param name="sessionId">The session Identity</param>
        /// <returns>IAsyncResult instance</returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAttach(int sessionId, AsyncCallback callback, object state);

        /// <summary>
        /// Attach to an exisiting session
        /// </summary>
        /// <param name="result">The IAsyncResult instance</param>
        /// <returns>IAsyncResult instance</returns>
        BrokerInitializationResult EndAttach(IAsyncResult result);

        /// <summary>
        /// Clean up all the resource related to this session.
        /// Finish the session Job
        /// </summary>
        /// <param name="sessionId">The session id</param>
        [OperationContract]
        [FaultContract(typeof(SessionFault), Action = SessionFault.Action)]
        void Close(int sessionId);

        /// <summary>
        /// Clean up all the resource related to this session.
        /// Finish the session Job
        /// </summary>
        /// <param name="sessionId">The session id</param>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginClose(int sessionId, AsyncCallback callback, object state);

        /// <summary>
        /// Clean up all the resource related to this session.
        /// Finish the session Job
        /// </summary>
        void EndClose(IAsyncResult result);

        /// <summary>
        /// Gets the active broker id list
        /// </summary>
        /// <returns>the list of active broker's session id</returns>
        [OperationContract]
        [FaultContract(typeof(SessionFault), Action = SessionFault.Action)]
        int[] GetActiveBrokerIdList();

        /// <summary>
        /// Gets the active broker id list
        /// </summary>
        /// <returns>the list of active broker's session id</returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetActiveBrokerIdList(AsyncCallback callback, object state);

        /// <summary>
        /// Gets the active broker id list
        /// </summary>
        /// <returns>the list of active broker's session id</returns>
        int[] EndGetActiveBrokerIdList(IAsyncResult result);
    }
}