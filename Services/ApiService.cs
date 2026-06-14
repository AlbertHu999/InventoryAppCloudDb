// Services/ApiService.cs
using InventoryAppCloudDb.DTOs;
using InventoryAppCloudDb.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InventoryAppCloudDb.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    // JSON 序列化設定（camelCase 對應 API 回傳）
    private static readonly JsonSerializerOptions _jsonOpt = new()
    {
        PropertyNameCaseInsensitive = true   // 不分大小寫對應
    };

    public ApiService()
    {
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        _baseUrl = config["ApiSettings:BaseUrl"]!;
    }

    // ── 私有：自動加上 Token Header ─────────────────
    private void SetAuthHeader()
    {
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", AppSession.Token);
    }

    // ── 私有：統一處理 401 Token 過期 ───────────────────
    private async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> request)
    {
        SetAuthHeader();
        var response = await request();

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Token 已過期");

        return response;
    }
    
    // ── 私有：把物件序列化成 JSON Body ──────────────
    private static StringContent ToJson(object obj)
        => new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    // ── 登入 ────────────────────────────────────────
    public async Task<(bool Success, string Token, string Username, string Role, string Message)>
    LoginAsync(string username, string password)
    {
        var dto = new LoginDto { Username = username, Password = password };
        var response = await _http.PostAsync($"{_baseUrl}/api/auth/login", ToJson(dto));
        var json = await response.Content.ReadAsStringAsync();
        //MessageBox.Show(json);  // ← 加這行，看原始回傳內容

        if (!response.IsSuccessStatusCode)
            return (false, "", "", "", "帳號或密碼錯誤");

        var result = JsonSerializer.Deserialize<ServiceResultJson<LoginResponseDto>>(json, _jsonOpt);
        var data = result?.Data;

        return (true, data?.Token ?? "", data?.Username ?? "", data?.Role ?? "", "登入成功");
    }

    // ── 查詢全部商品 ──────────────────────────────────
    public async Task<List<ProductDto>> GetProductsAsync()
    {
        var response = await SendAsync(() => _http.GetAsync($"{_baseUrl}/api/products"));
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ServiceResultJson<List<ProductDto>>>(json, _jsonOpt);
        return result?.Data ?? [];
    }

    // ── 查詢單筆 ──────────────────────────────────────
    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        SetAuthHeader();
        var response = await _http.GetAsync($"{_baseUrl}/api/products/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ServiceResultJson<ProductDto>>(json, _jsonOpt);
        return result?.Data;
    }

    // ── 新增商品 ──────────────────────────────────────
    public async Task<(bool Success, string Message)> CreateProductAsync(CreateProductDto dto)
    {
        SetAuthHeader();
        var response = await _http.PostAsync($"{_baseUrl}/api/products", ToJson(dto));
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ServiceResultJson<ProductDto>>(json, _jsonOpt);

        return response.IsSuccessStatusCode
            ? (true, "新增成功")
            : (false, result?.Message ?? "新增失敗");
    }

    // ── 修改商品 ──────────────────────────────────────
    public async Task<(bool Success, string Message)> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        SetAuthHeader();
        var response = await _http.PutAsync($"{_baseUrl}/api/products/{id}", ToJson(dto));
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ServiceResultJson<ProductDto>>(json, _jsonOpt);

        return response.IsSuccessStatusCode
            ? (true, "修改成功")
            : (false, result?.Message ?? "修改失敗");
    }

    // ── 刪除商品 ──────────────────────────────────────
    public async Task<(bool Success, string Message)> DeleteProductAsync(int id)
    {
        SetAuthHeader();
        var response = await _http.DeleteAsync($"{_baseUrl}/api/products/{id}");

        return response.IsSuccessStatusCode
            ? (true, "刪除成功")
            : (false, "刪除失敗");
    }

    // ════════════════════════════════════════════════════════
    //  進貨單 Purchase Orders
    // ════════════════════════════════════════════════════════

    /// <summary>取得所有進貨單（含明細）</summary>
    public async Task<List<PurchaseOrderDto>> GetPurchaseOrdersAsync()
    {
        var response = await SendAsync(() => _http.GetAsync($"{_baseUrl}/api/purchases"));
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ServiceResultJson<List<PurchaseOrderDto>>>(json, _jsonOpt);
        return result?.Data ?? [];
    }

    /// <summary>建立進貨單（同時更新庫存）</summary>
    public async Task<(bool Success, string Message)> CreatePurchaseOrderAsync(
        CreatePurchaseOrderDto dto)
    {
        var response = await SendAsync(() => _http.PostAsync($"{_baseUrl}/api/purchases", ToJson(dto)));
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ServiceResultJson<PurchaseOrderDto>>(json, _jsonOpt);
        return response.IsSuccessStatusCode
            ? (true, result?.Message ?? "進貨單建立成功")
            : (false, result?.Message ?? "進貨單建立失敗");
    }

    // ════════════════════════════════════════════════════════
    //  銷貨單 Sales Orders
    // ════════════════════════════════════════════════════════

    /// <summary>取得所有銷貨單（含明細）</summary>
    public async Task<List<SalesOrderDto>> GetSalesOrdersAsync()
    {
        var response = await SendAsync(() => _http.GetAsync($"{_baseUrl}/api/sales"));
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ServiceResultJson<List<SalesOrderDto>>>(json, _jsonOpt);
        return result?.Data ?? [];
    }

    /// <summary>建立銷貨單（同時扣庫存）</summary>
    public async Task<(bool Success, string Message)> CreateSalesOrderAsync(
        CreateSalesOrderDto dto)
    {
        var response = await SendAsync(() => _http.PostAsync($"{_baseUrl}/api/sales", ToJson(dto)));
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ServiceResultJson<SalesOrderDto>>(json, _jsonOpt);
        return response.IsSuccessStatusCode
            ? (true, result?.Message ?? "銷貨單建立成功")
            : (false, result?.Message ?? "銷貨單建立失敗");
    }
    // ── 私有：對應 API 的 ServiceResult<T> JSON 結構 ─
    private class ServiceResultJson<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }
    }
}