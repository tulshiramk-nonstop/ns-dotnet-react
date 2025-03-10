using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure HTTPS redirection for non-development environments
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpsRedirection(options =>
    {
        // Get the HTTPS port from configuration or use a default
        options.HttpsPort = int.Parse(builder.Configuration["HTTPS_PORT"] ?? "443");
        // Make sure redirections are permanent
        options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Add production middleware
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // Enable HSTS (HTTP Strict Transport Security)
}

app.UseCors("CorsPolicy");

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Sample data
var products = new List<Product>
{
    new Product { Id = 1, Name = "Laptop", Price = 1299.99m },
    new Product { Id = 2, Name = "Smartphone", Price = 699.99m },
    new Product { Id = 3, Name = "Headphones", Price = 149.99m }
};

// API endpoints
app.MapGet("/api/products", () => products)
   .WithName("GetProducts");

app.MapGet("/api/products/{id}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product != null ? Results.Ok(product) : Results.NotFound();
})
.WithName("GetProductById");

app.MapPost("/api/products", ([FromBody] Product product) =>
{
    product.Id = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
    products.Add(product);
    return Results.Created($"/api/products/{product.Id}", product);
})
.WithName("CreateProduct");

app.Run();

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}