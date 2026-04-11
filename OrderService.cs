namespace ProductAPI;
using Azure.Messaging.ServiceBus;

public class OrderService
{
    const string ServiceBusConnection = "Get Azure connectionString defined in appSettings.json";
    const string topicName = "orders-topic";

    //Producer
    public async Task SendMessageAsync()
    {
        var client = new ServiceBusClient(ServiceBusConnection);
        ServiceBusSender sender = client.CreateSender(topicName); // publish to topic

        // Example messages
        var messages = new[]
        {
            new ServiceBusMessage("Order #1001 created"),
            new ServiceBusMessage("Order #1002 created"),
            new ServiceBusMessage("Order #1003 created")
        };

        // Send messages
        foreach (var message in messages) await sender.SendMessageAsync(message);
    }
    public async Task ProcessDLQMessages()
    {
        var client = new ServiceBusClient(ServiceBusConnection);

        /* DLQ is a sub - path under each queue or subscription:
         If a message is delivered too many times (default is 10 attempts), Service Bus moves it to the DLQ.
        Explicitly Dead‑Lettered by Your Application.
         If the message stays in queue longer than its TTL, it goes to DLQ.
          <queue-name>/$DeadLetterQueue => orders-queue/$DeadLetterQueue */

        var receiver = client.CreateReceiver(topicName, new ServiceBusReceiverOptions
        {
            SubQueue = SubQueue.DeadLetter
        });

        ServiceBusReceivedMessage msg = await receiver.ReceiveMessageAsync();

        Console.WriteLine("Dead-lettered because: " + msg.DeadLetterReason);
        Console.WriteLine("Error description: " + msg.DeadLetterErrorDescription);
    }
}
