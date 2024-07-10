using Microsoft.Azure.Devices;

namespace Poc.SimuladorDispositivos.Interfaces.Services;

public interface IIoTHubService
{
    Task<Device?> AddDeviceAsync(string deviceId);
    Task SendDeviceToCloudMessageAsync(Device? device, string message);
}