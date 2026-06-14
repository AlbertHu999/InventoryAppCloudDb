using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Middleware;
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
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<ISalesService, SalesService>();

builder.Services.AddOpenApi();

var app = builder.Build();

// ── Middleware ───────────────────
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<TokenAuthMiddleware>();

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

// ── 進貨單 API ─────────────────────────────────────────
app.MapGet("/api/purchases", async (IPurchaseService svc) =>
{
    var result = await svc.GetAllAsync();
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
}).WithTags("進貨管理");

app.MapGet("/api/purchases/{id:int}", async (int id, IPurchaseService svc) =>
{
    var result = await svc.GetByIdAsync(id);
    return result.Success ? Results.Ok(result) : Results.NotFound(result);
}).WithTags("進貨管理");

app.MapPost("/api/purchases", async (
    CreatePurchaseOrderDto dto, IPurchaseService svc, HttpContext ctx) =>
{
    var createdBy = ctx.Items["Username"]?.ToString() ?? "";
    var result = await svc.CreateAsync(dto, createdBy);
    return result.Success
        ? Results.Created($"/api/purchases/{result.Data!.Id}", result)
        : Results.BadRequest(result);
}).WithTags("進貨管理");

// ── 銷貨單 API ─────────────────────────────────────────
app.MapGet("/api/sales", async (ISalesService svc) =>
{
    var result = await svc.GetAllAsync();
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
}).WithTags("銷貨管理");

app.MapGet("/api/sales/{id:int}", async (int id, ISalesService svc) =>
{
    var result = await svc.GetByIdAsync(id);
    return result.Success ? Results.Ok(result) : Results.NotFound(result);
}).WithTags("銷貨管理");

app.MapPost("/api/sales", async (
    CreateSalesOrderDto dto, ISalesService svc, HttpContext ctx) =>
{
    var createdBy = ctx.Items["Username"]?.ToString() ?? "";
    var result = await svc.CreateAsync(dto, createdBy);
    return result.Success
        ? Results.Created($"/api/sales/{result.Data!.Id}", result)
        : Results.BadRequest(result);
}).WithTags("銷貨管理");

// ── 驗證 API ─────────────────────────────────────────
app.MapPost("/api/auth/login", async (LoginDto dto, IAuthService svc) =>
{
    var result = await svc.LoginAsync(dto.Username, dto.Password);
    return result.Success
        ? Results.Ok(result)
        : Results.Unauthorized();
})
.WithTags("驗證");

app.Run();
     

