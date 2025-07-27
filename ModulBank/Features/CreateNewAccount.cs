namespace ModulBank.Features;

public class CreateNewAccount
{
    public void RegisterHttpEndpoint(IEndpointRouteBuilder b)
    {
        b.MapPost("/accounts", () => throw new NotImplementedException())
            .WithDescription("Creates new account for specific user.")
            .Produces(201)
            .Produces(400)
            .Produces(500);
    }
}