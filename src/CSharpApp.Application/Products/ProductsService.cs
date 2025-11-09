using System.Net;

using System.Net.Http.Json;

namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<ProductsService> _logger;

    public ProductsService(HttpClient httpClient, IOptions<RestApiSettings> restApiSettings,
        ILogger<ProductsService> logger)
    {
        _httpClient = httpClient;
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Product>> GetProducts()
    {
        var response = await _httpClient.GetAsync(_restApiSettings.Products);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Product>>(content);

        return res.AsReadOnly();
    }

        public async Task<Product?> GetProductById(int id)

        {

            var response = await _httpClient.GetAsync($"{_restApiSettings.Products}/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)

            {

                return null;

            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var res = JsonSerializer.Deserialize<Product>(content);

    

            return res;

        }

    

        public async Task<Product?> CreateProduct(Product product)

        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_restApiSettings.Products, product);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var res = JsonSerializer.Deserialize<Product>(content);
                    return res;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error creating product: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown while creating product");
                return null;
            }
        }

    }

    