using InventoryAppCloudDb.Services;

namespace InventoryAppCloudDb;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // 建立 ApiService 單一實例，之後所有 Form 共用
        var apiService = new ApiService();

        // 先開登入畫面
        Application.Run(new LoginForm(apiService));
        //Application.Run(new Form1());
    }
}