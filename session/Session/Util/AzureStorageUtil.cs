﻿namespace Microsoft.Telepathy.Session.Util
{
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using System;

    // TODO: refactor me into shared project
    public static class AzureStorageUtil
    {
        /// <summary>
        /// Constructs a container shared access signature.
        /// </summary>
        /// <param name="cloudStorageAccount">The cloud storage account.</param>
        /// <param name="containerName">The container name to construct a SAS for.</param>
        /// <param name="permissions">The permissions to generate the SAS with.</param>
        /// <returns>The container URL with the SAS and specified permissions.</returns>
        public static string ConstructContainerSas(CloudStorageAccount cloudStorageAccount, string containerName, SharedAccessBlobPermissions permissions = SharedAccessBlobPermissions.Read)
        {
            // Lowercase the container name because containers must always be all lower case
            containerName = containerName.ToLower();

            CloudBlobClient client = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference(containerName);

            DateTimeOffset sasStartTime = DateTime.UtcNow;
            TimeSpan sasDuration = TimeSpan.FromHours(2);
            DateTimeOffset sasEndTime = sasStartTime.Add(sasDuration);

            SharedAccessBlobPolicy sasPolicy = new SharedAccessBlobPolicy() { Permissions = permissions, SharedAccessExpiryTime = sasEndTime };

            string sasString = container.GetSharedAccessSignature(sasPolicy);
            return $"{container.Uri}{sasString}";
        }
    }
}
