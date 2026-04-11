namespace ProductAPI;
using Azure.Messaging.ServiceBus;

public class ShippingService
{
    // Connection string to your Service Bus namespace
    const string ServiceBusConnection = "get Azure connectionString defined in appSettings.json";
    const string topicName = "orders-topic"; // Topic name
    const string subscriptionName = "shipping-subscription"; // Subscription name

    //Producer
    public async Task ReceiveMessageAsync()
    {
        var client = new ServiceBusClient(ServiceBusConnection);
        ServiceBusSender sender = client.CreateSender(topicName);

        // Create processor for subscription to continuously listen and process.
        var processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false, // Manual control
            MaxConcurrentCalls = 2
        });

        // Define message handler
        processor.ProcessMessageAsync += async args =>
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"ShippingService received OrderCreated for {body}");

            // Billing logic here (e.g., process order)




            await args.CompleteMessageAsync(args.Message); // After successful processing remove the message.
        };

        // Define error handler
        processor.ProcessErrorAsync += async args =>
        {
            Console.WriteLine($"Error: {args.Exception.Message}");
        };

        // Start processing
        await processor.StartProcessingAsync();

        // Stop processing
        await processor.StopProcessingAsync();
    }
}
