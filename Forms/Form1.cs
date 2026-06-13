using InventoryAppCloudDb.DTOs;
using InventoryAppCloudDb.Models;
using InventoryAppCloudDb.Services;
using System.ComponentModel;

namespace InventoryAppCloudDb.Forms;

public partial class Form1 : Form
{
    private readonly ApiService _api;
    private readonly BindingList<ProductDto> _products = new();
    private readonly BindingSource _bindingSource = new();

    public Form1(ApiService api)
    {
        InitializeComponent();
        _api = api;
        InitializeForm();
        _ = LoadProductsAsync();   // 非同步載入，不阻塞 UI
    }

    // ── 初始化 ────────────────────────────────────────
    private void InitializeForm()
    {
        // 顯示登入者資訊
        lblUser.Text = $"登入者：{AppSession.Username}（{AppSession.Role}）";

        // Admin 才能刪除
        btnDelete.Enabled = AppSession.IsAdmin;

        //分類初始化
        cboCategory.Items.AddRange(new[] { "飲料", "零食", "3C", "文具", "其他" });
        cboCategory.SelectedIndex = 0;

        // 設定 DataGridView
        _bindingSource.DataSource = _products;
        dgvProducts.DataSource = _bindingSource;
        dgvProducts.AutoGenerateColumns = false;
        dgvProducts.Columns.Clear();

        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colId",
            HeaderText = "編號",
            DataPropertyName = "Id",
            Width = 55
        });
        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colName",
            HeaderText = "商品名稱",
            DataPropertyName = "Name",
            Width = 160
        });
        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colPrice",
            HeaderText = "售價",
            DataPropertyName = "Price",
            Width = 90,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = "N2",
                Alignment = DataGridViewContentAlignment.MiddleRight
            }
        });
        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colStock",
            HeaderText = "庫存",
            DataPropertyName = "Stock",
            Width = 70,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight
            }
        });
        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colCategory",
            HeaderText = "分類",
            DataPropertyName = "Category",
            Width = 80
        });

        dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvProducts.ReadOnly = true;
        dgvProducts.AllowUserToAddRows = false;

        // 事件訂閱
        btnAdd.Click += btnAdd_Click;
        btnUpdate.Click += btnUpdate_Click;
        btnDelete.Click += btnDelete_Click;
        btnLogout.Click += btnLogout_Click;
        btnClear.Click += (s, e) => ClearInputs();
        btnStats.Click += btnStats_Click;
        dgvProducts.SelectionChanged += dgvProducts_SelectionChanged;
        txtSearch.TextChanged += txtSearch_TextChanged;
    }

    // ── 載入商品清單 ──────────────────────────────────
    private async Task LoadProductsAsync()
    {
        try
        {
            SetLoading(true);
            var list = await _api.GetProductsAsync();

            _products.Clear();
            foreach (var p in list)
                _products.Add(p);

            lblStatus.Text = $"共 {_products.Count} 筆商品";
        }
        catch (HttpRequestException)
        {
            MessageBox.Show("無法連線到伺服器，請確認網路", "連線錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (UnauthorizedAccessException)
        {
            RedirectToLogin();
        }
        finally
        {
            SetLoading(false);
        }
    }

    // ── 新增 ──────────────────────────────────────────
    private async void btnAdd_Click(object? sender, EventArgs e)
    {
        if (!ValidateInputs(out var dto)) return;

        var createDto = new CreateProductDto
        {
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock,
            Category = dto.Category
        };

        SetLoading(true);
        var (success, message) = await _api.CreateProductAsync(createDto);
        SetLoading(false);

        if (success)
        {
            lblStatus.Text = $"✅ 已新增：{dto.Name}";
            ClearInputs();
            await LoadProductsAsync();
        }
        else
        {
            MessageBox.Show(message, "新增失敗",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    // ── 修改 ──────────────────────────────────────────
    private async void btnUpdate_Click(object? sender, EventArgs e)
    {
        if (_bindingSource.Current is not ProductDto selected)
        {
            MessageBox.Show("請先選取要修改的商品", "提示");
            return;
        }
        if (!ValidateInputs(out var dto)) return;

        SetLoading(true);
        var (success, message) = await _api.UpdateProductAsync(selected.Id, dto);
        SetLoading(false);

        if (success)
        {
            lblStatus.Text = $"✅ 已修改：{dto.Name}";
            ClearInputs();
            await LoadProductsAsync();
        }
        else
        {
            MessageBox.Show(message, "修改失敗",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    // ── 刪除 ──────────────────────────────────────────
    private async void btnDelete_Click(object? sender, EventArgs e)
    {
        if (_bindingSource.Current is not ProductDto selected) return;

        var confirm = MessageBox.Show(
            $"確定要刪除「{selected.Name}」嗎？",
            "刪除確認",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        SetLoading(true);
        var (success, message) = await _api.DeleteProductAsync(selected.Id);
        SetLoading(false);

        if (success)
        {
            lblStatus.Text = $"🗑️ 已刪除：{selected.Name}";
            await LoadProductsAsync();
        }
        else
        {
            MessageBox.Show(message, "刪除失敗",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    // ── 選取列時填回輸入欄位 ──────────────────────────
    private void dgvProducts_SelectionChanged(object? sender, EventArgs e)
    {
        if (_bindingSource.Current is not ProductDto p) return;

        txtName.Text = p.Name;
        txtPrice.Text = p.Price.ToString();
        txtStock.Text = p.Stock.ToString();
        cboCategory.SelectedItem = p.Category;
    }

    // ── 即時搜尋 ──────────────────────────────────────
    private async void txtSearch_TextChanged(object? sender, EventArgs e)
    {
        var keyword = txtSearch.Text.Trim();
        if (string.IsNullOrEmpty(keyword))
        {
            await LoadProductsAsync();
            return;
        }

        var filtered = _products
            .Where(p => p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        p.Category.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();

        _products.Clear();
        foreach (var p in filtered)
            _products.Add(p);

        lblStatus.Text = $"搜尋「{keyword}」找到 {filtered.Count} 筆";
    }

    // ── 登出 ──────────────────────────────────────────
    private void btnLogout_Click(object? sender, EventArgs e)
    {
        AppSession.Clear();
        var loginForm = new LoginForm(_api);
        loginForm.Show();
        this.Close();
    }

    // ── 輔助方法 ──────────────────────────────────────
    private bool ValidateInputs(out UpdateProductDto dto)
    {
        dto = new UpdateProductDto();

        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("請輸入商品名稱", "驗證錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtName.Focus();
            return false;
        }
        if (!decimal.TryParse(txtPrice.Text, out var price) || price < 0)
        {
            MessageBox.Show("售價請輸入有效的正數", "驗證錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtPrice.Focus();
            return false;
        }
        if (!int.TryParse(txtStock.Text, out var stock) || stock < 0)
        {
            MessageBox.Show("庫存請輸入有效的正整數", "驗證錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtStock.Focus();
            return false;
        }

        dto.Name = txtName.Text.Trim();
        dto.Price = price;
        dto.Stock = stock;
        dto.Category = cboCategory.SelectedItem?.ToString() ?? "";
        return true;
    }

    private void ClearInputs()
    {
        txtName.Text = "";
        txtPrice.Text = "";
        txtStock.Text = "";
        cboCategory.SelectedIndex = 0;
        txtName.Focus();
    }

    private void SetLoading(bool isLoading)
    {
        btnAdd.Enabled = !isLoading;
        btnUpdate.Enabled = !isLoading;
        btnDelete.Enabled = !isLoading && AppSession.IsAdmin;
        Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
    }

    private void RedirectToLogin()
    {
        MessageBox.Show("登入已過期，請重新登入", "驗證失敗",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        AppSession.Clear();
        var loginForm = new LoginForm(_api);
        loginForm.Show();
        this.Close();
    }
    private void btnStats_Click(object? sender, EventArgs e)
    {
        var all = _products.ToList();

        if (!all.Any())
        {
            MessageBox.Show("目前沒有商品資料", "統計");
            return;
        }

        int totalKinds = all.Count;
        decimal totalValue = all.Sum(p => p.Price * p.Stock);
        int lowStockCount = all.Count(p => p.Stock < 10);
        var mostExpensive = all.OrderByDescending(p => p.Price).FirstOrDefault();

        string msg = $"""
        📊 商品統計
        ─────────────────
        商品總種數：{totalKinds} 種
        庫存總值：NT$ {totalValue:N0}
        低庫存商品（< 10件）：{lowStockCount} 種
        最貴商品：{mostExpensive?.Name ?? "無"} NT${mostExpensive?.Price:N0}
        """;

        MessageBox.Show(msg, "商品統計", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}