using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MassTransitScopeFilterTest;

public class MySendFilter<T> : IFilter<SendContext<T>> where T : class
{
    private readonly ILogger<MySendFilter<T>> _logger;

    public MySendFilter(ILogger<MySendFilter<T>> logger)
    {
        _logger = logger;
    }

    public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        _logger.LogInformation("Before sending message...");
        await next.Send(context);
    }

    public void Probe(ProbeContext context) { }
}