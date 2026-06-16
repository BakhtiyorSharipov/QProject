using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.Auth.Queries.Login;

public record LoginQuery( string EmailAddress, string Password): IRequest<AuthResponse>;