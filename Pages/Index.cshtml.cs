using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Poc.SimuladorDispositivos.Interfaces.Services;

namespace Poc.SimuladorDispositivos.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IIoTHubService _ioTHubService;
    private readonly CosmosDbService _cosmosDbService;

    public IndexModel(ILogger<IndexModel> logger, IIoTHubService ioTHubService, CosmosDbService cosmosDbService)
    {
        _logger = logger;
        _ioTHubService = ioTHubService;
        _cosmosDbService = cosmosDbService;
    }

    public IEnumerable<Medicao> Medicoes { get; private set; } = default!;

    public async Task OnGet()
    {
        Medicoes = await _cosmosDbService.ObterMedicoes();
    }

    public async Task<IActionResult> OnPostAsync(string deviceId, string message)
    {
        var device = await _ioTHubService.AddDeviceAsync(deviceId);

        await _ioTHubService.SendDeviceToCloudMessageAsync(device, message);



        return RedirectToPage();
    }
}
