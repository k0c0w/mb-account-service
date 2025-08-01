namespace AccountService.Validation;

public sealed record PropertyValidationError
{
    public required string Property { get; init; }
    
    public required string Error { get; init; }
}