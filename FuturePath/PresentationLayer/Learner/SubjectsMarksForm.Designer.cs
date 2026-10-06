namespace FuturePath.PresentationLayer
{
    partial class SubjectsMarksForm
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
            System.Windows.Forms.DataGridViewCellStyle dgvSubjectsAltStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvSubjectsHeaderStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvSubjectsCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colMarkStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colRemoveStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlBody = new System.Windows.Forms.Panel();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlAddCard = new System.Windows.Forms.Panel();
            this.lblAddHeadingBar = new System.Windows.Forms.Panel();
            this.lblAddHeading = new System.Windows.Forms.Label();
            this.lblSubject = new System.Windows.Forms.Label();
            this.lblSubjectStar = new System.Windows.Forms.Label();
            this.cboSubject = new System.Windows.Forms.ComboBox();
            this.lblSubjectError = new System.Windows.Forms.Label();
            this.lblSubjectLevel = new System.Windows.Forms.Label();
            this.txtSubjectLevel = new System.Windows.Forms.TextBox();
            this.lblMark = new System.Windows.Forms.Label();
            this.lblMarkStar = new System.Windows.Forms.Label();
            this.numMark = new System.Windows.Forms.NumericUpDown();
            this.lblMarkError = new System.Windows.Forms.Label();
            this.btnAddSubject = new System.Windows.Forms.Button();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.pnlCapturedCard = new System.Windows.Forms.Panel();
            this.lblCapturedHeadingBar = new System.Windows.Forms.Panel();
            this.lblCapturedHeading = new System.Windows.Forms.Label();
            this.lblCapturedCount = new System.Windows.Forms.Label();
            this.dgvSubjects = new System.Windows.Forms.DataGridView();
            this.colSubjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubjectLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAchievementLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRemove = new System.Windows.Forms.DataGridViewButtonColumn();
            this.lblFooter = new System.Windows.Forms.Label();
            this.pnlNoticeGap = new System.Windows.Forms.Panel();
            this.pnlNotice = new System.Windows.Forms.Panel();
            this.lblNotice = new System.Windows.Forms.Label();
            this.pnlNoticeBar = new System.Windows.Forms.Panel();
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
            this.btnSaveContinue = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
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
            this.pnlAddCard.SuspendLayout();
            this.lblAddHeadingBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMark)).BeginInit();
            this.pnlSummary.SuspendLayout();
            this.pnlCapturedCard.SuspendLayout();
            this.lblCapturedHeadingBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubjects)).BeginInit();
            this.pnlNoticeGap.SuspendLayout();
            this.pnlNotice.SuspendLayout();
            this.pnlNoticeBar.SuspendLayout();
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
            this.pnlBody.Controls.Add(this.pnlNoticeGap);
            this.pnlBody.Controls.Add(this.pnlNotice);
            this.pnlBody.Controls.Add(this.pnlContextGap);
            this.pnlBody.Controls.Add(this.pnlContext);
            this.pnlBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Location = new System.Drawing.Point(210, 84);
            this.pnlBody.Padding = new System.Windows.Forms.Padding(20, 12, 20, 12);
            this.pnlBody.Size = new System.Drawing.Size(974, 587);
            this.pnlBody.TabIndex = 0;
            this.pnlBody.Name = "pnlBody";
            //
            // pnlContent
            //
            this.pnlContent.Controls.Add(this.pnlAddCard);
            this.pnlContent.Controls.Add(this.pnlSummary);
            this.pnlContent.Controls.Add(this.pnlCapturedCard);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(20, 12);
            this.pnlContent.Size = new System.Drawing.Size(934, 563);
            this.pnlContent.TabIndex = 0;
            this.pnlContent.Name = "pnlContent";
            //
            // pnlAddCard
            //
            this.pnlAddCard.Controls.Add(this.lblAddHeadingBar);
            this.pnlAddCard.Controls.Add(this.lblAddHeading);
            this.pnlAddCard.Controls.Add(this.lblSubject);
            this.pnlAddCard.Controls.Add(this.lblSubjectStar);
            this.pnlAddCard.Controls.Add(this.cboSubject);
            this.pnlAddCard.Controls.Add(this.lblSubjectError);
            this.pnlAddCard.Controls.Add(this.lblSubjectLevel);
            this.pnlAddCard.Controls.Add(this.txtSubjectLevel);
            this.pnlAddCard.Controls.Add(this.lblMark);
            this.pnlAddCard.Controls.Add(this.lblMarkStar);
            this.pnlAddCard.Controls.Add(this.numMark);
            this.pnlAddCard.Controls.Add(this.lblMarkError);
            this.pnlAddCard.Controls.Add(this.btnAddSubject);
            this.pnlAddCard.BackColor = System.Drawing.Color.White;
            this.pnlAddCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAddCard.Location = new System.Drawing.Point(0, 0);
            this.pnlAddCard.Size = new System.Drawing.Size(934, 116);
            this.pnlAddCard.TabIndex = 0;
            this.pnlAddCard.Name = "pnlAddCard";
            this.pnlAddCard.Enter += new System.EventHandler(this.pnlAddCard_Enter);
            this.pnlAddCard.Leave += new System.EventHandler(this.pnlAddCard_Leave);
            //
            // lblAddHeadingBar
            //
            this.lblAddHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblAddHeadingBar.Location = new System.Drawing.Point(16, 12);
            this.lblAddHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblAddHeadingBar.TabIndex = 0;
            this.lblAddHeadingBar.Name = "lblAddHeadingBar";
            //
            // lblAddHeading
            //
            this.lblAddHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblAddHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblAddHeading.Location = new System.Drawing.Point(26, 8);
            this.lblAddHeading.Size = new System.Drawing.Size(500, 28);
            this.lblAddHeading.TabIndex = 1;
            this.lblAddHeading.Text = "Add a subject";
            this.lblAddHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAddHeading.Name = "lblAddHeading";
            //
            // lblSubject
            //
            this.lblSubject.AutoSize = true;
            this.lblSubject.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSubject.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblSubject.Location = new System.Drawing.Point(20, 40);
            this.lblSubject.TabIndex = 2;
            this.lblSubject.Text = "Subject";
            this.lblSubject.Name = "lblSubject";
            //
            // lblSubjectStar
            //
            this.lblSubjectStar.AutoSize = true;
            this.lblSubjectStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSubjectStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblSubjectStar.Location = new System.Drawing.Point(83, 40);
            this.lblSubjectStar.Size = new System.Drawing.Size(14, 24);
            this.lblSubjectStar.TabIndex = 3;
            this.lblSubjectStar.Text = "*";
            this.lblSubjectStar.Name = "lblSubjectStar";
            //
            // cboSubject
            //
            this.cboSubject.AccessibleName = "Subject (required)";
            this.cboSubject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSubject.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboSubject.FormattingEnabled = true;
            this.cboSubject.Location = new System.Drawing.Point(20, 66);
            this.cboSubject.Size = new System.Drawing.Size(340, 25);
            this.cboSubject.TabIndex = 4;
            this.cboSubject.Name = "cboSubject";
            this.cboSubject.SelectedIndexChanged += new System.EventHandler(this.cboSubject_SelectedIndexChanged);
            this.cboSubject.Leave += new System.EventHandler(this.cboSubject_Leave);
            //
            // lblSubjectError
            //
            this.lblSubjectError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSubjectError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblSubjectError.Location = new System.Drawing.Point(20, 93);
            this.lblSubjectError.Size = new System.Drawing.Size(590, 16);
            this.lblSubjectError.TabIndex = 5;
            this.lblSubjectError.Text = "";
            this.lblSubjectError.Visible = false;
            this.lblSubjectError.Name = "lblSubjectError";
            //
            // lblSubjectLevel
            //
            this.lblSubjectLevel.AutoSize = true;
            this.lblSubjectLevel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSubjectLevel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblSubjectLevel.Location = new System.Drawing.Point(380, 40);
            this.lblSubjectLevel.TabIndex = 6;
            this.lblSubjectLevel.Text = "Subject level";
            this.lblSubjectLevel.Name = "lblSubjectLevel";
            //
            // txtSubjectLevel
            //
            this.txtSubjectLevel.AccessibleName = "Subject level (read-only, follows from the subject)";
            this.txtSubjectLevel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.txtSubjectLevel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSubjectLevel.Location = new System.Drawing.Point(380, 66);
            this.txtSubjectLevel.ReadOnly = true;
            this.txtSubjectLevel.Size = new System.Drawing.Size(220, 25);
            this.txtSubjectLevel.TabIndex = 7;
            this.txtSubjectLevel.TabStop = false;
            this.txtSubjectLevel.Name = "txtSubjectLevel";
            //
            // lblMark
            //
            this.lblMark.AutoSize = true;
            this.lblMark.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblMark.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblMark.Location = new System.Drawing.Point(620, 40);
            this.lblMark.TabIndex = 8;
            this.lblMark.Text = "Mark % (0\u2013100)";
            this.lblMark.Name = "lblMark";
            //
            // lblMarkStar
            //
            this.lblMarkStar.AutoSize = true;
            this.lblMarkStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblMarkStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblMarkStar.Location = new System.Drawing.Point(746, 40);
            this.lblMarkStar.Size = new System.Drawing.Size(14, 24);
            this.lblMarkStar.TabIndex = 9;
            this.lblMarkStar.Text = "*";
            this.lblMarkStar.Name = "lblMarkStar";
            //
            // numMark
            //
            this.numMark.AccessibleName = "Mark percent from 0 to 100, one decimal (required)";
            this.numMark.DecimalPlaces = 1;
            this.numMark.Font = new System.Drawing.Font("Consolas", 10F);
            this.numMark.Location = new System.Drawing.Point(620, 66);
            this.numMark.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numMark.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numMark.Size = new System.Drawing.Size(140, 23);
            this.numMark.TabIndex = 10;
            this.numMark.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numMark.Value = new decimal(new int[] { 0, 0, 0, 0 });
            this.numMark.Name = "numMark";
            this.numMark.TextChanged += new System.EventHandler(this.numMark_TextChanged);
            this.numMark.Leave += new System.EventHandler(this.numMark_Leave);
            //
            // lblMarkError
            //
            this.lblMarkError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblMarkError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblMarkError.Location = new System.Drawing.Point(620, 93);
            this.lblMarkError.Size = new System.Drawing.Size(300, 16);
            this.lblMarkError.TabIndex = 11;
            this.lblMarkError.Text = "";
            this.lblMarkError.Visible = false;
            this.lblMarkError.Name = "lblMarkError";
            //
            // btnAddSubject
            //
            this.btnAddSubject.AccessibleName = "Add subject";
            this.btnAddSubject.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.btnAddSubject.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddSubject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddSubject.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnAddSubject.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnAddSubject.Location = new System.Drawing.Point(780, 62);
            this.btnAddSubject.Size = new System.Drawing.Size(134, 32);
            this.btnAddSubject.TabIndex = 12;
            this.btnAddSubject.Text = "Add subject";
            this.btnAddSubject.UseVisualStyleBackColor = false;
            this.btnAddSubject.Name = "btnAddSubject";
            this.btnAddSubject.FlatAppearance.BorderSize = 0;
            this.btnAddSubject.Click += new System.EventHandler(this.btnAddSubject_Click);
            //
            // pnlSummary
            //
            this.pnlSummary.Controls.Add(this.lblSummary);
            this.pnlSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.pnlSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSummary.Location = new System.Drawing.Point(0, 126);
            this.pnlSummary.Size = new System.Drawing.Size(934, 70);
            this.pnlSummary.TabIndex = 1;
            this.pnlSummary.Visible = false;
            this.pnlSummary.Name = "pnlSummary";
            //
            // lblSummary
            //
            this.lblSummary.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblSummary.Location = new System.Drawing.Point(12, 6);
            this.lblSummary.Size = new System.Drawing.Size(908, 58);
            this.lblSummary.TabIndex = 0;
            this.lblSummary.Text = "";
            this.lblSummary.Name = "lblSummary";
            //
            // pnlCapturedCard
            //
            this.pnlCapturedCard.Controls.Add(this.lblCapturedHeadingBar);
            this.pnlCapturedCard.Controls.Add(this.lblCapturedHeading);
            this.pnlCapturedCard.Controls.Add(this.lblCapturedCount);
            this.pnlCapturedCard.Controls.Add(this.dgvSubjects);
            this.pnlCapturedCard.Controls.Add(this.lblFooter);
            this.pnlCapturedCard.BackColor = System.Drawing.Color.White;
            this.pnlCapturedCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCapturedCard.Location = new System.Drawing.Point(0, 126);
            this.pnlCapturedCard.Size = new System.Drawing.Size(934, 333);
            this.pnlCapturedCard.TabIndex = 2;
            this.pnlCapturedCard.Name = "pnlCapturedCard";
            //
            // lblCapturedHeadingBar
            //
            this.lblCapturedHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblCapturedHeadingBar.Location = new System.Drawing.Point(16, 12);
            this.lblCapturedHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblCapturedHeadingBar.TabIndex = 0;
            this.lblCapturedHeadingBar.Name = "lblCapturedHeadingBar";
            //
            // lblCapturedHeading
            //
            this.lblCapturedHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblCapturedHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblCapturedHeading.Location = new System.Drawing.Point(26, 8);
            this.lblCapturedHeading.Size = new System.Drawing.Size(500, 28);
            this.lblCapturedHeading.TabIndex = 1;
            this.lblCapturedHeading.Text = "Captured subjects";
            this.lblCapturedHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCapturedHeading.Name = "lblCapturedHeading";
            //
            // lblCapturedCount
            //
            this.lblCapturedCount.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblCapturedCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblCapturedCount.Location = new System.Drawing.Point(616, 12);
            this.lblCapturedCount.Size = new System.Drawing.Size(300, 20);
            this.lblCapturedCount.TabIndex = 2;
            this.lblCapturedCount.Text = "";
            this.lblCapturedCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblCapturedCount.Name = "lblCapturedCount";
            //
            // dgvSubjects
            //
            this.dgvSubjects.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSubjectName,
            this.colSubjectLevel,
            this.colMark,
            this.colAchievementLevel,
            this.colRemove});
            dgvSubjectsAltStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.dgvSubjects.AlternatingRowsDefaultCellStyle = dgvSubjectsAltStyle;
            dgvSubjectsHeaderStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dgvSubjectsHeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            dgvSubjectsHeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            dgvSubjectsHeaderStyle.ForeColor = System.Drawing.Color.White;
            dgvSubjectsHeaderStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            dgvSubjectsHeaderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            dgvSubjectsHeaderStyle.SelectionForeColor = System.Drawing.Color.White;
            dgvSubjectsHeaderStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSubjects.ColumnHeadersDefaultCellStyle = dgvSubjectsHeaderStyle;
            dgvSubjectsCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dgvSubjectsCellStyle.BackColor = System.Drawing.Color.White;
            dgvSubjectsCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            dgvSubjectsCellStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            dgvSubjectsCellStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            dgvSubjectsCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            dgvSubjectsCellStyle.SelectionForeColor = System.Drawing.Color.White;
            dgvSubjectsCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSubjects.DefaultCellStyle = dgvSubjectsCellStyle;
            this.dgvSubjects.AccessibleName = "Captured subjects";
            this.dgvSubjects.AllowUserToAddRows = false;
            this.dgvSubjects.AllowUserToDeleteRows = false;
            this.dgvSubjects.AllowUserToResizeRows = false;
            this.dgvSubjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSubjects.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSubjects.BackgroundColor = System.Drawing.Color.White;
            this.dgvSubjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgvSubjects.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvSubjects.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvSubjects.ColumnHeadersHeight = 34;
            this.dgvSubjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvSubjects.EnableHeadersVisualStyles = false;
            this.dgvSubjects.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            this.dgvSubjects.Location = new System.Drawing.Point(16, 40);
            this.dgvSubjects.MultiSelect = false;
            this.dgvSubjects.ReadOnly = true;
            this.dgvSubjects.RowHeadersVisible = false;
            this.dgvSubjects.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSubjects.Size = new System.Drawing.Size(900, 257);
            this.dgvSubjects.TabIndex = 3;
            this.dgvSubjects.Name = "dgvSubjects";
            this.dgvSubjects.RowTemplate.Height = 30;
            this.colMark.HeaderCell.Style.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.dgvSubjects.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSubjects_CellContentClick);
            this.dgvSubjects.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvSubjects_DataBindingComplete);
            //
            // colSubjectName
            //
            this.colSubjectName.DataPropertyName = "SubjectName";
            this.colSubjectName.FillWeight = 34F;
            this.colSubjectName.HeaderText = "Subject";
            this.colSubjectName.ReadOnly = true;
            this.colSubjectName.Name = "colSubjectName";
            //
            // colSubjectLevel
            //
            this.colSubjectLevel.DataPropertyName = "SubjectLevel";
            this.colSubjectLevel.FillWeight = 22F;
            this.colSubjectLevel.HeaderText = "Level";
            this.colSubjectLevel.ReadOnly = true;
            this.colSubjectLevel.Name = "colSubjectLevel";
            //
            // colMark
            //
            colMarkStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            colMarkStyle.Font = new System.Drawing.Font("Consolas", 10F);
            colMarkStyle.Format = "0.0";
            this.colMark.DefaultCellStyle = colMarkStyle;
            this.colMark.DataPropertyName = "Mark";
            this.colMark.FillWeight = 12F;
            this.colMark.HeaderText = "Mark %";
            this.colMark.ReadOnly = true;
            this.colMark.Name = "colMark";
            //
            // colAchievementLevel
            //
            this.colAchievementLevel.DataPropertyName = "AchievementLevel";
            this.colAchievementLevel.FillWeight = 20F;
            this.colAchievementLevel.HeaderText = "Achievement level";
            this.colAchievementLevel.ReadOnly = true;
            this.colAchievementLevel.Name = "colAchievementLevel";
            //
            // colRemove
            //
            colRemoveStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            colRemoveStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(243)))), ((int)(((byte)(224)))));
            colRemoveStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(243)))), ((int)(((byte)(224)))));
            colRemoveStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            colRemoveStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            colRemoveStyle.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.colRemove.DefaultCellStyle = colRemoveStyle;
            this.colRemove.FillWeight = 12F;
            this.colRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colRemove.HeaderText = "Remove";
            this.colRemove.Text = "Remove";
            this.colRemove.ToolTipText = "Remove this subject from the captured list";
            this.colRemove.UseColumnTextForButtonValue = true;
            this.colRemove.Name = "colRemove";
            //
            // lblFooter
            //
            this.lblFooter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFooter.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblFooter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFooter.Location = new System.Drawing.Point(16, 301);
            this.lblFooter.Size = new System.Drawing.Size(900, 26);
            this.lblFooter.TabIndex = 4;
            this.lblFooter.Text = "0 subjects captured";
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFooter.Name = "lblFooter";
            //
            // pnlNoticeGap
            //
            this.pnlNoticeGap.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlNoticeGap.Location = new System.Drawing.Point(20, 0);
            this.pnlNoticeGap.Size = new System.Drawing.Size(934, 10);
            this.pnlNoticeGap.TabIndex = 1;
            this.pnlNoticeGap.Name = "pnlNoticeGap";
            //
            // pnlNotice
            //
            this.pnlNotice.Controls.Add(this.lblNotice);
            this.pnlNotice.Controls.Add(this.pnlNoticeBar);
            this.pnlNotice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.pnlNotice.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlNotice.Location = new System.Drawing.Point(20, 0);
            this.pnlNotice.Size = new System.Drawing.Size(934, 48);
            this.pnlNotice.TabIndex = 2;
            this.pnlNotice.Name = "pnlNotice";
            //
            // lblNotice
            //
            this.lblNotice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNotice.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNotice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblNotice.Location = new System.Drawing.Point(5, 0);
            this.lblNotice.Padding = new System.Windows.Forms.Padding(14, 0, 10, 0);
            this.lblNotice.Size = new System.Drawing.Size(929, 48);
            this.lblNotice.TabIndex = 0;
            this.lblNotice.Text = "";
            this.lblNotice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNotice.Name = "lblNotice";
            //
            // pnlNoticeBar
            //
            this.pnlNoticeBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.pnlNoticeBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlNoticeBar.Location = new System.Drawing.Point(0, 0);
            this.pnlNoticeBar.Size = new System.Drawing.Size(5, 48);
            this.pnlNoticeBar.TabIndex = 1;
            this.pnlNoticeBar.Name = "pnlNoticeBar";
            //
            // pnlContextGap
            //
            this.pnlContextGap.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlContextGap.Location = new System.Drawing.Point(20, 0);
            this.pnlContextGap.Size = new System.Drawing.Size(934, 10);
            this.pnlContextGap.TabIndex = 3;
            this.pnlContextGap.Name = "pnlContextGap";
            //
            // pnlContext
            //
            this.pnlContext.Controls.Add(this.lblContext);
            this.pnlContext.Controls.Add(this.pnlContextBar);
            this.pnlContext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.pnlContext.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlContext.Location = new System.Drawing.Point(20, 12);
            this.pnlContext.Size = new System.Drawing.Size(934, 36);
            this.pnlContext.TabIndex = 4;
            this.pnlContext.Name = "pnlContext";
            //
            // lblContext
            //
            this.lblContext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblContext.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblContext.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblContext.Location = new System.Drawing.Point(5, 0);
            this.lblContext.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblContext.Size = new System.Drawing.Size(929, 36);
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
            this.pnlTitle.Size = new System.Drawing.Size(974, 84);
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
            this.lblTitle.Text = "Subjects and marks";
            this.lblTitle.Name = "lblTitle";
            //
            // lblSubtitle
            //
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.lblSubtitle.Location = new System.Drawing.Point(24, 54);
            this.lblSubtitle.Size = new System.Drawing.Size(914, 18);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Step 2 of 6 \u00B7 Enter every subject on your latest report, then continue";
            this.lblSubtitle.Name = "lblSubtitle";
            //
            // lblBadge
            //
            this.lblBadge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBadge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblBadge.Font = new System.Drawing.Font("Segoe UI Semibold", 8F);
            this.lblBadge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblBadge.Location = new System.Drawing.Point(804, 16);
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
            this.pnlTitleRule.Size = new System.Drawing.Size(974, 3);
            this.pnlTitleRule.TabIndex = 3;
            this.pnlTitleRule.Name = "pnlTitleRule";
            //
            // pnlActions
            //
            this.pnlActions.Controls.Add(this.pnlActionRule);
            this.pnlActions.Controls.Add(this.btnSaveContinue);
            this.pnlActions.Controls.Add(this.btnBack);
            this.pnlActions.Controls.Add(this.btnCancel);
            this.pnlActions.Controls.Add(this.lblActionHint);
            this.pnlActions.BackColor = System.Drawing.Color.White;
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlActions.Location = new System.Drawing.Point(210, 671);
            this.pnlActions.Size = new System.Drawing.Size(974, 64);
            this.pnlActions.TabIndex = 2;
            this.pnlActions.Name = "pnlActions";
            //
            // pnlActionRule
            //
            this.pnlActionRule.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            this.pnlActionRule.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlActionRule.Location = new System.Drawing.Point(0, 0);
            this.pnlActionRule.Size = new System.Drawing.Size(974, 1);
            this.pnlActionRule.TabIndex = 0;
            this.pnlActionRule.Name = "pnlActionRule";
            //
            // btnSaveContinue
            //
            this.btnSaveContinue.AccessibleName = "Save and continue";
            this.btnSaveContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveContinue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.btnSaveContinue.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveContinue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveContinue.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnSaveContinue.ForeColor = System.Drawing.Color.White;
            this.btnSaveContinue.Location = new System.Drawing.Point(774, 12);
            this.btnSaveContinue.Size = new System.Drawing.Size(180, 40);
            this.btnSaveContinue.TabIndex = 1;
            this.btnSaveContinue.Text = "Save and continue";
            this.btnSaveContinue.UseVisualStyleBackColor = false;
            this.btnSaveContinue.Name = "btnSaveContinue";
            this.btnSaveContinue.FlatAppearance.BorderSize = 0;
            this.btnSaveContinue.Click += new System.EventHandler(this.btnSaveContinue_Click);
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
            this.btnBack.Location = new System.Drawing.Point(634, 12);
            this.btnBack.Size = new System.Drawing.Size(130, 40);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Name = "btnBack";
            this.btnBack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            //
            // btnCancel
            //
            this.btnCancel.AccessibleName = "Cancel";
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnCancel.Location = new System.Drawing.Point(494, 12);
            this.btnCancel.Size = new System.Drawing.Size(130, 40);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // lblActionHint
            //
            this.lblActionHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblActionHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblActionHint.Location = new System.Drawing.Point(20, 12);
            this.lblActionHint.Size = new System.Drawing.Size(454, 40);
            this.lblActionHint.TabIndex = 4;
            this.lblActionHint.Text = "Step 2 of 6 \u00B7 Cancel and Back leave without saving your subjects.";
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
            this.pnlRail.Size = new System.Drawing.Size(210, 735);
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
            this.lblStep2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(64)))), ((int)(((byte)(104)))));
            this.lblStep2.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblStep2.ForeColor = System.Drawing.Color.White;
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
            this.lblStep3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(130)))), ((int)(((byte)(158)))));
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
            this.lblStep4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(130)))), ((int)(((byte)(158)))));
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
            this.lblStep5.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStep5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(130)))), ((int)(((byte)(158)))));
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
            this.lblRailFooter.Location = new System.Drawing.Point(0, 655);
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
            this.pnlStatus.Location = new System.Drawing.Point(0, 735);
            this.pnlStatus.Size = new System.Drawing.Size(1184, 26);
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
            this.lblStatus.Size = new System.Drawing.Size(1184, 26);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatus.Name = "lblStatus";
            //
            // SubjectsMarksForm
            //
            this.AcceptButton = this.btnSaveContinue;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Future Path \u2014 Subjects and marks";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlTitle);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlRail);
            this.Controls.Add(this.pnlStatus);
            this.Name = "SubjectsMarksForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SubjectsMarksForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SubjectsMarksForm_FormClosed);
            this.VisibleChanged += new System.EventHandler(this.SubjectsMarksForm_VisibleChanged);
            this.pnlBody.ResumeLayout(false);
            this.pnlBody.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.pnlAddCard.ResumeLayout(false);
            this.pnlAddCard.PerformLayout();
            this.lblAddHeadingBar.ResumeLayout(false);
            this.lblAddHeadingBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMark)).EndInit();
            this.pnlSummary.ResumeLayout(false);
            this.pnlSummary.PerformLayout();
            this.pnlCapturedCard.ResumeLayout(false);
            this.pnlCapturedCard.PerformLayout();
            this.lblCapturedHeadingBar.ResumeLayout(false);
            this.lblCapturedHeadingBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubjects)).EndInit();
            this.pnlNoticeGap.ResumeLayout(false);
            this.pnlNoticeGap.PerformLayout();
            this.pnlNotice.ResumeLayout(false);
            this.pnlNotice.PerformLayout();
            this.pnlNoticeBar.ResumeLayout(false);
            this.pnlNoticeBar.PerformLayout();
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
        private System.Windows.Forms.Panel pnlAddCard;
        private System.Windows.Forms.Panel lblAddHeadingBar;
        private System.Windows.Forms.Label lblAddHeading;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.Label lblSubjectStar;
        private System.Windows.Forms.ComboBox cboSubject;
        private System.Windows.Forms.Label lblSubjectError;
        private System.Windows.Forms.Label lblSubjectLevel;
        private System.Windows.Forms.TextBox txtSubjectLevel;
        private System.Windows.Forms.Label lblMark;
        private System.Windows.Forms.Label lblMarkStar;
        private System.Windows.Forms.NumericUpDown numMark;
        private System.Windows.Forms.Label lblMarkError;
        private System.Windows.Forms.Button btnAddSubject;
        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Panel pnlCapturedCard;
        private System.Windows.Forms.Panel lblCapturedHeadingBar;
        private System.Windows.Forms.Label lblCapturedHeading;
        private System.Windows.Forms.Label lblCapturedCount;
        private System.Windows.Forms.DataGridView dgvSubjects;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSubjectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSubjectLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMark;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAchievementLevel;
        private System.Windows.Forms.DataGridViewButtonColumn colRemove;
        private System.Windows.Forms.Label lblFooter;
        private System.Windows.Forms.Panel pnlNoticeGap;
        private System.Windows.Forms.Panel pnlNotice;
        private System.Windows.Forms.Label lblNotice;
        private System.Windows.Forms.Panel pnlNoticeBar;
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
        private System.Windows.Forms.Button btnSaveContinue;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnCancel;
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
