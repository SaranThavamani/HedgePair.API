using HedgePair.API.DTOs;
using HedgePair.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace HedgePair.API.Controllers;

/// <summary>Provides financial instrument data for UI drop['l=[o=\[down population.</summary>
[ApiController]
[Route("api/financial-instrument")]
[Produces("application/json")]
public class FinancialInstrumentController : ControllerBase
{
    private readonly IFinancialInstrumentService _service;

    public FinancialInstrumentController(IFinancialInstrumentService service)
        => _service = service;

    /// <summary>
    /// Returns all Active, unpaired Financial Instruments (Bonds and Swaps).
    /// Used to populate the Bond and Swap selection dropdowns in the UI.
    /// </summary>
    /// <returns>Array of active unpaired FinancialInstrumentDto objects.</returns>
    [HttpGet("active-fins")]
    [ProducesResponseType(typeof(IEnumerable<FinancialInstrumentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveUnpaired()
    {
        var instruments = await _service.GetActiveUnpairedAsync();
        return Ok(instruments);
    }
}
