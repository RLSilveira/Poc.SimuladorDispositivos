using System.Buffers.Text;
using System.Text;
using System.Text.Unicode;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Poc.SimuladorDispositivos.Pages;

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

    public async Task<IEnumerable<Medicao>> ObterMedicoes()
    {
        var query = _container.GetItemQueryIterator<CosmosDbEntity>();
        var results = new List<Medicao>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(
                response.ToList().Select(x => DeserializarMedicaoBase64(x.Body)));
        }
        return results;
    }

    private Medicao DeserializarMedicaoBase64(string base64)
    {
        var bytes = Convert.FromBase64String(base64);
        var json = Encoding.UTF8.GetString(bytes);
        
        return JsonConvert.DeserializeObject<Medicao>(json) 
            ?? throw new Exception("Erro ao deserializar a medição");
    }

    private record CosmosDbEntity(string Body);
}



