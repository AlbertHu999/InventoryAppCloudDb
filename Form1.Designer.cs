namespace InventoryAppCloudDb
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            txtName = new TextBox();
            txtPrice = new TextBox();
            txtStock = new TextBox();
            cboCategory = new ComboBox();
            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            btnClear = new Button();
            label5 = new Label();
            txtSearch = new TextBox();
            dgvProducts = new DataGridView();
            lblStatus = new Label();
            btnStats = new Button();
            lblUser = new Label();
            btnLogout = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 33);
            label1.Name = "label1";
            label1.Size = new Size(55, 15);
            label1.TabIndex = 0;
            label1.Text = "商品名稱";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(158, 33);
            label2.Name = "label2";
            label2.Size = new Size(31, 15);
            label2.TabIndex = 1;
            label2.Text = "售價";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(282, 33);
            label3.Name = "label3";
            label3.Size = new Size(31, 15);
            label3.TabIndex = 2;
            label3.Text = "庫存";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(405, 33);
            label4.Name = "label4";
            label4.Size = new Size(31, 15);
            label4.TabIndex = 3;
            label4.Text = "分類";
            // 
            // txtName
            // 
            txtName.Location = new Point(27, 71);
            txtName.Name = "txtName";
            txtName.Size = new Size(100, 23);
            txtName.TabIndex = 4;
            // 
            // txtPrice
            // 
            txtPrice.Location = new Point(158, 71);
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(100, 23);
            txtPrice.TabIndex = 5;
            // 
            // txtStock
            // 
            txtStock.Location = new Point(282, 71);
            txtStock.Name = "txtStock";
            txtStock.Size = new Size(100, 23);
            txtStock.TabIndex = 6;
            // 
            // cboCategory
            // 
            cboCategory.FormattingEnabled = true;
            cboCategory.Location = new Point(405, 71);
            cboCategory.Name = "cboCategory";
            cboCategory.Size = new Size(121, 23);
            cboCategory.TabIndex = 7;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(39, 116);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 8;
            btnAdd.Text = "新增";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(174, 116);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(75, 23);
            btnUpdate.TabIndex = 9;
            btnUpdate.Text = "編輯";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(294, 116);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(75, 23);
            btnDelete.TabIndex = 10;
            btnDelete.Text = "刪除";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(427, 116);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(75, 23);
            btnClear.TabIndex = 11;
            btnClear.Text = "清除";
            btnClear.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(60, 171);
            label5.Name = "label5";
            label5.Size = new Size(31, 15);
            label5.TabIndex = 12;
            label5.Text = "搜尋";
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(111, 168);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(271, 23);
            txtSearch.TabIndex = 13;
            // 
            // dgvProducts
            // 
            dgvProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProducts.Location = new Point(39, 210);
            dgvProducts.Name = "dgvProducts";
            dgvProducts.RowHeadersWidth = 62;
            dgvProducts.Size = new Size(728, 275);
            dgvProducts.TabIndex = 14;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(327, 515);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(62, 15);
            lblStatus.TabIndex = 15;
            lblStatus.Text = "共0筆商品";
            // 
            // btnStats
            // 
            btnStats.Location = new Point(427, 167);
            btnStats.Name = "btnStats";
            btnStats.Size = new Size(75, 23);
            btnStats.TabIndex = 16;
            btnStats.Text = "統計";
            btnStats.UseVisualStyleBackColor = true;
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Location = new Point(711, 9);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(43, 15);
            lblUser.TabIndex = 17;
            lblUser.Text = "登入者";
            // 
            // btnLogout
            // 
            btnLogout.Location = new Point(564, 116);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(75, 23);
            btnLogout.TabIndex = 18;
            btnLogout.Text = "登出";
            btnLogout.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(871, 561);
            Controls.Add(btnLogout);
            Controls.Add(lblUser);
            Controls.Add(btnStats);
            Controls.Add(lblStatus);
            Controls.Add(dgvProducts);
            Controls.Add(txtSearch);
            Controls.Add(label5);
            Controls.Add(btnClear);
            Controls.Add(btnDelete);
            Controls.Add(btnUpdate);
            Controls.Add(btnAdd);
            Controls.Add(cboCategory);
            Controls.Add(txtStock);
            Controls.Add(txtPrice);
            Controls.Add(txtName);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)dgvProducts).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox txtName;
        private TextBox txtPrice;
        private TextBox txtStock;
        private ComboBox cboCategory;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;
        private Label label5;
        private TextBox txtSearch;
        private DataGridView dgvProducts;
        private Label lblStatus;
        private Button btnStats;
        private Label lblUser;
        private Button btnLogout;
    }
}
