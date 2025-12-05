using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using WMS_Suite.DataContracts;
using WMS_Suite.Repositories;

namespace WMS_Suite.Services
{
    public class ShopifyService : IShopifyService
    {
        private readonly string _storeUrl;
        private readonly string _accessToken;

        public ShopifyService(string storeUrl, string accessToken)
        {
            _storeUrl = storeUrl;
            _accessToken = accessToken;
        }

        public async Task SyncInventoryAsync(List<InventoryItem> localItems, IInventoryRepository repo, Action<string> logMismatch)
        {
            if (string.IsNullOrEmpty(_accessToken)) return;

            var client = GetClient();
            var request = new GraphQLRequest
            {
                Query = @"
        {
            products(first: 50) {
                edges {
                    node {
                        id
                        title
                        variants(first: 10) {
                            edges {
                                node {
                                    id
                                    sku
                                    inventoryQuantity
                                }
                            }
                        }
                    }
                }
            }
        }"
            };

            var response = await client.SendQueryAsync<dynamic>(request);
            if (response.Errors?.Any() ?? false) throw new Exception(string.Join(", ", response.Errors.Select(e => e.Message)));

            var products = response.Data.products.edges;
            foreach (var product in products)
            {
                var title = product.node.title.ToString();
                var variants = product.node.variants.edges;
                foreach (var variant in variants)
                {
                    var sku = variant.node.sku?.ToString();
                    if (string.IsNullOrEmpty(sku)) continue; // Skip invalid

                    var shopQty = (int)variant.node.inventoryQuantity;

                    var local = localItems.FirstOrDefault(i => i.Sku == sku);
                    if (local != null)
                    {
                        if (local.Quantity != shopQty)
                        {
                            logMismatch?.Invoke($"Mismatch for {sku}: Local {local.Quantity}, Shopify {shopQty}. Updating local.");
                            local.Quantity = shopQty;
                            await repo.UpdateAsync(local);
                        }
                    }
                    else
                    {
                        var newItem = new InventoryItem { Sku = sku, Name = title, Quantity = shopQty };
                        await repo.AddAsync(newItem);
                        localItems.Add(newItem);
                        logMismatch?.Invoke($"Added new item {sku} from Shopify with qty {shopQty}.");
                    }
                }
            }
        }

        public async Task<List<SalesHistory>> FetchRecentSalesAsync(int itemId)
        {
            // Similar query for orders/lineItems; stub for now
            return new List<SalesHistory>(); // Implement GraphQL for orders
        }

        private GraphQLHttpClient GetClient()
        {
            var endpoint = $"https://{_storeUrl}/admin/api/2025-01/graphql.json"; // Use latest version
            var client = new GraphQLHttpClient(endpoint, new NewtonsoftJsonSerializer());
            client.HttpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", _accessToken);
            return client;
        }
    }
}