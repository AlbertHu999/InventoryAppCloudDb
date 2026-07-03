using System.Data;
using InventoryAppCloudDb.Services;

namespace InventoryAppCloudDb.Forms;

public partial class ImportForm : Form
{
    private readonly ApiService _api;
    private string _filePath = "";

    // 記住每個 Sheet 名稱對應的 DataTable
    private readonly Dictionary<string, DataTable> _sheetData = new();

    public ImportForm(ApiService api)
    {
        InitializeComponent();
        _api = api;

        btnSelectFile.Click += btnSelectFile_Click;
        btnProcess.Click += async (_, _) => await ProcessAllSheetsAsync();
    }
    // ── 驗證並批次匯入所有 Sheet ────────────────────────
    private async Task ProcessAllSheetsAsync()
    {
        btnProcess.Enabled = false;
        var reportLines = new List<string>();

        try
        {
            // 進貨/銷貨需要對照商品名稱找 ProductId，先載入目前系統的啟用商品
            var existingProducts = await _api.GetActiveProductsAsync();

            foreach (var (sheetName, dt) in _sheetData)
            {
                // DataGridView 綁定的是同一個 DataTable，
                // 使用者在畫面上的編輯/刪除已經反映在 dt 裡了

                if (sheetName.Contains("商品"))
                {
                    var result = ExcelImportValidator.ValidateProducts(dt, existingProducts);
                    reportLines.Add($"【{sheetName}】驗證通過 {result.ValidItems.Count} 筆，錯誤 {result.Errors.Count} 筆");
                    reportLines.AddRange(result.Errors);

                    int successCount = 0;
                    foreach (var dto in result.ValidItems)
                    {
                        var (success, msg) = await _api.CreateProductAsync(dto);
                        if (success) successCount++;
                        else reportLines.Add($"  匯入失敗「{dto.Name}」：{msg}");
                    }

                    // 成功匯入後，清空這個 Sheet 的資料，避免重複送出
                    if (successCount > 0)
                        ClearSheetAfterImport(sheetName, dt);
                }
                else if (sheetName.Contains("進貨"))
                {
                    var result = ExcelImportValidator.ValidatePurchases(dt, existingProducts);
                    reportLines.Add($"【{sheetName}】驗證通過 {result.ValidItems.Count} 筆，錯誤 {result.Errors.Count} 筆");
                    reportLines.AddRange(result.Errors);

                    int successCount = 0;
                    foreach (var dto in result.ValidItems)
                    {
                        var (success, msg) = await _api.CreatePurchaseOrderAsync(dto);
                        if (success) successCount++;
                        else reportLines.Add($"  匯入失敗（供應商:{dto.Supplier}）：{msg}");
                    }

                    if (successCount > 0)
                        ClearSheetAfterImport(sheetName, dt);
                }
                else if (sheetName.Contains("銷貨"))
                {
                    var result = ExcelImportValidator.ValidateSales(dt, existingProducts);
                    reportLines.Add($"【{sheetName}】驗證通過 {result.ValidItems.Count} 筆，錯誤 {result.Errors.Count} 筆");
                    reportLines.AddRange(result.Errors);

                    int successCount = 0;
                    foreach (var dto in result.ValidItems)
                    {
                        var (success, msg) = await _api.CreateSalesOrderAsync(dto);
                        if (success)
                        {
                            successCount++;
                        }
                        else
                        {
                            reportLines.Add($"  匯入失敗（客戶:{dto.Customer}）：{msg}");
                        }
                    }
                    if (successCount > 0)
                        ClearSheetAfterImport(sheetName, dt);
                }
                else
                {
                    reportLines.Add($"【{sheetName}】無法辨識工作表類型（名稱需包含「商品」「進貨」或「銷貨」），已略過");
                }
            }

            txtReport.Text = string.Join(Environment.NewLine, reportLines);
            MessageBox.Show("匯入處理完成，請查看下方報告", "完成",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (UnauthorizedAccessException)
        {
            MessageBox.Show("登入已過期，請重新登入。", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"匯入處理發生錯誤：{ex.Message}", "錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnProcess.Enabled = true;
        }
    }
    // ── 匯入成功後清空該 Sheet 的資料列，避免重複送出同一批 ──
    private void ClearSheetAfterImport(string sheetName, DataTable dt)
    {
        dt.Rows.Clear();

        var tabPage = tabControl1.TabPages
            .Cast<TabPage>()
            .FirstOrDefault(tp => tp.Text == sheetName);

        if (tabPage != null)
            tabPage.Text = $"{sheetName}（已處理）";
    }
    // ── 選擇 Excel 檔案 ────────────────────────────────
    private void btnSelectFile_Click(object? sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog { Filter = "Excel 檔案|*.xlsx" };
        if (ofd.ShowDialog() != DialogResult.OK) return;

        _filePath = ofd.FileName;
        txtFilePath.Text = _filePath;

        try
        {
            LoadAllSheets();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"讀取檔案失敗：{ex.Message}", "錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── 讀取所有 Sheet，動態建立 Tab + DataGridView ────
    private void LoadAllSheets()
    {
        tabControl1.TabPages.Clear();
        _sheetData.Clear();

        var sheetNames = ExcelImportService.GetSheetNames(_filePath);
        int loadedCount = 0;

        foreach (var sheetName in sheetNames)
        {
            var dt = ExcelImportService.ReadSheetToDataTable(_filePath, sheetName);
            if (dt.Rows.Count == 0) continue;   // 跳過空白 Sheet

            _sheetData[sheetName] = dt;
            loadedCount++;

            var tabPage = new TabPage(sheetName);

            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                DataSource = dt,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = true,   // 使用者能刪掉不要匯入的列
                Name = $"dgv_{sheetName}",
            };

            tabPage.Controls.Add(dgv);
            tabControl1.TabPages.Add(tabPage);
        }

        lblStatus.Text = $"已載入 {loadedCount} 個工作表";
        btnProcess.Enabled = loadedCount > 0;
    }
}