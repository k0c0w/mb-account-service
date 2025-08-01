namespace AccountService.Features;

public static class MbResultWithError<TError>
{
    public static MbResult<TError> Fail(params TError[] err)
    {
        return new MbResult<TError>(err);
    }
    
    public static MbResult<TError> Ok()
    {
        return new MbResult<TError>();
    }
}