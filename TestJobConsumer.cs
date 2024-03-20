using MassTransit;
using System;
using System.Threading.Tasks;

namespace MassTransitScopeFilterTest;

public class TestJobConsumer : IJobConsumer<TestJobRequest>
{
    private readonly TestService _testService;

    public TestJobConsumer(TestService testService)
    {
        _testService = testService;
    }

    /// <inheritdoc />
    public async Task Run(JobContext<TestJobRequest> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        await _testService.DoSomething();
    }
}