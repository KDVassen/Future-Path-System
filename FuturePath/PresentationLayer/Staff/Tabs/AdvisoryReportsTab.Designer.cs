namespace FuturePath.PresentationLayer
{
    partial class AdvisoryReportsTab
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
            System.Windows.Forms.DataGridViewCellStyle grdReportsAltStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle grdReportsHeaderStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle grdReportsCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colRowsInScopeStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colLastGeneratedStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlSearchCard = new System.Windows.Forms.Panel();
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblFromStar = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblDateRangeError = new System.Windows.Forms.Label();
            this.lblTo = new System.Windows.Forms.Label();
            this.lblToStar = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblFaculty = new System.Windows.Forms.Label();
            this.cboFaculty = new System.Windows.Forms.ComboBox();
            this.lblGrade = new System.Windows.Forms.Label();
            this.cboGrade = new System.Windows.Forms.ComboBox();
            this.lblOutcome = new System.Windows.Forms.Label();
            this.cboOutcome = new System.Windows.Forms.ComboBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.lblSearchHint = new System.Windows.Forms.Label();
            this.pnlResultsCard = new System.Windows.Forms.Panel();
            this.lblResultsHeadingBar = new System.Windows.Forms.Panel();
            this.lblResultsHeading = new System.Windows.Forms.Label();
            this.lblResultsCount = new System.Windows.Forms.Label();
            this.lblReportsNote = new System.Windows.Forms.Label();
            this.grdReports = new System.Windows.Forms.DataGridView();
            this.colReportCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReport = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colScope = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRowsInScope = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLastGenerated = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlSearchCard.SuspendLayout();
            this.pnlResultsCard.SuspendLayout();
            this.lblResultsHeadingBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdReports)).BeginInit();
            this.SuspendLayout();
            //
            // pnlSearchCard
            //
            this.pnlSearchCard.Controls.Add(this.lblFrom);
            this.pnlSearchCard.Controls.Add(this.lblFromStar);
            this.pnlSearchCard.Controls.Add(this.dtpFrom);
            this.pnlSearchCard.Controls.Add(this.lblDateRangeError);
            this.pnlSearchCard.Controls.Add(this.lblTo);
            this.pnlSearchCard.Controls.Add(this.lblToStar);
            this.pnlSearchCard.Controls.Add(this.dtpTo);
            this.pnlSearchCard.Controls.Add(this.lblFaculty);
            this.pnlSearchCard.Controls.Add(this.cboFaculty);
            this.pnlSearchCard.Controls.Add(this.lblGrade);
            this.pnlSearchCard.Controls.Add(this.cboGrade);
            this.pnlSearchCard.Controls.Add(this.lblOutcome);
            this.pnlSearchCard.Controls.Add(this.cboOutcome);
            this.pnlSearchCard.Controls.Add(this.btnFind);
            this.pnlSearchCard.Controls.Add(this.lblSearchHint);
            this.pnlSearchCard.BackColor = System.Drawing.Color.White;
            this.pnlSearchCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSearchCard.Location = new System.Drawing.Point(0, 0);
            this.pnlSearchCard.Size = new System.Drawing.Size(926, 84);
            this.pnlSearchCard.TabIndex = 0;
            this.pnlSearchCard.Name = "pnlSearchCard";
            //
            // lblFrom
            //
            this.lblFrom.AutoSize = true;
            this.lblFrom.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFrom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFrom.Location = new System.Drawing.Point(16, 8);
            this.lblFrom.TabIndex = 0;
            this.lblFrom.Text = "Sessions from";
            this.lblFrom.Name = "lblFrom";
            //
            // lblFromStar
            //
            this.lblFromStar.AutoSize = true;
            this.lblFromStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFromStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblFromStar.Location = new System.Drawing.Point(132, 8);
            this.lblFromStar.Size = new System.Drawing.Size(14, 24);
            this.lblFromStar.TabIndex = 1;
            this.lblFromStar.Text = "*";
            this.lblFromStar.Name = "lblFromStar";
            //
            // dtpFrom
            //
            this.dtpFrom.AccessibleName = "Sessions from (required)";
            this.dtpFrom.CustomFormat = "dd/MM/yyyy";
            this.dtpFrom.Font = new System.Drawing.Font("Consolas", 10F);
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom.Location = new System.Drawing.Point(16, 36);
            this.dtpFrom.Size = new System.Drawing.Size(130, 23);
            this.dtpFrom.TabIndex = 2;
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Leave += new System.EventHandler(this.DateRange_Leave);
            //
            // lblDateRangeError
            //
            this.lblDateRangeError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblDateRangeError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblDateRangeError.Location = new System.Drawing.Point(16, 63);
            this.lblDateRangeError.Size = new System.Drawing.Size(290, 16);
            this.lblDateRangeError.TabIndex = 3;
            this.lblDateRangeError.Text = "";
            this.lblDateRangeError.Visible = false;
            this.lblDateRangeError.Name = "lblDateRangeError";
            //
            // lblTo
            //
            this.lblTo.AutoSize = true;
            this.lblTo.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblTo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblTo.Location = new System.Drawing.Point(162, 8);
            this.lblTo.TabIndex = 4;
            this.lblTo.Text = "Sessions to";
            this.lblTo.Name = "lblTo";
            //
            // lblToStar
            //
            this.lblToStar.AutoSize = true;
            this.lblToStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblToStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblToStar.Location = new System.Drawing.Point(258, 8);
            this.lblToStar.Size = new System.Drawing.Size(14, 24);
            this.lblToStar.TabIndex = 5;
            this.lblToStar.Text = "*";
            this.lblToStar.Name = "lblToStar";
            //
            // dtpTo
            //
            this.dtpTo.AccessibleName = "Sessions to (required)";
            this.dtpTo.CustomFormat = "dd/MM/yyyy";
            this.dtpTo.Font = new System.Drawing.Font("Consolas", 10F);
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTo.Location = new System.Drawing.Point(162, 36);
            this.dtpTo.Size = new System.Drawing.Size(130, 23);
            this.dtpTo.TabIndex = 6;
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Leave += new System.EventHandler(this.DateRange_Leave);
            //
            // lblFaculty
            //
            this.lblFaculty.AutoSize = true;
            this.lblFaculty.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFaculty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFaculty.Location = new System.Drawing.Point(308, 8);
            this.lblFaculty.TabIndex = 7;
            this.lblFaculty.Text = "Faculty";
            this.lblFaculty.Name = "lblFaculty";
            //
            // cboFaculty
            //
            this.cboFaculty.AccessibleName = "Faculty (optional)";
            this.cboFaculty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFaculty.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboFaculty.FormattingEnabled = true;
            this.cboFaculty.Location = new System.Drawing.Point(308, 36);
            this.cboFaculty.Size = new System.Drawing.Size(220, 25);
            this.cboFaculty.TabIndex = 8;
            this.cboFaculty.Name = "cboFaculty";
            //
            // lblGrade
            //
            this.lblGrade.AutoSize = true;
            this.lblGrade.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblGrade.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblGrade.Location = new System.Drawing.Point(544, 8);
            this.lblGrade.TabIndex = 9;
            this.lblGrade.Text = "Grade";
            this.lblGrade.Name = "lblGrade";
            //
            // cboGrade
            //
            this.cboGrade.AccessibleName = "Grade (optional)";
            this.cboGrade.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGrade.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboGrade.FormattingEnabled = true;
            this.cboGrade.Location = new System.Drawing.Point(544, 36);
            this.cboGrade.Size = new System.Drawing.Size(120, 25);
            this.cboGrade.TabIndex = 10;
            this.cboGrade.Name = "cboGrade";
            //
            // lblOutcome
            //
            this.lblOutcome.AutoSize = true;
            this.lblOutcome.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblOutcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblOutcome.Location = new System.Drawing.Point(680, 8);
            this.lblOutcome.TabIndex = 11;
            this.lblOutcome.Text = "Advisory outcome";
            this.lblOutcome.Name = "lblOutcome";
            //
            // cboOutcome
            //
            this.cboOutcome.AccessibleName = "Advisory outcome (optional)";
            this.cboOutcome.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOutcome.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboOutcome.FormattingEnabled = true;
            this.cboOutcome.Location = new System.Drawing.Point(680, 36);
            this.cboOutcome.Size = new System.Drawing.Size(164, 25);
            this.cboOutcome.TabIndex = 12;
            this.cboOutcome.Name = "cboOutcome";
            //
            // btnFind
            //
            this.btnFind.AccessibleName = "Find";
            this.btnFind.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.btnFind.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFind.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnFind.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnFind.Location = new System.Drawing.Point(860, 34);
            this.btnFind.Size = new System.Drawing.Size(58, 30);
            this.btnFind.TabIndex = 13;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = false;
            this.btnFind.Name = "btnFind";
            this.btnFind.FlatAppearance.BorderSize = 0;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            //
            // lblSearchHint
            //
            this.lblSearchHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSearchHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblSearchHint.Location = new System.Drawing.Point(544, 63);
            this.lblSearchHint.Size = new System.Drawing.Size(370, 16);
            this.lblSearchHint.TabIndex = 14;
            this.lblSearchHint.Text = "Blank faculty, grade or outcome means all.";
            this.lblSearchHint.Name = "lblSearchHint";
            //
            // pnlResultsCard
            //
            this.pnlResultsCard.Controls.Add(this.lblResultsHeadingBar);
            this.pnlResultsCard.Controls.Add(this.lblResultsHeading);
            this.pnlResultsCard.Controls.Add(this.lblResultsCount);
            this.pnlResultsCard.Controls.Add(this.lblReportsNote);
            this.pnlResultsCard.Controls.Add(this.grdReports);
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
            this.lblResultsHeading.Size = new System.Drawing.Size(160, 28);
            this.lblResultsHeading.TabIndex = 1;
            this.lblResultsHeading.Text = "Advisory reports";
            this.lblResultsHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblResultsHeading.Name = "lblResultsHeading";
            //
            // lblResultsCount
            //
            this.lblResultsCount.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblResultsCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblResultsCount.Location = new System.Drawing.Point(186, 16);
            this.lblResultsCount.Size = new System.Drawing.Size(330, 18);
            this.lblResultsCount.TabIndex = 2;
            this.lblResultsCount.Text = "";
            this.lblResultsCount.Name = "lblResultsCount";
            //
            // lblReportsNote
            //
            this.lblReportsNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblReportsNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblReportsNote.Location = new System.Drawing.Point(16, 214);
            this.lblReportsNote.Size = new System.Drawing.Size(892, 36);
            this.lblReportsNote.TabIndex = 3;
            this.lblReportsNote.Text = "Generate report opens Report generation (UI 11) with these criteria already applied. FP-LSR covers one session at a time; open it from the Advisory session tab.";
            this.lblReportsNote.Name = "lblReportsNote";
            //
            // grdReports
            //
            this.grdReports.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colReportCode,
            this.colReport,
            this.colScope,
            this.colRowsInScope,
            this.colLastGenerated});
            grdReportsAltStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.grdReports.AlternatingRowsDefaultCellStyle = grdReportsAltStyle;
            grdReportsHeaderStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grdReportsHeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            grdReportsHeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            grdReportsHeaderStyle.ForeColor = System.Drawing.Color.White;
            grdReportsHeaderStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            grdReportsHeaderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            grdReportsHeaderStyle.SelectionForeColor = System.Drawing.Color.White;
            grdReportsHeaderStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdReports.ColumnHeadersDefaultCellStyle = grdReportsHeaderStyle;
            grdReportsCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grdReportsCellStyle.BackColor = System.Drawing.Color.White;
            grdReportsCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            grdReportsCellStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            grdReportsCellStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            grdReportsCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            grdReportsCellStyle.SelectionForeColor = System.Drawing.Color.White;
            grdReportsCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grdReports.DefaultCellStyle = grdReportsCellStyle;
            this.grdReports.AccessibleName = "Reports available";
            this.grdReports.AllowUserToAddRows = false;
            this.grdReports.AllowUserToDeleteRows = false;
            this.grdReports.AllowUserToResizeRows = false;
            this.grdReports.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdReports.BackgroundColor = System.Drawing.Color.White;
            this.grdReports.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.grdReports.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grdReports.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.grdReports.ColumnHeadersHeight = 34;
            this.grdReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdReports.EnableHeadersVisualStyles = false;
            this.grdReports.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            this.grdReports.Location = new System.Drawing.Point(16, 46);
            this.grdReports.MultiSelect = false;
            this.grdReports.ReadOnly = true;
            this.grdReports.RowHeadersVisible = false;
            this.grdReports.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdReports.Size = new System.Drawing.Size(892, 160);
            this.grdReports.TabIndex = 4;
            this.grdReports.Name = "grdReports";
            this.grdReports.RowTemplate.Height = 30;
            this.grdReports.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.grdReports_CellFormatting);
            this.grdReports.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdReports_CellDoubleClick);
            //
            // colReportCode
            //
            this.colReportCode.DataPropertyName = "ReportCode";
            this.colReportCode.FillWeight = 5F;
            this.colReportCode.HeaderText = "Code";
            this.colReportCode.ReadOnly = true;
            this.colReportCode.Visible = false;
            this.colReportCode.Name = "colReportCode";
            //
            // colReport
            //
            this.colReport.DataPropertyName = "ReportName";
            this.colReport.FillWeight = 34F;
            this.colReport.HeaderText = "Report";
            this.colReport.ReadOnly = true;
            this.colReport.Name = "colReport";
            //
            // colScope
            //
            this.colScope.DataPropertyName = "Scope";
            this.colScope.FillWeight = 34F;
            this.colScope.HeaderText = "Scope";
            this.colScope.ReadOnly = true;
            this.colScope.Name = "colScope";
            //
            // colRowsInScope
            //
            colRowsInScopeStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            colRowsInScopeStyle.Font = new System.Drawing.Font("Consolas", 10F);
            this.colRowsInScope.DefaultCellStyle = colRowsInScopeStyle;
            this.colRowsInScope.DataPropertyName = "RowsInScope";
            this.colRowsInScope.FillWeight = 13F;
            this.colRowsInScope.HeaderText = "Rows in scope";
            this.colRowsInScope.ReadOnly = true;
            this.colRowsInScope.Name = "colRowsInScope";
            //
            // colLastGenerated
            //
            colLastGeneratedStyle.Font = new System.Drawing.Font("Consolas", 10F);
            this.colLastGenerated.DefaultCellStyle = colLastGeneratedStyle;
            this.colLastGenerated.DataPropertyName = "LastGenerated";
            this.colLastGenerated.FillWeight = 19F;
            this.colLastGenerated.HeaderText = "Last generated";
            this.colLastGenerated.ReadOnly = true;
            this.colLastGenerated.Name = "colLastGenerated";
            //
            // AdvisoryReportsTab
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.Size = new System.Drawing.Size(926, 527);
            this.Controls.Add(this.pnlSearchCard);
            this.Controls.Add(this.pnlResultsCard);
            this.Name = "AdvisoryReportsTab";
            this.pnlSearchCard.ResumeLayout(false);
            this.pnlSearchCard.PerformLayout();
            this.pnlResultsCard.ResumeLayout(false);
            this.pnlResultsCard.PerformLayout();
            this.lblResultsHeadingBar.ResumeLayout(false);
            this.lblResultsHeadingBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdReports)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSearchCard;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblFromStar;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label lblDateRangeError;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Label lblToStar;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label lblFaculty;
        private System.Windows.Forms.ComboBox cboFaculty;
        private System.Windows.Forms.Label lblGrade;
        private System.Windows.Forms.ComboBox cboGrade;
        private System.Windows.Forms.Label lblOutcome;
        private System.Windows.Forms.ComboBox cboOutcome;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Label lblSearchHint;
        private System.Windows.Forms.Panel pnlResultsCard;
        private System.Windows.Forms.Panel lblResultsHeadingBar;
        private System.Windows.Forms.Label lblResultsHeading;
        private System.Windows.Forms.Label lblResultsCount;
        private System.Windows.Forms.Label lblReportsNote;
        private System.Windows.Forms.DataGridView grdReports;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReportCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReport;
        private System.Windows.Forms.DataGridViewTextBoxColumn colScope;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRowsInScope;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLastGenerated;
    }
}
