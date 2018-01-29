using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WorkerRoleWithSBQueue1
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        const string QueueName = "classifier-queue";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient Client;
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            Client.OnMessage((receivedMessage) =>
                {
                    try
                    {
                        // Process the message
                        Trace.WriteLine("Processing Service Bus message: " + receivedMessage.SequenceNumber.ToString());
                        var stream = receivedMessage.GetBody<Stream>();
                        var bytes = ReadFully(stream);
                        var uri = Encoding.UTF8.GetString(bytes);

                        AnalysisResult analysisResult;
                        var features = new VisualFeature[] { VisualFeature.Tags, VisualFeature.Description };
                        var visionClient = new VisionServiceClient("<-- ADD API KEY -->", "https://southcentralus.api.cognitive.microsoft.com/vision/v1.0"); 
                        analysisResult = visionClient.AnalyzeImageAsync(uri, features).Result; //

                        // Speak a string.

                        bool isCat = false;

                        foreach (var caption in analysisResult.Description.Captions)
                        {
                            if (caption.Text.Contains("cat"))
                            {
                                isCat = true;
                            }
                        }

                        foreach (var tag in analysisResult.Description.Tags)
                        {
                            if (tag.Contains("cat"))
                            {
                                isCat = true;
                            }
                        }


                        var tableStorageService = new TableStorageService();
                        tableStorageService.InsertResult(uri, isCat);
                        Console.WriteLine("Is Cat:     " + isCat.ToString());
                        Console.Read();
                    }
                    catch
                    {
                        // Handle any message processing specific exceptions here
                    }
                });

            CompletedEvent.WaitOne();
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }

            // Initialize the connection to Service Bus Queue
            Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            Client.Close();
            CompletedEvent.Set();
            base.OnStop();
        }
    }
}
