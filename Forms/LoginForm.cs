using InventoryAppCloudDb.Models;
using InventoryAppCloudDb.Services;

namespace InventoryAppCloudDb.Forms;

public partial class LoginForm : Form
{
    private readonly ApiService _api;

    public LoginForm(ApiService api)
    {
        InitializeComponent();
        _api = api;
    }

    // ── 登入按鈕 ──────────────────────────────────────
    private async void btnLogin_Click(object sender, EventArgs e)
    {
        // 基本驗證
        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            lblMessage.Text = "請輸入帳號";
            txtUsername.Focus();
            return;
        }
        if (string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            lblMessage.Text = "請輸入密碼";
            txtPassword.Focus();
            return;
        }

        // 避免重複點擊
        btnLogin.Enabled = false;
        lblMessage.Text = "登入中...";

        try
        {
            var (success, token, username, role, message) =
            await _api.LoginAsync(txtUsername.Text.Trim(), txtPassword.Text);

            if (success)
            {
                // 存入全域 Session
                AppSession.Token = token;
                AppSession.Username = txtUsername.Text.Trim();
                AppSession.Role = role;

                // 開啟主畫面，關閉登入畫面
                var mainForm = new Form1(_api);
                mainForm.Show();
                this.Hide();

                // 主畫面關閉時才結束程式
                mainForm.FormClosed += (s, args) => Application.Exit();
            }
            else
            {
                lblMessage.Text = message;
                txtPassword.Clear();
                txtPassword.Focus();
                btnLogin.Enabled = true;
            }
        }
        catch (HttpRequestException)
        {
            lblMessage.Text = "無法連線到伺服器，請確認網路";
            btnLogin.Enabled = true;
        }
        catch (TaskCanceledException)
        {
            lblMessage.Text = "連線逾時，請稍後再試";
            btnLogin.Enabled = true;
        }
    }

    // ── 按 Enter 觸發登入 ─────────────────────────────
    private void txtPassword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
            btnLogin_Click(sender, e);
    }

    private void txtPassword_TextChanged(object sender, EventArgs e)
    {

    }
}