namespace FuturePath.PresentationLayer
{
    partial class FundingTab
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
            System.Windows.Forms.DataGridViewCellStyle dgvFundingAltStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvFundingHeaderStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvFundingCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colClosingDateStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colStatusStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlGridCard = new System.Windows.Forms.Panel();
            this.lblGridHeadingBar = new System.Windows.Forms.Panel();
            this.lblGridHeading = new System.Windows.Forms.Label();
            this.lblFundingCount = new System.Windows.Forms.Label();
            this.dgvFunding = new System.Windows.Forms.DataGridView();
            this.colFundingName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFundingType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colClosingDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFundingId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlFormCard = new System.Windows.Forms.Panel();
            this.lblFormHeadingBar = new System.Windows.Forms.Panel();
            this.lblFormHeading = new System.Windows.Forms.Label();
            this.lblFundingName = new System.Windows.Forms.Label();
            this.lblFundingNameStar = new System.Windows.Forms.Label();
            this.txtFundingName = new System.Windows.Forms.TextBox();
            this.lblFundingNameError = new System.Windows.Forms.Label();
            this.lblFundingType = new System.Windows.Forms.Label();
            this.lblFundingTypeStar = new System.Windows.Forms.Label();
            this.cboFundingType = new System.Windows.Forms.ComboBox();
            this.lblFundingTypeError = new System.Windows.Forms.Label();
            this.lblClosingDate = new System.Windows.Forms.Label();
            this.dtpClosingDate = new System.Windows.Forms.DateTimePicker();
            this.lblClosingDateHint = new System.Windows.Forms.Label();
            this.lblContact = new System.Windows.Forms.Label();
            this.txtContact = new System.Windows.Forms.TextBox();
            this.lblEligibility = new System.Windows.Forms.Label();
            this.lblEligibilityStar = new System.Windows.Forms.Label();
            this.txtEligibility = new System.Windows.Forms.TextBox();
            this.lblEligibilityError = new System.Windows.Forms.Label();
            this.lblDocuments = new System.Windows.Forms.Label();
            this.txtDocuments = new System.Windows.Forms.TextBox();
            this.chkMeritBased = new System.Windows.Forms.CheckBox();
            this.chkNeedBased = new System.Windows.Forms.CheckBox();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.lblLinkedFaculties = new System.Windows.Forms.Label();
            this.clbFaculties = new System.Windows.Forms.CheckedListBox();
            this.lblLinkedProgrammes = new System.Windows.Forms.Label();
            this.clbProgrammes = new System.Windows.Forms.CheckedListBox();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.pnlGridCard.SuspendLayout();
            this.lblGridHeadingBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFunding)).BeginInit();
            this.pnlFormCard.SuspendLayout();
            this.lblFormHeadingBar.SuspendLayout();
            this.pnlSummary.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlGridCard
            //
            this.pnlGridCard.Controls.Add(this.lblGridHeadingBar);
            this.pnlGridCard.Controls.Add(this.lblGridHeading);
            this.pnlGridCard.Controls.Add(this.lblFundingCount);
            this.pnlGridCard.Controls.Add(this.dgvFunding);
            this.pnlGridCard.BackColor = System.Drawing.Color.White;
            this.pnlGridCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGridCard.Location = new System.Drawing.Point(0, 0);
            this.pnlGridCard.Size = new System.Drawing.Size(430, 527);
            this.pnlGridCard.TabIndex = 0;
            this.pnlGridCard.Name = "pnlGridCard";
            //
            // lblGridHeadingBar
            //
            this.lblGridHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblGridHeadingBar.Location = new System.Drawing.Point(16, 10);
            this.lblGridHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblGridHeadingBar.TabIndex = 0;
            this.lblGridHeadingBar.Name = "lblGridHeadingBar";
            //
            // lblGridHeading
            //
            this.lblGridHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblGridHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblGridHeading.Location = new System.Drawing.Point(26, 6);
            this.lblGridHeading.Size = new System.Drawing.Size(360, 28);
            this.lblGridHeading.TabIndex = 1;
            this.lblGridHeading.Text = "Stored funding options";
            this.lblGridHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblGridHeading.Name = "lblGridHeading";
            //
            // lblFundingCount
            //
            this.lblFundingCount.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFundingCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblFundingCount.Location = new System.Drawing.Point(16, 36);
            this.lblFundingCount.Size = new System.Drawing.Size(396, 18);
            this.lblFundingCount.TabIndex = 2;
            this.lblFundingCount.Text = "";
            this.lblFundingCount.Name = "lblFundingCount";
            //
            // dgvFunding
            //
            this.dgvFunding.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFundingName,
            this.colFundingType,
            this.colClosingDate,
            this.colStatus,
            this.colFundingId});
            dgvFundingAltStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.dgvFunding.AlternatingRowsDefaultCellStyle = dgvFundingAltStyle;
            dgvFundingHeaderStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dgvFundingHeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            dgvFundingHeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            dgvFundingHeaderStyle.ForeColor = System.Drawing.Color.White;
            dgvFundingHeaderStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            dgvFundingHeaderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            dgvFundingHeaderStyle.SelectionForeColor = System.Drawing.Color.White;
            dgvFundingHeaderStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFunding.ColumnHeadersDefaultCellStyle = dgvFundingHeaderStyle;
            dgvFundingCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dgvFundingCellStyle.BackColor = System.Drawing.Color.White;
            dgvFundingCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            dgvFundingCellStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            dgvFundingCellStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            dgvFundingCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            dgvFundingCellStyle.SelectionForeColor = System.Drawing.Color.White;
            dgvFundingCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFunding.DefaultCellStyle = dgvFundingCellStyle;
            this.dgvFunding.AccessibleName = "Stored funding options";
            this.dgvFunding.AllowUserToAddRows = false;
            this.dgvFunding.AllowUserToDeleteRows = false;
            this.dgvFunding.AllowUserToResizeRows = false;
            this.dgvFunding.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFunding.BackgroundColor = System.Drawing.Color.White;
            this.dgvFunding.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgvFunding.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvFunding.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvFunding.ColumnHeadersHeight = 34;
            this.dgvFunding.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvFunding.EnableHeadersVisualStyles = false;
            this.dgvFunding.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            this.dgvFunding.Location = new System.Drawing.Point(16, 58);
            this.dgvFunding.MultiSelect = false;
            this.dgvFunding.ReadOnly = true;
            this.dgvFunding.RowHeadersVisible = false;
            this.dgvFunding.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFunding.Size = new System.Drawing.Size(396, 453);
            this.dgvFunding.TabIndex = 3;
            this.dgvFunding.Name = "dgvFunding";
            this.dgvFunding.RowTemplate.Height = 30;
            this.dgvFunding.SelectionChanged += new System.EventHandler(this.dgvFunding_SelectionChanged);
            this.dgvFunding.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvFunding_CellFormatting);
            //
            // colFundingName
            //
            this.colFundingName.DataPropertyName = "FundingName";
            this.colFundingName.FillWeight = 34F;
            this.colFundingName.HeaderText = "Funding name";
            this.colFundingName.ReadOnly = true;
            this.colFundingName.Name = "colFundingName";
            //
            // colFundingType
            //
            this.colFundingType.DataPropertyName = "FundingType";
            this.colFundingType.FillWeight = 22F;
            this.colFundingType.HeaderText = "Type";
            this.colFundingType.ReadOnly = true;
            this.colFundingType.Name = "colFundingType";
            //
            // colClosingDate
            //
            colClosingDateStyle.Font = new System.Drawing.Font("Consolas", 10F);
            colClosingDateStyle.Format = "dd MMM yyyy";
            colClosingDateStyle.NullValue = "\u2014";
            this.colClosingDate.DefaultCellStyle = colClosingDateStyle;
            this.colClosingDate.DataPropertyName = "ClosingDate";
            this.colClosingDate.FillWeight = 26F;
            this.colClosingDate.HeaderText = "Closing date";
            this.colClosingDate.ReadOnly = true;
            this.colClosingDate.Name = "colClosingDate";
            //
            // colStatus
            //
            colStatusStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            colStatusStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 8F);
            this.colStatus.DefaultCellStyle = colStatusStyle;
            this.colStatus.DataPropertyName = "IsActive";
            this.colStatus.FillWeight = 14F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.ReadOnly = true;
            this.colStatus.Name = "colStatus";
            //
            // colFundingId
            //
            this.colFundingId.DataPropertyName = "FundingID";
            this.colFundingId.FillWeight = 1F;
            this.colFundingId.HeaderText = "FundingID";
            this.colFundingId.ReadOnly = true;
            this.colFundingId.Visible = false;
            this.colFundingId.Name = "colFundingId";
            //
            // pnlFormCard
            //
            this.pnlFormCard.Controls.Add(this.lblFormHeadingBar);
            this.pnlFormCard.Controls.Add(this.lblFormHeading);
            this.pnlFormCard.Controls.Add(this.lblFundingName);
            this.pnlFormCard.Controls.Add(this.lblFundingNameStar);
            this.pnlFormCard.Controls.Add(this.txtFundingName);
            this.pnlFormCard.Controls.Add(this.lblFundingNameError);
            this.pnlFormCard.Controls.Add(this.lblFundingType);
            this.pnlFormCard.Controls.Add(this.lblFundingTypeStar);
            this.pnlFormCard.Controls.Add(this.cboFundingType);
            this.pnlFormCard.Controls.Add(this.lblFundingTypeError);
            this.pnlFormCard.Controls.Add(this.lblClosingDate);
            this.pnlFormCard.Controls.Add(this.dtpClosingDate);
            this.pnlFormCard.Controls.Add(this.lblClosingDateHint);
            this.pnlFormCard.Controls.Add(this.lblContact);
            this.pnlFormCard.Controls.Add(this.txtContact);
            this.pnlFormCard.Controls.Add(this.lblEligibility);
            this.pnlFormCard.Controls.Add(this.lblEligibilityStar);
            this.pnlFormCard.Controls.Add(this.txtEligibility);
            this.pnlFormCard.Controls.Add(this.lblEligibilityError);
            this.pnlFormCard.Controls.Add(this.lblDocuments);
            this.pnlFormCard.Controls.Add(this.txtDocuments);
            this.pnlFormCard.Controls.Add(this.chkMeritBased);
            this.pnlFormCard.Controls.Add(this.chkNeedBased);
            this.pnlFormCard.Controls.Add(this.chkActive);
            this.pnlFormCard.Controls.Add(this.lblLinkedFaculties);
            this.pnlFormCard.Controls.Add(this.clbFaculties);
            this.pnlFormCard.Controls.Add(this.lblLinkedProgrammes);
            this.pnlFormCard.Controls.Add(this.clbProgrammes);
            this.pnlFormCard.Controls.Add(this.pnlSummary);
            this.pnlFormCard.BackColor = System.Drawing.Color.White;
            this.pnlFormCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFormCard.Location = new System.Drawing.Point(440, 0);
            this.pnlFormCard.Size = new System.Drawing.Size(486, 527);
            this.pnlFormCard.TabIndex = 1;
            this.pnlFormCard.Name = "pnlFormCard";
            //
            // lblFormHeadingBar
            //
            this.lblFormHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblFormHeadingBar.Location = new System.Drawing.Point(16, 10);
            this.lblFormHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblFormHeadingBar.TabIndex = 0;
            this.lblFormHeadingBar.Name = "lblFormHeadingBar";
            //
            // lblFormHeading
            //
            this.lblFormHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFormHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFormHeading.Location = new System.Drawing.Point(26, 6);
            this.lblFormHeading.Size = new System.Drawing.Size(440, 28);
            this.lblFormHeading.TabIndex = 1;
            this.lblFormHeading.Text = "Add or edit a funding option";
            this.lblFormHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFormHeading.Name = "lblFormHeading";
            //
            // lblFundingName
            //
            this.lblFundingName.AutoSize = true;
            this.lblFundingName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFundingName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFundingName.Location = new System.Drawing.Point(16, 40);
            this.lblFundingName.TabIndex = 2;
            this.lblFundingName.Text = "Funding name";
            this.lblFundingName.Name = "lblFundingName";
            //
            // lblFundingNameStar
            //
            this.lblFundingNameStar.AutoSize = true;
            this.lblFundingNameStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFundingNameStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblFundingNameStar.Location = new System.Drawing.Point(131, 40);
            this.lblFundingNameStar.Size = new System.Drawing.Size(14, 24);
            this.lblFundingNameStar.TabIndex = 3;
            this.lblFundingNameStar.Text = "*";
            this.lblFundingNameStar.Name = "lblFundingNameStar";
            //
            // txtFundingName
            //
            this.txtFundingName.AccessibleName = "Funding name (required)";
            this.txtFundingName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtFundingName.Location = new System.Drawing.Point(16, 66);
            this.txtFundingName.MaxLength = 100;
            this.txtFundingName.Size = new System.Drawing.Size(260, 25);
            this.txtFundingName.TabIndex = 4;
            this.txtFundingName.Name = "txtFundingName";
            this.txtFundingName.Leave += new System.EventHandler(this.txtFundingName_Leave);
            //
            // lblFundingNameError
            //
            this.lblFundingNameError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFundingNameError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblFundingNameError.Location = new System.Drawing.Point(16, 93);
            this.lblFundingNameError.Size = new System.Drawing.Size(270, 16);
            this.lblFundingNameError.TabIndex = 5;
            this.lblFundingNameError.Text = "";
            this.lblFundingNameError.Visible = false;
            this.lblFundingNameError.Name = "lblFundingNameError";
            //
            // lblFundingType
            //
            this.lblFundingType.AutoSize = true;
            this.lblFundingType.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFundingType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFundingType.Location = new System.Drawing.Point(292, 40);
            this.lblFundingType.TabIndex = 6;
            this.lblFundingType.Text = "Funding type";
            this.lblFundingType.Name = "lblFundingType";
            //
            // lblFundingTypeStar
            //
            this.lblFundingTypeStar.AutoSize = true;
            this.lblFundingTypeStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFundingTypeStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblFundingTypeStar.Location = new System.Drawing.Point(398, 40);
            this.lblFundingTypeStar.Size = new System.Drawing.Size(14, 24);
            this.lblFundingTypeStar.TabIndex = 7;
            this.lblFundingTypeStar.Text = "*";
            this.lblFundingTypeStar.Name = "lblFundingTypeStar";
            //
            // cboFundingType
            //
            this.cboFundingType.AccessibleName = "Funding type (required)";
            this.cboFundingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFundingType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboFundingType.FormattingEnabled = true;
            this.cboFundingType.Location = new System.Drawing.Point(292, 66);
            this.cboFundingType.Size = new System.Drawing.Size(178, 25);
            this.cboFundingType.TabIndex = 8;
            this.cboFundingType.Name = "cboFundingType";
            this.cboFundingType.Leave += new System.EventHandler(this.cboFundingType_Leave);
            //
            // lblFundingTypeError
            //
            this.lblFundingTypeError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFundingTypeError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblFundingTypeError.Location = new System.Drawing.Point(292, 93);
            this.lblFundingTypeError.Size = new System.Drawing.Size(180, 16);
            this.lblFundingTypeError.TabIndex = 9;
            this.lblFundingTypeError.Text = "";
            this.lblFundingTypeError.Visible = false;
            this.lblFundingTypeError.Name = "lblFundingTypeError";
            //
            // lblClosingDate
            //
            this.lblClosingDate.AutoSize = true;
            this.lblClosingDate.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblClosingDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblClosingDate.Location = new System.Drawing.Point(16, 114);
            this.lblClosingDate.TabIndex = 10;
            this.lblClosingDate.Text = "Closing date";
            this.lblClosingDate.Name = "lblClosingDate";
            //
            // dtpClosingDate
            //
            this.dtpClosingDate.AccessibleName = "Closing date (optional, tick to set)";
            this.dtpClosingDate.Checked = false;
            this.dtpClosingDate.CustomFormat = "dd/MM/yyyy";
            this.dtpClosingDate.Font = new System.Drawing.Font("Consolas", 10F);
            this.dtpClosingDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpClosingDate.Location = new System.Drawing.Point(16, 140);
            this.dtpClosingDate.ShowCheckBox = true;
            this.dtpClosingDate.Size = new System.Drawing.Size(160, 23);
            this.dtpClosingDate.TabIndex = 11;
            this.dtpClosingDate.Name = "dtpClosingDate";
            //
            // lblClosingDateHint
            //
            this.lblClosingDateHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblClosingDateHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblClosingDateHint.Location = new System.Drawing.Point(16, 166);
            this.lblClosingDateHint.Size = new System.Drawing.Size(170, 16);
            this.lblClosingDateHint.TabIndex = 12;
            this.lblClosingDateHint.Text = "Untick when there is none";
            this.lblClosingDateHint.Name = "lblClosingDateHint";
            //
            // lblContact
            //
            this.lblContact.AutoSize = true;
            this.lblContact.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblContact.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblContact.Location = new System.Drawing.Point(192, 114);
            this.lblContact.TabIndex = 13;
            this.lblContact.Text = "Contact information";
            this.lblContact.Name = "lblContact";
            //
            // txtContact
            //
            this.txtContact.AccessibleName = "Contact information (optional)";
            this.txtContact.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtContact.Location = new System.Drawing.Point(192, 140);
            this.txtContact.MaxLength = 150;
            this.txtContact.Size = new System.Drawing.Size(278, 25);
            this.txtContact.TabIndex = 14;
            this.txtContact.Name = "txtContact";
            //
            // lblEligibility
            //
            this.lblEligibility.AutoSize = true;
            this.lblEligibility.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblEligibility.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblEligibility.Location = new System.Drawing.Point(16, 188);
            this.lblEligibility.TabIndex = 15;
            this.lblEligibility.Text = "Eligibility description";
            this.lblEligibility.Name = "lblEligibility";
            //
            // lblEligibilityStar
            //
            this.lblEligibilityStar.AutoSize = true;
            this.lblEligibilityStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblEligibilityStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblEligibilityStar.Location = new System.Drawing.Point(187, 188);
            this.lblEligibilityStar.Size = new System.Drawing.Size(14, 24);
            this.lblEligibilityStar.TabIndex = 16;
            this.lblEligibilityStar.Text = "*";
            this.lblEligibilityStar.Name = "lblEligibilityStar";
            //
            // txtEligibility
            //
            this.txtEligibility.AcceptsReturn = true;
            this.txtEligibility.AccessibleName = "Eligibility description (required, up to 500 characters)";
            this.txtEligibility.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtEligibility.Location = new System.Drawing.Point(16, 214);
            this.txtEligibility.MaxLength = 500;
            this.txtEligibility.Multiline = true;
            this.txtEligibility.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEligibility.Size = new System.Drawing.Size(454, 52);
            this.txtEligibility.TabIndex = 17;
            this.txtEligibility.Name = "txtEligibility";
            this.txtEligibility.Leave += new System.EventHandler(this.txtEligibility_Leave);
            //
            // lblEligibilityError
            //
            this.lblEligibilityError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblEligibilityError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblEligibilityError.Location = new System.Drawing.Point(16, 268);
            this.lblEligibilityError.Size = new System.Drawing.Size(454, 16);
            this.lblEligibilityError.TabIndex = 18;
            this.lblEligibilityError.Text = "";
            this.lblEligibilityError.Visible = false;
            this.lblEligibilityError.Name = "lblEligibilityError";
            //
            // lblDocuments
            //
            this.lblDocuments.AutoSize = true;
            this.lblDocuments.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblDocuments.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblDocuments.Location = new System.Drawing.Point(16, 288);
            this.lblDocuments.TabIndex = 19;
            this.lblDocuments.Text = "Required documents";
            this.lblDocuments.Name = "lblDocuments";
            //
            // txtDocuments
            //
            this.txtDocuments.AccessibleName = "Required documents (optional)";
            this.txtDocuments.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDocuments.Location = new System.Drawing.Point(16, 314);
            this.txtDocuments.MaxLength = 255;
            this.txtDocuments.Size = new System.Drawing.Size(454, 25);
            this.txtDocuments.TabIndex = 20;
            this.txtDocuments.Name = "txtDocuments";
            //
            // chkMeritBased
            //
            this.chkMeritBased.AccessibleName = "Merit-based";
            this.chkMeritBased.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkMeritBased.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.chkMeritBased.Location = new System.Drawing.Point(16, 348);
            this.chkMeritBased.Size = new System.Drawing.Size(130, 24);
            this.chkMeritBased.TabIndex = 21;
            this.chkMeritBased.Text = "Merit-based";
            this.chkMeritBased.UseVisualStyleBackColor = true;
            this.chkMeritBased.Name = "chkMeritBased";
            //
            // chkNeedBased
            //
            this.chkNeedBased.AccessibleName = "Need-based";
            this.chkNeedBased.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkNeedBased.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.chkNeedBased.Location = new System.Drawing.Point(156, 348);
            this.chkNeedBased.Size = new System.Drawing.Size(130, 24);
            this.chkNeedBased.TabIndex = 22;
            this.chkNeedBased.Text = "Need-based";
            this.chkNeedBased.UseVisualStyleBackColor = true;
            this.chkNeedBased.Name = "chkNeedBased";
            //
            // chkActive
            //
            this.chkActive.AccessibleName = "Funding option is active";
            this.chkActive.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.chkActive.Location = new System.Drawing.Point(296, 348);
            this.chkActive.Size = new System.Drawing.Size(90, 24);
            this.chkActive.TabIndex = 23;
            this.chkActive.Text = "Active";
            this.chkActive.UseVisualStyleBackColor = true;
            this.chkActive.Name = "chkActive";
            //
            // lblLinkedFaculties
            //
            this.lblLinkedFaculties.AutoSize = true;
            this.lblLinkedFaculties.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblLinkedFaculties.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblLinkedFaculties.Location = new System.Drawing.Point(16, 378);
            this.lblLinkedFaculties.TabIndex = 24;
            this.lblLinkedFaculties.Text = "Linked faculties";
            this.lblLinkedFaculties.Name = "lblLinkedFaculties";
            //
            // clbFaculties
            //
            this.clbFaculties.AccessibleName = "Linked faculties (tick every faculty this funding applies to)";
            this.clbFaculties.CheckOnClick = true;
            this.clbFaculties.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.clbFaculties.FormattingEnabled = true;
            this.clbFaculties.IntegralHeight = false;
            this.clbFaculties.Location = new System.Drawing.Point(16, 404);
            this.clbFaculties.Size = new System.Drawing.Size(219, 64);
            this.clbFaculties.TabIndex = 25;
            this.clbFaculties.Name = "clbFaculties";
            //
            // lblLinkedProgrammes
            //
            this.lblLinkedProgrammes.AutoSize = true;
            this.lblLinkedProgrammes.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblLinkedProgrammes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblLinkedProgrammes.Location = new System.Drawing.Point(251, 378);
            this.lblLinkedProgrammes.TabIndex = 26;
            this.lblLinkedProgrammes.Text = "Linked programmes";
            this.lblLinkedProgrammes.Name = "lblLinkedProgrammes";
            //
            // clbProgrammes
            //
            this.clbProgrammes.AccessibleName = "Linked programmes (tick every programme this funding applies to)";
            this.clbProgrammes.CheckOnClick = true;
            this.clbProgrammes.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.clbProgrammes.FormattingEnabled = true;
            this.clbProgrammes.IntegralHeight = false;
            this.clbProgrammes.Location = new System.Drawing.Point(251, 404);
            this.clbProgrammes.Size = new System.Drawing.Size(219, 64);
            this.clbProgrammes.TabIndex = 27;
            this.clbProgrammes.Name = "clbProgrammes";
            //
            // pnlSummary
            //
            this.pnlSummary.Controls.Add(this.lblSummary);
            this.pnlSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.pnlSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSummary.Location = new System.Drawing.Point(16, 474);
            this.pnlSummary.Size = new System.Drawing.Size(454, 46);
            this.pnlSummary.TabIndex = 28;
            this.pnlSummary.Visible = false;
            this.pnlSummary.Name = "pnlSummary";
            //
            // lblSummary
            //
            this.lblSummary.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblSummary.Location = new System.Drawing.Point(12, 6);
            this.lblSummary.Size = new System.Drawing.Size(428, 34);
            this.lblSummary.TabIndex = 0;
            this.lblSummary.Text = "";
            this.lblSummary.Name = "lblSummary";
            //
            // FundingTab
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.Size = new System.Drawing.Size(926, 527);
            this.Controls.Add(this.pnlGridCard);
            this.Controls.Add(this.pnlFormCard);
            this.Name = "FundingTab";
            this.pnlGridCard.ResumeLayout(false);
            this.pnlGridCard.PerformLayout();
            this.lblGridHeadingBar.ResumeLayout(false);
            this.lblGridHeadingBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFunding)).EndInit();
            this.pnlFormCard.ResumeLayout(false);
            this.pnlFormCard.PerformLayout();
            this.lblFormHeadingBar.ResumeLayout(false);
            this.lblFormHeadingBar.PerformLayout();
            this.pnlSummary.ResumeLayout(false);
            this.pnlSummary.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlGridCard;
        private System.Windows.Forms.Panel lblGridHeadingBar;
        private System.Windows.Forms.Label lblGridHeading;
        private System.Windows.Forms.Label lblFundingCount;
        private System.Windows.Forms.DataGridView dgvFunding;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFundingName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFundingType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClosingDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFundingId;
        private System.Windows.Forms.Panel pnlFormCard;
        private System.Windows.Forms.Panel lblFormHeadingBar;
        private System.Windows.Forms.Label lblFormHeading;
        private System.Windows.Forms.Label lblFundingName;
        private System.Windows.Forms.Label lblFundingNameStar;
        private System.Windows.Forms.TextBox txtFundingName;
        private System.Windows.Forms.Label lblFundingNameError;
        private System.Windows.Forms.Label lblFundingType;
        private System.Windows.Forms.Label lblFundingTypeStar;
        private System.Windows.Forms.ComboBox cboFundingType;
        private System.Windows.Forms.Label lblFundingTypeError;
        private System.Windows.Forms.Label lblClosingDate;
        private System.Windows.Forms.DateTimePicker dtpClosingDate;
        private System.Windows.Forms.Label lblClosingDateHint;
        private System.Windows.Forms.Label lblContact;
        private System.Windows.Forms.TextBox txtContact;
        private System.Windows.Forms.Label lblEligibility;
        private System.Windows.Forms.Label lblEligibilityStar;
        private System.Windows.Forms.TextBox txtEligibility;
        private System.Windows.Forms.Label lblEligibilityError;
        private System.Windows.Forms.Label lblDocuments;
        private System.Windows.Forms.TextBox txtDocuments;
        private System.Windows.Forms.CheckBox chkMeritBased;
        private System.Windows.Forms.CheckBox chkNeedBased;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.Label lblLinkedFaculties;
        private System.Windows.Forms.CheckedListBox clbFaculties;
        private System.Windows.Forms.Label lblLinkedProgrammes;
        private System.Windows.Forms.CheckedListBox clbProgrammes;
        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Label lblSummary;
    }
}
