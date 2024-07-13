namespace Poc.SimuladorDispositivos.Models;

public record DeviceMeasurement(
    string DeviceIdentifier,
    DateTime Timestamp,
    string Value);