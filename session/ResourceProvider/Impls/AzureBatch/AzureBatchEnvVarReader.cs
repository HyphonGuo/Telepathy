﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Telepathy.ResourceProvider.Impls.AzureBatch
{
    using System;

    public static class AzureBatchEnvVarReader
    {
        public static string GetJobId()
        {
            return Environment.GetEnvironmentVariable("AZ_BATCH_JOB_ID");
        }
    }
}