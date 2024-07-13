using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Poc.SimuladorDispositivos.Interfaces.Services;
using Poc.SimuladorDispositivos.Models;

namespace Poc.SimuladorDispositivos.Pages;

public class IndexModel : PageModel
{
    private readonly IIoTHubService _ioTHubService;
    private readonly CosmosDbService _cosmosDbService;

    public IndexModel(IIoTHubService ioTHubService, CosmosDbService cosmosDbService)
    {
        _ioTHubService = ioTHubService;
        _cosmosDbService = cosmosDbService;
    }

    public IEnumerable<DeviceMeasurement> Measurement { get; private set; } = default!;

    public async Task OnGet()
    {
        Measurement = await _cosmosDbService.GetMeasurements();
    }

    public async Task<IActionResult> OnPostAsync(string deviceId, string message)
    {
        var device = await _ioTHubService.AddDeviceAsync(deviceId);

        await _ioTHubService.SendDeviceToCloudMessageAsync(device, message);

        return RedirectToPage();
    }
}
