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
            btnEdit = new Button();
            btnDelete = new Button();
            btnClear = new Button();
            label5 = new Label();
            txtSearch = new TextBox();
            dgvProducts = new DataGridView();
            lblStatus = new Label();
            btnStats = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(42, 51);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(82, 23);
            label1.TabIndex = 0;
            label1.Text = "商品名稱";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(248, 51);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(46, 23);
            label2.TabIndex = 1;
            label2.Text = "售價";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(443, 51);
            label3.Margin = new Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new Size(46, 23);
            label3.TabIndex = 2;
            label3.Text = "庫存";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(636, 51);
            label4.Margin = new Padding(5, 0, 5, 0);
            label4.Name = "label4";
            label4.Size = new Size(46, 23);
            label4.TabIndex = 3;
            label4.Text = "分類";
            // 
            // txtName
            // 
            txtName.Location = new Point(42, 109);
            txtName.Margin = new Padding(5);
            txtName.Name = "txtName";
            txtName.Size = new Size(155, 30);
            txtName.TabIndex = 4;
            // 
            // txtPrice
            // 
            txtPrice.Location = new Point(248, 109);
            txtPrice.Margin = new Padding(5);
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(155, 30);
            txtPrice.TabIndex = 5;
            // 
            // txtStock
            // 
            txtStock.Location = new Point(443, 109);
            txtStock.Margin = new Padding(5);
            txtStock.Name = "txtStock";
            txtStock.Size = new Size(155, 30);
            txtStock.TabIndex = 6;
            // 
            // cboCategory
            // 
            cboCategory.FormattingEnabled = true;
            cboCategory.Location = new Point(636, 109);
            cboCategory.Margin = new Padding(5);
            cboCategory.Name = "cboCategory";
            cboCategory.Size = new Size(188, 31);
            cboCategory.TabIndex = 7;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(61, 178);
            btnAdd.Margin = new Padding(5);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(118, 35);
            btnAdd.TabIndex = 8;
            btnAdd.Text = "新增";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnEdit
            // 
            btnEdit.Location = new Point(273, 178);
            btnEdit.Margin = new Padding(5);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(118, 35);
            btnEdit.TabIndex = 9;
            btnEdit.Text = "編輯";
            btnEdit.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(462, 178);
            btnDelete.Margin = new Padding(5);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(118, 35);
            btnDelete.TabIndex = 10;
            btnDelete.Text = "刪除";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(671, 178);
            btnClear.Margin = new Padding(5);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(118, 35);
            btnClear.TabIndex = 11;
            btnClear.Text = "清除";
            btnClear.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(94, 262);
            label5.Margin = new Padding(5, 0, 5, 0);
            label5.Name = "label5";
            label5.Size = new Size(46, 23);
            label5.TabIndex = 12;
            label5.Text = "搜尋";
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(174, 258);
            txtSearch.Margin = new Padding(5);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(424, 30);
            txtSearch.TabIndex = 13;
            // 
            // dgvProducts
            // 
            dgvProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProducts.Location = new Point(42, 342);
            dgvProducts.Margin = new Padding(5);
            dgvProducts.Name = "dgvProducts";
            dgvProducts.RowHeadersWidth = 62;
            dgvProducts.Size = new Size(1144, 422);
            dgvProducts.TabIndex = 14;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(514, 790);
            lblStatus.Margin = new Padding(5, 0, 5, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(92, 23);
            lblStatus.TabIndex = 15;
            lblStatus.Text = "共0筆商品";
            // 
            // btnStats
            // 
            btnStats.Location = new Point(671, 256);
            btnStats.Margin = new Padding(5);
            btnStats.Name = "btnStats";
            btnStats.Size = new Size(118, 35);
            btnStats.TabIndex = 16;
            btnStats.Text = "統計";
            btnStats.UseVisualStyleBackColor = true;
            btnStats.Click += btnStats_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1232, 860);
            Controls.Add(btnStats);
            Controls.Add(lblStatus);
            Controls.Add(dgvProducts);
            Controls.Add(txtSearch);
            Controls.Add(label5);
            Controls.Add(btnClear);
            Controls.Add(btnDelete);
            Controls.Add(btnEdit);
            Controls.Add(btnAdd);
            Controls.Add(cboCategory);
            Controls.Add(txtStock);
            Controls.Add(txtPrice);
            Controls.Add(txtName);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Margin = new Padding(5);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "進銷存管理系統";
            Load += Form1_Load;
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
        private Button btnEdit;
        private Button btnDelete;
        private Button btnClear;
        private Label label5;
        private TextBox txtSearch;
        private DataGridView dgvProducts;
        private Label lblStatus;
        private Button btnStats;
    }
}
