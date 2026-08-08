using InventoryAppCloudDb.Models;
using InventoryAppCloudDb.Services;

namespace InventoryAppCloudDb.Forms;

public partial class MainForm : Form
{
    private readonly ApiService _api;

    public MainForm(ApiService api)
    {
        InitializeComponent();
        _api = api;

        // 顯示登入者
        lblUser.Text = $"  登入者：{AppSession.Username}（{AppSession.Role}）";

        // 事件訂閱
        mnuProduct.Click += (_, _) => OpenForm<productForm>(() => new productForm(_api));
        mnuPurchase.Click += (_, _) => OpenForm<PurchaseForm>(() => new PurchaseForm());
        mnuSales.Click += (_, _) => OpenForm<SalesForm>(() => new SalesForm());
        mnuLedger.Click += (_, _) => OpenForm<InventoryLedgerForm>(() => new InventoryLedgerForm());
        mnuImport.Click += (_, _) => OpenForm<ImportForm>(() => new ImportForm(_api));
        mnuLogout.Click += MnuLogout_Click;
        mnuExit.Click += (_, _) => Application.Exit();

        // 預設開啟商品管理
        Load += (_, _) => OpenForm<productForm>(() => new productForm(_api));
    }

    // ── 開啟子視窗（嵌入 Panel）────────────────────────
    private void OpenForm<T>(Func<Form> createForm) where T : Form
    {
        // 如果同類型已開啟，不重複開
        foreach (Control c in pnlContent.Controls)
        {
            if (c is T existingForm)
            {
                existingForm.BringToFront();
                lblStatus.Text = existingForm.Text;
                return;
            }
        }

        var form = createForm();
        form.TopLevel = false;           // 嵌入 Panel 必須設 false
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        form.Visible = true;

        // 移除舊的，加入新的
        pnlContent.Controls.Clear();
        pnlContent.Controls.Add(form);

        lblStatus.Text = form.Text;
    }

    // ── 登出 ─────────────────────────────────────────
    // ── 登出 ─────────────────────────────────────────
    private async void MnuLogout_Click(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show("確定要登出嗎？", "登出確認",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        try
        {
            await _api.LogoutAsync(AppSession.Token);   // ✅ 新增：真正通知伺服器撤銷 Token
        }
        catch
        {
            // 撤銷失敗不應該卡住登出流程，頂多這個舊 Token 要等自然過期
        }

        AppSession.Clear();
        var loginForm = new LoginForm(_api);
        loginForm.Show();
        this.Close();
    }
}