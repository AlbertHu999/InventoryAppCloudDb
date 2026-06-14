namespace InventoryAppCloudDb.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        // ── MenuStrip ──
        menuStrip = new MenuStrip();

        mnuProduct = new ToolStripMenuItem("商品管理(&P)");
        mnuPurchase = new ToolStripMenuItem("進貨管理(&I)");
        mnuSales = new ToolStripMenuItem("銷貨管理(&S)");
        mnuSystem = new ToolStripMenuItem("系統(&Y)");
        mnuLogout = new ToolStripMenuItem("登出");
        mnuExit = new ToolStripMenuItem("結束程式");

        mnuSystem.DropDownItems.Add(mnuLogout);
        mnuSystem.DropDownItems.Add(new ToolStripSeparator());
        mnuSystem.DropDownItems.Add(mnuExit);

        menuStrip.Items.AddRange(new ToolStripItem[]
        {
            mnuProduct,
            mnuPurchase,
            mnuSales,
            mnuSystem
        });

        // ── 狀態列 ──
        statusStrip = new StatusStrip();
        lblUser = new ToolStripStatusLabel();
        lblStatus = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleCenter };
        statusStrip.Items.AddRange(new ToolStripItem[] { lblUser, lblStatus });

        // ── Panel 主內容區 ──
        pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.WhiteSmoke
        };

        // ── Form 設定 ──
        components = new System.ComponentModel.Container();
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1024, 640);
        Text = "進銷存系統";
        Font = new Font("Microsoft JhengHei", 9f);
        StartPosition = FormStartPosition.CenterScreen;
        MainMenuStrip = menuStrip;

        Controls.Add(pnlContent);
        Controls.Add(menuStrip);
        Controls.Add(statusStrip);
    }

    private MenuStrip menuStrip;
    private ToolStripMenuItem mnuProduct, mnuPurchase, mnuSales;
    private ToolStripMenuItem mnuSystem, mnuLogout, mnuExit;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblUser, lblStatus;
    private Panel pnlContent;
}