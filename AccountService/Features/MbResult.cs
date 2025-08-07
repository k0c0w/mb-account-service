namespace AccountService.Features;

public record MbResult<TError>
{
    public TError? MbError { get; private init; }

    internal MbResult(TError error)
    {
        MbError = error;
    }

    internal MbResult() { }
}

public sealed record MbResult<TResult, TError> : MbResult<TError>
{ 
    public TResult? Data { get; private init; }

    private MbResult(TError error) : base(error) {}

    private MbResult(TResult result)
    {
        Data = result;
    }
    
    public static MbResult<TResult, TError> Fail(TError err)
    {
        return new MbResult<TResult, TError>(err);
    }
    
    public static MbResult<TResult, TError> Ok(TResult result)
    {
        return new MbResult<TResult, TError>(result);
    }
}