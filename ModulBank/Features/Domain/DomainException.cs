namespace ModulBank.Features.Domain;

public class DomainException : Exception
{
    public DomainExceptionType Type { get; }
    
    public enum DomainExceptionType
    {
        ValidationError = 1,
        ExistenceError,
    }

    private DomainException(string message, DomainExceptionType type, Exception innerException)
    :base(message, innerException)
    {
        Type = type;
    }
    
    public static DomainException CreateValidationException(string message, Exception innerException)
    {
        return new DomainException(message, DomainExceptionType.ValidationError, innerException);
    }
    
    public static DomainException CreateExistenceException(string message)
    {
        return new DomainException(message, DomainExceptionType.ExistenceError, new InvalidOperationException());
    }
}