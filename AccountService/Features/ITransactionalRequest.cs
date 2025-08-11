using MediatR;

namespace AccountService.Features;

/// <summary>
/// Marker interface to mark commands which must be run in transaction
/// </summary>
public interface ITransactionalRequest : IBaseRequest;
