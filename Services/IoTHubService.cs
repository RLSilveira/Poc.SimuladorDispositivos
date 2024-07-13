using System.Text;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Poc.SimuladorDispositivos.Interfaces.Services;
using Poc.SimuladorDispositivos.Models;

namespace Poc.SimuladorDispositivos.Services;

public class IoTHubService(IConfiguration configuration) : IIoTHubService
{
    private readonly RegistryManager _registryManager = RegistryManager.CreateFromConnectionString(configuration["IoTHubConnectionString"]);

    public async Task<Device?> AddDeviceAsync(string deviceId)
    {
        try
        {
            return await _registryManager.GetDeviceAsync(deviceId)
                ?? await _registryManager.AddDeviceAsync(new Device(deviceId));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar o dispositivo: {ex.Message}");
            return default;
        }
    }

    public async Task SendDeviceToCloudMessageAsync(Device? device, string message)
    {
        ArgumentNullException.ThrowIfNull(device, nameof(device));
        
        var deviceConnectionString = BuildConnectionString(device);

        var deviceClient = DeviceClient.CreateFromConnectionString(
            deviceConnectionString,
            Microsoft.Azure.Devices.Client.TransportType.Mqtt);

        var body = new DeviceMeasurement(device.Id, DateTime.Now, message);

        var messagePayload = new Microsoft.Azure.Devices.Client.Message(
            Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(body)));

        await deviceClient.SendEventAsync(messagePayload);
    }

    private string BuildConnectionString(Device device)
    {
        var iotHubHostName = configuration.GetValue<string>("IoTHubHostName");
        var deviceId = device.Id;
        var primaryKey = device.Authentication.SymmetricKey.PrimaryKey;

        var deviceConnectionString = $"HostName={iotHubHostName};DeviceId={deviceId};SharedAccessKey={primaryKey}";
        
        return deviceConnectionString;
    }
}