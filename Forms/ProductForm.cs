using InventoryAppCloudDb.DTOs;
using InventoryAppCloudDb.Models;
using InventoryAppCloudDb.Services;
using System.ComponentModel;

namespace InventoryAppCloudDb.Forms;

public partial class productForm : Form
{
    private readonly ApiService _api;
    private readonly BindingList<ProductDto> _products = new();
    private readonly BindingSource _bindingSource = new();

    public productForm(ApiService api)
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
        //btnDelete.Enabled = AppSession.IsAdmin;
        // 停用/啟用鈕的可用性由 UpdateActiveButtons() 統一控制
        // （同時考量 Admin 權限 + 商品目前狀態）

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
        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colIsActive",
            HeaderText = "狀態",
            DataPropertyName = "IsActive",
            Width = 70,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            }
        });

        dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvProducts.ReadOnly = true;
        dgvProducts.AllowUserToAddRows = false;

        // 事件訂閱
        btnAdd.Click += btnAdd_Click;
        btnUpdate.Click += btnUpdate_Click;
        btnDelete.Click += btnDelete_Click;
        btnActivate.Click += btnActivate_Click;
        btnExportExcel.Click += btnExportExcel_Click;
        //btnLogout.Click += btnLogout_Click;
        btnLogout.Visible = false;   // 登出改由主選單負責
        btnClear.Click += (s, e) => ClearInputs();
        btnStats.Click += btnStats_Click;
        dgvProducts.SelectionChanged += dgvProducts_SelectionChanged;
        dgvProducts.CellFormatting += DgvProducts_CellFormatting;
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
            UpdateActiveButtons();   // ← 載入後依第一筆狀態更新按鈕
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

    // ── 把 IsActive 的 True/False 顯示成「啟用/停用」──
    private void DgvProducts_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (dgvProducts.Columns[e.ColumnIndex].Name == "colIsActive" && e.Value is bool isActive)
        {
            e.Value = isActive ? "啟用" : "停用";
            e.FormattingApplied = true;
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
    // ── 停用商品（原刪除，改為停用，保留歷史記錄）──
    private async void btnDelete_Click(object? sender, EventArgs e)
    {
        if (_bindingSource.Current is not ProductDto selected) return;

        var confirm = MessageBox.Show(
            $"確定要停用「{selected.Name}」嗎？\n（不會刪除歷史記錄，只是不再顯示於新增單據的選單中）",
            "停用確認",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        SetLoading(true);
        try
        {
            var (success, message) = await _api.DeactivateProductAsync(selected.Id);

            if (success)
            {
                lblStatus.Text = $"✅ 已停用：{selected.Name}";
                await LoadProductsAsync();
            }
            else
            {
                MessageBox.Show(message, "停用失敗",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (InvalidOperationException ex)   // 403 沒權限
        {
            MessageBox.Show(ex.Message, "權限不足",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (UnauthorizedAccessException)    // 401 登入過期
        {
            MessageBox.Show("登入已過期，請重新登入。", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            SetLoading(false);
        }
    }

    // ── 啟用商品 ──────────────────────────────────────
    // ── 啟用商品 ──────────────────────────────────────
    private async void btnActivate_Click(object? sender, EventArgs e)
    {
        if (_bindingSource.Current is not ProductDto selected) return;

        var confirm = MessageBox.Show(
            $"確定要啟用「{selected.Name}」嗎？\n（啟用後將重新顯示於新增單據的選單中）",
            "啟用確認",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        SetLoading(true);
        try
        {
            var (success, message) = await _api.ActivateProductAsync(selected.Id);

            if (success)
            {
                lblStatus.Text = $"✅ 已啟用：{selected.Name}";
                await LoadProductsAsync();
            }
            else
            {
                MessageBox.Show(message, "啟用失敗",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (InvalidOperationException ex)   // 403 沒權限
        {
            MessageBox.Show(ex.Message, "權限不足",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (UnauthorizedAccessException)    // 401 登入過期
        {
            MessageBox.Show("登入已過期，請重新登入。", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            SetLoading(false);
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
        UpdateActiveButtons();   // ← 新增：選擇變動時更新停用/啟用鈕狀態
    }
    // ── 依選中商品狀態 + Admin 權限，切換停用/啟用鈕可用性 ──
    private void UpdateActiveButtons()
    {
        bool isAdmin = AppSession.IsAdmin;

        if (_bindingSource.Current is ProductDto selected)
        {
            // 除錯用，確認後刪掉
            System.Diagnostics.Debug.WriteLine($"[UpdateActiveButtons] 選中={selected.Name}, IsActive={selected.IsActive}, Admin={isAdmin}");

            // 必須是 Admin，且：啟用中→可停用、停用中→可啟用
            btnDelete.Enabled = isAdmin && selected.IsActive;    // 停用鈕
            btnActivate.Enabled = isAdmin && !selected.IsActive;   // 啟用鈕
        }
        else
        {
            btnDelete.Enabled = false;
            btnActivate.Enabled = false;
        }
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
    //private void btnLogout_Click(object? sender, EventArgs e)
    //{
    //    AppSession.Clear();
    //    var loginForm = new LoginForm(_api);
    //    loginForm.Show();
    //    this.Close();
    //}

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
        Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;

        if (isLoading)
        {
            // 載入中：停用/啟用鈕都鎖住
            btnDelete.Enabled = false;
            btnActivate.Enabled = false;
        }
        else
        {
            // 載入完成：交給 UpdateActiveButtons 依「權限+商品狀態」決定
            UpdateActiveButtons();
        }
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
    // ── 匯出商品庫存表 Excel ──────────────────────────
    private void btnExportExcel_Click(object? sender, EventArgs e)
    {
        if (_products.Count == 0)
        {
            MessageBox.Show("目前沒有商品資料可匯出。", "提示");
            return;
        }

        using var sfd = new SaveFileDialog
        {
            Filter = "Excel 檔案|*.xlsx",
            FileName = $"商品庫存表_{DateTime.Now:yyyyMMdd}.xlsx"
        };

        if (sfd.ShowDialog() != DialogResult.OK) return;

        try
        {
            ExcelExportService.ExportProducts(_products.ToList(), sfd.FileName);
            ShowExportSuccess(sfd.FileName);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"匯出失敗：{ex.Message}", "錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── 共用：匯出成功後詢問是否開啟檔案 ──────────────
    private void ShowExportSuccess(string filePath)
    {
        var open = MessageBox.Show("匯出成功！是否立即開啟檔案？", "完成",
            MessageBoxButtons.YesNo, MessageBoxIcon.Information);

        if (open == DialogResult.Yes)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true   // 用系統預設程式（Excel）開啟
            });
        }
    }
}