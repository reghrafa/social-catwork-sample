using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure;

namespace WorkerRoleWithSBQueue1
{
    public class TableStorageService
    {
        private CloudStorageAccount storageAccount = new CloudStorageAccount(
       new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
       "<-- ADD ACCOUNT NAME -->",
       "<-- ADD KEY -->"), true);

        CloudTableClient tableClient;
        CloudTable classifierTable;

        public TableStorageService()
        {
            this.tableClient = storageAccount.CreateCloudTableClient();
            this.classifierTable = tableClient.GetTableReference("classifierTable");

            classifierTable.CreateIfNotExists();
        }

        public void InsertResult(string url, bool isCat)
        {
            var catalyse = new Catanalysis(url.Split('/').Last())
            {
                IsCat = isCat
            };

            TableOperation insertOperation = TableOperation.Insert(catalyse);

            try
            {
                classifierTable.Execute(insertOperation);
            }
            catch (Exception ex)
            {
                var x = 0;
            }
            classifierTable.Execute(insertOperation);
        }
    }
}
