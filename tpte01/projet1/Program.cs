using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MinimalAPIExample", Version = "v1" });
});

var app = builder.Build();

var products = new List<Product>
{
    new Product
    {
        Id = 1,
        Name = "Maça",
        Price = 12.2m,
        Description = "Neogico massa",
        Quantity = 100
    },
    new Product
    {
        Id = 2,
        Name = "Lapsi",
        Price = 20.75m,
        Description = "Neogico muito bom",
        Quantity = 50
    }
};

// Criando uma variável estática para o contador de ID
int productIdCounter = 2;

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapPost(
        "/products",
        async (HttpContext context, Product newProduct) =>
        {
            if (newProduct.Price <= 0)
            {
                return Results.BadRequest("O preço do produto deve ser maior que zero.");
            }

            if (newProduct.Quantity < 0)
            {
                return Results.BadRequest("A quantidade do produto deve ser maior ou igual a zero.");
            }

            // Incrementar o contador de ID e atribuir ao novo produto
            newProduct.Id = ++productIdCounter;

            products.Add(newProduct);

            return Results.Created("Produto inserido com sucesso! ID:", newProduct.Id);
        }
    )
    .WithName("AddProduct")
    .WithMetadata(new HttpMethodMetadata(new[] { "POST" }))
    .WithOpenApi();

app.MapGet(
        "/products",
        async () =>
        {
            return Results.Ok(products);
        }
    )
    .WithName("GetProducts")
    .WithMetadata(new HttpMethodMetadata(new[] { "GET" }))
    .WithOpenApi();

app.MapGet(
        "/products/{id}",
        async (int id) =>
        {

            var product = products.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                return Results.Ok(product);
            }
            else
            {
                return Results.NotFound($"Produto com {id} não encontrado.");
            }
        }
    )
    .WithName("GetProductById")
    .WithMetadata(new HttpMethodMetadata(new[] { "GET" }))
    .WithOpenApi();

app.MapPut(
        "/products/{id}",
        async (int id, Product newProduct) =>
        {
            if (newProduct.Price <= 0)
            {
                return Results.BadRequest("O preço do produto deve ser maior que zero.");
            }
            if (newProduct.Quantity < 0)
            {
                return Results.BadRequest("A quantidade do produto deve ser maior ou igual a zero.");
            }

            var existingProductIndex = products.FindIndex(p => p.Id == id);
            
            if (existingProductIndex != -1)
            {
                var existingProduct = products[existingProductIndex];

                var updatedProduct = existingProduct with
                {
                    Name = newProduct.Name,
                    Price = newProduct.Price,
                    Description = newProduct.Description,
                    Quantity = newProduct.Quantity
                };

                products[existingProductIndex] = updatedProduct;

                return Results.Ok(updatedProduct);
            }
            else
            {
                return Results.NotFound($"Produto com {id} não encontrado.");
            }
        }
    )
    .WithName("UpdateProduct")
    .WithMetadata(new HttpMethodMetadata(new[] { "PUT" }))
    .WithOpenApi();

app.MapDelete(
        "/products/{id}",
        (int id) =>
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                products.Remove(product);
                return Results.Ok();
            }
            else
            {
                return Results.NotFound($"Produto com {id} não encontrado.");
            }
        }
    )
    .WithName("DeleteProduct")
    .WithMetadata(new HttpMethodMetadata(new[] { "DELETE" }))
    .WithOpenApi();

app.Run();

record Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
}
