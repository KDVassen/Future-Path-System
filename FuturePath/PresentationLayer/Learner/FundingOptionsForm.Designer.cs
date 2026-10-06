namespace FuturePath.PresentationLayer
{
    partial class FundingOptionsForm
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
            System.Windows.Forms.DataGridViewCellStyle dgvFundingAltStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvFundingHeaderStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvFundingCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colClosingDateStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlBody = new System.Windows.Forms.Panel();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlFilterCard = new System.Windows.Forms.Panel();
            this.lblFaculty = new System.Windows.Forms.Label();
            this.cboFaculty = new System.Windows.Forms.ComboBox();
            this.lblFundingType = new System.Windows.Forms.Label();
            this.cboFundingType = new System.Windows.Forms.ComboBox();
            this.chkOpenOnly = new System.Windows.Forms.CheckBox();
            this.pnlFundingCard = new System.Windows.Forms.Panel();
            this.lblFundingHeadingBar = new System.Windows.Forms.Panel();
            this.lblFundingHeading = new System.Windows.Forms.Label();
            this.lblFundingCount = new System.Windows.Forms.Label();
            this.dgvFunding = new System.Windows.Forms.DataGridView();
            this.colFundingName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFundingType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLinkedTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colClosingDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFundingId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEligibility = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDocuments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colContact = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlDetailCard = new System.Windows.Forms.Panel();
            this.lblDetailHeadingBar = new System.Windows.Forms.Panel();
            this.lblDetailHeading = new System.Windows.Forms.Label();
            this.lblEligibility = new System.Windows.Forms.Label();
            this.txtEligibility = new System.Windows.Forms.TextBox();
            this.lblDocuments = new System.Windows.Forms.Label();
            this.txtDocuments = new System.Windows.Forms.TextBox();
            this.lblContact = new System.Windows.Forms.Label();
            this.txtContact = new System.Windows.Forms.TextBox();
            this.pnlSampleNote = new System.Windows.Forms.Panel();
            this.lblSampleNote = new System.Windows.Forms.Label();
            this.pnlSampleNoteBar = new System.Windows.Forms.Panel();
            this.pnlContextGap = new System.Windows.Forms.Panel();
            this.pnlContext = new System.Windows.Forms.Panel();
            this.lblContext = new System.Windows.Forms.Label();
            this.pnlContextBar = new System.Windows.Forms.Panel();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblBadge = new System.Windows.Forms.Label();
            this.pnlTitleRule = new System.Windows.Forms.Panel();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.pnlActionRule = new System.Windows.Forms.Panel();
            this.btnAddToSummary = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.lblActionHint = new System.Windows.Forms.Label();
            this.pnlRail = new System.Windows.Forms.Panel();
            this.lblBrand = new System.Windows.Forms.Label();
            this.pnlBrandRule = new System.Windows.Forms.Panel();
            this.btnNavHome = new System.Windows.Forms.Button();
            this.lblStep1 = new System.Windows.Forms.Label();
            this.lblStep2 = new System.Windows.Forms.Label();
            this.lblStep3 = new System.Windows.Forms.Label();
            this.lblStep4 = new System.Windows.Forms.Label();
            this.lblStep5 = new System.Windows.Forms.Label();
            this.lblStep6 = new System.Windows.Forms.Label();
            this.lblRailFooter = new System.Windows.Forms.Label();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlBody.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlFilterCard.SuspendLayout();
            this.pnlFundingCard.SuspendLayout();
            this.lblFundingHeadingBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFunding)).BeginInit();
            this.pnlDetailCard.SuspendLayout();
            this.lblDetailHeadingBar.SuspendLayout();
            this.pnlSampleNote.SuspendLayout();
            this.pnlSampleNoteBar.SuspendLayout();
            this.pnlContextGap.SuspendLayout();
            this.pnlContext.SuspendLayout();
            this.pnlContextBar.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlTitleRule.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.pnlActionRule.SuspendLayout();
            this.pnlRail.SuspendLayout();
            this.pnlBrandRule.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlBody
            //
            this.pnlBody.Controls.Add(this.pnlContent);
            this.pnlBody.Controls.Add(this.pnlContextGap);
            this.pnlBody.Controls.Add(this.pnlContext);
            this.pnlBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Location = new System.Drawing.Point(210, 84);
            this.pnlBody.Padding = new System.Windows.Forms.Padding(20, 12, 20, 12);
            this.pnlBody.Size = new System.Drawing.Size(774, 507);
            this.pnlBody.TabIndex = 0;
            this.pnlBody.Name = "pnlBody";
            //
            // pnlContent
            //
            this.pnlContent.Controls.Add(this.pnlFilterCard);
            this.pnlContent.Controls.Add(this.pnlFundingCard);
            this.pnlContent.Controls.Add(this.pnlDetailCard);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(20, 12);
            this.pnlContent.Size = new System.Drawing.Size(734, 483);
            this.pnlContent.TabIndex = 0;
            this.pnlContent.Name = "pnlContent";
            //
            // pnlFilterCard
            //
            this.pnlFilterCard.Controls.Add(this.lblFaculty);
            this.pnlFilterCard.Controls.Add(this.cboFaculty);
            this.pnlFilterCard.Controls.Add(this.lblFundingType);
            this.pnlFilterCard.Controls.Add(this.cboFundingType);
            this.pnlFilterCard.Controls.Add(this.chkOpenOnly);
            this.pnlFilterCard.BackColor = System.Drawing.Color.White;
            this.pnlFilterCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFilterCard.Location = new System.Drawing.Point(0, 0);
            this.pnlFilterCard.Size = new System.Drawing.Size(734, 70);
            this.pnlFilterCard.TabIndex = 0;
            this.pnlFilterCard.Name = "pnlFilterCard";
            //
            // lblFaculty
            //
            this.lblFaculty.AutoSize = true;
            this.lblFaculty.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFaculty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFaculty.Location = new System.Drawing.Point(16, 6);
            this.lblFaculty.TabIndex = 0;
            this.lblFaculty.Text = "Faculty";
            this.lblFaculty.Name = "lblFaculty";
            //
            // cboFaculty
            //
            this.cboFaculty.AccessibleName = "Faculty";
            this.cboFaculty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFaculty.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboFaculty.FormattingEnabled = true;
            this.cboFaculty.Location = new System.Drawing.Point(16, 36);
            this.cboFaculty.Size = new System.Drawing.Size(210, 25);
            this.cboFaculty.TabIndex = 1;
            this.cboFaculty.Name = "cboFaculty";
            this.cboFaculty.SelectionChangeCommitted += new System.EventHandler(this.cboFaculty_SelectionChangeCommitted);
            //
            // lblFundingType
            //
            this.lblFundingType.AutoSize = true;
            this.lblFundingType.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFundingType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFundingType.Location = new System.Drawing.Point(246, 6);
            this.lblFundingType.TabIndex = 2;
            this.lblFundingType.Text = "Funding type";
            this.lblFundingType.Name = "lblFundingType";
            //
            // cboFundingType
            //
            this.cboFundingType.AccessibleName = "Funding type";
            this.cboFundingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFundingType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboFundingType.FormattingEnabled = true;
            this.cboFundingType.Location = new System.Drawing.Point(246, 36);
            this.cboFundingType.Size = new System.Drawing.Size(210, 25);
            this.cboFundingType.TabIndex = 3;
            this.cboFundingType.Name = "cboFundingType";
            this.cboFundingType.SelectionChangeCommitted += new System.EventHandler(this.cboFundingType_SelectionChangeCommitted);
            //
            // chkOpenOnly
            //
            this.chkOpenOnly.AccessibleName = "Show only options that are still open";
            this.chkOpenOnly.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkOpenOnly.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.chkOpenOnly.Location = new System.Drawing.Point(476, 36);
            this.chkOpenOnly.Size = new System.Drawing.Size(250, 24);
            this.chkOpenOnly.TabIndex = 4;
            this.chkOpenOnly.Text = "Show only options that are still open";
            this.chkOpenOnly.UseVisualStyleBackColor = true;
            this.chkOpenOnly.Name = "chkOpenOnly";
            this.chkOpenOnly.CheckedChanged += new System.EventHandler(this.chkOpenOnly_CheckedChanged);
            //
            // pnlFundingCard
            //
            this.pnlFundingCard.Controls.Add(this.lblFundingHeadingBar);
            this.pnlFundingCard.Controls.Add(this.lblFundingHeading);
            this.pnlFundingCard.Controls.Add(this.lblFundingCount);
            this.pnlFundingCard.Controls.Add(this.dgvFunding);
            this.pnlFundingCard.BackColor = System.Drawing.Color.White;
            this.pnlFundingCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFundingCard.Location = new System.Drawing.Point(0, 80);
            this.pnlFundingCard.Size = new System.Drawing.Size(734, 176);
            this.pnlFundingCard.TabIndex = 1;
            this.pnlFundingCard.Name = "pnlFundingCard";
            //
            // lblFundingHeadingBar
            //
            this.lblFundingHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblFundingHeadingBar.Location = new System.Drawing.Point(16, 12);
            this.lblFundingHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblFundingHeadingBar.TabIndex = 0;
            this.lblFundingHeadingBar.Name = "lblFundingHeadingBar";
            //
            // lblFundingHeading
            //
            this.lblFundingHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFundingHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFundingHeading.Location = new System.Drawing.Point(26, 8);
            this.lblFundingHeading.Size = new System.Drawing.Size(460, 28);
            this.lblFundingHeading.TabIndex = 1;
            this.lblFundingHeading.Text = "Funding linked to your faculty";
            this.lblFundingHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFundingHeading.Name = "lblFundingHeading";
            //
            // lblFundingCount
            //
            this.lblFundingCount.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFundingCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblFundingCount.Location = new System.Drawing.Point(480, 14);
            this.lblFundingCount.Size = new System.Drawing.Size(236, 18);
            this.lblFundingCount.TabIndex = 2;
            this.lblFundingCount.Text = "";
            this.lblFundingCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblFundingCount.Name = "lblFundingCount";
            //
            // dgvFunding
            //
            this.dgvFunding.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFundingName,
            this.colFundingType,
            this.colLinkedTo,
            this.colClosingDate,
            this.colStatus,
            this.colFundingId,
            this.colEligibility,
            this.colDocuments,
            this.colContact});
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
            this.dgvFunding.AccessibleName = "Funding linked to your faculty and programme";
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
            this.dgvFunding.Location = new System.Drawing.Point(16, 42);
            this.dgvFunding.MultiSelect = false;
            this.dgvFunding.ReadOnly = true;
            this.dgvFunding.RowHeadersVisible = false;
            this.dgvFunding.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFunding.Size = new System.Drawing.Size(700, 128);
            this.dgvFunding.TabIndex = 3;
            this.dgvFunding.Name = "dgvFunding";
            this.dgvFunding.RowTemplate.Height = 30;
            this.dgvFunding.SelectionChanged += new System.EventHandler(this.dgvFunding_SelectionChanged);
            this.dgvFunding.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvFunding_CellFormatting);
            this.dgvFunding.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvFunding_DataBindingComplete);
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
            this.colFundingType.FillWeight = 18F;
            this.colFundingType.HeaderText = "Funding type";
            this.colFundingType.ReadOnly = true;
            this.colFundingType.Name = "colFundingType";
            //
            // colLinkedTo
            //
            this.colLinkedTo.DataPropertyName = "LinkedTo";
            this.colLinkedTo.FillWeight = 22F;
            this.colLinkedTo.HeaderText = "Linked to";
            this.colLinkedTo.ReadOnly = true;
            this.colLinkedTo.Name = "colLinkedTo";
            //
            // colClosingDate
            //
            colClosingDateStyle.Font = new System.Drawing.Font("Consolas", 10F);
            colClosingDateStyle.Format = "dd/MM/yyyy";
            this.colClosingDate.DefaultCellStyle = colClosingDateStyle;
            this.colClosingDate.DataPropertyName = "ClosingDate";
            this.colClosingDate.FillWeight = 16F;
            this.colClosingDate.HeaderText = "Closing date";
            this.colClosingDate.ReadOnly = true;
            this.colClosingDate.Name = "colClosingDate";
            //
            // colStatus
            //
            this.colStatus.FillWeight = 12F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.ReadOnly = true;
            this.colStatus.Name = "colStatus";
            //
            // colFundingId
            //
            this.colFundingId.DataPropertyName = "FundingID";
            this.colFundingId.FillWeight = 1F;
            this.colFundingId.HeaderText = "Funding ID";
            this.colFundingId.ReadOnly = true;
            this.colFundingId.Visible = false;
            this.colFundingId.Name = "colFundingId";
            //
            // colEligibility
            //
            this.colEligibility.DataPropertyName = "EligibilityDescription";
            this.colEligibility.FillWeight = 1F;
            this.colEligibility.HeaderText = "Eligibility";
            this.colEligibility.ReadOnly = true;
            this.colEligibility.Visible = false;
            this.colEligibility.Name = "colEligibility";
            //
            // colDocuments
            //
            this.colDocuments.DataPropertyName = "RequiredDocuments";
            this.colDocuments.FillWeight = 1F;
            this.colDocuments.HeaderText = "Required documents";
            this.colDocuments.ReadOnly = true;
            this.colDocuments.Visible = false;
            this.colDocuments.Name = "colDocuments";
            //
            // colContact
            //
            this.colContact.DataPropertyName = "ContactInformation";
            this.colContact.FillWeight = 1F;
            this.colContact.HeaderText = "Contact";
            this.colContact.ReadOnly = true;
            this.colContact.Visible = false;
            this.colContact.Name = "colContact";
            //
            // pnlDetailCard
            //
            this.pnlDetailCard.Controls.Add(this.lblDetailHeadingBar);
            this.pnlDetailCard.Controls.Add(this.lblDetailHeading);
            this.pnlDetailCard.Controls.Add(this.lblEligibility);
            this.pnlDetailCard.Controls.Add(this.txtEligibility);
            this.pnlDetailCard.Controls.Add(this.lblDocuments);
            this.pnlDetailCard.Controls.Add(this.txtDocuments);
            this.pnlDetailCard.Controls.Add(this.lblContact);
            this.pnlDetailCard.Controls.Add(this.txtContact);
            this.pnlDetailCard.Controls.Add(this.pnlSampleNote);
            this.pnlDetailCard.BackColor = System.Drawing.Color.White;
            this.pnlDetailCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDetailCard.Location = new System.Drawing.Point(0, 264);
            this.pnlDetailCard.Size = new System.Drawing.Size(734, 172);
            this.pnlDetailCard.TabIndex = 2;
            this.pnlDetailCard.Name = "pnlDetailCard";
            //
            // lblDetailHeadingBar
            //
            this.lblDetailHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblDetailHeadingBar.Location = new System.Drawing.Point(16, 10);
            this.lblDetailHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblDetailHeadingBar.TabIndex = 0;
            this.lblDetailHeadingBar.Name = "lblDetailHeadingBar";
            //
            // lblDetailHeading
            //
            this.lblDetailHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblDetailHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblDetailHeading.Location = new System.Drawing.Point(26, 6);
            this.lblDetailHeading.Size = new System.Drawing.Size(700, 28);
            this.lblDetailHeading.TabIndex = 1;
            this.lblDetailHeading.Text = "Select a funding option to see its details";
            this.lblDetailHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDetailHeading.Name = "lblDetailHeading";
            //
            // lblEligibility
            //
            this.lblEligibility.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblEligibility.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblEligibility.Location = new System.Drawing.Point(16, 38);
            this.lblEligibility.Size = new System.Drawing.Size(222, 22);
            this.lblEligibility.TabIndex = 2;
            this.lblEligibility.Text = "Eligibility";
            this.lblEligibility.Name = "lblEligibility";
            //
            // txtEligibility
            //
            this.txtEligibility.AccessibleName = "Eligibility (read-only)";
            this.txtEligibility.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.txtEligibility.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtEligibility.Location = new System.Drawing.Point(16, 62);
            this.txtEligibility.Multiline = true;
            this.txtEligibility.ReadOnly = true;
            this.txtEligibility.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtEligibility.Size = new System.Drawing.Size(222, 58);
            this.txtEligibility.TabIndex = 3;
            this.txtEligibility.TabStop = false;
            this.txtEligibility.Name = "txtEligibility";
            //
            // lblDocuments
            //
            this.lblDocuments.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblDocuments.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblDocuments.Location = new System.Drawing.Point(256, 38);
            this.lblDocuments.Size = new System.Drawing.Size(222, 22);
            this.lblDocuments.TabIndex = 4;
            this.lblDocuments.Text = "Required documents";
            this.lblDocuments.Name = "lblDocuments";
            //
            // txtDocuments
            //
            this.txtDocuments.AccessibleName = "Required documents (read-only)";
            this.txtDocuments.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.txtDocuments.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDocuments.Location = new System.Drawing.Point(256, 62);
            this.txtDocuments.Multiline = true;
            this.txtDocuments.ReadOnly = true;
            this.txtDocuments.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtDocuments.Size = new System.Drawing.Size(222, 58);
            this.txtDocuments.TabIndex = 5;
            this.txtDocuments.TabStop = false;
            this.txtDocuments.Name = "txtDocuments";
            //
            // lblContact
            //
            this.lblContact.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblContact.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblContact.Location = new System.Drawing.Point(496, 38);
            this.lblContact.Size = new System.Drawing.Size(222, 22);
            this.lblContact.TabIndex = 6;
            this.lblContact.Text = "Contact or application information";
            this.lblContact.Name = "lblContact";
            //
            // txtContact
            //
            this.txtContact.AccessibleName = "Contact or application information (read-only)";
            this.txtContact.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.txtContact.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtContact.Location = new System.Drawing.Point(496, 62);
            this.txtContact.Multiline = true;
            this.txtContact.ReadOnly = true;
            this.txtContact.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtContact.Size = new System.Drawing.Size(222, 58);
            this.txtContact.TabIndex = 7;
            this.txtContact.TabStop = false;
            this.txtContact.Name = "txtContact";
            //
            // pnlSampleNote
            //
            this.pnlSampleNote.Controls.Add(this.lblSampleNote);
            this.pnlSampleNote.Controls.Add(this.pnlSampleNoteBar);
            this.pnlSampleNote.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(243)))), ((int)(((byte)(224)))));
            this.pnlSampleNote.Location = new System.Drawing.Point(16, 126);
            this.pnlSampleNote.Size = new System.Drawing.Size(700, 38);
            this.pnlSampleNote.TabIndex = 8;
            this.pnlSampleNote.Name = "pnlSampleNote";
            //
            // lblSampleNote
            //
            this.lblSampleNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSampleNote.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSampleNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(93)))), ((int)(((byte)(24)))));
            this.lblSampleNote.Location = new System.Drawing.Point(5, 0);
            this.lblSampleNote.Padding = new System.Windows.Forms.Padding(12, 0, 8, 0);
            this.lblSampleNote.Size = new System.Drawing.Size(695, 38);
            this.lblSampleNote.TabIndex = 0;
            this.lblSampleNote.Text = "Sample information. Verify every closing date and requirement with the funder before you apply. Future Path does not submit applications.";
            this.lblSampleNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSampleNote.Name = "lblSampleNote";
            //
            // pnlSampleNoteBar
            //
            this.pnlSampleNoteBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.pnlSampleNoteBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSampleNoteBar.Location = new System.Drawing.Point(0, 0);
            this.pnlSampleNoteBar.Size = new System.Drawing.Size(5, 38);
            this.pnlSampleNoteBar.TabIndex = 1;
            this.pnlSampleNoteBar.Name = "pnlSampleNoteBar";
            //
            // pnlContextGap
            //
            this.pnlContextGap.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlContextGap.Location = new System.Drawing.Point(20, 0);
            this.pnlContextGap.Size = new System.Drawing.Size(734, 10);
            this.pnlContextGap.TabIndex = 1;
            this.pnlContextGap.Name = "pnlContextGap";
            //
            // pnlContext
            //
            this.pnlContext.Controls.Add(this.lblContext);
            this.pnlContext.Controls.Add(this.pnlContextBar);
            this.pnlContext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.pnlContext.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlContext.Location = new System.Drawing.Point(20, 12);
            this.pnlContext.Size = new System.Drawing.Size(734, 36);
            this.pnlContext.TabIndex = 2;
            this.pnlContext.Name = "pnlContext";
            //
            // lblContext
            //
            this.lblContext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblContext.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblContext.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblContext.Location = new System.Drawing.Point(5, 0);
            this.lblContext.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblContext.Size = new System.Drawing.Size(729, 36);
            this.lblContext.TabIndex = 0;
            this.lblContext.Text = "";
            this.lblContext.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblContext.Name = "lblContext";
            //
            // pnlContextBar
            //
            this.pnlContextBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.pnlContextBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlContextBar.Location = new System.Drawing.Point(0, 0);
            this.pnlContextBar.Size = new System.Drawing.Size(5, 36);
            this.pnlContextBar.TabIndex = 1;
            this.pnlContextBar.Name = "pnlContextBar";
            //
            // pnlTitle
            //
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Controls.Add(this.lblSubtitle);
            this.pnlTitle.Controls.Add(this.lblBadge);
            this.pnlTitle.Controls.Add(this.pnlTitleRule);
            this.pnlTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(210, 0);
            this.pnlTitle.Size = new System.Drawing.Size(774, 84);
            this.pnlTitle.TabIndex = 1;
            this.pnlTitle.Name = "pnlTitle";
            //
            // lblTitle
            //
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 10);
            this.lblTitle.Size = new System.Drawing.Size(560, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Funding options";
            this.lblTitle.Name = "lblTitle";
            //
            // lblSubtitle
            //
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.lblSubtitle.Location = new System.Drawing.Point(24, 54);
            this.lblSubtitle.Size = new System.Drawing.Size(714, 18);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Step 5 of 6 \u00B7 Funding linked to your selected faculty and qualification";
            this.lblSubtitle.Name = "lblSubtitle";
            //
            // lblBadge
            //
            this.lblBadge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBadge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblBadge.Font = new System.Drawing.Font("Segoe UI Semibold", 8F);
            this.lblBadge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblBadge.Location = new System.Drawing.Point(604, 16);
            this.lblBadge.Size = new System.Drawing.Size(150, 24);
            this.lblBadge.TabIndex = 2;
            this.lblBadge.Text = "LEARNER";
            this.lblBadge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBadge.Name = "lblBadge";
            //
            // pnlTitleRule
            //
            this.pnlTitleRule.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.pnlTitleRule.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTitleRule.Location = new System.Drawing.Point(0, 81);
            this.pnlTitleRule.Size = new System.Drawing.Size(774, 3);
            this.pnlTitleRule.TabIndex = 3;
            this.pnlTitleRule.Name = "pnlTitleRule";
            //
            // pnlActions
            //
            this.pnlActions.Controls.Add(this.pnlActionRule);
            this.pnlActions.Controls.Add(this.btnAddToSummary);
            this.pnlActions.Controls.Add(this.btnBack);
            this.pnlActions.Controls.Add(this.lblActionHint);
            this.pnlActions.BackColor = System.Drawing.Color.White;
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlActions.Location = new System.Drawing.Point(210, 591);
            this.pnlActions.Size = new System.Drawing.Size(774, 64);
            this.pnlActions.TabIndex = 2;
            this.pnlActions.Name = "pnlActions";
            //
            // pnlActionRule
            //
            this.pnlActionRule.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            this.pnlActionRule.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlActionRule.Location = new System.Drawing.Point(0, 0);
            this.pnlActionRule.Size = new System.Drawing.Size(774, 1);
            this.pnlActionRule.TabIndex = 0;
            this.pnlActionRule.Name = "pnlActionRule";
            //
            // btnAddToSummary
            //
            this.btnAddToSummary.AccessibleName = "Add to Summary";
            this.btnAddToSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddToSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.btnAddToSummary.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddToSummary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddToSummary.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnAddToSummary.ForeColor = System.Drawing.Color.White;
            this.btnAddToSummary.Location = new System.Drawing.Point(574, 12);
            this.btnAddToSummary.Size = new System.Drawing.Size(180, 40);
            this.btnAddToSummary.TabIndex = 1;
            this.btnAddToSummary.Text = "Add to Summary";
            this.btnAddToSummary.UseVisualStyleBackColor = false;
            this.btnAddToSummary.Name = "btnAddToSummary";
            this.btnAddToSummary.FlatAppearance.BorderSize = 0;
            this.btnAddToSummary.Click += new System.EventHandler(this.btnAddToSummary_Click);
            //
            // btnBack
            //
            this.btnBack.AccessibleName = "Back";
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.BackColor = System.Drawing.Color.White;
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnBack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnBack.Location = new System.Drawing.Point(434, 12);
            this.btnBack.Size = new System.Drawing.Size(130, 40);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Name = "btnBack";
            this.btnBack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            //
            // lblActionHint
            //
            this.lblActionHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblActionHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblActionHint.Location = new System.Drawing.Point(20, 12);
            this.lblActionHint.Size = new System.Drawing.Size(394, 40);
            this.lblActionHint.TabIndex = 3;
            this.lblActionHint.Text = "Step 5 of 6";
            this.lblActionHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblActionHint.Name = "lblActionHint";
            //
            // pnlRail
            //
            this.pnlRail.Controls.Add(this.lblBrand);
            this.pnlRail.Controls.Add(this.pnlBrandRule);
            this.pnlRail.Controls.Add(this.btnNavHome);
            this.pnlRail.Controls.Add(this.lblStep1);
            this.pnlRail.Controls.Add(this.lblStep2);
            this.pnlRail.Controls.Add(this.lblStep3);
            this.pnlRail.Controls.Add(this.lblStep4);
            this.pnlRail.Controls.Add(this.lblStep5);
            this.pnlRail.Controls.Add(this.lblStep6);
            this.pnlRail.Controls.Add(this.lblRailFooter);
            this.pnlRail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.pnlRail.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlRail.Location = new System.Drawing.Point(0, 0);
            this.pnlRail.Size = new System.Drawing.Size(210, 655);
            this.pnlRail.TabIndex = 3;
            this.pnlRail.Name = "pnlRail";
            //
            // lblBrand
            //
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblBrand.ForeColor = System.Drawing.Color.White;
            this.lblBrand.Location = new System.Drawing.Point(0, 0);
            this.lblBrand.Size = new System.Drawing.Size(210, 66);
            this.lblBrand.TabIndex = 0;
            this.lblBrand.Text = "Future Path";
            this.lblBrand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBrand.Name = "lblBrand";
            //
            // pnlBrandRule
            //
            this.pnlBrandRule.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.pnlBrandRule.Location = new System.Drawing.Point(24, 66);
            this.pnlBrandRule.Size = new System.Drawing.Size(162, 2);
            this.pnlBrandRule.TabIndex = 1;
            this.pnlBrandRule.Name = "pnlBrandRule";
            //
            // btnNavHome
            //
            this.btnNavHome.AccessibleName = "Home";
            this.btnNavHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.btnNavHome.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavHome.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNavHome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.btnNavHome.Location = new System.Drawing.Point(0, 84);
            this.btnNavHome.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.btnNavHome.Size = new System.Drawing.Size(210, 36);
            this.btnNavHome.TabIndex = 2;
            this.btnNavHome.Text = "Home";
            this.btnNavHome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavHome.UseVisualStyleBackColor = false;
            this.btnNavHome.Name = "btnNavHome";
            this.btnNavHome.FlatAppearance.BorderSize = 0;
            this.btnNavHome.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(64)))), ((int)(((byte)(104)))));
            this.btnNavHome.Click += new System.EventHandler(this.btnNavHome_Click);
            //
            // lblStep1
            //
            this.lblStep1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStep1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.lblStep1.Location = new System.Drawing.Point(0, 120);
            this.lblStep1.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.lblStep1.Size = new System.Drawing.Size(210, 36);
            this.lblStep1.TabIndex = 3;
            this.lblStep1.Text = "Learner profile";
            this.lblStep1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStep1.Name = "lblStep1";
            //
            // lblStep2
            //
            this.lblStep2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStep2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.lblStep2.Location = new System.Drawing.Point(0, 156);
            this.lblStep2.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.lblStep2.Size = new System.Drawing.Size(210, 36);
            this.lblStep2.TabIndex = 4;
            this.lblStep2.Text = "Subjects and marks";
            this.lblStep2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStep2.Name = "lblStep2";
            //
            // lblStep3
            //
            this.lblStep3.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStep3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.lblStep3.Location = new System.Drawing.Point(0, 192);
            this.lblStep3.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.lblStep3.Size = new System.Drawing.Size(210, 36);
            this.lblStep3.TabIndex = 5;
            this.lblStep3.Text = "Interest questionnaire";
            this.lblStep3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStep3.Name = "lblStep3";
            //
            // lblStep4
            //
            this.lblStep4.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStep4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.lblStep4.Location = new System.Drawing.Point(0, 228);
            this.lblStep4.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.lblStep4.Size = new System.Drawing.Size(210, 36);
            this.lblStep4.TabIndex = 6;
            this.lblStep4.Text = "Faculty and department";
            this.lblStep4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStep4.Name = "lblStep4";
            //
            // lblStep5
            //
            this.lblStep5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(64)))), ((int)(((byte)(104)))));
            this.lblStep5.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblStep5.ForeColor = System.Drawing.Color.White;
            this.lblStep5.Location = new System.Drawing.Point(0, 264);
            this.lblStep5.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.lblStep5.Size = new System.Drawing.Size(210, 36);
            this.lblStep5.TabIndex = 7;
            this.lblStep5.Text = "Eligibility results";
            this.lblStep5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStep5.Name = "lblStep5";
            //
            // lblStep6
            //
            this.lblStep6.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStep6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(130)))), ((int)(((byte)(158)))));
            this.lblStep6.Location = new System.Drawing.Point(0, 300);
            this.lblStep6.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.lblStep6.Size = new System.Drawing.Size(210, 36);
            this.lblStep6.TabIndex = 8;
            this.lblStep6.Text = "Advisory summary";
            this.lblStep6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStep6.Name = "lblStep6";
            //
            // lblRailFooter
            //
            this.lblRailFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblRailFooter.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblRailFooter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.lblRailFooter.Location = new System.Drawing.Point(0, 575);
            this.lblRailFooter.Padding = new System.Windows.Forms.Padding(16, 0, 8, 0);
            this.lblRailFooter.Size = new System.Drawing.Size(210, 80);
            this.lblRailFooter.TabIndex = 9;
            this.lblRailFooter.Text = "Session\r\nSelf-service";
            this.lblRailFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRailFooter.Name = "lblRailFooter";
            //
            // pnlStatus
            //
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(31)))), ((int)(((byte)(58)))));
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatus.Location = new System.Drawing.Point(0, 655);
            this.pnlStatus.Size = new System.Drawing.Size(984, 26);
            this.pnlStatus.TabIndex = 4;
            this.pnlStatus.Name = "pnlStatus";
            //
            // lblStatus
            //
            this.lblStatus.AccessibleName = "Status";
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(0, 0);
            this.lblStatus.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lblStatus.Size = new System.Drawing.Size(984, 26);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatus.Name = "lblStatus";
            //
            // FundingOptionsForm
            //
            this.AcceptButton = this.btnAddToSummary;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnBack;
            this.ClientSize = new System.Drawing.Size(984, 681);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Future Path \u2014 Funding options";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlTitle);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlRail);
            this.Controls.Add(this.pnlStatus);
            this.Name = "FundingOptionsForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FundingOptionsForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FundingOptionsForm_FormClosed);
            this.VisibleChanged += new System.EventHandler(this.FundingOptionsForm_VisibleChanged);
            this.pnlBody.ResumeLayout(false);
            this.pnlBody.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.pnlFilterCard.ResumeLayout(false);
            this.pnlFilterCard.PerformLayout();
            this.pnlFundingCard.ResumeLayout(false);
            this.pnlFundingCard.PerformLayout();
            this.lblFundingHeadingBar.ResumeLayout(false);
            this.lblFundingHeadingBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFunding)).EndInit();
            this.pnlDetailCard.ResumeLayout(false);
            this.pnlDetailCard.PerformLayout();
            this.lblDetailHeadingBar.ResumeLayout(false);
            this.lblDetailHeadingBar.PerformLayout();
            this.pnlSampleNote.ResumeLayout(false);
            this.pnlSampleNote.PerformLayout();
            this.pnlSampleNoteBar.ResumeLayout(false);
            this.pnlSampleNoteBar.PerformLayout();
            this.pnlContextGap.ResumeLayout(false);
            this.pnlContextGap.PerformLayout();
            this.pnlContext.ResumeLayout(false);
            this.pnlContext.PerformLayout();
            this.pnlContextBar.ResumeLayout(false);
            this.pnlContextBar.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlTitleRule.ResumeLayout(false);
            this.pnlTitleRule.PerformLayout();
            this.pnlActions.ResumeLayout(false);
            this.pnlActions.PerformLayout();
            this.pnlActionRule.ResumeLayout(false);
            this.pnlActionRule.PerformLayout();
            this.pnlRail.ResumeLayout(false);
            this.pnlRail.PerformLayout();
            this.pnlBrandRule.ResumeLayout(false);
            this.pnlBrandRule.PerformLayout();
            this.pnlStatus.ResumeLayout(false);
            this.pnlStatus.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlBody;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlFilterCard;
        private System.Windows.Forms.Label lblFaculty;
        private System.Windows.Forms.ComboBox cboFaculty;
        private System.Windows.Forms.Label lblFundingType;
        private System.Windows.Forms.ComboBox cboFundingType;
        private System.Windows.Forms.CheckBox chkOpenOnly;
        private System.Windows.Forms.Panel pnlFundingCard;
        private System.Windows.Forms.Panel lblFundingHeadingBar;
        private System.Windows.Forms.Label lblFundingHeading;
        private System.Windows.Forms.Label lblFundingCount;
        private System.Windows.Forms.DataGridView dgvFunding;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFundingName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFundingType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLinkedTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClosingDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFundingId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEligibility;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDocuments;
        private System.Windows.Forms.DataGridViewTextBoxColumn colContact;
        private System.Windows.Forms.Panel pnlDetailCard;
        private System.Windows.Forms.Panel lblDetailHeadingBar;
        private System.Windows.Forms.Label lblDetailHeading;
        private System.Windows.Forms.Label lblEligibility;
        private System.Windows.Forms.TextBox txtEligibility;
        private System.Windows.Forms.Label lblDocuments;
        private System.Windows.Forms.TextBox txtDocuments;
        private System.Windows.Forms.Label lblContact;
        private System.Windows.Forms.TextBox txtContact;
        private System.Windows.Forms.Panel pnlSampleNote;
        private System.Windows.Forms.Label lblSampleNote;
        private System.Windows.Forms.Panel pnlSampleNoteBar;
        private System.Windows.Forms.Panel pnlContextGap;
        private System.Windows.Forms.Panel pnlContext;
        private System.Windows.Forms.Label lblContext;
        private System.Windows.Forms.Panel pnlContextBar;
        private System.Windows.Forms.Panel pnlTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblBadge;
        private System.Windows.Forms.Panel pnlTitleRule;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Panel pnlActionRule;
        private System.Windows.Forms.Button btnAddToSummary;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label lblActionHint;
        private System.Windows.Forms.Panel pnlRail;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.Panel pnlBrandRule;
        private System.Windows.Forms.Button btnNavHome;
        private System.Windows.Forms.Label lblStep1;
        private System.Windows.Forms.Label lblStep2;
        private System.Windows.Forms.Label lblStep3;
        private System.Windows.Forms.Label lblStep4;
        private System.Windows.Forms.Label lblStep5;
        private System.Windows.Forms.Label lblStep6;
        private System.Windows.Forms.Label lblRailFooter;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblStatus;
    }
}
