using InventoryAppCloudDb.DTOs;
using InventoryAppCloudDb.Services;
using System.ComponentModel;

namespace InventoryAppCloudDb.Forms;

public partial class SalesForm : Form
{
    private readonly ApiService _api = new();

    // 暫存「目前正在編輯」的明細清單
    private readonly BindingList<CreateSalesDetailDto> _details = new();

    // 商品下拉選單的資料來源
    private List<ProductDto> _products = [];

    public SalesForm()
    {
        InitializeComponent();
        dgvDetails.DataSource = _details;
        dgvDetails.AutoGenerateColumns = false;
        dgvDetails.Columns.Clear();
        dgvDetails.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "商品Id",
            DataPropertyName = "ProductId",
            Width = 70
        });
        dgvDetails.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "數量",
            DataPropertyName = "Quantity",
            Width = 70
        });
        dgvDetails.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "售價",
            DataPropertyName = "UnitPrice",
            Width = 80
        });
        
        Load += async (_, _) => await LoadDataAsync();
        btnRefresh.Click += async (_, _) => await LoadDataAsync();
        btnAddDetail.Click += BtnAddDetail_Click;
        btnRemoveDetail.Click += BtnRemoveDetail_Click;
        btnCreate.Click += async (_, _) => await BtnCreate_ClickAsync();
    }

    // ── 載入：銷貨單列表 + 商品下拉 ──────────────────
    private async Task LoadDataAsync()
    {
        try
        {
            // 載入商品清單供下拉選取
            _products = await _api.GetProductsAsync();
            cboProduct.DataSource = _products;
            cboProduct.DisplayMember = "Name";
            cboProduct.ValueMember = "Id";

            // 載入銷貨單列表
            var orders = await _api.GetSalesOrdersAsync();
            var display = orders.Select(o => new
            {
                o.Id,
                客戶 = o.Customer,
                備註 = o.Note,
                建立者 = o.CreatedBy,
                建立時間 = o.OrderDate.ToLocalTime().ToString("yyyy/MM/dd HH:mm")
            }).ToList();

            dgvOrders.DataSource = null;
            dgvOrders.DataSource = display;
        }
        catch (UnauthorizedAccessException)
        {
            MessageBox.Show("登入已過期，請重新登入。", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"載入失敗：{ex.Message}", "錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── 加入明細 ─────────────────────────────────────
    private void BtnAddDetail_Click(object? sender, EventArgs e)
    {
        if (cboProduct.SelectedItem is not ProductDto product)
        {
            MessageBox.Show("請選擇商品。", "提示"); return;
        }
        if (!int.TryParse(txtQty.Text.Trim(), out int qty) || qty <= 0)
        {
            MessageBox.Show("請輸入有效數量（正整數）。", "提示"); return;
        }
        if (!decimal.TryParse(txtPrice.Text.Trim(), out decimal price) || price < 0)
        {
            MessageBox.Show("請輸入有效售價。", "提示"); return;
        }

        _details.Add(new CreateSalesDetailDto
        {
            ProductId = product.Id,
            Quantity = qty,
            UnitPrice = price
        });

        // 清空輸入欄，方便繼續加下一筆
        txtQty.Clear();
        txtPrice.Clear();
    }

    // ── 移除明細 ─────────────────────────────────────
    private void BtnRemoveDetail_Click(object? sender, EventArgs e)
    {
        if (dgvDetails.CurrentRow == null) return;
        _details.RemoveAt(dgvDetails.CurrentRow.Index);
    }

    // ── 建立銷貨單 ────────────────────────────────────
    private async Task BtnCreate_ClickAsync()
    {
        if (string.IsNullOrWhiteSpace(txtCustomer.Text))
        {
            MessageBox.Show("請輸入客戶名稱。", "提示"); return;
        }
        if (_details.Count == 0)
        {
            MessageBox.Show("請至少加入一筆銷貨明細。", "提示"); return;
        }

        var dto = new CreateSalesOrderDto
        {
            Customer = txtCustomer.Text.Trim(),
            Details = _details.ToList()
        };

        var (success, message) = await _api.CreateSalesOrderAsync(dto);

        if (success)
        {
            MessageBox.Show("銷貨單建立成功！庫存已扣除。", "成功",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtCustomer.Clear();
            _details.Clear();
            await LoadDataAsync();  // 重新整理列表
        }
        else
        {
            MessageBox.Show($"建立失敗：{message}", "錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}