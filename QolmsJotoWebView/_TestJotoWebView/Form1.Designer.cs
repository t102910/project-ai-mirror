
namespace _TestJotoWebView
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.web = new System.Windows.Forms.WebBrowser();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAccount = new System.Windows.Forms.TextBox();
            this.cboPage = new System.Windows.Forms.ComboBox();
            this.btnshow = new System.Windows.Forms.Button();
            this.cboEv = new System.Windows.Forms.ComboBox();
            this.cboPath = new System.Windows.Forms.ComboBox();
            this.btnback = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // web
            // 
            this.web.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.web.Location = new System.Drawing.Point(0, 192);
            this.web.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.web.MinimumSize = new System.Drawing.Size(20, 20);
            this.web.Name = "web";
            this.web.Size = new System.Drawing.Size(637, 686);
            this.web.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.txtURL);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtAccount);
            this.panel1.Controls.Add(this.cboPage);
            this.panel1.Controls.Add(this.btnshow);
            this.panel1.Controls.Add(this.cboEv);
            this.panel1.Controls.Add(this.cboPath);
            this.panel1.Controls.Add(this.btnback);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Location = new System.Drawing.Point(0, -5);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(637, 194);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(3, 231);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(637, 638);
            this.panel2.TabIndex = 13;
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(103, 119);
            this.txtURL.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtURL.Multiline = true;
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(488, 56);
            this.txtURL.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 15);
            this.label1.TabIndex = 12;
            this.label1.Text = "アカウントキー";
            // 
            // txtAccount
            // 
            this.txtAccount.Location = new System.Drawing.Point(103, 91);
            this.txtAccount.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtAccount.Name = "txtAccount";
            this.txtAccount.Size = new System.Drawing.Size(328, 22);
            this.txtAccount.TabIndex = 10;
            // 
            // cboPage
            // 
            this.cboPage.AutoCompleteCustomSource.AddRange(new string[] {
            "ローカル",
            "Azure検証",
            "Azure本番"});
            this.cboPage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPage.FormattingEnabled = true;
            this.cboPage.Items.AddRange(new object[] {
            "PortalLocalIdVerification",
            "HealthAge",
            "NoteExamination",
            "NoteMonshin",
            "PointHistory",
            "LocalHistory",
            "Calomeal"});
            this.cboPage.Location = new System.Drawing.Point(103, 65);
            this.cboPage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboPage.Name = "cboPage";
            this.cboPage.Size = new System.Drawing.Size(372, 23);
            this.cboPage.TabIndex = 9;
            // 
            // btnshow
            // 
            this.btnshow.Location = new System.Drawing.Point(491, 86);
            this.btnshow.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnshow.Name = "btnshow";
            this.btnshow.Size = new System.Drawing.Size(75, 22);
            this.btnshow.TabIndex = 8;
            this.btnshow.Text = "表示する";
            this.btnshow.UseVisualStyleBackColor = true;
            this.btnshow.Click += new System.EventHandler(this.btnshow_Click);
            // 
            // cboEv
            // 
            this.cboEv.AutoCompleteCustomSource.AddRange(new string[] {
            "タニタ",
            "＊＊＊",
            "＊＊＊",
            "＊＊＊",
            "＊＊＊",
            "＊＊＊",
            "マイナポータル（Import）",
            "マイナポータル  (Viewer）",
            "マイナポータル（API）",
            "ローカル",
            "Azure検証",
            "Azure本番"});
            this.cboEv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEv.FormattingEnabled = true;
            this.cboEv.Items.AddRange(new object[] {
            "ローカル",
            "Azure検証"});
            this.cboEv.Location = new System.Drawing.Point(103, 39);
            this.cboEv.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboEv.Name = "cboEv";
            this.cboEv.Size = new System.Drawing.Size(372, 23);
            this.cboEv.TabIndex = 7;
            // 
            // cboPath
            // 
            this.cboPath.FormattingEnabled = true;
            this.cboPath.Items.AddRange(new object[] {
            "https://localhost:44384/start/sso",
            "https://qolms-dev-joto-west-app02.azurewebsites.net/start/sso"});
            this.cboPath.Location = new System.Drawing.Point(103, 15);
            this.cboPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboPath.Name = "cboPath";
            this.cboPath.Size = new System.Drawing.Size(372, 23);
            this.cboPath.TabIndex = 4;
            // 
            // btnback
            // 
            this.btnback.Location = new System.Drawing.Point(12, 44);
            this.btnback.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnback.Name = "btnback";
            this.btnback.Size = new System.Drawing.Size(35, 22);
            this.btnback.TabIndex = 3;
            this.btnback.Text = "＜";
            this.btnback.UseVisualStyleBackColor = true;
            this.btnback.Click += new System.EventHandler(this.btnback_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(713, 12);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 22);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 879);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.web);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser web;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAccount;
        private System.Windows.Forms.ComboBox cboPage;
        private System.Windows.Forms.Button btnshow;
        private System.Windows.Forms.ComboBox cboEv;
        private System.Windows.Forms.ComboBox cboPath;
        private System.Windows.Forms.Button btnback;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtURL;
    }
}

