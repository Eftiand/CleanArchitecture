namespace coaches.Modules.Shared.Domain.BaseEntities.Base;

public interface IBaseIntegrationEvent;

public record BaseIntegrationEvent<T> : BaseEvent, IBaseIntegrationEvent;
