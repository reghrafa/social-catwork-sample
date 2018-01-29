using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.ServiceBus;

namespace Catwork.Api
{
    public class ServiceBusService : IServiceBusService
    {
        const string ServiceBusConnectionString = "<-- ADD CONNECTION STRING -->";
        const string QueueName = "classifier-queue";
        static IQueueClient queueClient;

        public ServiceBusService()
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
        }

        public async Task SendMessageAsync(string message)
        {
            var sbMessage = new Message(Encoding.UTF8.GetBytes(message));
            await queueClient.SendAsync(sbMessage);
        }

        public async void Dispose()
        {
            await queueClient.CloseAsync();
        }
    }
}
