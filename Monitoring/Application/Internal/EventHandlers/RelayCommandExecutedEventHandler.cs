using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Events;
using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Processing.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Application.Internal.EventHandler;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using MediatR;

namespace Hampcoders.Electrolink.API.Monitoring.Application.Internal.EventHandlers;

public class RelayCommandExecutedEventHandler(
    IMediator mediator,
    ILogger<RelayCommandExecutedEventHandler> logger)
    : IEventHandler<RelayCommandExecutedEvent>
{
    public async Task Handle(RelayCommandExecutedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Monitoring] Relay command executed: Device={DeviceId}, State={State}, By={RequestedBy}",
            @event.DeviceId, @event.FinalRelayState, @event.RequestedBy);

        var circuitEvent = new CircuitToggledRemotelyEvent(
            ExecutionId: ServiceExecutionId.NewId(),
            DeviceId: DeviceId.From(@event.DeviceId),
            PropertyId: PropertyId.From(@event.PropertyId),
            TargetState: Enum.Parse<ERelayState>(@event.FinalRelayState, ignoreCase: true),
            TechnicianId: @event.RequestedBy,
            ToggledAt: @event.ExecutedAt);

        await mediator.Publish(circuitEvent, cancellationToken);

        logger.LogInformation(
            "[Monitoring] CircuitToggledRemotely published for Device={DeviceId}",
            @event.DeviceId);
    }
}
