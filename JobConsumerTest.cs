using MassTransit;
using MassTransit.Contracts.JobService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MassTransitScopeFilterTest;

public class JobConsumerTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public JobConsumerTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task JobConsumer_Test()
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder();
        hostBuilder.ConfigureServices(services =>
        {
            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseDelayedMessageScheduler();

                    cfg.UseMessageScope(context);
                    cfg.UseSendFilter(typeof(MySendFilter<>), context);

                    cfg.ConfigureEndpoints(context);
                });

                x.AddConsumer<TestJobConsumer, TestJobConsumerDefinition>();
                x.AddDelayedMessageScheduler();
                x.SetJobConsumerOptions();
                x.SetInMemorySagaRepositoryProvider();
                x.AddJobSagaStateMachines();
            })
            .AddScoped<TestService>()
            .AddSingleton<TestState>();
        });
        hostBuilder.ConfigureLogging(l =>
        {
            l.ClearProviders();
            l.AddXUnit(_testOutputHelper, o => o.IncludeScopes = true);
            l.SetMinimumLevel(LogLevel.Trace);
        });
        using (IHost host = hostBuilder.Build())
        {
            host.RunAsync();

            // arrange
            var bus = host.Services.GetRequiredService<IBus>();

            // act                
            ISendEndpoint sendEndpoint = await bus.GetSendEndpoint(new Uri("queue:TestJob"));
            await sendEndpoint.Send<SubmitJob<TestJobRequest>>(new
            {
                JobId = Guid.NewGuid(),
                Job = new
                {
                    JobName = "MyTestJob"
                }
            });

            // assert
            await Task.Delay(2000);
            var state = host.Services.GetRequiredService<TestState>();
            Assert.True(state.IsOperationCompleted);
        }
    }
}