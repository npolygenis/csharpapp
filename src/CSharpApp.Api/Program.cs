using CSharpApp.Core.Dtos;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CSharpApp.Core.Settings;
using CSharpApp.Core.Interfaces;
using CSharpApp.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.ClearProviders().AddSerilog(logger);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<RestApiSettings>(builder.Configuration.GetSection(nameof(RestApiSettings)));
builder.Services.Configure<HttpClientSettings>(builder.Configuration.GetSection(nameof(HttpClientSettings)));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
builder.Services.AddDefaultConfiguration();
builder.Services.AddHttpConfiguration();
builder.Services.AddProblemDetails();
builder.Services.AddApiVersioning();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseMiddleware<PerformanceMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

var versionedEndpointRouteBuilder = app.NewVersionedApi();

versionedEndpointRouteBuilder.MapPost("api/v{version:apiVersion}/auth/login", async (IAuthService authService, UserCredentials userCredentials) =>
    {
        var token = await authService.Login(userCredentials);
        return token is not null ? Results.Ok(token) : Results.Unauthorized();
    })
    .WithName("Login")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/products", async (IProductsService productsService) =>
    {
        var products = await productsService.GetProducts();
        return products;
    })
    .WithName("GetProducts")
    .RequireAuthorization()
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/products/{id:int}", async (IProductsService productsService, int id) =>
    {
        var product = await productsService.GetProductById(id);
        return product is not null ? Results.Ok(product) : Results.NotFound();
    })
    .WithName("GetProductById")
    .RequireAuthorization()
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPost("api/v{version:apiVersion}/products", async (IProductsService productsService, Product product) =>
    {
        var createdProduct = await productsService.CreateProduct(product);
        return Results.Ok(createdProduct);
    })
    .WithName("CreateProduct")
    .RequireAuthorization()
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/categories", async (ICategoriesService categoriesService) =>
{
    var categories = await categoriesService.GetCategories();
    return categories;
})
.WithName("GetCategories")
.RequireAuthorization()
.HasApiVersion(1.0);
    

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/categories/{id:int}", async (ICategoriesService categoriesService, int id) =>
    {
        var category = await categoriesService.GetCategoryById(id);
        return category is not null ? Results.Ok(category) : Results.NotFound();
    })
    .WithName("GetCategoryById")
    .RequireAuthorization()
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPost("api/v{version:apiVersion}/categories", async (ICategoriesService categoriesService, Category category) =>
    {
        var createdCategory = await categoriesService.CreateCategory(category);
        return Results.Ok(createdCategory);
    })
    .WithName("CreateCategory")
    .RequireAuthorization()
    .HasApiVersion(1.0);

app.Run();