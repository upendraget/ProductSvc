using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;

namespace ProductAPI;

public class InventoryProcessor(ILogger logger)
{
    private readonly ILogger _logger = logger;


    [Function("InventoryProcessor")]
    public async Task Run(
    [ServiceBusTrigger("inventory-topic", "inventory-service", Connection = "ServiceBusConnection")]
    string message)
    {
        if (message.StartsWith("ROLLBACK"))
        {
            _logger.LogInformation($"Rolling back inventory for {message}");
            return;
        }

        _logger.LogInformation($"Reserving inventory for Order {message}");

        // Simulate inventory logic
        await Task.Delay(300);
    }
}
