namespace YAEC.Shared.Mediator.Abstractions;

public interface IRequest;

public interface IRequest<out TResponse> : IRequest;