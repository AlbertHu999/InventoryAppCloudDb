// Services/ExcelImportValidator.cs
using System.Data;
using InventoryAppCloudDb.DTOs;

namespace InventoryAppCloudDb.Services;

// ── 驗證結果容器：有效資料 + 錯誤清單 ──────────────────
public class ImportValidationResult<T>
{
    public List<T> ValidItems { get; } = new();
    public List<string> Errors { get; } = new();
}

public static class ExcelImportValidator
{
    // ── 驗證並轉換「商品」Sheet ─────────────────────────
    // 預期欄位：商品名稱、售價、庫存、分類
    // ── 驗證並轉換「商品」Sheet ─────────────────────────
    // 預期欄位：商品名稱、售價、庫存、分類
    public static ImportValidationResult<CreateProductDto> ValidateProducts(
            DataTable dt, List<ProductDto> allExistingProducts)   // ← 參數名稱改清楚：全部商品（含停用）
    {
        var result = new ImportValidationResult<CreateProductDto>();

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            var row = dt.Rows[i];
            var rowNum = i + 2;
            var errors = new List<string>();

            var name = row.Table.Columns.Contains("商品名稱")
                ? row["商品名稱"]?.ToString()?.Trim() ?? ""
                : "";
            if (string.IsNullOrEmpty(name))
                errors.Add("商品名稱不可空白");

            // ── 用「全部商品」（含停用）判斷是否重複，不受啟用狀態影響 ──
            else
            {
                var duplicate = allExistingProducts.FirstOrDefault(p => p.Name == name);
                if (duplicate != null)
                {
                    var statusText = duplicate.IsActive ? "啟用中" : "已停用";
                    errors.Add($"商品名稱「{name}」已存在（狀態：{statusText}），略過此列，如需修改請至商品管理編輯");
                }
            }

            var priceText = row.Table.Columns.Contains("售價") ? row["售價"]?.ToString() : null;
            if (!decimal.TryParse(priceText, out var price) || price < 0)
                errors.Add("售價格式錯誤或為負數");

            var stockText = row.Table.Columns.Contains("庫存") ? row["庫存"]?.ToString() : null;
            if (!int.TryParse(stockText, out var stock) || stock < 0)
                errors.Add("庫存格式錯誤或為負數");

            var category = row.Table.Columns.Contains("分類")
                ? row["分類"]?.ToString()?.Trim() ?? ""
                : "";

            if (errors.Count > 0)
            {
                result.Errors.Add($"第 {rowNum} 列：{string.Join("；", errors)}");
                continue;
            }

            result.ValidItems.Add(new CreateProductDto
            {
                Name = name,
                Price = price,
                Stock = stock,
                Category = category
            });
        }

        return result;
    }
    // ── 驗證並轉換「進貨」Sheet ─────────────────────────
    // 預期欄位：供應商、商品名稱、數量、單價
    // 每列視為「一張單一明細的進貨單」（簡化版，不合併同供應商）
    public static ImportValidationResult<CreatePurchaseOrderDto> ValidatePurchases(
        DataTable dt, List<ProductDto> existingProducts)
    {
        var result = new ImportValidationResult<CreatePurchaseOrderDto>();

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            var row = dt.Rows[i];
            var rowNum = i + 2;
            var errors = new List<string>();

            var supplier = row.Table.Columns.Contains("供應商")
                ? row["供應商"]?.ToString()?.Trim() ?? ""
                : "";
            var productName = row.Table.Columns.Contains("商品名稱")
                ? row["商品名稱"]?.ToString()?.Trim() ?? ""
                : "";

            var product = existingProducts.FirstOrDefault(p => p.Name == productName);
            if (product == null)
                errors.Add($"找不到商品「{productName}」，請確認名稱是否與系統一致");

            var qtyText = row.Table.Columns.Contains("數量") ? row["數量"]?.ToString() : null;
            if (!int.TryParse(qtyText, out var qty) || qty <= 0)
                errors.Add("數量格式錯誤或必須大於 0");

            var priceText = row.Table.Columns.Contains("單價") ? row["單價"]?.ToString() : null;
            if (!decimal.TryParse(priceText, out var price) || price < 0)
                errors.Add("單價格式錯誤或為負數");

            if (errors.Count > 0)
            {
                result.Errors.Add($"第 {rowNum} 列：{string.Join("；", errors)}");
                continue;
            }

            result.ValidItems.Add(new CreatePurchaseOrderDto
            {
                Supplier = supplier,
                Details = new List<CreatePurchaseDetailDto>
                {
                    new() { ProductId = product!.Id, Quantity = qty, UnitPrice = price }
                }
            });
        }

        return result;
    }

    // ── 驗證並轉換「銷貨」Sheet ─────────────────────────
    // 預期欄位：客戶、商品名稱、數量、單價
    public static ImportValidationResult<CreateSalesOrderDto> ValidateSales(
        DataTable dt, List<ProductDto> existingProducts)
    {
        var result = new ImportValidationResult<CreateSalesOrderDto>();

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            var row = dt.Rows[i];
            var rowNum = i + 2;
            var errors = new List<string>();

            var customer = row.Table.Columns.Contains("客戶")
                ? row["客戶"]?.ToString()?.Trim() ?? ""
                : "";
            var productName = row.Table.Columns.Contains("商品名稱")
                ? row["商品名稱"]?.ToString()?.Trim() ?? ""
                : "";

            var product = existingProducts.FirstOrDefault(p => p.Name == productName);
            if (product == null)
                errors.Add($"找不到商品「{productName}」");

            var qtyText = row.Table.Columns.Contains("數量") ? row["數量"]?.ToString() : null;
            if (!int.TryParse(qtyText, out var qty) || qty <= 0)
                errors.Add("數量格式錯誤或必須大於 0");

            var priceText = row.Table.Columns.Contains("單價") ? row["單價"]?.ToString() : null;
            if (!decimal.TryParse(priceText, out var price) || price < 0)
                errors.Add("單價格式錯誤或為負數");

            if (errors.Count > 0)
            {
                result.Errors.Add($"第 {rowNum} 列：{string.Join("；", errors)}");
                continue;
            }

            result.ValidItems.Add(new CreateSalesOrderDto
            {
                Customer = customer,
                Details = new List<CreateSalesDetailDto>
                {
                    new() { ProductId = product!.Id, Quantity = qty, UnitPrice = price }
                }
            });
        }

        return result;
    }
}