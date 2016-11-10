using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HttpClientSample
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public SubProduct sub { get; set; }
    }

    public class SubProduct
    {
        public string subname { get; set; }
    }

    public class Request
    {
        private String query;

        public String getQuery()
        {
            return query;
        }

        public void setQuery(String query)
        {
            this.query = query;
        }
    }

    class Program
    {
        static HttpClient client = new HttpClient();

        static void ShowProduct(Product product)
        {
            Console.WriteLine($"Name: {product.Name}\tPrice: {product.Price}\tCategory: {product.Category}");
        }

        static async Task<String> CreateProductAsync(Product product)
        {
            var json = JsonConvert.SerializeObject(product);
            var req = new Request();
            req.setQuery(json.ToString());

            Console.WriteLine(json.ToString());

            HttpResponseMessage response = await client.PostAsJsonAsync<String>("query", json.ToString());
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            string ret = await response.Content.ReadAsStringAsync();
            return ret;
        }

        static async Task<String> GetProductAsync(string path)
        {
            String product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsStringAsync();
            }
            return product;
        }

        static async Task<Product> UpdateProductAsync(Product product)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync($"api/products/{product.Id}", product);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            product = await response.Content.ReadAsAsync<Product>();
            return product;
        }

        static async Task<HttpStatusCode> DeleteProductAsync(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"api/products/{id}");
            return response.StatusCode;
        }

        static void Main()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:8080/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Create a new product             
                SubProduct subP = new SubProduct { subname = "subN" };
                Product product = new Product { Id = "1", Name = "Gizmo", Price = 100, Category = "Widgets", sub = subP };

                var url = await CreateProductAsync(product);
                Console.WriteLine($"Created at {url}");

                // Get the product
                String track = await GetProductAsync("query-get");
                Console.WriteLine(track);

                // Update the product
                //Console.WriteLine("Updating price...");
                //product.Price = 80;
                //await UpdateProductAsync(product);

                // Get the updated product
                //String product = await GetProductAsync($"/test-spec/fragment");
                //Console.WriteLine($"product at {product}");

                // Delete the product
                //var statusCode = await DeleteProductAsync(product.Id);
                //Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

    }
}
