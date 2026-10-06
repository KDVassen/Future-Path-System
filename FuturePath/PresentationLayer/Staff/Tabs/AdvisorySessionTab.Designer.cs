namespace FuturePath.PresentationLayer
{
    partial class AdvisorySessionTab
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
            System.Windows.Forms.DataGridViewCellStyle grdSessionsAltStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle grdSessionsHeaderStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle grdSessionsCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colSessionStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colDateStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colGrStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlSearchCard = new System.Windows.Forms.Panel();
            this.lblSessionNumber = new System.Windows.Forms.Label();
            this.txtSessionNumber = new System.Windows.Forms.TextBox();
            this.lblSessionNumberError = new System.Windows.Forms.Label();
            this.lblSurname = new System.Windows.Forms.Label();
            this.txtSurname = new System.Windows.Forms.TextBox();
            this.lblSessionFrom = new System.Windows.Forms.Label();
            this.dtpSessionFrom = new System.Windows.Forms.DateTimePicker();
            this.lblDateRangeError = new System.Windows.Forms.Label();
            this.lblSessionTo = new System.Windows.Forms.Label();
            this.dtpSessionTo = new System.Windows.Forms.DateTimePicker();
            this.lblOutcome = new System.Windows.Forms.Label();
            this.cboOutcome = new System.Windows.Forms.ComboBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblSearchHint = new System.Windows.Forms.Label();
            this.pnlResultsCard = new System.Windows.Forms.Panel();
            this.lblResultsHeadingBar = new System.Windows.Forms.Panel();
            this.lblResultsHeading = new System.Windows.Forms.Label();
            this.lblResultsCount = new System.Windows.Forms.Label();
            this.btnExportSummary = new System.Windows.Forms.Button();
            this.grdSessions = new System.Windows.Forms.DataGridView();
            this.colSessionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSessionStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSession = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLearner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProgramme = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOutcome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colExported = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlSearchCard.SuspendLayout();
            this.pnlResultsCard.SuspendLayout();
            this.lblResultsHeadingBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdSessions)).BeginInit();
            this.SuspendLayout();
            //
            // pnlSearchCard
            //
            this.pnlSearchCard.Controls.Add(this.lblSessionNumber);
            this.pnlSearchCard.Controls.Add(this.txtSessionNumber);
            this.pnlSearchCard.Controls.Add(this.lblSessionNumberError);
            this.pnlSearchCard.Controls.Add(this.lblSurname);
            this.pnlSearchCard.Controls.Add(this.txtSurname);
            this.pnlSearchCard.Controls.Add(this.lblSessionFrom);
            this.pnlSearchCard.Controls.Add(this.dtpSessionFrom);
            this.pnlSearchCard.Controls.Add(this.lblDateRangeError);
            this.pnlSearchCard.Controls.Add(this.lblSessionTo);
            this.pnlSearchCard.Controls.Add(this.dtpSessionTo);
            this.pnlSearchCard.Controls.Add(this.lblOutcome);
            this.pnlSearchCard.Controls.Add(this.cboOutcome);
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
            // lblSessionNumber
            //
            this.lblSessionNumber.AutoSize = true;
            this.lblSessionNumber.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSessionNumber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblSessionNumber.Location = new System.Drawing.Point(16, 8);
            this.lblSessionNumber.TabIndex = 0;
            this.lblSessionNumber.Text = "Session number";
            this.lblSessionNumber.Name = "lblSessionNumber";
            //
            // txtSessionNumber
            //
            this.txtSessionNumber.AccessibleName = "Session number (optional), format yyyy-NNNN";
            this.txtSessionNumber.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtSessionNumber.Location = new System.Drawing.Point(16, 36);
            this.txtSessionNumber.MaxLength = 9;
            this.txtSessionNumber.Size = new System.Drawing.Size(120, 25);
            this.txtSessionNumber.TabIndex = 1;
            this.txtSessionNumber.Name = "txtSessionNumber";
            this.txtSessionNumber.Leave += new System.EventHandler(this.txtSessionNumber_Leave);
            //
            // lblSessionNumberError
            //
            this.lblSessionNumberError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSessionNumberError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblSessionNumberError.Location = new System.Drawing.Point(16, 63);
            this.lblSessionNumberError.Size = new System.Drawing.Size(290, 16);
            this.lblSessionNumberError.TabIndex = 2;
            this.lblSessionNumberError.Text = "";
            this.lblSessionNumberError.Visible = false;
            this.lblSessionNumberError.Name = "lblSessionNumberError";
            //
            // lblSurname
            //
            this.lblSurname.AutoSize = true;
            this.lblSurname.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSurname.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblSurname.Location = new System.Drawing.Point(152, 8);
            this.lblSurname.TabIndex = 3;
            this.lblSurname.Text = "Learner surname";
            this.lblSurname.Name = "lblSurname";
            //
            // txtSurname
            //
            this.txtSurname.AccessibleName = "Learner surname (optional)";
            this.txtSurname.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSurname.Location = new System.Drawing.Point(152, 36);
            this.txtSurname.MaxLength = 50;
            this.txtSurname.Size = new System.Drawing.Size(150, 25);
            this.txtSurname.TabIndex = 4;
            this.txtSurname.Name = "txtSurname";
            //
            // lblSessionFrom
            //
            this.lblSessionFrom.AutoSize = true;
            this.lblSessionFrom.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSessionFrom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblSessionFrom.Location = new System.Drawing.Point(318, 8);
            this.lblSessionFrom.TabIndex = 5;
            this.lblSessionFrom.Text = "Session from";
            this.lblSessionFrom.Name = "lblSessionFrom";
            //
            // dtpSessionFrom
            //
            this.dtpSessionFrom.AccessibleName = "Session from (optional)";
            this.dtpSessionFrom.Checked = false;
            this.dtpSessionFrom.CustomFormat = "dd/MM/yyyy";
            this.dtpSessionFrom.Font = new System.Drawing.Font("Consolas", 10F);
            this.dtpSessionFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSessionFrom.Location = new System.Drawing.Point(318, 36);
            this.dtpSessionFrom.ShowCheckBox = true;
            this.dtpSessionFrom.Size = new System.Drawing.Size(136, 23);
            this.dtpSessionFrom.TabIndex = 6;
            this.dtpSessionFrom.Name = "dtpSessionFrom";
            //
            // lblDateRangeError
            //
            this.lblDateRangeError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblDateRangeError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblDateRangeError.Location = new System.Drawing.Point(318, 63);
            this.lblDateRangeError.Size = new System.Drawing.Size(300, 16);
            this.lblDateRangeError.TabIndex = 7;
            this.lblDateRangeError.Text = "";
            this.lblDateRangeError.Visible = false;
            this.lblDateRangeError.Name = "lblDateRangeError";
            //
            // lblSessionTo
            //
            this.lblSessionTo.AutoSize = true;
            this.lblSessionTo.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSessionTo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblSessionTo.Location = new System.Drawing.Point(470, 8);
            this.lblSessionTo.TabIndex = 8;
            this.lblSessionTo.Text = "Session to";
            this.lblSessionTo.Name = "lblSessionTo";
            //
            // dtpSessionTo
            //
            this.dtpSessionTo.AccessibleName = "Session to (optional)";
            this.dtpSessionTo.Checked = false;
            this.dtpSessionTo.CustomFormat = "dd/MM/yyyy";
            this.dtpSessionTo.Font = new System.Drawing.Font("Consolas", 10F);
            this.dtpSessionTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSessionTo.Location = new System.Drawing.Point(470, 36);
            this.dtpSessionTo.ShowCheckBox = true;
            this.dtpSessionTo.Size = new System.Drawing.Size(136, 23);
            this.dtpSessionTo.TabIndex = 9;
            this.dtpSessionTo.Name = "dtpSessionTo";
            //
            // lblOutcome
            //
            this.lblOutcome.AutoSize = true;
            this.lblOutcome.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblOutcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblOutcome.Location = new System.Drawing.Point(622, 8);
            this.lblOutcome.TabIndex = 10;
            this.lblOutcome.Text = "Advisory outcome";
            this.lblOutcome.Name = "lblOutcome";
            //
            // cboOutcome
            //
            this.cboOutcome.AccessibleName = "Advisory outcome (optional)";
            this.cboOutcome.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOutcome.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboOutcome.FormattingEnabled = true;
            this.cboOutcome.Location = new System.Drawing.Point(622, 36);
            this.cboOutcome.Size = new System.Drawing.Size(162, 25);
            this.cboOutcome.TabIndex = 11;
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
            this.btnFind.Location = new System.Drawing.Point(796, 34);
            this.btnFind.Size = new System.Drawing.Size(58, 30);
            this.btnFind.TabIndex = 12;
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
            this.btnClear.TabIndex = 13;
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
            this.lblSearchHint.Location = new System.Drawing.Point(622, 63);
            this.lblSearchHint.Size = new System.Drawing.Size(290, 16);
            this.lblSearchHint.TabIndex = 14;
            this.lblSearchHint.Text = "None required \u00B7 tick a date to use it";
            this.lblSearchHint.Name = "lblSearchHint";
            //
            // pnlResultsCard
            //
            this.pnlResultsCard.Controls.Add(this.lblResultsHeadingBar);
            this.pnlResultsCard.Controls.Add(this.lblResultsHeading);
            this.pnlResultsCard.Controls.Add(this.lblResultsCount);
            this.pnlResultsCard.Controls.Add(this.btnExportSummary);
            this.pnlResultsCard.Controls.Add(this.grdSessions);
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
            this.lblResultsHeading.Size = new System.Drawing.Size(170, 28);
            this.lblResultsHeading.TabIndex = 1;
            this.lblResultsHeading.Text = "Advisory sessions";
            this.lblResultsHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblResultsHeading.Name = "lblResultsHeading";
            //
            // lblResultsCount
            //
            this.lblResultsCount.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblResultsCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblResultsCount.Location = new System.Drawing.Point(196, 16);
            this.lblResultsCount.Size = new System.Drawing.Size(330, 18);
            this.lblResultsCount.TabIndex = 2;
            this.lblResultsCount.Text = "";
            this.lblResultsCount.Name = "lblResultsCount";
            //
            // btnExportSummary
            //
            this.btnExportSummary.AccessibleName = "Export advisory summary";
            this.btnExportSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.btnExportSummary.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportSummary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportSummary.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnExportSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnExportSummary.Location = new System.Drawing.Point(708, 8);
            this.btnExportSummary.Size = new System.Drawing.Size(200, 30);
            this.btnExportSummary.TabIndex = 3;
            this.btnExportSummary.Text = "Export advisory summary";
            this.btnExportSummary.UseVisualStyleBackColor = false;
            this.btnExportSummary.Name = "btnExportSummary";
            this.btnExportSummary.FlatAppearance.BorderSize = 0;
            this.btnExportSummary.Click += new System.EventHandler(this.btnExportSummary_Click);
            //
            // grdSessions
            //
            this.grdSessions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSessionId,
            this.colSessionStatus,
            this.colSession,
            this.colDate,
            this.colLearner,
            this.colGr,
            this.colProgramme,
            this.colOutcome,
            this.colExported});
            grdSessionsAltStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.grdSessions.AlternatingRowsDefaultCellStyle = grdSessionsAltStyle;
            grdSessionsHeaderStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grdSessionsHeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            grdSessionsHeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            grdSessionsHeaderStyle.ForeColor = System.Drawing.Color.White;
            grdSessionsHeaderStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            grdSessionsHeaderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            grdSessionsHeaderStyle.SelectionForeColor = System.Drawing.Color.White;
            grdSessionsHeaderStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdSessions.ColumnHeadersDefaultCellStyle = grdSessionsHeaderStyle;
            grdSessionsCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grdSessionsCellStyle.BackColor = System.Drawing.Color.White;
            grdSessionsCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            grdSessionsCellStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            grdSessionsCellStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            grdSessionsCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            grdSessionsCellStyle.SelectionForeColor = System.Drawing.Color.White;
            grdSessionsCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grdSessions.DefaultCellStyle = grdSessionsCellStyle;
            this.grdSessions.AccessibleName = "Advisory sessions found";
            this.grdSessions.AllowUserToAddRows = false;
            this.grdSessions.AllowUserToDeleteRows = false;
            this.grdSessions.AllowUserToResizeRows = false;
            this.grdSessions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdSessions.BackgroundColor = System.Drawing.Color.White;
            this.grdSessions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.grdSessions.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grdSessions.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.grdSessions.ColumnHeadersHeight = 34;
            this.grdSessions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdSessions.EnableHeadersVisualStyles = false;
            this.grdSessions.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            this.grdSessions.Location = new System.Drawing.Point(16, 46);
            this.grdSessions.MultiSelect = false;
            this.grdSessions.ReadOnly = true;
            this.grdSessions.RowHeadersVisible = false;
            this.grdSessions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdSessions.Size = new System.Drawing.Size(892, 360);
            this.grdSessions.TabIndex = 4;
            this.grdSessions.Name = "grdSessions";
            this.grdSessions.RowTemplate.Height = 30;
            this.grdSessions.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.grdSessions_CellFormatting);
            this.grdSessions.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.grdSessions_CellPainting);
            this.grdSessions.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdSessions_CellDoubleClick);
            //
            // colSessionId
            //
            this.colSessionId.DataPropertyName = "SessionID";
            this.colSessionId.FillWeight = 5F;
            this.colSessionId.HeaderText = "Session ID";
            this.colSessionId.ReadOnly = true;
            this.colSessionId.Visible = false;
            this.colSessionId.Name = "colSessionId";
            //
            // colSessionStatus
            //
            this.colSessionStatus.DataPropertyName = "SessionStatus";
            this.colSessionStatus.FillWeight = 5F;
            this.colSessionStatus.HeaderText = "Status";
            this.colSessionStatus.ReadOnly = true;
            this.colSessionStatus.Visible = false;
            this.colSessionStatus.Name = "colSessionStatus";
            //
            // colSession
            //
            colSessionStyle.Font = new System.Drawing.Font("Consolas", 10F);
            this.colSession.DefaultCellStyle = colSessionStyle;
            this.colSession.DataPropertyName = "SessionNumber";
            this.colSession.FillWeight = 11F;
            this.colSession.HeaderText = "Session";
            this.colSession.ReadOnly = true;
            this.colSession.Name = "colSession";
            //
            // colDate
            //
            colDateStyle.Font = new System.Drawing.Font("Consolas", 10F);
            this.colDate.DefaultCellStyle = colDateStyle;
            this.colDate.DataPropertyName = "SessionDate";
            this.colDate.FillWeight = 11F;
            this.colDate.HeaderText = "Date";
            this.colDate.ReadOnly = true;
            this.colDate.Name = "colDate";
            //
            // colLearner
            //
            this.colLearner.DataPropertyName = "LearnerName";
            this.colLearner.FillWeight = 17F;
            this.colLearner.HeaderText = "Learner";
            this.colLearner.ReadOnly = true;
            this.colLearner.Name = "colLearner";
            //
            // colGr
            //
            colGrStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colGr.DefaultCellStyle = colGrStyle;
            this.colGr.DataPropertyName = "Grade";
            this.colGr.FillWeight = 5F;
            this.colGr.HeaderText = "Gr";
            this.colGr.ReadOnly = true;
            this.colGr.Name = "colGr";
            //
            // colProgramme
            //
            this.colProgramme.DataPropertyName = "ProgrammeName";
            this.colProgramme.FillWeight = 25F;
            this.colProgramme.HeaderText = "Selected programme";
            this.colProgramme.ReadOnly = true;
            this.colProgramme.Name = "colProgramme";
            //
            // colOutcome
            //
            this.colOutcome.DataPropertyName = "OutcomeCode";
            this.colOutcome.FillWeight = 21F;
            this.colOutcome.HeaderText = "Advisory outcome";
            this.colOutcome.ReadOnly = true;
            this.colOutcome.Name = "colOutcome";
            //
            // colExported
            //
            this.colExported.DataPropertyName = "IsExported";
            this.colExported.FillWeight = 9F;
            this.colExported.HeaderText = "Exported";
            this.colExported.ReadOnly = true;
            this.colExported.Name = "colExported";
            //
            // AdvisorySessionTab
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.Size = new System.Drawing.Size(926, 527);
            this.Controls.Add(this.pnlSearchCard);
            this.Controls.Add(this.pnlResultsCard);
            this.Name = "AdvisorySessionTab";
            this.pnlSearchCard.ResumeLayout(false);
            this.pnlSearchCard.PerformLayout();
            this.pnlResultsCard.ResumeLayout(false);
            this.pnlResultsCard.PerformLayout();
            this.lblResultsHeadingBar.ResumeLayout(false);
            this.lblResultsHeadingBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdSessions)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSearchCard;
        private System.Windows.Forms.Label lblSessionNumber;
        private System.Windows.Forms.TextBox txtSessionNumber;
        private System.Windows.Forms.Label lblSessionNumberError;
        private System.Windows.Forms.Label lblSurname;
        private System.Windows.Forms.TextBox txtSurname;
        private System.Windows.Forms.Label lblSessionFrom;
        private System.Windows.Forms.DateTimePicker dtpSessionFrom;
        private System.Windows.Forms.Label lblDateRangeError;
        private System.Windows.Forms.Label lblSessionTo;
        private System.Windows.Forms.DateTimePicker dtpSessionTo;
        private System.Windows.Forms.Label lblOutcome;
        private System.Windows.Forms.ComboBox cboOutcome;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblSearchHint;
        private System.Windows.Forms.Panel pnlResultsCard;
        private System.Windows.Forms.Panel lblResultsHeadingBar;
        private System.Windows.Forms.Label lblResultsHeading;
        private System.Windows.Forms.Label lblResultsCount;
        private System.Windows.Forms.Button btnExportSummary;
        private System.Windows.Forms.DataGridView grdSessions;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSessionId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSessionStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSession;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLearner;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGr;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProgramme;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOutcome;
        private System.Windows.Forms.DataGridViewTextBoxColumn colExported;
    }
}
