namespace InventoryAppCloudDb;

partial class ProductDetailForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        label1 = new Label();
        txtName = new TextBox();
        label2 = new Label();
        txtPrice = new TextBox();
        btnSave = new Button();
        btnCancel = new Button();
        SuspendLayout();
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(53, 11);
        label1.Name = "label1";
        label1.Size = new Size(55, 15);
        label1.TabIndex = 0;
        label1.Text = "商品名稱";
        // 
        // txtName
        // 
        txtName.Location = new Point(114, 8);
        txtName.Name = "txtName";
        txtName.Size = new Size(100, 23);
        txtName.TabIndex = 1;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(53, 40);
        label2.Name = "label2";
        label2.Size = new Size(31, 15);
        label2.TabIndex = 2;
        label2.Text = "售價";
        // 
        // txtPrice
        // 
        txtPrice.Location = new Point(114, 40);
        txtPrice.Name = "txtPrice";
        txtPrice.Size = new Size(100, 23);
        txtPrice.TabIndex = 3;

        // 
        // btnSave
        // 
        btnSave.Location = new Point(53, 89);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(75, 23);
        btnSave.TabIndex = 4;
        btnSave.Text = "儲存";
        btnSave.UseVisualStyleBackColor = true;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(160, 89);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(75, 23);
        btnCancel.TabIndex = 6;
        btnCancel.Text = "取消";
        btnCancel.UseVisualStyleBackColor = true;
        // 
        // ProductDetailForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(284, 161);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(txtPrice);
        Controls.Add(label2);
        Controls.Add(txtName);
        Controls.Add(label1);
        Name = "ProductDetailForm";
        Text = "編輯商品";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label label1;
    private TextBox txtName;
    private Label label2;
    private TextBox txtPrice;
    private Button btnSave;
    private Button btnCancel;
}