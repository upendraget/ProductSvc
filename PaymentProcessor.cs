using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;

namespace ProductAPI;

public class PaymentProcessor(ILogger logger)
{
    private readonly ILogger _logger = logger;
    
    // Integrate with Azure Service Bus for microservice messaging.
    // Azure Functions runtime activates the function.
    // Service Bus Trigger Function (Payment Processing)
    // This function gets triggered automatically, when a message arrives in the mentioned topic under the subscription,
    //  because of the ServiceBusTrigger attribute. The message content is passed as a parameter to the function. 
    [Function("PaymentProcessor")]
    public async Task Run(
    [ServiceBusTrigger("payment-topic", "payment-service", Connection = "ServiceBusConnection")]
    string message)
    {
        if (message.StartsWith("ROLLBACK"))
        {
            _logger.LogInformation($"Rolling back payment for {message}");
            return;
        }

        _logger.LogInformation($"Processing payment for Order {message}");
        // Simulate payment logic
        await Task.Delay(300);
    }
}
