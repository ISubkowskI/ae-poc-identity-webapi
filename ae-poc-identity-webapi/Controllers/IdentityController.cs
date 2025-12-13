using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Ae.Poc.Identity.Services;

namespace Ae.Poc.Identity.Controllers;

[ApiController]
[Route("api/v2/identity")]
[Produces("application/json")]
public sealed class IdentityController : ControllerBase
{
    private readonly ILogger<IdentityController> _logger;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public IdentityController(ILogger<IdentityController> logger, IIdentityService identityService, IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet(".well-known/openid-configuration")]
    public async Task<IActionResult> GetDiscoveryDocumentAsync(string resource, string rel, CancellationToken ct)
    {
        _logger.LogInformation("Start {MethodName} ...", nameof(GetDiscoveryDocumentAsync));

        //if (discoveryDoc.IsError)
        //     throw new ApplicationException(discoveryDoc.Error);
        //var tokenEndpoint = discoveryDoc.TokenEndpoint;

        //System.Security.Claims.ClaimsPrincipal cp = this.User;
        //var res = await _identityService.GetDiscoveryDocumentAsync(uri, ct);
        //if (res is null)
        //{
        //    return NotFound();
        //}
        //return Ok(res);

        //OpenID Provider Configuration Response

        return Ok("ToDo");
    }

    //RequestClientCredentialsTokenAsync

}
