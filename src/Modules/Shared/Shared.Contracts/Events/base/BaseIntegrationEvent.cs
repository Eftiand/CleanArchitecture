namespace coaches.Modules.Shared.Contracts.Events.@base;

public interface IBaseIntegrationEvent;

public record BaseIntegrationEvent<T> : BaseCommand<T>, IBaseIntegrationEvent;
