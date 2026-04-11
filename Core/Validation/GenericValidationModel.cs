using Vulpes.Electrum.Domain.Validation;

namespace Vulpes.Zinc.Core.Validation;

public record GenericValidationModel<TSubject> : IValidationModel<TSubject>
{
    public TSubject Value { get; init; }
    public ValidationBuilder ValidationBuilder { get; init; }

    public GenericValidationModel(TSubject value, ValidationBuilder validationBuilder)
    {
        Value = value;
        ValidationBuilder = validationBuilder;
    }
}
