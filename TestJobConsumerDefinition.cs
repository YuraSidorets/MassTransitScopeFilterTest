using MassTransit;

namespace MassTransitScopeFilterTest;

public class TestJobConsumerDefinition : ConsumerDefinition<TestJobConsumer>
{
    public TestJobConsumerDefinition()
    {
        EndpointName = "TestJob";
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TestJobConsumer> consumerConfigurator, IRegistrationContext context)
    {
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator, context);
    }
}