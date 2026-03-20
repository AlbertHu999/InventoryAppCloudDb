#nullable disable
namespace InventoryAppLocalDb_EF;

public partial class ProductDetailForm : Form
{
    public Product EditedProduct { get; private set; }
    private Product _original;

    public ProductDetailForm(Product product)
    {
        InitializeComponent();
        _original = product;

        // 填入原始資料
        txtName.Text = product.Name;
        txtPrice.Text = product.Price.ToString();

        // 綁定事件
        btnSave.Click += btnSave_Click;
        btnCancel.Click += btnCancel_Click;
    }

    private void btnSave_Click(object sender, EventArgs e)
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

        EditedProduct = new Product(
            _original.Id,
            txtName.Text.Trim(),
            price,
            _original.Stock,
            _original.Category
        );

        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}