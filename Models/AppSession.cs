// Models/AppSession.cs
namespace InventoryAppCloudDb.Models;

// 靜態類別：整個程式只有一份，任何地方都能存取
// 等同於 VB6 的全域變數，但結構化
public static class AppSession
{
    public static string Token { get; set; } = "";
    public static string Username { get; set; } = "";
    public static string Role { get; set; } = "";

    public static bool IsLoggedIn => !string.IsNullOrEmpty(Token);

    public static bool IsAdmin => Role == "Admin";

    // 登出時清空
    public static void Clear()
    {
        Token = "";
        Username = "";
        Role = "";
    }
}