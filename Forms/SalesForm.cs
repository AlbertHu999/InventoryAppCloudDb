using InventoryAppCloudDb.DTOs;
using InventoryAppCloudDb.Models;
using InventoryAppCloudDb.Services;
using System.ComponentModel;

namespace InventoryAppCloudDb.Forms;

public partial class SalesForm : Form
{
    private readonly ApiService _api = new();
    private readonly BindingList<CreateSalesDetailDto> _details = new();
    private List<ProductDto> _products = [];
    private List<SalesOrderDto> _orders = [];

    private FormMode _mode = FormMode.Initial;

    public SalesForm()
    {
        InitializeComponent();
        dgvDetails.DataSource = _details;
        dgvDetails.AutoGenerateColumns = false;
        dgvDetails.Columns.Clear();
        dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "商品Id", DataPropertyName = "ProductId", Width = 70 });
        dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "數量", DataPropertyName = "Quantity", Width = 70 });
        dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "售價", DataPropertyName = "UnitPrice", Width = 80 });

        Load += async (_, _) => await LoadDataAsync();
        btnRefresh.Click += async (_, _) => { SetMode(FormMode.Initial); await LoadDataAsync(); };
        btnAddDetail.Click += BtnAddDetail_Click;
        btnRemoveDetail.Click += BtnRemoveDetail_Click;
        btnCreate.Click += async (_, _) => await BtnSave_ClickAsync();
        btnNew.Click += BtnNew_Click;
        btnCancel.Click += BtnCancel_Click;
        btnVoid.Click += async (_, _) => await BtnVoid_ClickAsync();
        dgvOrders.SelectionChanged += DgvOrders_SelectionChanged;

        SetMode(FormMode.Initial);
    }

    private void SetMode(FormMode mode)
    {
        _mode = mode;
        bool editable = mode == FormMode.Creating;

        txtCustomer.ReadOnly = !editable;
        txtNote.ReadOnly = !editable;
        cboProduct.Enabled = editable;
        txtQty.ReadOnly = !editable;
        txtPrice.ReadOnly = !editable;
        btnAddDetail.Enabled = editable;
        btnRemoveDetail.Enabled = editable;

        btnNew.Visible = mode != FormMode.Creating;
        btnCreate.Visible = mode == FormMode.Creating;
        btnCancel.Visible = mode == FormMode.Creating;

        bool canVoid = mode == FormMode.Viewing
                    && AppSession.IsAdmin
                    && dgvOrders.CurrentRow?.Index is int idx
                    && idx >= 0 && idx < _orders.Count
                    && _orders[idx].Status == "Posted";
        btnVoid.Visible = canVoid;

        dgvOrders.Enabled = mode != FormMode.Creating;
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _products = await _api.GetProductsAsync();
            cboProduct.DataSource = _products;
            cboProduct.DisplayMember = "Name";
            cboProduct.ValueMember = "Id";

            _orders = await _api.GetSalesOrdersAsync();
            var display = _orders.Select(o => new {
                o.Id,
                客戶 = o.Customer,
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

    private void BtnNew_Click(object? sender, EventArgs e)
    {
        txtCustomer.Clear();
        txtNote.Clear();
        txtQty.Clear();
        txtPrice.Clear();
        _details.Clear();
        dgvOrders.ClearSelection();
        SetMode(FormMode.Creating);
    }

    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        txtCustomer.Clear();
        txtNote.Clear();
        _details.Clear();
        SetMode(FormMode.Initial);
    }

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

        _details.Add(new CreateSalesDetailDto { ProductId = product.Id, Quantity = qty, UnitPrice = price });
        txtQty.Clear();
        txtPrice.Clear();
    }

    private void BtnRemoveDetail_Click(object? sender, EventArgs e)
    {
        if (dgvDetails.CurrentRow == null) return;
        _details.RemoveAt(dgvDetails.CurrentRow.Index);
    }

    private void DgvOrders_SelectionChanged(object? sender, EventArgs e)
    {
        if (_mode == FormMode.Creating) return;

        if (dgvOrders.CurrentRow == null) return;
        var rowIndex = dgvOrders.CurrentRow.Index;
        if (rowIndex < 0 || rowIndex >= _orders.Count) return;

        var order = _orders[rowIndex];
        txtCustomer.Text = order.Customer;
        txtNote.Text = order.Note;

        _details.Clear();
        foreach (var d in order.Details)
            _details.Add(new CreateSalesDetailDto { ProductId = d.ProductId, Quantity = d.Quantity, UnitPrice = d.UnitPrice });

        SetMode(FormMode.Viewing);
    }

    private async Task BtnSave_ClickAsync()
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
            MessageBox.Show("銷貨單建立成功！庫存已扣除。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SetMode(FormMode.Initial);
            await LoadDataAsync();
        }
        else
        {
            MessageBox.Show($"建立失敗：{message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task BtnVoid_ClickAsync()
    {
        var idx = dgvOrders.CurrentRow?.Index ?? -1;
        if (idx < 0 || idx >= _orders.Count) return;
        var order = _orders[idx];

        var reason = Microsoft.VisualBasic.Interaction.InputBox("請輸入作廢原因：", "作廢銷貨單", "");
        if (string.IsNullOrWhiteSpace(reason)) return;

        var confirm = MessageBox.Show(
            $"確定要作廢「{order.Customer}」這張銷貨單嗎？\n此操作會將對應庫存加回。",
            "作廢確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        try
        {
            var (success, message) = await _api.VoidSalesOrderAsync(order.Id, reason);
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
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message, "權限不足", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}