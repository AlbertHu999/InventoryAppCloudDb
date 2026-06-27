namespace InventoryAppCloudDb.Forms;

partial class PurchaseForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        // ── 主表 DataGridView ──
        dgvOrders = new DataGridView();
        dgvOrders.Location = new Point(12, 50);
        dgvOrders.Size = new Size(560, 480);
        dgvOrders.ReadOnly = true;
        dgvOrders.AllowUserToAddRows = false;
        dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvOrders.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;

        // ── 右側：供應商 ──
        lblSupplier = new Label { Text = "供應商：", Location = new Point(590, 50), AutoSize = true };
        txtSupplier = new TextBox { Location = new Point(660, 47), Width = 200 };

        // ── 右側：明細標題 ──
        lblDetail = new Label { Text = "進貨明細：", Location = new Point(590, 90), AutoSize = true };

        // ── 商品下拉 ──
        lblProduct = new Label { Text = "商品：", Location = new Point(590, 120), AutoSize = true };
        cboProduct = new ComboBox
        {
            Location = new Point(640, 117),
            Width = 160,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        // ── 數量 ──
        lblQty = new Label { Text = "數量：", Location = new Point(590, 155), AutoSize = true };
        txtQty = new TextBox { Location = new Point(640, 152), Width = 80 };

        // ── 單價 ──
        lblCost = new Label { Text = "單價：", Location = new Point(590, 185), AutoSize = true };
        txtCost = new TextBox { Location = new Point(640, 182), Width = 80 };

        // ── 加入明細按鈕 ──
        btnAddDetail = new Button
        {
            Text = "+ 加入明細",
            Location = new Point(730, 150),
            Size = new Size(100, 55)
        };

        // ── 明細 DataGridView ──
        dgvDetails = new DataGridView();
        dgvDetails.Location = new Point(590, 245);
        dgvDetails.Size = new Size(560, 195); dgvDetails.AllowUserToAddRows = false;
        dgvDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        // ── 移除明細按鈕 ──
        btnRemoveDetail = new Button
        {
            Text = "移除選取明細",
            Location = new Point(590, 450),
            Size = new Size(130, 30)
        };

        // ── 建立進貨單按鈕 ──
        btnCreate = new Button
        {
            Text = "建立進貨單",
            Location = new Point(1010, 450),
            Size = new Size(140, 35),
            BackColor = Color.SteelBlue,
            ForeColor = Color.White,
            Font = new Font("Microsoft JhengHei", 10f, FontStyle.Bold)
        };

        // ── 重新整理按鈕 ──
        btnRefresh = new Button
        {
            Text = "重新整理",
            Location = new Point(12, 15),
            Size = new Size(100, 28)
        };

        // ── 備註 ──
        lblNote = new Label { Text = "備註：", Location = new Point(590, 215), AutoSize = true };
        txtNote = new TextBox { Location = new Point(660, 212), Width = 200 };

        // ── 新增按鈕 ──
        btnNew = new Button
        {
            Text = "新增進貨單",
            Location = new Point(870, 450),
            Size = new Size(120, 35),
            BackColor = Color.ForestGreen,
            ForeColor = Color.White,
            Font = new Font("Microsoft JhengHei", 10f, FontStyle.Bold)
        };

        // ── 取消按鈕 ──
        btnCancel = new Button
        {
            Text = "取消",
            Location = new Point(870, 450),
            Size = new Size(120, 35),
            Visible = false
        };

        // ── 作廢按鈕 ──
        btnVoid = new Button
        {
            Text = "作廢此單",
            Location = new Point(730, 450),
            Size = new Size(120, 35),
            BackColor = Color.Firebrick,
            ForeColor = Color.White,
            Font = new Font("Microsoft JhengHei", 10f, FontStyle.Bold),
            Visible = false
        };

        // ── Form 設定 ──
        components = new System.ComponentModel.Container();
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1180, 560);
        Text = "進貨單管理";
        Font = new Font("Microsoft JhengHei", 9f);

        Controls.AddRange(new Control[]
                {
            dgvOrders, btnRefresh,
            lblSupplier, txtSupplier,
            lblNote, txtNote,
            lblDetail,
            lblProduct, cboProduct,
            lblQty, txtQty,
            lblCost, txtCost,
            btnAddDetail,
            dgvDetails, btnRemoveDetail,
            btnCreate, btnNew, btnCancel, btnVoid
                });
    }

    // ── 控制項宣告 ──
    private DataGridView dgvOrders;
    private DataGridView dgvDetails;
    private Label lblSupplier, lblNote, lblDetail, lblProduct, lblQty, lblCost;
    private TextBox txtSupplier, txtNote, txtQty, txtCost;
    private ComboBox cboProduct;
    private Button btnAddDetail, btnRemoveDetail, btnCreate, btnRefresh, btnNew, btnCancel, btnVoid;
}