using InventoryAppCloudDb.DTOs;
using InventoryAppCloudDb.Models;
using InventoryAppCloudDb.Services;
using System.ComponentModel;

namespace InventoryAppCloudDb.Forms;

public partial class PurchaseForm : Form
{
    private readonly ApiService _api = new();
    private readonly BindingList<CreatePurchaseDetailDto> _details = new();
    private List<ProductDto> _products = [];
    private List<PurchaseOrderDto> _orders = [];

    private FormMode _mode = FormMode.Initial;   // 三態：Initial / Creating / Viewing

    public PurchaseForm()
    {
        InitializeComponent();
        dgvDetails.DataSource = _details;
        dgvDetails.AutoGenerateColumns = false;
        dgvDetails.Columns.Clear();
        dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "商品Id", DataPropertyName = "ProductId", Width = 70 });
        dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "數量", DataPropertyName = "Quantity", Width = 70 });
        dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "單價", DataPropertyName = "UnitPrice", Width = 80 });

        Load += async (_, _) => await LoadDataAsync();
        btnRefresh.Click += async (_, _) => { SetMode(FormMode.Initial); await LoadDataAsync(); };
        btnAddDetail.Click += BtnAddDetail_Click;
        btnRemoveDetail.Click += BtnRemoveDetail_Click;
        btnCreate.Click += async (_, _) => await BtnSave_ClickAsync();   // btnCreate 當「儲存」
        btnNew.Click += BtnNew_Click;
        btnCancel.Click += BtnCancel_Click;
        btnVoid.Click += async (_, _) => await BtnVoid_ClickAsync();
        dgvOrders.SelectionChanged += DgvOrders_SelectionChanged;

        SetMode(FormMode.Initial);
    }

    // ── 核心：依模式切換 UI 狀態 ──────────────────────
    private void SetMode(FormMode mode)
    {
        _mode = mode;
        bool editable = mode == FormMode.Creating;

        txtSupplier.ReadOnly = !editable;
        txtNote.ReadOnly = !editable;
        cboProduct.Enabled = editable;
        txtQty.ReadOnly = !editable;
        txtCost.ReadOnly = !editable;
        btnAddDetail.Enabled = editable;
        btnRemoveDetail.Enabled = editable;

        btnNew.Visible = mode != FormMode.Creating;
        btnCreate.Visible = mode == FormMode.Creating;   // 儲存鈕
        btnCancel.Visible = mode == FormMode.Creating;

        // 作廢鈕：只有「查看模式 + Admin + 該單為 Posted」才顯示
        bool canVoid = mode == FormMode.Viewing
                    && AppSession.IsAdmin
                    && dgvOrders.CurrentRow?.Index is int idx
                    && idx >= 0 && idx < _orders.Count
                    && _orders[idx].Status == "Posted";
        btnVoid.Visible = canVoid;

        dgvOrders.Enabled = mode != FormMode.Creating;   // 新增中不可點列表
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _products = await _api.GetProductsAsync();
            cboProduct.DataSource = _products;
            cboProduct.DisplayMember = "Name";
            cboProduct.ValueMember = "Id";

            _orders = await _api.GetPurchaseOrdersAsync();
            var display = _orders.Select(o => new {
                o.Id,
                供應商 = o.Supplier,
                備註 = o.Note,
                狀態 = o.Status == "Voided" ? "已作廢" : "正常",
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

    // ── 新增：清空進入新增模式 ──────────────────────
    private void BtnNew_Click(object? sender, EventArgs e)
    {
        txtSupplier.Clear();
        txtNote.Clear();
        txtQty.Clear();
        txtCost.Clear();
        _details.Clear();
        dgvOrders.ClearSelection();
        SetMode(FormMode.Creating);
    }

    // ── 取消：捨棄變更回初始 ────────────────────────
    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        txtSupplier.Clear();
        txtNote.Clear();
        _details.Clear();
        SetMode(FormMode.Initial);
    }

    // ── 加入明細（只在 Creating 可用）────────────────
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

        _details.Add(new CreatePurchaseDetailDto { ProductId = product.Id, Quantity = qty, UnitPrice = cost });
        txtQty.Clear();
        txtCost.Clear();
    }

    private void BtnRemoveDetail_Click(object? sender, EventArgs e)
    {
        if (dgvDetails.CurrentRow == null) return;
        _details.RemoveAt(dgvDetails.CurrentRow.Index);
    }

    // ── 點列表：進入查看模式（唯讀顯示明細）──────────
    private void DgvOrders_SelectionChanged(object? sender, EventArgs e)
    {
        if (_mode == FormMode.Creating) return;   // 新增中不受列表點選影響

        if (dgvOrders.CurrentRow == null) return;
        var rowIndex = dgvOrders.CurrentRow.Index;
        if (rowIndex < 0 || rowIndex >= _orders.Count) return;

        var order = _orders[rowIndex];
        txtSupplier.Text = order.Supplier;
        txtNote.Text = order.Note;

        _details.Clear();
        foreach (var d in order.Details)
            _details.Add(new CreatePurchaseDetailDto { ProductId = d.ProductId, Quantity = d.Quantity, UnitPrice = d.UnitPrice });

        SetMode(FormMode.Viewing);
    }

    // ── 儲存（建立進貨單，只在 Creating 模式）────────
    private async Task BtnSave_ClickAsync()
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
            MessageBox.Show("進貨單建立成功！庫存已更新。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SetMode(FormMode.Initial);
            await LoadDataAsync();
        }
        else
        {
            MessageBox.Show($"建立失敗：{message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── 作廢（只在 Viewing + Admin + Posted）────────
    private async Task BtnVoid_ClickAsync()
    {
        var idx = dgvOrders.CurrentRow?.Index ?? -1;
        if (idx < 0 || idx >= _orders.Count) return;
        var order = _orders[idx];

        var reason = Microsoft.VisualBasic.Interaction.InputBox("請輸入作廢原因：", "作廢進貨單", "");
        if (string.IsNullOrWhiteSpace(reason)) return;

        var confirm = MessageBox.Show(
            $"確定要作廢「{order.Supplier}」這張進貨單嗎？\n此操作會將對應庫存扣回。",
            "作廢確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        try
        {
            var (success, message) = await _api.VoidPurchaseOrderAsync(order.Id, reason);
            if (success)
            {
                MessageBox.Show("作廢成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetMode(FormMode.Initial);
                await LoadDataAsync();
            }
            else
            {
                MessageBox.Show(message, "作廢失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (InvalidOperationException ex)   // 403
        {
            MessageBox.Show(ex.Message, "權限不足", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}