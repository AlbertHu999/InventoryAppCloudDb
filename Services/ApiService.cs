// Services/ApiService.cs
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using InventoryAppCloudDb.DTOs;
using InventoryAppCloudDb.Models;

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
        _baseUrl = "https://inventory-api-194340759475.asia-east1.run.app";
        // 之後可以從 appsettings.json 讀取，不要硬碼
    }

    // ── 私有：自動加上 Token Header ─────────────────
    private void SetAuthHeader()
    {
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", AppSession.Token);
    }

    // ── 私有：把物件序列化成 JSON Body ──────────────
    private static StringContent ToJson(object obj)
        => new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    // ── 登入 ────────────────────────────────────────
    public async Task<(bool Success, string Token, string Message)> LoginAsync(
        string username, string password)
    {
        var dto = new LoginDto { Username = username, Password = password };
        var response = await _http.PostAsync($"{_baseUrl}/api/auth/login", ToJson(dto));
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return (false, "", "帳號或密碼錯誤");

        // 解析回傳的 ServiceResult<string>
        var result = JsonSerializer.Deserialize<ServiceResultJson<string>>(json, _jsonOpt);
        return (true, result?.Data ?? "", "登入成功");
    }

    // ── 查詢全部商品 ──────────────────────────────────
    public async Task<List<ProductDto>> GetProductsAsync()
    {
        SetAuthHeader();
        var response = await _http.GetAsync($"{_baseUrl}/api/products");
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

    // ── 私有：對應 API 的 ServiceResult<T> JSON 結構 ─
    private class ServiceResultJson<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }
    }
}