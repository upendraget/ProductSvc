using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;

namespace ProductAPI;

public class Processor(ILogger logger)
{
    private readonly ILogger _logger = logger;

    [Function("OrderSagaOrchestrator")]
    public async Task Run(
    [ServiceBusTrigger("orders-topic", "saga-orchestrator", Connection = "ServiceBusConnection")]
    string orderId)
    {
        _logger.LogInformation($"Saga started for Order {orderId}");

        try
        {
            // Step 1: Payment
          //  await _paymentSender.SendMessageAsync(new ServiceBusMessage(orderId));

            // Step 2: Inventory
        //    await _inventorySender.SendMessageAsync(new ServiceBusMessage(orderId));

            _logger.LogInformation($"Saga completed successfully for Order {orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Saga failed for Order {orderId}: {ex.Message}");

            // Rollback logic: publish compensation events
        //    await _paymentSender.SendMessageAsync(new ServiceBusMessage($"ROLLBACK:{orderId}"));
        //    await _inventorySender.SendMessageAsync(new ServiceBusMessage($"ROLLBACK:{orderId}"));
        }
    }
}
