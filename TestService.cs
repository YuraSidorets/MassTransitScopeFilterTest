using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MassTransitScopeFilterTest;

public sealed class TestService : IDisposable
{
    private readonly TestState _testState;
    private readonly ILogger<TestService> _logger;
    private bool _disposed;

    public TestService(TestState testState, ILogger<TestService> logger)
    {
        _testState = testState;
        _logger = logger;
    }

    public async Task DoSomething()
    {
        _logger.LogInformation("Start long running operation.");
        _testState.IsOperationCompleted = false;
        await Task.Delay(1000);
        if (_disposed)
        {
            throw new InvalidOperationException("Test service disposed.");
        }

        _testState.IsOperationCompleted = true;
        _logger.LogInformation("Finished long running operation.");
    }

    public void Dispose()
    {
        _disposed = true;
    }
}