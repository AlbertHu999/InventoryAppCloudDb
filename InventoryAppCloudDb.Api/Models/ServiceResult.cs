namespace InventoryAppCloudDb.Api.Models;

// Models/ServiceResult.cs
public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public T? Data { get; set; }

    // 靜態工廠方法，讓呼叫端寫起來更簡潔
    public static ServiceResult<T> Ok(T data)
        => new() { Success = true, Data = data };

    public static ServiceResult<T> Fail(string message)
        => new() { Success = false, Message = message };
}

// 不需要回傳資料時用這個（例如刪除成功）
public class ServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";

    public static ServiceResult Ok()
        => new() { Success = true };

    public static ServiceResult Fail(string message)
        => new() { Success = false, Message = message };
}