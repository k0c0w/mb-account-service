using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AccountService.Tests.IntegrationTests;

public class RabbitMqIntegrationTestBase
{
    protected static async Task<T?> ConsumeEventAsync<T>(IChannel channel, string queue, TimeSpan timeout)
    where T : class
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        var tcs = new TaskCompletionSource<T?>();

        consumer.ReceivedAsync += (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            tcs.TrySetResult(message);
            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(queue, true, consumer);
        var timeoutTask = Task.Delay(timeout).ContinueWith(_ => tcs.TrySetResult(default));
        await Task.WhenAny(tcs.Task, timeoutTask);

        return tcs.Task.Result;
    }

}