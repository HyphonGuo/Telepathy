﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Telepathy.ClientAPI
{
    public class SessionStartInfo
    {
        public string ServiceName => serviceName;

        private string serviceName;

        public Version ServiceVersion => serviceVersion;

        private Version serviceVersion;

        public bool SharedSession { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int SessionIdleTimeout { get; set; }

        public int ClientConnectTimeout { get; set; }

        public int ClientIdleTimeout { get; set; }

        public int MaxServiceNum { get; set; }

        public string TelepathyAddress => telepathyAddress;

        private string telepathyAddress;

        public SessionStartInfo(string address, string serviceName) : this(address, serviceName, null)
        {
        }

        public SessionStartInfo(string address, string serviceName, Version serviceVersion)
        {
            telepathyAddress = address;
            this.serviceName = serviceName;
            if (serviceVersion != Constant.NoServiceVersion) this.serviceVersion = serviceVersion;
        }
    }
}