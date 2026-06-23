using InventoryAppCloudDb.DTOs;
using InventoryAppCloudDb.Services;
using System.ComponentModel;

namespace InventoryAppCloudDb.Forms;

public partial class PurchaseForm : Form
{
    private readonly ApiService _api = new();

    // 暫存「目前正在編輯」的明細清單
    private readonly BindingList<CreatePurchaseDetailDto> _details = new();

    // 商品下拉選單的資料來源
    private List<ProductDto> _products = [];
 
    // 目前載入的進貨單清單（供點擊查看明細用）
    private List<PurchaseOrderDto> _orders = [];

    public PurchaseForm()
    {
        InitializeComponent();
        dgvDetails.DataSource = _details;
        // 明細欄位中文標題
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
            HeaderText = "單價",
            DataPropertyName = "UnitPrice",
            Width = 80
        });

        Load += async (_, _) => await LoadDataAsync();
        btnRefresh.Click += async (_, _) => await LoadDataAsync();
        btnAddDetail.Click += BtnAddDetail_Click;
        btnRemoveDetail.Click += BtnRemoveDetail_Click;
        btnCreate.Click += async (_, _) => await BtnCreate_ClickAsync();
        dgvOrders.SelectionChanged += DgvOrders_SelectionChanged;
    }

    // ── 載入：進貨單列表 + 商品下拉 ──────────────────
    private async Task LoadDataAsync()
    {
        try
        {
            // 載入商品清單供下拉選取
            _products = await _api.GetProductsAsync();
            cboProduct.DataSource = _products;
            cboProduct.DisplayMember = "Name";
            cboProduct.ValueMember = "Id";

            // 載入進貨單列表
            _orders = await _api.GetPurchaseOrdersAsync();
            var display = _orders.Select(o => new {
                o.Id,
                供應商 = o.Supplier,
                備註 = o.Note,
                建立者 = o.CreatedBy,
                建立時間 = o.OrderDate.ToLocalTime().ToString("yyyy/MM/dd HH:mm")
            }).ToList();

            dgvOrders.DataSource = null;
            dgvOrders.DataSource = display;
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
        if (!decimal.TryParse(txtCost.Text.Trim(), out decimal cost) || cost < 0)
        {
            MessageBox.Show("請輸入有效單價。", "提示"); return;
        }

        _details.Add(new CreatePurchaseDetailDto
        {
            ProductId = product.Id,
            Quantity = qty,
            UnitPrice = cost
        });

        // 清空輸入欄，方便繼續加下一筆
        txtQty.Clear();
        txtCost.Clear();
    }

    // ── 移除明細 ─────────────────────────────────────
    private void BtnRemoveDetail_Click(object? sender, EventArgs e)
    {
        if (dgvDetails.CurrentRow == null) return;
        _details.RemoveAt(dgvDetails.CurrentRow.Index);
    }

    // ── 點擊左側列表：把該筆明細顯示到右側明細欄 ──────
    private void DgvOrders_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvOrders.CurrentRow == null) return;

        var rowIndex = dgvOrders.CurrentRow.Index;
        if (rowIndex < 0 || rowIndex >= _orders.Count) return;

        var selectedOrder = _orders[rowIndex];

        // 把該筆進貨單的明細，轉成右側可顯示的格式
        _details.Clear();
        foreach (var d in selectedOrder.Details)
        {
            _details.Add(new CreatePurchaseDetailDto
            {
                ProductId = d.ProductId,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice
            });
        }

        // 同時把供應商名稱顯示出來（方便核對，但不能修改既有單）
        txtSupplier.Text = selectedOrder.Supplier;
    }

    // ── 建立進貨單 ────────────────────────────────────
    private async Task BtnCreate_ClickAsync()
    {
        if (string.IsNullOrWhiteSpace(txtSupplier.Text))
        {
            MessageBox.Show("請輸入供應商名稱。", "提示"); return;
        }
        if (_details.Count == 0)
        {
            MessageBox.Show("請至少加入一筆進貨明細。", "提示"); return;
        }

        var dto = new CreatePurchaseOrderDto
        {
            Supplier = txtSupplier.Text.Trim(),
            Details = _details.ToList()
        };

        var (success, message) = await _api.CreatePurchaseOrderAsync(dto);

        if (success)
        {
            MessageBox.Show("進貨單建立成功！庫存已更新。", "成功",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtSupplier.Clear();
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