using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Ae.Poc.Identity.Dtos;
using Ae.Poc.Identity.Services;
using Ae.Poc.Identity.Data;
using System.Net.Mime;

namespace Ae.Poc.Identity.Controllers;

/// <summary>
/// Controller for managing identity master data claims
/// </summary>
[ApiController]
[Route("api/v2/masterdata")]
[Produces("application/json")]
public sealed class IdentityMasterDataController(ILogger<IdentityController> logger, IIdentityMasterDataService masterDataService, IMapper mapper) : ControllerBase
{
    private readonly ILogger<IdentityController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IIdentityMasterDataService _masterDataService = masterDataService ?? throw new ArgumentNullException(nameof(masterDataService));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    /// <summary>
    /// Retrieves a paginated list of claims
    /// </summary>
    /// <param name="skipped">Number of items to skip</param>
    /// <param name="numberOf">Number of items to take (max 100)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of claims</returns>
    [HttpGet("claims")]
    [ProducesResponseType(typeof(IEnumerable<AppClaimOutgoingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetClaimsAsync(
        [FromQuery] int skipped = 0,
        [FromQuery] int numberOf = 50,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Start {ControllerName} {MethodName} ...", nameof(IdentityMasterDataController), nameof(GetClaimsAsync));

        if (skipped < 0)
        {
            return BadRequest("Invalid skipped value");
        }
        if (numberOf < 1 || numberOf > 100)
        {
            return BadRequest("Invalid numberOf value");
        }

        var res = await _masterDataService.GetClaimsAsync(skippedItems: skipped, numberOfItems: numberOf, ct);
        var outDto = _mapper.Map<IEnumerable<AppClaimOutgoingDto>>(res);
        return Ok(outDto);
    }

    /// <summary>
    /// Retrieves details of a specific claim
    /// </summary>
    /// <param name="claimId">The unique identifier of the claim</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The claim details if found</returns>
    [HttpGet("claims/{claimId}", Name = "GetClaimDetails")]
    [ProducesResponseType(typeof(AppClaimOutgoingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClaimDetailsAsync(string claimId, CancellationToken ct)
    {
        _logger.LogInformation("Start {ControllerName} {MethodName} ...", nameof(IdentityMasterDataController), nameof(GetClaimDetailsAsync));

        if (!Guid.TryParse(claimId, out var id))
        {
            return BadRequest("Invalid claimId");
        }

        var res = await _masterDataService.GetClaimDetailsAsync(id, ct);
        if (res is null)
        {
            return NotFound();
        }
        var outDto = _mapper.Map<AppClaimOutgoingDto>(res);
        return Ok(outDto);
    }

    /// <summary>
    /// Creates a new claim
    /// </summary>
    /// <param name="incomingDto">The claim data to create</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The created claim</returns>
    [HttpPost("claims")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(AppClaimOutgoingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateClaimAsync([Required][FromBody] AppClaimIncomingDto incomingDto, CancellationToken ct)
    {
        _logger.LogInformation("Start {ControllerName} {MethodName} ...", nameof(IdentityMasterDataController), nameof(CreateClaimAsync));

        try
        {
            if (incomingDto is null)
            {
                return BadRequest("Invalid request");
            }
            if (string.IsNullOrWhiteSpace(incomingDto.Type))
            {
                return BadRequest("Claim type is required");
            }
            if (incomingDto.Description?.Length > 500)
            {
                return BadRequest("Claim description must be less than 500 characters");
            }

            var claim = _mapper.Map<AppClaim>(incomingDto);
            var result = await _masterDataService.AddClaimAsync(claim, ct);
            if (!result.IsSuccess)
            {
                return BadRequest(result.InfoMessage);
            }

            var outDto = _mapper.Map<AppClaimOutgoingDto>(result.Value);

            var routeValues = new { claimId = $"{outDto.Id}" };
            //var urlAction = Url.Action(
            //    action: nameof(GetClaimDetailsAsync),
            //    controller: nameof(IdentityMasterDataController),
            //    values: routeValues,
            //    protocol: Request.Scheme);
            //return Created(urlAction, outDto);

            return CreatedAtRoute(
                routeName: "GetClaimDetails",
                routeValues: routeValues,
                value: outDto);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Error in {MethodName}", nameof(CreateClaimAsync));
            return StatusCode(StatusCodes.Status500InternalServerError, "Error add claim.");
        }
    }

    /// <summary>
    /// Deletes a specific claim
    /// </summary>
    /// <param name="claimId">The unique identifier of the claim to delete</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The deleted claim details</returns>
    [HttpDelete("claims/{claimId}")]
    [ProducesResponseType(typeof(AppClaimOutgoingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteClaimAsync(string claimId, CancellationToken ct)
    {
        _logger.LogInformation("Start {ControllerName} {MethodName} ...", nameof(IdentityMasterDataController), nameof(DeleteClaimAsync));

        try
        {
            if (!Guid.TryParse(claimId, out var id))
            {
                return BadRequest("Invalid claimId");
            }

            var existingClaim = await _masterDataService.GetClaimDetailsAsync(id, ct);
            if (existingClaim is null)
            {
                return NotFound("Claim not found");
            }

            var result = await _masterDataService.DeleteClaimAsync(id, ct);
            if (!result.IsSuccess)
            {
                return BadRequest(result.InfoMessage);
            }

            var outDto = _mapper.Map<AppClaimOutgoingDto>(result.Value);
            return Ok(outDto);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Error in {MethodName}", nameof(DeleteClaimAsync));
            return StatusCode(StatusCodes.Status500InternalServerError, "Error delete claim.");
        }
    }

    /// <summary>
    /// Updates an existing claim
    /// </summary>
    /// <param name="claimId">The unique identifier of the claim to update</param>
    /// <param name="incomingDto">The updated claim data</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The updated claim</returns>
    [HttpPatch("claims/{claimId}")] // [HttpPut("claims/{claimId}")]
    [ProducesResponseType(typeof(AppClaimOutgoingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateClaimAsync(
        string claimId,
        [Required][FromBody] AppClaimIncomingDto incomingDto,
        CancellationToken ct)
    {
        _logger.LogInformation("Start {ControllerName} {MethodName} ...", nameof(IdentityMasterDataController), nameof(UpdateClaimAsync));

        try
        {
            if (!Guid.TryParse(claimId, out var id))
            {
                return BadRequest("Invalid claimId");
            }

            if (incomingDto is null)
            {
                return BadRequest("Invalid request");
            }

            if (id != incomingDto.Id)
            {
                return BadRequest("ClaimId does not match");
            }
            if (string.IsNullOrWhiteSpace(incomingDto.Type) || string.IsNullOrWhiteSpace(incomingDto.Value))
            {
                return BadRequest("Claim type and value are required");
            }
            if (incomingDto.Description?.Length > 500)
            {
                return BadRequest("Claim description must be less than 500 characters");
            }

            var existingClaim = await _masterDataService.GetClaimDetailsAsync(id, ct);
            if (existingClaim is null)
            {
                return NotFound("Claim not found");
            }

            var claimToUpdate = _mapper.Map<AppClaim>(incomingDto);
            var result = await _masterDataService.UpdateClaimAsync(id, claimToUpdate, ct);
            if (!result.IsSuccess)
            {
                return BadRequest(result.InfoMessage);
            }

            var outDto = _mapper.Map<AppClaimOutgoingDto>(result.Value);
            return Ok(outDto);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Error in {MethodName}", nameof(UpdateClaimAsync));
            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating claim.");
        }
    }
}
