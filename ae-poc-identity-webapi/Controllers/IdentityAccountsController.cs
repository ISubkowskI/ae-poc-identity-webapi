using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Ae.Poc.Identity.Data;
using Ae.Poc.Identity.Dtos;
using Ae.Poc.Identity.Services;

namespace Ae.Poc.Identity.Controllers;

[ApiController]
[Route("api/v2/accounts")]
[Produces("application/json")]
public class IdentityAccountsController(ILogger<IdentityAccountsController> logger, IIdentityAccountsService accountsService, IMapper mapper) : ControllerBase
{
    private readonly ILogger<IdentityAccountsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IIdentityAccountsService _accountsService = accountsService ?? throw new ArgumentNullException(nameof(accountsService));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    [HttpGet("")]
    public async Task<IActionResult> GetAccountsAsync(CancellationToken ct)
    {
        _logger.LogInformation("Start {MethodName} ...", nameof(GetAccountsAsync));
        //System.Security.Claims.ClaimsPrincipal cp = this.User;

        var res = await _accountsService.GetAccountIdentitiesAsync(skippedItems: 0, numberOfItems: 50, ct);
        var outDto = _mapper.Map<IEnumerable<AccountIdentityOutgoingDto>>(res);
        return Ok(outDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(AccountRegistrationIncomingDto dto, CancellationToken ct)
    {
        if (dto is null)
        {
            return BadRequest("Invalid request");
        }
        var account = _mapper.Map<AccountRegistration>(dto);
        var result = await _accountsService.CreateAsync(account, ct);
        if (!result.IsSuccess)
        {
            return BadRequest(result.InfoMessage);
        }

        //return CreatedAtAction(nameof(Register), new
        //{
        //    id = result.Value.Id
        //}, result.Value);

        //ToDo
        return Ok(result);
    }
}
