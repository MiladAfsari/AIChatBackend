using Application.Service.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class TokenValidationFilter : IAuthorizationFilter
{
    private readonly ITokenService _tokenService;

    public TokenValidationFilter(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        // Check if the action or controller has the [AllowAnonymous] attribute
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .Any(em => em.GetType() == typeof(AllowAnonymousAttribute));

        if (allowAnonymous)
        {
            return;
        }

        var token = await _tokenService.GetTokenFromRequestAsync();
        if (string.IsNullOrEmpty(token) || ! await _tokenService.IsTokenValidAsync(token))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}