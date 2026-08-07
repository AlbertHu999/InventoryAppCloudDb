using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetByCategoryAsync(string category);
    Task<int> InsertAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task UpdateStockAsync(int productId, int newStock);   // ← Phase 5.5 新增
    Task UpdateActiveStatusAsync(int productId, bool isActive);
    Task<List<Product>> GetActiveAsync();
    // 相對增減庫存（原子操作，避免併發時的 Lost Update 問題）
    // delta 為正 = 入庫，為負 = 出庫
 
    // 相對增加庫存（永遠安全，用於入庫、作廢銷貨回沖等只增不減的情境）
    Task AdjustStockAsync(int productId, int delta);

    // 嘗試原子扣減庫存：只有在「當下庫存足夠」時才會真正扣減，回傳是否成功
    // 這是防止併發超賣的關鍵方法——檢查與扣減在資料庫層級合而為一，不會有競態空隙
    Task<bool> TryDecreaseStockAsync(int productId, int quantity);
}