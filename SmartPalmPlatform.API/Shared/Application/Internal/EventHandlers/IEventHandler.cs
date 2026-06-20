using MediatR;
using SmartPalmPlatform.API.Shared.Domain.Model.Events;

namespace SmartPalmPlatform.API.Shared.Application.Internal.EventHandlers;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IEvent { }

