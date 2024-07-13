using System.Text;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Poc.SimuladorDispositivos.Models;

public partial class CosmosDbService
{
    private Container _container;

    public CosmosDbService(IConfiguration configuration)
    {
        var connectionString = configuration["CosmosDb:ConnectionString"];
        var databaseName = configuration["CosmosDb:DatabaseName"];
        var containerName = configuration["CosmosDb:ContainerName"];
        var client = new CosmosClient(connectionString);
        _container = client.GetContainer(databaseName, containerName);
    }

    public async Task<IEnumerable<DeviceMeasurement>> GetMeasurements()
    {
        var query = _container.GetItemQueryIterator<CosmosDbEntity>();
        var results = new List<DeviceMeasurement>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(
                response.ToList().Select(x =>
                    ParseBase64Measurement(x.Body)));
        }
        return results;
    }

    private static DeviceMeasurement ParseBase64Measurement(string base64)
    {
        var bytes = Convert.FromBase64String(base64);
        var json = Encoding.UTF8.GetString(bytes);

        return JsonConvert.DeserializeObject<DeviceMeasurement>(json)
            ?? throw new Exception("Erro ao deserializar a medição");
    }

    private record CosmosDbEntity(string Body);
}



