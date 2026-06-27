using InventoryAppCloudDb.DTOs;
using InventoryAppCloudDb.Services;

namespace InventoryAppCloudDb.Forms;

public partial class InventoryLedgerForm : Form
{
    private readonly ApiService _api = new();

    public InventoryLedgerForm()
    {
        InitializeComponent();
        SetupGrid();
        btnRefresh.Click += async (_, _) => await LoadDataAsync();
        Load += async (_, _) => await LoadDataAsync();
    }

    private void SetupGrid()
    {
        dgvLedgers.AutoGenerateColumns = false;
        dgvLedgers.Columns.Clear();
        dgvLedgers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "時間", Name = "colTime", Width = 130 });
        dgvLedgers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "商品", Name = "colProduct", Width = 140 });
        dgvLedgers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "來源", Name = "colSource", Width = 110 });
        dgvLedgers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "方向", Name = "colDir", Width = 60 });
        dgvLedgers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "數量", Name = "colQty", Width = 60 });
        dgvLedgers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "單價", Name = "colPrice", Width = 80 });
        dgvLedgers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "單號", Name = "colOrder", Width = 60 });
        dgvLedgers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "操作者", Name = "colUser", Width = 80 });
        dgvLedgers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "備註", Name = "colRemark", Width = 220 });
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var ledgers = await _api.GetInventoryLedgersAsync();
            dgvLedgers.Rows.Clear();
            foreach (var l in ledgers)
            {
                var rowIdx = dgvLedgers.Rows.Add(
                    l.CreatedAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm"),
                    l.ProductName,
                    l.SourceType,
                    l.Direction == "In" ? "入庫" : "出庫",
                    l.Quantity,
                    l.UnitPrice.ToString("N2"),
                    l.SourceOrderId,
                    l.CreatedBy,
                    l.Remark ?? "");

                // In 綠色、Out 紅色，一眼看出進出
                dgvLedgers.Rows[rowIdx].DefaultCellStyle.ForeColor =
                    l.Direction == "In" ? Color.DarkGreen : Color.DarkRed;
            }
        }
        catch (UnauthorizedAccessException)
        {
            MessageBox.Show("登入已過期，請重新登入。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"載入失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}