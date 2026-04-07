using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;
using InventoryAppCloudDb.Api.Repositories;
using InventoryAppCloudDb.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== DI 登記（告訴系統：需要什麼服務，就給什麼實作）=====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ===== API 路由定義（對應四層架構的最外層）=====

// GET /api/products — 取得所有商品
app.MapGet("/api/products", async (IProductService svc) =>
{
    var products = await svc.GetAllAsync();
    return Results.Ok(products);
});

// GET /api/products/3 — 依 ID 取得單一商品
app.MapGet("/api/products/{id:int}", async (int id, IProductService svc) =>
{
    var product = await svc.GetByIdAsync(id);
    return product is not null
        ? Results.Ok(product)
        : Results.NotFound(new { message = $"找不到 Id={id} 的商品" });
});

// GET /api/products/category/飲料 — 依分類取得商品
app.MapGet("/api/products/category/{category}", async (string category, IProductService svc) =>
{
    var products = await svc.GetByCategoryAsync(category);
    return Results.Ok(products);
});

// POST /api/products — 新增商品
app.MapPost("/api/products", async (CreateProductDto dto, IProductService svc) =>
{
    var created = await svc.CreateAsync(dto);
    return created is not null
        ? Results.Created($"/api/products/{created.Id}", created)
        : Results.BadRequest(new { message = "新增失敗，請檢查輸入資料" });
});

// PUT /api/products/3 — 修改商品
app.MapPut("/api/products/{id:int}", async (int id, UpdateProductDto dto, IProductService svc) =>
{
    var success = await svc.UpdateAsync(id, dto);
    return success
        ? Results.Ok(new { message = "修改成功" })
        : Results.NotFound(new { message = $"找不到 Id={id} 的商品，或資料無效" });
});

// DELETE /api/products/3 — 刪除商品
app.MapDelete("/api/products/{id:int}", async (int id, IProductService svc) =>
{
    var success = await svc.DeleteAsync(id);
    return success
        ? Results.Ok(new { message = "刪除成功" })
        : Results.NotFound(new { message = $"找不到 Id={id} 的商品" });
});
app.Run();
     