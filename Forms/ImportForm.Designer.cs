namespace InventoryAppCloudDb.Forms;

partial class ImportForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        btnSelectFile = new Button
        {
            Text = "選擇 Excel 檔案",
            Location = new Point(12, 12),
            Size = new Size(120, 30)
        };

        txtFilePath = new TextBox
        {
            Location = new Point(140, 15),
            Size = new Size(500, 23),
            ReadOnly = true
        };

        lblStatus = new Label
        {
            Text = "尚未選擇檔案",
            Location = new Point(650, 18),
            AutoSize = true
        };

        tabControl1 = new TabControl
        {
            Location = new Point(12, 50),
            Size = new Size(1140, 390),   // 高度從 480 → 390，讓出空間給報告框
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        btnProcess = new Button
        {
            Text = "驗證並匯入",
            Location = new Point(12, 450),   // 往上移，跟著 tabControl1 縮小
            Size = new Size(120, 35),
            BackColor = Color.SteelBlue,
            ForeColor = Color.White,
            Font = new Font("Microsoft JhengHei", 10f, FontStyle.Bold),
            Enabled = false,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left
        };

        txtReport = new TextBox
        {
            Location = new Point(150, 450),
            Size = new Size(1000, 180),   // 高度從 90 → 180，一次看更多行報告
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true,
            Font = new Font("Consolas", 9f),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };
        components = new System.ComponentModel.Container();
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1170, 650);
        Text = "Excel 批次匯入";
        Font = new Font("Microsoft JhengHei", 9f);

        Controls.AddRange(new Control[]
        {
            btnSelectFile, txtFilePath, lblStatus,
            tabControl1, btnProcess, txtReport
        });
    }

    private Button btnSelectFile;
    private TextBox txtFilePath;
    private Label lblStatus;
    private TabControl tabControl1;
    private Button btnProcess;
    private TextBox txtReport;
}