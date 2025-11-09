using System.Net;

using System.Net.Http.Json;

namespace CSharpApp.Application.Categories;

public class CategoriesService : ICategoriesService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<CategoriesService> _logger;

    public CategoriesService(HttpClient httpClient, IOptions<RestApiSettings> restApiSettings,
        ILogger<CategoriesService> logger)
    {
        _httpClient = httpClient;
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Category>> GetCategories()
    {
        var response = await _httpClient.GetAsync(_restApiSettings.Categories);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Category>>(content);

        return res.AsReadOnly();
    }

        public async Task<Category?> GetCategoryById(int id)

        {

            var response = await _httpClient.GetAsync($"{_restApiSettings.Categories}/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)

            {

                return null;

            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var res = JsonSerializer.Deserialize<Category>(content);

    

            return res;

        }

    

        public async Task<Category?> CreateCategory(Category Category)

        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_restApiSettings.Categories, Category);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var res = JsonSerializer.Deserialize<Category>(content);
                    return res;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error creating category: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown while creating category");
                return null;
            }
        }

    }

    