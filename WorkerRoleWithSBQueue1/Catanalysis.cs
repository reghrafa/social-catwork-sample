using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRoleWithSBQueue1
{
    public class Catanalysis : TableEntity
    {
        public Catanalysis(string url)
        {
            this.PartitionKey = "catalysis";
            this.RowKey = url;
        }

        public Catanalysis() { }

        public bool IsCat { get; set; }
    }
}
