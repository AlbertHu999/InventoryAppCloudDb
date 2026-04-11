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

// ── Middleware ───────────────────
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ── 全域例外處理 ──────────────────────────────────────
app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    ctx.Response.StatusCode = 500;
    ctx.Response.ContentType = "application/json";
    var error = ctx.Features
        .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
    await ctx.Response.WriteAsJsonAsync(
        ServiceResult.Fail(error?.Error.Message ?? "伺服器發生未知錯誤")
    );
}));

// ===== API 路由定義（對應四層架構的最外層）=====
// ── 商品 API ─────────────────────────────────────────

// ===== API 路由定義（對應四層架構的最外層）=====

// GET /api/products — 取得所有商品
app.MapGet("/api/products", async (IProductService svc) =>
{
    var result = await svc.GetAllAsync();
    return result.Success
        ? Results.Ok(result)
        : Results.BadRequest(result);
})
.WithTags("商品管理");

// GET /api/products/3 — 依 ID 取得單一商品
app.MapGet("/api/products/{id:int}", async (int id, IProductService svc) =>
{
    var result = await svc.GetByIdAsync(id);
    return result.Success
        ? Results.Ok(result)
        : Results.NotFound(result);
})
.WithTags("商品管理");

// GET /api/products/category/飲料 — 依分類取得商品
app.MapGet("/api/products/category/{category}", async (string category, IProductService svc) =>
{
    var result = await svc.GetByCategoryAsync(category);
    return result.Success
        ? Results.Ok(result)
        : Results.BadRequest(result);
})
.WithTags("商品管理");

// POST /api/products — 新增商品
app.MapPost("/api/products", async (CreateProductDto dto, IProductService svc) =>
{
    var result = await svc.CreateAsync(dto);
    return result.Success
        ? Results.Created($"/api/products/{result.Data!.Id}", result)
        : Results.BadRequest(result);
})
.WithTags("商品管理");

// PUT /api/products/3 — 修改商品
app.MapPut("/api/products/{id:int}", async (int id, UpdateProductDto dto, IProductService svc) =>
{
    var result = await svc.UpdateAsync(id, dto);
    return result.Success
        ? Results.Ok(result)
        : Results.NotFound(result);
})
.WithTags("商品管理");

// DELETE /api/products/3 — 刪除商品
app.MapDelete("/api/products/{id:int}", async (int id, IProductService svc) =>
{
    var result = await svc.DeleteAsync(id);
    return result.Success
        ? Results.Ok(result)
        : Results.NotFound(result);
})
.WithTags("商品管理");

app.Run();
     

