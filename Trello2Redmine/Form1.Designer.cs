namespace Trello2Redmine
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tb_creatIssueIssuer = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cb_createIssuePriority = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cb_createIssueStatus = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cb_createIssueProject = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cb_CreateIssueTracker = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_ceateIssueSubject = new System.Windows.Forms.TextBox();
            this.btn_createIssue = new System.Windows.Forms.Button();
            this.trello_org_combobox = new System.Windows.Forms.ComboBox();
            this.trello_board_combobox = new System.Windows.Forms.ComboBox();
            this.create_or_update_btn = new System.Windows.Forms.Button();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.process_lable = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.tb_creatIssueIssuer);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cb_createIssuePriority);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.cb_createIssueStatus);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cb_createIssueProject);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cb_CreateIssueTracker);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tb_ceateIssueSubject);
            this.groupBox1.Controls.Add(this.btn_createIssue);
            this.groupBox1.Location = new System.Drawing.Point(6, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(236, 132);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Issue";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 176);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 13;
            this.label9.Text = "Issuer";
            // 
            // tb_creatIssueIssuer
            // 
            this.tb_creatIssueIssuer.Location = new System.Drawing.Point(51, 173);
            this.tb_creatIssueIssuer.Name = "tb_creatIssueIssuer";
            this.tb_creatIssueIssuer.Size = new System.Drawing.Size(167, 21);
            this.tb_creatIssueIssuer.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 129);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 11;
            this.label8.Text = "Priority";
            // 
            // cb_createIssuePriority
            // 
            this.cb_createIssuePriority.FormattingEnabled = true;
            this.cb_createIssuePriority.Location = new System.Drawing.Point(51, 126);
            this.cb_createIssuePriority.Name = "cb_createIssuePriority";
            this.cb_createIssuePriority.Size = new System.Drawing.Size(167, 20);
            this.cb_createIssuePriority.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 9;
            this.label7.Text = "Status";
            // 
            // cb_createIssueStatus
            // 
            this.cb_createIssueStatus.FormattingEnabled = true;
            this.cb_createIssueStatus.Location = new System.Drawing.Point(51, 100);
            this.cb_createIssueStatus.Name = "cb_createIssueStatus";
            this.cb_createIssueStatus.Size = new System.Drawing.Size(167, 20);
            this.cb_createIssueStatus.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "Project";
            // 
            // cb_createIssueProject
            // 
            this.cb_createIssueProject.FormattingEnabled = true;
            this.cb_createIssueProject.Location = new System.Drawing.Point(51, 74);
            this.cb_createIssueProject.Name = "cb_createIssueProject";
            this.cb_createIssueProject.Size = new System.Drawing.Size(167, 20);
            this.cb_createIssueProject.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "Tracker";
            // 
            // cb_CreateIssueTracker
            // 
            this.cb_CreateIssueTracker.FormattingEnabled = true;
            this.cb_CreateIssueTracker.Location = new System.Drawing.Point(51, 48);
            this.cb_CreateIssueTracker.Name = "cb_CreateIssueTracker";
            this.cb_CreateIssueTracker.Size = new System.Drawing.Size(167, 20);
            this.cb_CreateIssueTracker.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Subject";
            // 
            // tb_ceateIssueSubject
            // 
            this.tb_ceateIssueSubject.Location = new System.Drawing.Point(51, 19);
            this.tb_ceateIssueSubject.Name = "tb_ceateIssueSubject";
            this.tb_ceateIssueSubject.Size = new System.Drawing.Size(167, 21);
            this.tb_ceateIssueSubject.TabIndex = 2;
            // 
            // btn_createIssue
            // 
            this.btn_createIssue.Location = new System.Drawing.Point(0, 0);
            this.btn_createIssue.Name = "btn_createIssue";
            this.btn_createIssue.Size = new System.Drawing.Size(75, 23);
            this.btn_createIssue.TabIndex = 14;
            // 
            // trello_org_combobox
            // 
            this.trello_org_combobox.FormattingEnabled = true;
            this.trello_org_combobox.Location = new System.Drawing.Point(121, 20);
            this.trello_org_combobox.Name = "trello_org_combobox";
            this.trello_org_combobox.Size = new System.Drawing.Size(376, 20);
            this.trello_org_combobox.TabIndex = 20;
            this.trello_org_combobox.DropDown += new System.EventHandler(this.trello_org_combobox_DropDown);
            // 
            // trello_board_combobox
            // 
            this.trello_board_combobox.FormattingEnabled = true;
            this.trello_board_combobox.Location = new System.Drawing.Point(121, 50);
            this.trello_board_combobox.Name = "trello_board_combobox";
            this.trello_board_combobox.Size = new System.Drawing.Size(376, 20);
            this.trello_board_combobox.TabIndex = 21;
            this.trello_board_combobox.DropDown += new System.EventHandler(this.trello_board_combobox_DropDown);
            // 
            // create_or_update_btn
            // 
            this.create_or_update_btn.Location = new System.Drawing.Point(207, 82);
            this.create_or_update_btn.Name = "create_or_update_btn";
            this.create_or_update_btn.Size = new System.Drawing.Size(109, 23);
            this.create_or_update_btn.TabIndex = 22;
            this.create_or_update_btn.Text = "Create/Update";
            this.create_or_update_btn.UseVisualStyleBackColor = true;
            this.create_or_update_btn.Click += new System.EventHandler(this.create_or_update_btn_Click);
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(18, 54);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(77, 12);
            this.label32.TabIndex = 23;
            this.label32.Text = "Trello Board";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(18, 23);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(65, 12);
            this.label33.TabIndex = 24;
            this.label33.Text = "Trello Org";
            // 
            // timer1
            // 
            this.timer1.Interval = 3000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // process_lable
            // 
            this.process_lable.Location = new System.Drawing.Point(20, 122);
            this.process_lable.Name = "process_lable";
            this.process_lable.Size = new System.Drawing.Size(477, 39);
            this.process_lable.TabIndex = 25;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 182);
            this.Controls.Add(this.process_lable);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.create_or_update_btn);
            this.Controls.Add(this.trello_board_combobox);
            this.Controls.Add(this.trello_org_combobox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Trello2Redmine";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_ceateIssueSubject;
        private System.Windows.Forms.Button btn_createIssue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cb_CreateIssueTracker;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cb_createIssueProject;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cb_createIssuePriority;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cb_createIssueStatus;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tb_creatIssueIssuer;
        private System.Windows.Forms.ComboBox trello_org_combobox;
        private System.Windows.Forms.ComboBox trello_board_combobox;
        private System.Windows.Forms.Button create_or_update_btn;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label process_lable;
    }
}

