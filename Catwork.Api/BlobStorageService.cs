using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Catwork.Api
{
    public class BlobStorageService : IBlobStorageService
    {
        private CloudStorageAccount storageAccount = new CloudStorageAccount(
       new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
       "<-- ADD ACCOUNT NAME -->",
       "<-- ADD KEY -->"), true);

        private CloudBlobClient blobClient;

        private CloudBlobContainer container;

        public BlobStorageService()
        {
            // Verbindung mit BLOB Storage wird hergestellt.
            blobClient = storageAccount.CreateCloudBlobClient();
            container = blobClient.GetContainerReference("profilepictures");
            container.CreateIfNotExistsAsync().Wait();
            container.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            }).Wait();
        }

        public async Task<string> WriteProfileImageToBlob(Stream stream, string catId)
        {
            // Neuer BLOB wird angelegt
            var blockBlob = container.GetBlockBlobReference(catId);

            // BLOB wird geschrieben
            await blockBlob.UploadFromStreamAsync(stream);

            // URL wird zurückgegeben
            return blockBlob.Uri.ToString();
        }
    }
}
