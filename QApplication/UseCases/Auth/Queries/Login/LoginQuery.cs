using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Auth.Queries.Login;

public record LoginQuery( string EmailAddress, string Password): IRequest<AuthResponse>;