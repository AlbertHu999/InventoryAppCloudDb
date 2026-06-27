namespace InventoryAppCloudDb.Forms;

partial class InventoryLedgerForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        btnRefresh = new Button
        {
            Text = "重新整理",
            Location = new Point(12, 12),
            Size = new Size(100, 30)
        };

        lblTitle = new Label
        {
            Text = "庫存異動流水帳（In=入庫 綠色 / Out=出庫 紅色）",
            Location = new Point(130, 18),
            AutoSize = true,
            Font = new Font("Microsoft JhengHei", 10f, FontStyle.Bold)
        };

        dgvLedgers = new DataGridView
        {
            Location = new Point(12, 55),
            Size = new Size(1140, 540),
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        components = new System.ComponentModel.Container();
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1170, 610);
        Text = "庫存流水帳";
        Font = new Font("Microsoft JhengHei", 9f);

        Controls.AddRange(new Control[] { btnRefresh, lblTitle, dgvLedgers });
    }

    private Button btnRefresh;
    private Label lblTitle;
    private DataGridView dgvLedgers;
}