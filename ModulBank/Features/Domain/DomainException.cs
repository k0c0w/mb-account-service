namespace ModulBank.Features.Domain;

public class DomainException : Exception
{
    public DomainExceptionType Type { get; protected init; }
    
    public enum DomainExceptionType
    {
        ValidationError = 1,
        SystemError = 2
    }

    protected DomainException(string message, DomainExceptionType type, Exception innerException)
    :base(message, innerException)
    {
        Type = type;
    }
    
    public static DomainException CreateValidationException(string message, Exception innerException)
    {
        return new DomainException(message, DomainExceptionType.ValidationError, innerException);
    }
    
    public static DomainException CreateSystemException(string message, Exception innerException)
    {
        return new DomainException(message, DomainExceptionType.SystemError, innerException);
    }
}