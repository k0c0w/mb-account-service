namespace AccountService.Domain;

public class DomainException : Exception
{
    public DomainExceptionType Type { get; }
    
    public enum DomainExceptionType
    {
        ValidationError = 1,
        ExistenceError,
        ConcurrencyError
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
    
    public static DomainException CreateConcurrencyException(string message, Exception? innerException = null)
    {
        return new DomainException(message, DomainExceptionType.ConcurrencyError, innerException ?? new InvalidOperationException());
    }
}