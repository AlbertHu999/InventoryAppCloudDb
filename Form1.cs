#nullable disable
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Windows.Forms;

namespace InventoryAppLocalDb_EF;

    public partial class Form1 : Form
    {
        private readonly IProductRepository _repo;
        private BindingList<Product> _products = new BindingList<Product>();
        private BindingSource _bindingSource = new BindingSource();

    public Form1()
    {
        InitializeComponent();

        // 從 appsettings.json 讀取連線字串
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connStr = config.GetConnectionString("Default")!;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connStr)
            .Options;

        var context = new AppDbContext(options);
        _repo = new EFProductRepository(context);

        InitializeForm();
    }

    private void InitializeForm()
        {
            cboCategory.Items.AddRange(new[] { "飲料", "零食", "3C", "文具", "其他" });
            cboCategory.SelectedIndex = 0;
            SetupDataGridView();

            _bindingSource.DataSource = _products;
            dgvProducts.DataSource = _bindingSource;

            LoadProducts();  // ← 改成從資料庫載入

            btnAdd.Click += btnAdd_Click;
            btnClear.Click += btnClear_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
            dgvProducts.SelectionChanged += dgvProducts_SelectionChanged;
            txtSearch.TextChanged += txtSearch_TextChanged;
        }

        private void SetupDataGridView()
        {
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.Columns.Clear();
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colId",
                HeaderText = "編號",
                DataPropertyName = "Id",
                Width = 50
            });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colName",
                HeaderText = "商品名稱",
                DataPropertyName = "Name",
                Width = 150
            });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPrice",
                HeaderText = "售價",
                DataPropertyName = "Price",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colStock",
                HeaderText = "庫存",
                DataPropertyName = "Stock",
                Width = 60,
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
                Width = 70
            });
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.ReadOnly = true;
            dgvProducts.AllowUserToAddRows = false;
        }

        // ↓ 原本的 LoadSampleData() 改成這個
        private void LoadProducts()
        {
            _products.Clear();
            foreach (var p in _repo.GetAll())
                _products.Add(p);
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            lblStatus.Text = $"共 {_products.Count} 筆商品";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("請輸入商品名稱！", "驗證錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }
            if (!double.TryParse(txtPrice.Text, out double price) || price < 0)
            {
                MessageBox.Show("售價請輸入有效的正數！", "驗證錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrice.Focus();
                return;
            }
            if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("庫存請輸入有效的正整數！", "驗證錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStock.Focus();
                return;
            }

            var p = new Product
            {
                Name = txtName.Text.Trim(),
                Price = price,
                Stock = stock,
                Category = cboCategory.SelectedItem?.ToString() ?? "其他"
            };

            _repo.Insert(p);      // ← 存進資料庫
            LoadProducts();       // ← 重新從資料庫撈，畫面更新
            ClearInputs();
            lblStatus.Text = $"✅ 已新增：{p.Name}";
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (_bindingSource.Current is not Product selected) return;

            using var detailForm = new ProductDetailForm(selected);
            if (detailForm.ShowDialog() == DialogResult.OK)
            {
                var edited = detailForm.EditedProduct;
                edited.Id = selected.Id;  // ← 確保 id 帶過去

                _repo.Update(edited);     // ← 更新資料庫
                LoadProducts();           // ← 重新撈
                lblStatus.Text = $"✅ 已編輯：{edited.Name}";
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_bindingSource.Current is not Product selected) return;

            var confirm = MessageBox.Show(
                $"確定要刪除「{selected.Name}」嗎？",
                "刪除確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.Yes)
            {
                _repo.Delete(selected.Id);  // ← 從資料庫刪除
                LoadProducts();             // ← 重新撈
                lblStatus.Text = $"🗑️ 已刪除：{selected.Name}";
            }
        }

        private void btnClear_Click(object sender, EventArgs e) => ClearInputs();

        private void ClearInputs()
        {
            txtName.Text = "";
            txtPrice.Text = "";
            txtStock.Text = "";
            cboCategory.SelectedIndex = 0;
            txtName.Focus();
        }

        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (_bindingSource.Current is Product p)
            {
                txtName.Text = p.Name;
                txtPrice.Text = p.Price.ToString();
                txtStock.Text = p.Stock.ToString();
                cboCategory.SelectedItem = p.Category;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            var filtered = string.IsNullOrEmpty(keyword)
                ? _products
                : new BindingList<Product>(
                    _products.Where(p =>
                        p.Name.ToLower().Contains(keyword) ||
                        p.Category.ToLower().Contains(keyword)
                    ).ToList()
                  );

            _bindingSource.DataSource = filtered;
            lblStatus.Text = string.IsNullOrEmpty(keyword)
                ? $"共 {_products.Count} 筆商品"
                : $"搜尋「{keyword}」找到 {filtered.Count} 筆";
        }

        private void Form1_Load(object sender, EventArgs e) { }
    }
