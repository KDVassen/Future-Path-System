namespace FuturePath.PresentationLayer
{
    partial class DegreeRequirementsTab
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
            System.Windows.Forms.DataGridViewCellStyle dgvRequirementsAltStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvRequirementsHeaderStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvRequirementsCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colMinimumMarkStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colStatusStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblFaculty = new System.Windows.Forms.Label();
            this.cboFaculty = new System.Windows.Forms.ComboBox();
            this.lblProgramme = new System.Windows.Forms.Label();
            this.lstProgrammes = new System.Windows.Forms.ListBox();
            this.lblProgrammeCount = new System.Windows.Forms.Label();
            this.pnlGridCard = new System.Windows.Forms.Panel();
            this.lblGridHeadingBar = new System.Windows.Forms.Panel();
            this.lblGridHeading = new System.Windows.Forms.Label();
            this.lblRequirementCount = new System.Windows.Forms.Label();
            this.dgvRequirements = new System.Windows.Forms.DataGridView();
            this.colRequirementType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMinimumMark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlternative = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRequirementId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubjectId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlternativeSubjectId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAdvisoryNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlFormCard = new System.Windows.Forms.Panel();
            this.lblFormHeadingBar = new System.Windows.Forms.Panel();
            this.lblFormHeading = new System.Windows.Forms.Label();
            this.lblRequirementType = new System.Windows.Forms.Label();
            this.lblRequirementTypeStar = new System.Windows.Forms.Label();
            this.cboRequirementType = new System.Windows.Forms.ComboBox();
            this.lblRequirementTypeError = new System.Windows.Forms.Label();
            this.lblSubject = new System.Windows.Forms.Label();
            this.lblSubjectStar = new System.Windows.Forms.Label();
            this.cboSubject = new System.Windows.Forms.ComboBox();
            this.lblSubjectError = new System.Windows.Forms.Label();
            this.lblAlternative = new System.Windows.Forms.Label();
            this.lblAlternativeStar = new System.Windows.Forms.Label();
            this.cboAlternative = new System.Windows.Forms.ComboBox();
            this.lblAlternativeError = new System.Windows.Forms.Label();
            this.lblMinimumMark = new System.Windows.Forms.Label();
            this.lblMinimumMarkStar = new System.Windows.Forms.Label();
            this.numMinimumMark = new System.Windows.Forms.NumericUpDown();
            this.lblMinimumMarkError = new System.Windows.Forms.Label();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.lblAdvisoryNote = new System.Windows.Forms.Label();
            this.txtAdvisoryNote = new System.Windows.Forms.TextBox();
            this.lblAdvisoryNoteHint = new System.Windows.Forms.Label();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.pnlGridCard.SuspendLayout();
            this.lblGridHeadingBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequirements)).BeginInit();
            this.pnlFormCard.SuspendLayout();
            this.lblFormHeadingBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinimumMark)).BeginInit();
            this.pnlSummary.SuspendLayout();
            this.SuspendLayout();
            //
            // lblFaculty
            //
            this.lblFaculty.AutoSize = true;
            this.lblFaculty.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFaculty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFaculty.Location = new System.Drawing.Point(0, 0);
            this.lblFaculty.TabIndex = 0;
            this.lblFaculty.Text = "Faculty";
            this.lblFaculty.Name = "lblFaculty";
            //
            // cboFaculty
            //
            this.cboFaculty.AccessibleName = "Faculty (scope)";
            this.cboFaculty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFaculty.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboFaculty.FormattingEnabled = true;
            this.cboFaculty.Location = new System.Drawing.Point(0, 26);
            this.cboFaculty.Size = new System.Drawing.Size(230, 25);
            this.cboFaculty.TabIndex = 1;
            this.cboFaculty.Name = "cboFaculty";
            this.cboFaculty.SelectedIndexChanged += new System.EventHandler(this.cboFaculty_SelectedIndexChanged);
            //
            // lblProgramme
            //
            this.lblProgramme.AutoSize = true;
            this.lblProgramme.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblProgramme.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblProgramme.Location = new System.Drawing.Point(0, 62);
            this.lblProgramme.TabIndex = 2;
            this.lblProgramme.Text = "Degree programme";
            this.lblProgramme.Name = "lblProgramme";
            //
            // lstProgrammes
            //
            this.lstProgrammes.AccessibleName = "Degree programme (scope)";
            this.lstProgrammes.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lstProgrammes.FormattingEnabled = true;
            this.lstProgrammes.IntegralHeight = false;
            this.lstProgrammes.ItemHeight = 17;
            this.lstProgrammes.Location = new System.Drawing.Point(0, 88);
            this.lstProgrammes.Size = new System.Drawing.Size(230, 350);
            this.lstProgrammes.TabIndex = 3;
            this.lstProgrammes.Name = "lstProgrammes";
            this.lstProgrammes.SelectedIndexChanged += new System.EventHandler(this.lstProgrammes_SelectedIndexChanged);
            this.lstProgrammes.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.lstProgrammes_Format);
            //
            // lblProgrammeCount
            //
            this.lblProgrammeCount.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblProgrammeCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblProgrammeCount.Location = new System.Drawing.Point(0, 444);
            this.lblProgrammeCount.Size = new System.Drawing.Size(230, 64);
            this.lblProgrammeCount.TabIndex = 4;
            this.lblProgrammeCount.Text = "Choose a faculty to list its programmes.";
            this.lblProgrammeCount.Name = "lblProgrammeCount";
            //
            // pnlGridCard
            //
            this.pnlGridCard.Controls.Add(this.lblGridHeadingBar);
            this.pnlGridCard.Controls.Add(this.lblGridHeading);
            this.pnlGridCard.Controls.Add(this.lblRequirementCount);
            this.pnlGridCard.Controls.Add(this.dgvRequirements);
            this.pnlGridCard.BackColor = System.Drawing.Color.White;
            this.pnlGridCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGridCard.Location = new System.Drawing.Point(250, 0);
            this.pnlGridCard.Size = new System.Drawing.Size(676, 262);
            this.pnlGridCard.TabIndex = 5;
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
            this.lblGridHeading.Size = new System.Drawing.Size(440, 28);
            this.lblGridHeading.TabIndex = 1;
            this.lblGridHeading.Text = "Stored requirements \u2014 no programme selected";
            this.lblGridHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblGridHeading.Name = "lblGridHeading";
            //
            // lblRequirementCount
            //
            this.lblRequirementCount.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblRequirementCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblRequirementCount.Location = new System.Drawing.Point(460, 10);
            this.lblRequirementCount.Size = new System.Drawing.Size(198, 20);
            this.lblRequirementCount.TabIndex = 2;
            this.lblRequirementCount.Text = "";
            this.lblRequirementCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRequirementCount.Name = "lblRequirementCount";
            //
            // dgvRequirements
            //
            this.dgvRequirements.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRequirementType,
            this.colSubject,
            this.colMinimumMark,
            this.colAlternative,
            this.colStatus,
            this.colRequirementId,
            this.colSubjectId,
            this.colAlternativeSubjectId,
            this.colAdvisoryNote});
            dgvRequirementsAltStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.dgvRequirements.AlternatingRowsDefaultCellStyle = dgvRequirementsAltStyle;
            dgvRequirementsHeaderStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dgvRequirementsHeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            dgvRequirementsHeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            dgvRequirementsHeaderStyle.ForeColor = System.Drawing.Color.White;
            dgvRequirementsHeaderStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            dgvRequirementsHeaderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            dgvRequirementsHeaderStyle.SelectionForeColor = System.Drawing.Color.White;
            dgvRequirementsHeaderStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRequirements.ColumnHeadersDefaultCellStyle = dgvRequirementsHeaderStyle;
            dgvRequirementsCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dgvRequirementsCellStyle.BackColor = System.Drawing.Color.White;
            dgvRequirementsCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            dgvRequirementsCellStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            dgvRequirementsCellStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            dgvRequirementsCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            dgvRequirementsCellStyle.SelectionForeColor = System.Drawing.Color.White;
            dgvRequirementsCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvRequirements.DefaultCellStyle = dgvRequirementsCellStyle;
            this.dgvRequirements.AccessibleName = "Stored requirements for the selected programme";
            this.dgvRequirements.AllowUserToAddRows = false;
            this.dgvRequirements.AllowUserToDeleteRows = false;
            this.dgvRequirements.AllowUserToResizeRows = false;
            this.dgvRequirements.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRequirements.BackgroundColor = System.Drawing.Color.White;
            this.dgvRequirements.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgvRequirements.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvRequirements.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvRequirements.ColumnHeadersHeight = 34;
            this.dgvRequirements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvRequirements.EnableHeadersVisualStyles = false;
            this.dgvRequirements.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            this.dgvRequirements.Location = new System.Drawing.Point(16, 40);
            this.dgvRequirements.MultiSelect = false;
            this.dgvRequirements.ReadOnly = true;
            this.dgvRequirements.RowHeadersVisible = false;
            this.dgvRequirements.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRequirements.Size = new System.Drawing.Size(642, 208);
            this.dgvRequirements.TabIndex = 3;
            this.dgvRequirements.Name = "dgvRequirements";
            this.dgvRequirements.RowTemplate.Height = 30;
            this.dgvRequirements.SelectionChanged += new System.EventHandler(this.dgvRequirements_SelectionChanged);
            this.dgvRequirements.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvRequirements_CellFormatting);
            //
            // colRequirementType
            //
            this.colRequirementType.DataPropertyName = "RequirementType";
            this.colRequirementType.FillWeight = 21F;
            this.colRequirementType.HeaderText = "Requirement type";
            this.colRequirementType.ReadOnly = true;
            this.colRequirementType.Name = "colRequirementType";
            //
            // colSubject
            //
            this.colSubject.DataPropertyName = "SubjectName";
            this.colSubject.FillWeight = 24F;
            this.colSubject.HeaderText = "Subject";
            this.colSubject.ReadOnly = true;
            this.colSubject.Name = "colSubject";
            //
            // colMinimumMark
            //
            colMinimumMarkStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            colMinimumMarkStyle.Font = new System.Drawing.Font("Consolas", 10F);
            this.colMinimumMark.DefaultCellStyle = colMinimumMarkStyle;
            this.colMinimumMark.DataPropertyName = "MinimumMark";
            this.colMinimumMark.FillWeight = 15F;
            this.colMinimumMark.HeaderText = "Min. mark";
            this.colMinimumMark.ReadOnly = true;
            this.colMinimumMark.Name = "colMinimumMark";
            //
            // colAlternative
            //
            this.colAlternative.DataPropertyName = "AlternativeSubjectName";
            this.colAlternative.FillWeight = 26F;
            this.colAlternative.HeaderText = "Accepted alternative";
            this.colAlternative.ReadOnly = true;
            this.colAlternative.Name = "colAlternative";
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
            // colRequirementId
            //
            this.colRequirementId.DataPropertyName = "RequirementID";
            this.colRequirementId.FillWeight = 1F;
            this.colRequirementId.HeaderText = "RequirementID";
            this.colRequirementId.ReadOnly = true;
            this.colRequirementId.Visible = false;
            this.colRequirementId.Name = "colRequirementId";
            //
            // colSubjectId
            //
            this.colSubjectId.DataPropertyName = "SubjectID";
            this.colSubjectId.FillWeight = 1F;
            this.colSubjectId.HeaderText = "SubjectID";
            this.colSubjectId.ReadOnly = true;
            this.colSubjectId.Visible = false;
            this.colSubjectId.Name = "colSubjectId";
            //
            // colAlternativeSubjectId
            //
            this.colAlternativeSubjectId.DataPropertyName = "AlternativeSubjectID";
            this.colAlternativeSubjectId.FillWeight = 1F;
            this.colAlternativeSubjectId.HeaderText = "AlternativeSubjectID";
            this.colAlternativeSubjectId.ReadOnly = true;
            this.colAlternativeSubjectId.Visible = false;
            this.colAlternativeSubjectId.Name = "colAlternativeSubjectId";
            //
            // colAdvisoryNote
            //
            this.colAdvisoryNote.DataPropertyName = "AdvisoryNote";
            this.colAdvisoryNote.FillWeight = 1F;
            this.colAdvisoryNote.HeaderText = "AdvisoryNote";
            this.colAdvisoryNote.ReadOnly = true;
            this.colAdvisoryNote.Visible = false;
            this.colAdvisoryNote.Name = "colAdvisoryNote";
            //
            // pnlFormCard
            //
            this.pnlFormCard.Controls.Add(this.lblFormHeadingBar);
            this.pnlFormCard.Controls.Add(this.lblFormHeading);
            this.pnlFormCard.Controls.Add(this.lblRequirementType);
            this.pnlFormCard.Controls.Add(this.lblRequirementTypeStar);
            this.pnlFormCard.Controls.Add(this.cboRequirementType);
            this.pnlFormCard.Controls.Add(this.lblRequirementTypeError);
            this.pnlFormCard.Controls.Add(this.lblSubject);
            this.pnlFormCard.Controls.Add(this.lblSubjectStar);
            this.pnlFormCard.Controls.Add(this.cboSubject);
            this.pnlFormCard.Controls.Add(this.lblSubjectError);
            this.pnlFormCard.Controls.Add(this.lblAlternative);
            this.pnlFormCard.Controls.Add(this.lblAlternativeStar);
            this.pnlFormCard.Controls.Add(this.cboAlternative);
            this.pnlFormCard.Controls.Add(this.lblAlternativeError);
            this.pnlFormCard.Controls.Add(this.lblMinimumMark);
            this.pnlFormCard.Controls.Add(this.lblMinimumMarkStar);
            this.pnlFormCard.Controls.Add(this.numMinimumMark);
            this.pnlFormCard.Controls.Add(this.lblMinimumMarkError);
            this.pnlFormCard.Controls.Add(this.chkActive);
            this.pnlFormCard.Controls.Add(this.lblAdvisoryNote);
            this.pnlFormCard.Controls.Add(this.txtAdvisoryNote);
            this.pnlFormCard.Controls.Add(this.lblAdvisoryNoteHint);
            this.pnlFormCard.Controls.Add(this.pnlSummary);
            this.pnlFormCard.BackColor = System.Drawing.Color.White;
            this.pnlFormCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFormCard.Location = new System.Drawing.Point(250, 272);
            this.pnlFormCard.Size = new System.Drawing.Size(676, 255);
            this.pnlFormCard.TabIndex = 6;
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
            this.lblFormHeading.Size = new System.Drawing.Size(400, 28);
            this.lblFormHeading.TabIndex = 1;
            this.lblFormHeading.Text = "Add or edit a requirement";
            this.lblFormHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFormHeading.Name = "lblFormHeading";
            //
            // lblRequirementType
            //
            this.lblRequirementType.AutoSize = true;
            this.lblRequirementType.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblRequirementType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblRequirementType.Location = new System.Drawing.Point(16, 40);
            this.lblRequirementType.TabIndex = 2;
            this.lblRequirementType.Text = "Requirement type";
            this.lblRequirementType.Name = "lblRequirementType";
            //
            // lblRequirementTypeStar
            //
            this.lblRequirementTypeStar.AutoSize = true;
            this.lblRequirementTypeStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblRequirementTypeStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblRequirementTypeStar.Location = new System.Drawing.Point(156, 40);
            this.lblRequirementTypeStar.Size = new System.Drawing.Size(14, 24);
            this.lblRequirementTypeStar.TabIndex = 3;
            this.lblRequirementTypeStar.Text = "*";
            this.lblRequirementTypeStar.Name = "lblRequirementTypeStar";
            //
            // cboRequirementType
            //
            this.cboRequirementType.AccessibleName = "Requirement type (required)";
            this.cboRequirementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRequirementType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboRequirementType.FormattingEnabled = true;
            this.cboRequirementType.Location = new System.Drawing.Point(16, 66);
            this.cboRequirementType.Size = new System.Drawing.Size(190, 25);
            this.cboRequirementType.TabIndex = 4;
            this.cboRequirementType.Name = "cboRequirementType";
            this.cboRequirementType.Leave += new System.EventHandler(this.cboRequirementType_Leave);
            //
            // lblRequirementTypeError
            //
            this.lblRequirementTypeError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblRequirementTypeError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblRequirementTypeError.Location = new System.Drawing.Point(16, 93);
            this.lblRequirementTypeError.Size = new System.Drawing.Size(200, 16);
            this.lblRequirementTypeError.TabIndex = 5;
            this.lblRequirementTypeError.Text = "";
            this.lblRequirementTypeError.Visible = false;
            this.lblRequirementTypeError.Name = "lblRequirementTypeError";
            //
            // lblSubject
            //
            this.lblSubject.AutoSize = true;
            this.lblSubject.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSubject.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblSubject.Location = new System.Drawing.Point(222, 40);
            this.lblSubject.TabIndex = 6;
            this.lblSubject.Text = "Subject";
            this.lblSubject.Name = "lblSubject";
            //
            // lblSubjectStar
            //
            this.lblSubjectStar.AutoSize = true;
            this.lblSubjectStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSubjectStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblSubjectStar.Location = new System.Drawing.Point(285, 40);
            this.lblSubjectStar.Size = new System.Drawing.Size(14, 24);
            this.lblSubjectStar.TabIndex = 7;
            this.lblSubjectStar.Text = "*";
            this.lblSubjectStar.Name = "lblSubjectStar";
            //
            // cboSubject
            //
            this.cboSubject.AccessibleName = "Subject (required)";
            this.cboSubject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSubject.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboSubject.FormattingEnabled = true;
            this.cboSubject.Location = new System.Drawing.Point(222, 66);
            this.cboSubject.Size = new System.Drawing.Size(210, 25);
            this.cboSubject.TabIndex = 8;
            this.cboSubject.Name = "cboSubject";
            this.cboSubject.Leave += new System.EventHandler(this.cboSubject_Leave);
            //
            // lblSubjectError
            //
            this.lblSubjectError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSubjectError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblSubjectError.Location = new System.Drawing.Point(222, 93);
            this.lblSubjectError.Size = new System.Drawing.Size(216, 16);
            this.lblSubjectError.TabIndex = 9;
            this.lblSubjectError.Text = "";
            this.lblSubjectError.Visible = false;
            this.lblSubjectError.Name = "lblSubjectError";
            //
            // lblAlternative
            //
            this.lblAlternative.AutoSize = true;
            this.lblAlternative.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblAlternative.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblAlternative.Location = new System.Drawing.Point(448, 40);
            this.lblAlternative.TabIndex = 10;
            this.lblAlternative.Text = "Accepted alternative";
            this.lblAlternative.Name = "lblAlternative";
            //
            // lblAlternativeStar
            //
            this.lblAlternativeStar.AutoSize = true;
            this.lblAlternativeStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblAlternativeStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblAlternativeStar.Location = new System.Drawing.Point(612, 40);
            this.lblAlternativeStar.Size = new System.Drawing.Size(14, 24);
            this.lblAlternativeStar.TabIndex = 11;
            this.lblAlternativeStar.Text = "*";
            this.lblAlternativeStar.Name = "lblAlternativeStar";
            //
            // cboAlternative
            //
            this.cboAlternative.AccessibleName = "Accepted alternative (required)";
            this.cboAlternative.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAlternative.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboAlternative.FormattingEnabled = true;
            this.cboAlternative.Location = new System.Drawing.Point(448, 66);
            this.cboAlternative.Size = new System.Drawing.Size(210, 25);
            this.cboAlternative.TabIndex = 12;
            this.cboAlternative.Name = "cboAlternative";
            this.cboAlternative.Leave += new System.EventHandler(this.cboAlternative_Leave);
            //
            // lblAlternativeError
            //
            this.lblAlternativeError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblAlternativeError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblAlternativeError.Location = new System.Drawing.Point(448, 93);
            this.lblAlternativeError.Size = new System.Drawing.Size(216, 16);
            this.lblAlternativeError.TabIndex = 13;
            this.lblAlternativeError.Text = "";
            this.lblAlternativeError.Visible = false;
            this.lblAlternativeError.Name = "lblAlternativeError";
            //
            // lblMinimumMark
            //
            this.lblMinimumMark.AutoSize = true;
            this.lblMinimumMark.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblMinimumMark.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblMinimumMark.Location = new System.Drawing.Point(16, 114);
            this.lblMinimumMark.TabIndex = 14;
            this.lblMinimumMark.Text = "Min. mark %";
            this.lblMinimumMark.Name = "lblMinimumMark";
            //
            // lblMinimumMarkStar
            //
            this.lblMinimumMarkStar.AutoSize = true;
            this.lblMinimumMarkStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblMinimumMarkStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblMinimumMarkStar.Location = new System.Drawing.Point(114, 114);
            this.lblMinimumMarkStar.Size = new System.Drawing.Size(14, 24);
            this.lblMinimumMarkStar.TabIndex = 15;
            this.lblMinimumMarkStar.Text = "*";
            this.lblMinimumMarkStar.Name = "lblMinimumMarkStar";
            //
            // numMinimumMark
            //
            this.numMinimumMark.AccessibleName = "Minimum mark percent (required)";
            this.numMinimumMark.Font = new System.Drawing.Font("Consolas", 10F);
            this.numMinimumMark.Location = new System.Drawing.Point(16, 140);
            this.numMinimumMark.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numMinimumMark.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numMinimumMark.Size = new System.Drawing.Size(110, 23);
            this.numMinimumMark.TabIndex = 16;
            this.numMinimumMark.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numMinimumMark.Value = new decimal(new int[] { 0, 0, 0, 0 });
            this.numMinimumMark.Name = "numMinimumMark";
            this.numMinimumMark.Leave += new System.EventHandler(this.numMinimumMark_Leave);
            //
            // lblMinimumMarkError
            //
            this.lblMinimumMarkError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblMinimumMarkError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblMinimumMarkError.Location = new System.Drawing.Point(16, 167);
            this.lblMinimumMarkError.Size = new System.Drawing.Size(200, 16);
            this.lblMinimumMarkError.TabIndex = 17;
            this.lblMinimumMarkError.Text = "";
            this.lblMinimumMarkError.Visible = false;
            this.lblMinimumMarkError.Name = "lblMinimumMarkError";
            //
            // chkActive
            //
            this.chkActive.AccessibleName = "Requirement is active";
            this.chkActive.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.chkActive.Location = new System.Drawing.Point(140, 140);
            this.chkActive.Size = new System.Drawing.Size(76, 24);
            this.chkActive.TabIndex = 18;
            this.chkActive.Text = "Active";
            this.chkActive.UseVisualStyleBackColor = true;
            this.chkActive.Name = "chkActive";
            //
            // lblAdvisoryNote
            //
            this.lblAdvisoryNote.AutoSize = true;
            this.lblAdvisoryNote.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblAdvisoryNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblAdvisoryNote.Location = new System.Drawing.Point(222, 114);
            this.lblAdvisoryNote.TabIndex = 19;
            this.lblAdvisoryNote.Text = "Advisory note shown to the learner";
            this.lblAdvisoryNote.Name = "lblAdvisoryNote";
            //
            // txtAdvisoryNote
            //
            this.txtAdvisoryNote.AcceptsReturn = true;
            this.txtAdvisoryNote.AccessibleName = "Advisory note shown to the learner (optional, up to 255 characters)";
            this.txtAdvisoryNote.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtAdvisoryNote.Location = new System.Drawing.Point(222, 140);
            this.txtAdvisoryNote.MaxLength = 255;
            this.txtAdvisoryNote.Multiline = true;
            this.txtAdvisoryNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAdvisoryNote.Size = new System.Drawing.Size(436, 50);
            this.txtAdvisoryNote.TabIndex = 20;
            this.txtAdvisoryNote.Name = "txtAdvisoryNote";
            //
            // lblAdvisoryNoteHint
            //
            this.lblAdvisoryNoteHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblAdvisoryNoteHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblAdvisoryNoteHint.Location = new System.Drawing.Point(222, 191);
            this.lblAdvisoryNoteHint.Size = new System.Drawing.Size(436, 16);
            this.lblAdvisoryNoteHint.TabIndex = 21;
            this.lblAdvisoryNoteHint.Text = "Optional \u00B7 up to 255 characters";
            this.lblAdvisoryNoteHint.Name = "lblAdvisoryNoteHint";
            //
            // pnlSummary
            //
            this.pnlSummary.Controls.Add(this.lblSummary);
            this.pnlSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.pnlSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSummary.Location = new System.Drawing.Point(16, 210);
            this.pnlSummary.Size = new System.Drawing.Size(642, 38);
            this.pnlSummary.TabIndex = 22;
            this.pnlSummary.Visible = false;
            this.pnlSummary.Name = "pnlSummary";
            //
            // lblSummary
            //
            this.lblSummary.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblSummary.Location = new System.Drawing.Point(12, 6);
            this.lblSummary.Size = new System.Drawing.Size(616, 26);
            this.lblSummary.TabIndex = 0;
            this.lblSummary.Text = "";
            this.lblSummary.Name = "lblSummary";
            //
            // DegreeRequirementsTab
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.Size = new System.Drawing.Size(926, 527);
            this.Controls.Add(this.lblFaculty);
            this.Controls.Add(this.cboFaculty);
            this.Controls.Add(this.lblProgramme);
            this.Controls.Add(this.lstProgrammes);
            this.Controls.Add(this.lblProgrammeCount);
            this.Controls.Add(this.pnlGridCard);
            this.Controls.Add(this.pnlFormCard);
            this.Name = "DegreeRequirementsTab";
            this.pnlGridCard.ResumeLayout(false);
            this.pnlGridCard.PerformLayout();
            this.lblGridHeadingBar.ResumeLayout(false);
            this.lblGridHeadingBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequirements)).EndInit();
            this.pnlFormCard.ResumeLayout(false);
            this.pnlFormCard.PerformLayout();
            this.lblFormHeadingBar.ResumeLayout(false);
            this.lblFormHeadingBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinimumMark)).EndInit();
            this.pnlSummary.ResumeLayout(false);
            this.pnlSummary.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblFaculty;
        private System.Windows.Forms.ComboBox cboFaculty;
        private System.Windows.Forms.Label lblProgramme;
        private System.Windows.Forms.ListBox lstProgrammes;
        private System.Windows.Forms.Label lblProgrammeCount;
        private System.Windows.Forms.Panel pnlGridCard;
        private System.Windows.Forms.Panel lblGridHeadingBar;
        private System.Windows.Forms.Label lblGridHeading;
        private System.Windows.Forms.Label lblRequirementCount;
        private System.Windows.Forms.DataGridView dgvRequirements;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRequirementType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSubject;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMinimumMark;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlternative;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRequirementId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSubjectId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlternativeSubjectId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAdvisoryNote;
        private System.Windows.Forms.Panel pnlFormCard;
        private System.Windows.Forms.Panel lblFormHeadingBar;
        private System.Windows.Forms.Label lblFormHeading;
        private System.Windows.Forms.Label lblRequirementType;
        private System.Windows.Forms.Label lblRequirementTypeStar;
        private System.Windows.Forms.ComboBox cboRequirementType;
        private System.Windows.Forms.Label lblRequirementTypeError;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.Label lblSubjectStar;
        private System.Windows.Forms.ComboBox cboSubject;
        private System.Windows.Forms.Label lblSubjectError;
        private System.Windows.Forms.Label lblAlternative;
        private System.Windows.Forms.Label lblAlternativeStar;
        private System.Windows.Forms.ComboBox cboAlternative;
        private System.Windows.Forms.Label lblAlternativeError;
        private System.Windows.Forms.Label lblMinimumMark;
        private System.Windows.Forms.Label lblMinimumMarkStar;
        private System.Windows.Forms.NumericUpDown numMinimumMark;
        private System.Windows.Forms.Label lblMinimumMarkError;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.Label lblAdvisoryNote;
        private System.Windows.Forms.TextBox txtAdvisoryNote;
        private System.Windows.Forms.Label lblAdvisoryNoteHint;
        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Label lblSummary;
    }
}
