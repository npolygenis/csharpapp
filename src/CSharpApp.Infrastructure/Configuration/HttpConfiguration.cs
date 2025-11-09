using Polly;
using Polly.Extensions.Http;

namespace CSharpApp.Infrastructure.Configuration;

public static class HttpConfiguration
{
    public static IServiceCollection AddHttpConfiguration(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetService<IConfiguration>();
        var restApiSettings = configuration!.GetSection(nameof(RestApiSettings)).Get<RestApiSettings>();

        services.AddHttpClient<IProductsService, ProductsService>(client =>
            {
                client.BaseAddress = new Uri(restApiSettings!.BaseUrl!);
            })
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            }));

              services.AddHttpClient<ICategoriesService, CategoriesService>(client =>
            {
                client.BaseAddress = new Uri(restApiSettings!.BaseUrl!);
            })
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            }));
        
        return services;
    }
}