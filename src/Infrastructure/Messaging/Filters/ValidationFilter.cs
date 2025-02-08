using FluentValidation;
using MassTransit;

namespace coaches.Infrastructure.Messaging.Filters;

public class ValidationFilter<T>(IEnumerable<IValidator<T>> validators)
    : IFilter<ConsumeContext<T>>
    where T : class
{
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        if (!validators.Any())
        {
            await next.Send(context);
            return;
        }

        var validationContext = new ValidationContext<T>(context.Message);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(validationContext, context.CancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("validation-filter");
    }
}
