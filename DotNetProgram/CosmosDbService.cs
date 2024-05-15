namespace dotnetProgram
{
    using Microsoft.Azure.Cosmos;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using dotnetProgram.Entities;
    

    public class CosmosDbService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly string _databaseId;

        public CosmosDbService(CosmosClient cosmosClient, string databaseId)
        {
            _cosmosClient = cosmosClient;
            _databaseId = databaseId;
        }

        public async Task AddItemAsync<T>(T item, string containerId, string partitionKeyValue) where T : class
        {
            var container = _cosmosClient.GetContainer(_databaseId, containerId);
            await container.CreateItemAsync(item, new PartitionKey(partitionKeyValue));
        }

      
        public async Task<T> GetItemAsync<T>(string id, string containerId, string partitionKeyValue) where T : class
        {
            try
            {
                var container = _cosmosClient.GetContainer(_databaseId, containerId);
                ItemResponse<T> response = await container.ReadItemAsync<T>(id, new PartitionKey(partitionKeyValue));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

     
        public async Task UpdateItemAsync<T>(string id, T item, string containerId, string partitionKeyValue) where T : class
        {
            var container = _cosmosClient.GetContainer(_databaseId, containerId);
            await container.UpsertItemAsync(item, new PartitionKey(partitionKeyValue));
        }

 
        public async Task DeleteItemAsync<T>(string id, string containerId, string partitionKeyValue) where T : class
        {
            var container = _cosmosClient.GetContainer(_databaseId, containerId);
            await container.DeleteItemAsync<T>(id, new PartitionKey(partitionKeyValue));
        }

       
        public async Task<IEnumerable<T>> QueryItemsAsync<T>(string queryString, string containerId) where T : class
        {
            var container = _cosmosClient.GetContainer(_databaseId, containerId);
            var query = container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
            List<T> results = new List<T>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }
    

    }

}
