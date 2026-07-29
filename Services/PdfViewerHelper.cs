// Services/PdfViewerHelper.cs
namespace InventoryAppCloudDb.Services;

// ── 共用：把 PDF bytes 存成暫存檔並用系統預設程式開啟 ──
public static class PdfViewerHelper
{
    public static void SaveAndOpen(byte[] pdfBytes, string suggestedFileName)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), suggestedFileName);
        File.WriteAllBytes(tempPath, pdfBytes);

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = tempPath,
            UseShellExecute = true
        });
    }
}