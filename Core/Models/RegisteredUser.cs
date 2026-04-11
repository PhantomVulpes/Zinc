using Vulpes.Electrum.Domain.Models;
using Vulpes.Electrum.Domain.Validation;
using Vulpes.Zinc.Core.Logging;
using Vulpes.Zinc.Core.Security;

namespace Vulpes.Zinc.Core.Models;

public record RegisteredUser : AggregateRoot
{
    public static RegisteredUser Empty => new();
    public static RegisteredUser Default => Empty with
    {
        Key = Guid.NewGuid(),
        Role = Role.Basic,
        CreationDate = DateTimeOffset.UtcNow,
    };

    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public DateTimeOffset CreationDate { get; init; } = DateTimeOffset.MinValue;
    public DateTimeOffset LastLoginDate { get; init; } = DateTimeOffset.MinValue;

    public Role Role { get; init; } = Role.Unknown;

    public AggregateRootValidationModel<RegisteredUser> Validate()
    {
        var validationBuilder = new ValidationBuilder()
            .InvalidIf(() => Key == Guid.Empty, () => new(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Key)} is required"))
            .InvalidIf(() => string.IsNullOrEmpty(FirstName), () => new(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(FirstName)} is required"))
            .InvalidIf(() => string.IsNullOrEmpty(LastName), () => new(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(LastName)} is required"))
            .InvalidIf(() => string.IsNullOrEmpty(Username), () => new(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Username)} is required"))
            .InvalidIf(() => string.IsNullOrEmpty(PasswordHash), () => new(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(PasswordHash)} is required"))
            .InvalidIf(() => Role == Role.Unknown, () => new(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Role)} is required"))
            ;

        return new(this, validationBuilder);
    }

    public SaveModel<RegisteredUser> PrepareForSave()
    {
        var validatedObject = (this with
        {
            // Set any values that should be changed like last modified date or something.
            EditingToken = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        }).Validate();

        return new(validatedObject, EditingToken);
    }

    public InsertModel<RegisteredUser> PrepareForInsert()
    {
        var validatedObject = (this with
        {
            EditingToken = DateTime.UtcNow.ToLongDateString()
        }).Validate();

        return new(validatedObject);
    }

    public override string ToLogName() => $"{FirstName} {LastName} ({Username}) ({Key})";

}