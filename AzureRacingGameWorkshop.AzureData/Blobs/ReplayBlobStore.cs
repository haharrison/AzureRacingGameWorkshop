using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Diagnostics;
using System.IO;

namespace AzureRacingGameWorkshop.AzureData.Blobs
{
    public class ReplayBlobStore
    {
        private CloudBlobContainer m_BlobContainer;

        public ReplayBlobStore()
        {
            Debug.WriteLine(">>ReplayBlobStore()");


            CloudStorageAccount account;
            StorageCredentials creds = new StorageCredentials
                (Settings.StorageAccountName, Settings.StorageAccountKey);
            account = new CloudStorageAccount(creds, true);

            CloudBlobClient blobClient = account.CreateCloudBlobClient();

            m_BlobContainer = blobClient.GetContainerReference
                (Settings.ReplayBlobContainer);

            m_BlobContainer.CreateIfNotExists();

        }

        public void UploadStream(string blobName, MemoryStream stream)
        {
            Debug.WriteLine(">>ReplayBlobStore.UploadStream");

            try
            {
                CloudBlockBlob blob = m_BlobContainer.GetBlockBlobReference(blobName);
                stream.Position = 0;
                blob.UploadFromStream(stream);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ReplayBlobStore.UploadStream: " + ex.Message);
            }
        }

        public Stream DownloadStream(string blobName)
        {
            Debug.WriteLine(">>ReplayBlobStore.DownloadStream");
            CloudBlockBlob blob =
            m_BlobContainer.GetBlockBlobReference(blobName);
            MemoryStream stream = new MemoryStream();
            blob.DownloadToStream(stream);
            stream.Position = 0;
            return stream;

        }
    }
}
