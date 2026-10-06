namespace FuturePath.PresentationLayer
{
    partial class LearnerProfileTab
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle grdLearnersAltStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle grdLearnersHeaderStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle grdLearnersCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colSessionsStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colLastSessionStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlSearchCard = new System.Windows.Forms.Panel();
            this.lblSurname = new System.Windows.Forms.Label();
            this.txtSurname = new System.Windows.Forms.TextBox();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.lblGrade = new System.Windows.Forms.Label();
            this.cboGrade = new System.Windows.Forms.ComboBox();
            this.lblSchool = new System.Windows.Forms.Label();
            this.txtSchool = new System.Windows.Forms.TextBox();
            this.lblProvince = new System.Windows.Forms.Label();
            this.cboProvince = new System.Windows.Forms.ComboBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblSearchHint = new System.Windows.Forms.Label();
            this.pnlResultsCard = new System.Windows.Forms.Panel();
            this.lblResultsHeadingBar = new System.Windows.Forms.Panel();
            this.lblResultsHeading = new System.Windows.Forms.Label();
            this.lblResultsCount = new System.Windows.Forms.Label();
            this.grdLearners = new System.Windows.Forms.DataGridView();
            this.colLearnerId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLearner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSchool = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProvince = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSessions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLastSession = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlSearchCard.SuspendLayout();
            this.pnlResultsCard.SuspendLayout();
            this.lblResultsHeadingBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdLearners)).BeginInit();
            this.SuspendLayout();
            //
            // pnlSearchCard
            //
            this.pnlSearchCard.Controls.Add(this.lblSurname);
            this.pnlSearchCard.Controls.Add(this.txtSurname);
            this.pnlSearchCard.Controls.Add(this.lblFirstName);
            this.pnlSearchCard.Controls.Add(this.txtFirstName);
            this.pnlSearchCard.Controls.Add(this.lblGrade);
            this.pnlSearchCard.Controls.Add(this.cboGrade);
            this.pnlSearchCard.Controls.Add(this.lblSchool);
            this.pnlSearchCard.Controls.Add(this.txtSchool);
            this.pnlSearchCard.Controls.Add(this.lblProvince);
            this.pnlSearchCard.Controls.Add(this.cboProvince);
            this.pnlSearchCard.Controls.Add(this.btnFind);
            this.pnlSearchCard.Controls.Add(this.btnClear);
            this.pnlSearchCard.Controls.Add(this.lblSearchHint);
            this.pnlSearchCard.BackColor = System.Drawing.Color.White;
            this.pnlSearchCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSearchCard.Location = new System.Drawing.Point(0, 0);
            this.pnlSearchCard.Size = new System.Drawing.Size(926, 84);
            this.pnlSearchCard.TabIndex = 0;
            this.pnlSearchCard.Name = "pnlSearchCard";
            //
            // lblSurname
            //
            this.lblSurname.AutoSize = true;
            this.lblSurname.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSurname.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblSurname.Location = new System.Drawing.Point(16, 8);
            this.lblSurname.TabIndex = 0;
            this.lblSurname.Text = "Surname";
            this.lblSurname.Name = "lblSurname";
            //
            // txtSurname
            //
            this.txtSurname.AccessibleName = "Surname (optional)";
            this.txtSurname.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSurname.Location = new System.Drawing.Point(16, 36);
            this.txtSurname.MaxLength = 50;
            this.txtSurname.Size = new System.Drawing.Size(140, 25);
            this.txtSurname.TabIndex = 1;
            this.txtSurname.Name = "txtSurname";
            //
            // lblFirstName
            //
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFirstName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFirstName.Location = new System.Drawing.Point(172, 8);
            this.lblFirstName.TabIndex = 2;
            this.lblFirstName.Text = "First name";
            this.lblFirstName.Name = "lblFirstName";
            //
            // txtFirstName
            //
            this.txtFirstName.AccessibleName = "First name (optional)";
            this.txtFirstName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtFirstName.Location = new System.Drawing.Point(172, 36);
            this.txtFirstName.MaxLength = 50;
            this.txtFirstName.Size = new System.Drawing.Size(130, 25);
            this.txtFirstName.TabIndex = 3;
            this.txtFirstName.Name = "txtFirstName";
            //
            // lblGrade
            //
            this.lblGrade.AutoSize = true;
            this.lblGrade.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblGrade.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblGrade.Location = new System.Drawing.Point(318, 8);
            this.lblGrade.TabIndex = 4;
            this.lblGrade.Text = "Grade";
            this.lblGrade.Name = "lblGrade";
            //
            // cboGrade
            //
            this.cboGrade.AccessibleName = "Grade (optional)";
            this.cboGrade.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGrade.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboGrade.FormattingEnabled = true;
            this.cboGrade.Location = new System.Drawing.Point(318, 36);
            this.cboGrade.Size = new System.Drawing.Size(110, 25);
            this.cboGrade.TabIndex = 5;
            this.cboGrade.Name = "cboGrade";
            //
            // lblSchool
            //
            this.lblSchool.AutoSize = true;
            this.lblSchool.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSchool.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblSchool.Location = new System.Drawing.Point(444, 8);
            this.lblSchool.TabIndex = 6;
            this.lblSchool.Text = "School";
            this.lblSchool.Name = "lblSchool";
            //
            // txtSchool
            //
            this.txtSchool.AccessibleName = "School (optional)";
            this.txtSchool.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSchool.Location = new System.Drawing.Point(444, 36);
            this.txtSchool.MaxLength = 100;
            this.txtSchool.Size = new System.Drawing.Size(180, 25);
            this.txtSchool.TabIndex = 7;
            this.txtSchool.Name = "txtSchool";
            //
            // lblProvince
            //
            this.lblProvince.AutoSize = true;
            this.lblProvince.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblProvince.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblProvince.Location = new System.Drawing.Point(640, 8);
            this.lblProvince.TabIndex = 8;
            this.lblProvince.Text = "Province";
            this.lblProvince.Name = "lblProvince";
            //
            // cboProvince
            //
            this.cboProvince.AccessibleName = "Province (optional)";
            this.cboProvince.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProvince.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboProvince.FormattingEnabled = true;
            this.cboProvince.Location = new System.Drawing.Point(640, 36);
            this.cboProvince.Size = new System.Drawing.Size(140, 25);
            this.cboProvince.TabIndex = 9;
            this.cboProvince.Name = "cboProvince";
            //
            // btnFind
            //
            this.btnFind.AccessibleName = "Find";
            this.btnFind.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.btnFind.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFind.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnFind.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnFind.Location = new System.Drawing.Point(796, 34);
            this.btnFind.Size = new System.Drawing.Size(58, 30);
            this.btnFind.TabIndex = 10;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = false;
            this.btnFind.Name = "btnFind";
            this.btnFind.FlatAppearance.BorderSize = 0;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            //
            // btnClear
            //
            this.btnClear.AccessibleName = "Clear";
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnClear.Location = new System.Drawing.Point(860, 34);
            this.btnClear.Size = new System.Drawing.Size(58, 30);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Name = "btnClear";
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            //
            // lblSearchHint
            //
            this.lblSearchHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSearchHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblSearchHint.Location = new System.Drawing.Point(16, 62);
            this.lblSearchHint.Size = new System.Drawing.Size(600, 16);
            this.lblSearchHint.TabIndex = 12;
            this.lblSearchHint.Text = "None of these fields is required. Leave them blank to widen the search.";
            this.lblSearchHint.Name = "lblSearchHint";
            //
            // pnlResultsCard
            //
            this.pnlResultsCard.Controls.Add(this.lblResultsHeadingBar);
            this.pnlResultsCard.Controls.Add(this.lblResultsHeading);
            this.pnlResultsCard.Controls.Add(this.lblResultsCount);
            this.pnlResultsCard.Controls.Add(this.grdLearners);
            this.pnlResultsCard.BackColor = System.Drawing.Color.White;
            this.pnlResultsCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlResultsCard.Location = new System.Drawing.Point(0, 98);
            this.pnlResultsCard.Size = new System.Drawing.Size(926, 425);
            this.pnlResultsCard.TabIndex = 1;
            this.pnlResultsCard.Name = "pnlResultsCard";
            //
            // lblResultsHeadingBar
            //
            this.lblResultsHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblResultsHeadingBar.Location = new System.Drawing.Point(16, 14);
            this.lblResultsHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblResultsHeadingBar.TabIndex = 0;
            this.lblResultsHeadingBar.Name = "lblResultsHeadingBar";
            //
            // lblResultsHeading
            //
            this.lblResultsHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblResultsHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblResultsHeading.Location = new System.Drawing.Point(26, 10);
            this.lblResultsHeading.Size = new System.Drawing.Size(110, 28);
            this.lblResultsHeading.TabIndex = 1;
            this.lblResultsHeading.Text = "Learners";
            this.lblResultsHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblResultsHeading.Name = "lblResultsHeading";
            //
            // lblResultsCount
            //
            this.lblResultsCount.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblResultsCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblResultsCount.Location = new System.Drawing.Point(136, 16);
            this.lblResultsCount.Size = new System.Drawing.Size(330, 18);
            this.lblResultsCount.TabIndex = 2;
            this.lblResultsCount.Text = "";
            this.lblResultsCount.Name = "lblResultsCount";
            //
            // grdLearners
            //
            this.grdLearners.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLearnerId,
            this.colLearner,
            this.colGrade,
            this.colSchool,
            this.colProvince,
            this.colSessions,
            this.colLastSession});
            grdLearnersAltStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.grdLearners.AlternatingRowsDefaultCellStyle = grdLearnersAltStyle;
            grdLearnersHeaderStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grdLearnersHeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            grdLearnersHeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            grdLearnersHeaderStyle.ForeColor = System.Drawing.Color.White;
            grdLearnersHeaderStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            grdLearnersHeaderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            grdLearnersHeaderStyle.SelectionForeColor = System.Drawing.Color.White;
            grdLearnersHeaderStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdLearners.ColumnHeadersDefaultCellStyle = grdLearnersHeaderStyle;
            grdLearnersCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grdLearnersCellStyle.BackColor = System.Drawing.Color.White;
            grdLearnersCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            grdLearnersCellStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            grdLearnersCellStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            grdLearnersCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            grdLearnersCellStyle.SelectionForeColor = System.Drawing.Color.White;
            grdLearnersCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grdLearners.DefaultCellStyle = grdLearnersCellStyle;
            this.grdLearners.AccessibleName = "Learners found";
            this.grdLearners.AllowUserToAddRows = false;
            this.grdLearners.AllowUserToDeleteRows = false;
            this.grdLearners.AllowUserToResizeRows = false;
            this.grdLearners.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdLearners.BackgroundColor = System.Drawing.Color.White;
            this.grdLearners.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.grdLearners.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grdLearners.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.grdLearners.ColumnHeadersHeight = 34;
            this.grdLearners.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdLearners.EnableHeadersVisualStyles = false;
            this.grdLearners.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            this.grdLearners.Location = new System.Drawing.Point(16, 46);
            this.grdLearners.MultiSelect = false;
            this.grdLearners.ReadOnly = true;
            this.grdLearners.RowHeadersVisible = false;
            this.grdLearners.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdLearners.Size = new System.Drawing.Size(892, 360);
            this.grdLearners.TabIndex = 3;
            this.grdLearners.Name = "grdLearners";
            this.grdLearners.RowTemplate.Height = 30;
            this.grdLearners.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.grdLearners_CellFormatting);
            this.grdLearners.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdLearners_CellDoubleClick);
            //
            // colLearnerId
            //
            this.colLearnerId.DataPropertyName = "LearnerID";
            this.colLearnerId.FillWeight = 5F;
            this.colLearnerId.HeaderText = "Learner ID";
            this.colLearnerId.ReadOnly = true;
            this.colLearnerId.Visible = false;
            this.colLearnerId.Name = "colLearnerId";
            //
            // colLearner
            //
            this.colLearner.DataPropertyName = "LearnerName";
            this.colLearner.FillWeight = 26F;
            this.colLearner.HeaderText = "Learner";
            this.colLearner.ReadOnly = true;
            this.colLearner.Name = "colLearner";
            //
            // colGrade
            //
            this.colGrade.DataPropertyName = "GradeDescription";
            this.colGrade.FillWeight = 12F;
            this.colGrade.HeaderText = "Grade";
            this.colGrade.ReadOnly = true;
            this.colGrade.Name = "colGrade";
            //
            // colSchool
            //
            this.colSchool.DataPropertyName = "SchoolName";
            this.colSchool.FillWeight = 28F;
            this.colSchool.HeaderText = "School";
            this.colSchool.ReadOnly = true;
            this.colSchool.Name = "colSchool";
            //
            // colProvince
            //
            this.colProvince.DataPropertyName = "Province";
            this.colProvince.FillWeight = 16F;
            this.colProvince.HeaderText = "Province";
            this.colProvince.ReadOnly = true;
            this.colProvince.Name = "colProvince";
            //
            // colSessions
            //
            colSessionsStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            colSessionsStyle.Font = new System.Drawing.Font("Consolas", 10F);
            this.colSessions.DefaultCellStyle = colSessionsStyle;
            this.colSessions.DataPropertyName = "SessionCount";
            this.colSessions.FillWeight = 10F;
            this.colSessions.HeaderText = "Sessions";
            this.colSessions.ReadOnly = true;
            this.colSessions.Name = "colSessions";
            //
            // colLastSession
            //
            colLastSessionStyle.Font = new System.Drawing.Font("Consolas", 10F);
            this.colLastSession.DefaultCellStyle = colLastSessionStyle;
            this.colLastSession.DataPropertyName = "LastSessionDate";
            this.colLastSession.FillWeight = 14F;
            this.colLastSession.HeaderText = "Last session";
            this.colLastSession.ReadOnly = true;
            this.colLastSession.Name = "colLastSession";
            //
            // LearnerProfileTab
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.Size = new System.Drawing.Size(926, 527);
            this.Controls.Add(this.pnlSearchCard);
            this.Controls.Add(this.pnlResultsCard);
            this.Name = "LearnerProfileTab";
            this.pnlSearchCard.ResumeLayout(false);
            this.pnlSearchCard.PerformLayout();
            this.pnlResultsCard.ResumeLayout(false);
            this.pnlResultsCard.PerformLayout();
            this.lblResultsHeadingBar.ResumeLayout(false);
            this.lblResultsHeadingBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdLearners)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSearchCard;
        private System.Windows.Forms.Label lblSurname;
        private System.Windows.Forms.TextBox txtSurname;
        private System.Windows.Forms.Label lblFirstName;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.Label lblGrade;
        private System.Windows.Forms.ComboBox cboGrade;
        private System.Windows.Forms.Label lblSchool;
        private System.Windows.Forms.TextBox txtSchool;
        private System.Windows.Forms.Label lblProvince;
        private System.Windows.Forms.ComboBox cboProvince;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblSearchHint;
        private System.Windows.Forms.Panel pnlResultsCard;
        private System.Windows.Forms.Panel lblResultsHeadingBar;
        private System.Windows.Forms.Label lblResultsHeading;
        private System.Windows.Forms.Label lblResultsCount;
        private System.Windows.Forms.DataGridView grdLearners;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLearnerId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLearner;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGrade;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSchool;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProvince;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSessions;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLastSession;
    }
}
