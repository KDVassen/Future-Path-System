namespace FuturePath.PresentationLayer
{
    partial class UniversityInfoTab
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
            System.Windows.Forms.DataGridViewCellStyle dgvProgrammesAltStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvProgrammesHeaderStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvProgrammesCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colMinimumAverageStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colStatusStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblFaculties = new System.Windows.Forms.Label();
            this.lstFaculties = new System.Windows.Forms.ListBox();
            this.lblFacultyCount = new System.Windows.Forms.Label();
            this.pnlFacultyCard = new System.Windows.Forms.Panel();
            this.lblFacultyHeadingBar = new System.Windows.Forms.Panel();
            this.lblFacultyHeading = new System.Windows.Forms.Label();
            this.lblFacultyName = new System.Windows.Forms.Label();
            this.lblFacultyNameStar = new System.Windows.Forms.Label();
            this.txtFacultyName = new System.Windows.Forms.TextBox();
            this.lblFacultyNameError = new System.Windows.Forms.Label();
            this.btnAddFaculty = new System.Windows.Forms.Button();
            this.btnRenameFaculty = new System.Windows.Forms.Button();
            this.btnDeactivateFaculty = new System.Windows.Forms.Button();
            this.lblFacultyHint = new System.Windows.Forms.Label();
            this.pnlGridCard = new System.Windows.Forms.Panel();
            this.lblGridHeadingBar = new System.Windows.Forms.Panel();
            this.lblGridHeading = new System.Windows.Forms.Label();
            this.lblProgrammeCount = new System.Windows.Forms.Label();
            this.dgvProgrammes = new System.Windows.Forms.DataGridView();
            this.colProgrammeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMinimumAverage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProgrammeId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFacultyId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlFormCard = new System.Windows.Forms.Panel();
            this.lblFormHeadingBar = new System.Windows.Forms.Panel();
            this.lblFormHeading = new System.Windows.Forms.Label();
            this.lblProgrammeFaculty = new System.Windows.Forms.Label();
            this.lblProgrammeFacultyStar = new System.Windows.Forms.Label();
            this.cboProgrammeFaculty = new System.Windows.Forms.ComboBox();
            this.lblProgrammeFacultyError = new System.Windows.Forms.Label();
            this.lblProgrammeName = new System.Windows.Forms.Label();
            this.lblProgrammeNameStar = new System.Windows.Forms.Label();
            this.txtProgrammeName = new System.Windows.Forms.TextBox();
            this.lblProgrammeNameError = new System.Windows.Forms.Label();
            this.lblMinimumAverage = new System.Windows.Forms.Label();
            this.numMinimumAverage = new System.Windows.Forms.NumericUpDown();
            this.lblMinimumAverageError = new System.Windows.Forms.Label();
            this.lblMinimumAverageHint = new System.Windows.Forms.Label();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.pnlFacultyCard.SuspendLayout();
            this.lblFacultyHeadingBar.SuspendLayout();
            this.pnlGridCard.SuspendLayout();
            this.lblGridHeadingBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProgrammes)).BeginInit();
            this.pnlFormCard.SuspendLayout();
            this.lblFormHeadingBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinimumAverage)).BeginInit();
            this.pnlSummary.SuspendLayout();
            this.SuspendLayout();
            //
            // lblFaculties
            //
            this.lblFaculties.AutoSize = true;
            this.lblFaculties.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFaculties.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFaculties.Location = new System.Drawing.Point(0, 0);
            this.lblFaculties.TabIndex = 0;
            this.lblFaculties.Text = "Faculty";
            this.lblFaculties.Name = "lblFaculties";
            //
            // lstFaculties
            //
            this.lstFaculties.AccessibleName = "Faculty (scope)";
            this.lstFaculties.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lstFaculties.FormattingEnabled = true;
            this.lstFaculties.IntegralHeight = false;
            this.lstFaculties.ItemHeight = 17;
            this.lstFaculties.Location = new System.Drawing.Point(0, 26);
            this.lstFaculties.Size = new System.Drawing.Size(230, 226);
            this.lstFaculties.TabIndex = 1;
            this.lstFaculties.Name = "lstFaculties";
            this.lstFaculties.SelectedIndexChanged += new System.EventHandler(this.lstFaculties_SelectedIndexChanged);
            this.lstFaculties.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.lstFaculties_Format);
            //
            // lblFacultyCount
            //
            this.lblFacultyCount.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFacultyCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblFacultyCount.Location = new System.Drawing.Point(0, 256);
            this.lblFacultyCount.Size = new System.Drawing.Size(230, 36);
            this.lblFacultyCount.TabIndex = 2;
            this.lblFacultyCount.Text = "Select a faculty to list its programmes.";
            this.lblFacultyCount.Name = "lblFacultyCount";
            //
            // pnlFacultyCard
            //
            this.pnlFacultyCard.Controls.Add(this.lblFacultyHeadingBar);
            this.pnlFacultyCard.Controls.Add(this.lblFacultyHeading);
            this.pnlFacultyCard.Controls.Add(this.lblFacultyName);
            this.pnlFacultyCard.Controls.Add(this.lblFacultyNameStar);
            this.pnlFacultyCard.Controls.Add(this.txtFacultyName);
            this.pnlFacultyCard.Controls.Add(this.lblFacultyNameError);
            this.pnlFacultyCard.Controls.Add(this.btnAddFaculty);
            this.pnlFacultyCard.Controls.Add(this.btnRenameFaculty);
            this.pnlFacultyCard.Controls.Add(this.btnDeactivateFaculty);
            this.pnlFacultyCard.Controls.Add(this.lblFacultyHint);
            this.pnlFacultyCard.BackColor = System.Drawing.Color.White;
            this.pnlFacultyCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFacultyCard.Location = new System.Drawing.Point(0, 297);
            this.pnlFacultyCard.Size = new System.Drawing.Size(230, 230);
            this.pnlFacultyCard.TabIndex = 3;
            this.pnlFacultyCard.Name = "pnlFacultyCard";
            //
            // lblFacultyHeadingBar
            //
            this.lblFacultyHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblFacultyHeadingBar.Location = new System.Drawing.Point(12, 10);
            this.lblFacultyHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblFacultyHeadingBar.TabIndex = 0;
            this.lblFacultyHeadingBar.Name = "lblFacultyHeadingBar";
            //
            // lblFacultyHeading
            //
            this.lblFacultyHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFacultyHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFacultyHeading.Location = new System.Drawing.Point(22, 6);
            this.lblFacultyHeading.Size = new System.Drawing.Size(200, 28);
            this.lblFacultyHeading.TabIndex = 1;
            this.lblFacultyHeading.Text = "Faculty details";
            this.lblFacultyHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFacultyHeading.Name = "lblFacultyHeading";
            //
            // lblFacultyName
            //
            this.lblFacultyName.AutoSize = true;
            this.lblFacultyName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFacultyName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFacultyName.Location = new System.Drawing.Point(16, 40);
            this.lblFacultyName.TabIndex = 2;
            this.lblFacultyName.Text = "Faculty name";
            this.lblFacultyName.Name = "lblFacultyName";
            //
            // lblFacultyNameStar
            //
            this.lblFacultyNameStar.AutoSize = true;
            this.lblFacultyNameStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFacultyNameStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblFacultyNameStar.Location = new System.Drawing.Point(127, 40);
            this.lblFacultyNameStar.Size = new System.Drawing.Size(14, 24);
            this.lblFacultyNameStar.TabIndex = 3;
            this.lblFacultyNameStar.Text = "*";
            this.lblFacultyNameStar.Name = "lblFacultyNameStar";
            //
            // txtFacultyName
            //
            this.txtFacultyName.AccessibleName = "Faculty name (required)";
            this.txtFacultyName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtFacultyName.Location = new System.Drawing.Point(16, 66);
            this.txtFacultyName.MaxLength = 60;
            this.txtFacultyName.Size = new System.Drawing.Size(196, 25);
            this.txtFacultyName.TabIndex = 4;
            this.txtFacultyName.Name = "txtFacultyName";
            this.txtFacultyName.Leave += new System.EventHandler(this.txtFacultyName_Leave);
            //
            // lblFacultyNameError
            //
            this.lblFacultyNameError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFacultyNameError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblFacultyNameError.Location = new System.Drawing.Point(16, 93);
            this.lblFacultyNameError.Size = new System.Drawing.Size(200, 16);
            this.lblFacultyNameError.TabIndex = 5;
            this.lblFacultyNameError.Text = "";
            this.lblFacultyNameError.Visible = false;
            this.lblFacultyNameError.Name = "lblFacultyNameError";
            //
            // btnAddFaculty
            //
            this.btnAddFaculty.AccessibleName = "Add faculty";
            this.btnAddFaculty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.btnAddFaculty.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddFaculty.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddFaculty.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnAddFaculty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnAddFaculty.Location = new System.Drawing.Point(16, 116);
            this.btnAddFaculty.Size = new System.Drawing.Size(96, 30);
            this.btnAddFaculty.TabIndex = 6;
            this.btnAddFaculty.Text = "Add faculty";
            this.btnAddFaculty.UseVisualStyleBackColor = false;
            this.btnAddFaculty.Name = "btnAddFaculty";
            this.btnAddFaculty.FlatAppearance.BorderSize = 0;
            this.btnAddFaculty.Click += new System.EventHandler(this.btnAddFaculty_Click);
            //
            // btnRenameFaculty
            //
            this.btnRenameFaculty.AccessibleName = "Rename";
            this.btnRenameFaculty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.btnRenameFaculty.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRenameFaculty.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRenameFaculty.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnRenameFaculty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnRenameFaculty.Location = new System.Drawing.Point(116, 116);
            this.btnRenameFaculty.Size = new System.Drawing.Size(96, 30);
            this.btnRenameFaculty.TabIndex = 7;
            this.btnRenameFaculty.Text = "Rename";
            this.btnRenameFaculty.UseVisualStyleBackColor = false;
            this.btnRenameFaculty.Name = "btnRenameFaculty";
            this.btnRenameFaculty.FlatAppearance.BorderSize = 0;
            this.btnRenameFaculty.Click += new System.EventHandler(this.btnRenameFaculty_Click);
            //
            // btnDeactivateFaculty
            //
            this.btnDeactivateFaculty.AccessibleName = "Deactivate faculty";
            this.btnDeactivateFaculty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.btnDeactivateFaculty.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeactivateFaculty.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeactivateFaculty.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnDeactivateFaculty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnDeactivateFaculty.Location = new System.Drawing.Point(16, 154);
            this.btnDeactivateFaculty.Size = new System.Drawing.Size(196, 30);
            this.btnDeactivateFaculty.TabIndex = 8;
            this.btnDeactivateFaculty.Text = "Deactivate faculty";
            this.btnDeactivateFaculty.UseVisualStyleBackColor = false;
            this.btnDeactivateFaculty.Name = "btnDeactivateFaculty";
            this.btnDeactivateFaculty.FlatAppearance.BorderSize = 0;
            this.btnDeactivateFaculty.Click += new System.EventHandler(this.btnDeactivateFaculty_Click);
            //
            // lblFacultyHint
            //
            this.lblFacultyHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFacultyHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblFacultyHint.Location = new System.Drawing.Point(16, 190);
            this.lblFacultyHint.Size = new System.Drawing.Size(200, 32);
            this.lblFacultyHint.TabIndex = 9;
            this.lblFacultyHint.Text = "Faculty buttons save at once. Faculties are deactivated, never deleted.";
            this.lblFacultyHint.Name = "lblFacultyHint";
            //
            // pnlGridCard
            //
            this.pnlGridCard.Controls.Add(this.lblGridHeadingBar);
            this.pnlGridCard.Controls.Add(this.lblGridHeading);
            this.pnlGridCard.Controls.Add(this.lblProgrammeCount);
            this.pnlGridCard.Controls.Add(this.dgvProgrammes);
            this.pnlGridCard.BackColor = System.Drawing.Color.White;
            this.pnlGridCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGridCard.Location = new System.Drawing.Point(250, 0);
            this.pnlGridCard.Size = new System.Drawing.Size(676, 277);
            this.pnlGridCard.TabIndex = 4;
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
            this.lblGridHeading.Text = "Programmes \u2014 no faculty selected";
            this.lblGridHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblGridHeading.Name = "lblGridHeading";
            //
            // lblProgrammeCount
            //
            this.lblProgrammeCount.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblProgrammeCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblProgrammeCount.Location = new System.Drawing.Point(460, 10);
            this.lblProgrammeCount.Size = new System.Drawing.Size(198, 20);
            this.lblProgrammeCount.TabIndex = 2;
            this.lblProgrammeCount.Text = "";
            this.lblProgrammeCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblProgrammeCount.Name = "lblProgrammeCount";
            //
            // dgvProgrammes
            //
            this.dgvProgrammes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colProgrammeName,
            this.colMinimumAverage,
            this.colStatus,
            this.colProgrammeId,
            this.colFacultyId});
            dgvProgrammesAltStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.dgvProgrammes.AlternatingRowsDefaultCellStyle = dgvProgrammesAltStyle;
            dgvProgrammesHeaderStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dgvProgrammesHeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            dgvProgrammesHeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            dgvProgrammesHeaderStyle.ForeColor = System.Drawing.Color.White;
            dgvProgrammesHeaderStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            dgvProgrammesHeaderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            dgvProgrammesHeaderStyle.SelectionForeColor = System.Drawing.Color.White;
            dgvProgrammesHeaderStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvProgrammes.ColumnHeadersDefaultCellStyle = dgvProgrammesHeaderStyle;
            dgvProgrammesCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dgvProgrammesCellStyle.BackColor = System.Drawing.Color.White;
            dgvProgrammesCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            dgvProgrammesCellStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            dgvProgrammesCellStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            dgvProgrammesCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            dgvProgrammesCellStyle.SelectionForeColor = System.Drawing.Color.White;
            dgvProgrammesCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvProgrammes.DefaultCellStyle = dgvProgrammesCellStyle;
            this.dgvProgrammes.AccessibleName = "Programmes in the selected faculty";
            this.dgvProgrammes.AllowUserToAddRows = false;
            this.dgvProgrammes.AllowUserToDeleteRows = false;
            this.dgvProgrammes.AllowUserToResizeRows = false;
            this.dgvProgrammes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProgrammes.BackgroundColor = System.Drawing.Color.White;
            this.dgvProgrammes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgvProgrammes.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvProgrammes.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvProgrammes.ColumnHeadersHeight = 34;
            this.dgvProgrammes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvProgrammes.EnableHeadersVisualStyles = false;
            this.dgvProgrammes.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            this.dgvProgrammes.Location = new System.Drawing.Point(16, 40);
            this.dgvProgrammes.MultiSelect = false;
            this.dgvProgrammes.ReadOnly = true;
            this.dgvProgrammes.RowHeadersVisible = false;
            this.dgvProgrammes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProgrammes.Size = new System.Drawing.Size(642, 223);
            this.dgvProgrammes.TabIndex = 3;
            this.dgvProgrammes.Name = "dgvProgrammes";
            this.dgvProgrammes.RowTemplate.Height = 30;
            this.dgvProgrammes.SelectionChanged += new System.EventHandler(this.dgvProgrammes_SelectionChanged);
            this.dgvProgrammes.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvProgrammes_CellFormatting);
            //
            // colProgrammeName
            //
            this.colProgrammeName.DataPropertyName = "ProgrammeName";
            this.colProgrammeName.FillWeight = 56F;
            this.colProgrammeName.HeaderText = "Programme";
            this.colProgrammeName.ReadOnly = true;
            this.colProgrammeName.Name = "colProgrammeName";
            //
            // colMinimumAverage
            //
            colMinimumAverageStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            colMinimumAverageStyle.Font = new System.Drawing.Font("Consolas", 10F);
            this.colMinimumAverage.DefaultCellStyle = colMinimumAverageStyle;
            this.colMinimumAverage.DataPropertyName = "MinimumOverallAverage";
            this.colMinimumAverage.FillWeight = 22F;
            this.colMinimumAverage.HeaderText = "Min. overall average";
            this.colMinimumAverage.ReadOnly = true;
            this.colMinimumAverage.Name = "colMinimumAverage";
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
            // colProgrammeId
            //
            this.colProgrammeId.DataPropertyName = "ProgrammeID";
            this.colProgrammeId.FillWeight = 1F;
            this.colProgrammeId.HeaderText = "ProgrammeID";
            this.colProgrammeId.ReadOnly = true;
            this.colProgrammeId.Visible = false;
            this.colProgrammeId.Name = "colProgrammeId";
            //
            // colFacultyId
            //
            this.colFacultyId.DataPropertyName = "FacultyID";
            this.colFacultyId.FillWeight = 1F;
            this.colFacultyId.HeaderText = "FacultyID";
            this.colFacultyId.ReadOnly = true;
            this.colFacultyId.Visible = false;
            this.colFacultyId.Name = "colFacultyId";
            //
            // pnlFormCard
            //
            this.pnlFormCard.Controls.Add(this.lblFormHeadingBar);
            this.pnlFormCard.Controls.Add(this.lblFormHeading);
            this.pnlFormCard.Controls.Add(this.lblProgrammeFaculty);
            this.pnlFormCard.Controls.Add(this.lblProgrammeFacultyStar);
            this.pnlFormCard.Controls.Add(this.cboProgrammeFaculty);
            this.pnlFormCard.Controls.Add(this.lblProgrammeFacultyError);
            this.pnlFormCard.Controls.Add(this.lblProgrammeName);
            this.pnlFormCard.Controls.Add(this.lblProgrammeNameStar);
            this.pnlFormCard.Controls.Add(this.txtProgrammeName);
            this.pnlFormCard.Controls.Add(this.lblProgrammeNameError);
            this.pnlFormCard.Controls.Add(this.lblMinimumAverage);
            this.pnlFormCard.Controls.Add(this.numMinimumAverage);
            this.pnlFormCard.Controls.Add(this.lblMinimumAverageError);
            this.pnlFormCard.Controls.Add(this.lblMinimumAverageHint);
            this.pnlFormCard.Controls.Add(this.chkActive);
            this.pnlFormCard.Controls.Add(this.pnlSummary);
            this.pnlFormCard.BackColor = System.Drawing.Color.White;
            this.pnlFormCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFormCard.Location = new System.Drawing.Point(250, 287);
            this.pnlFormCard.Size = new System.Drawing.Size(676, 240);
            this.pnlFormCard.TabIndex = 5;
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
            this.lblFormHeading.Text = "Add or edit a programme";
            this.lblFormHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFormHeading.Name = "lblFormHeading";
            //
            // lblProgrammeFaculty
            //
            this.lblProgrammeFaculty.AutoSize = true;
            this.lblProgrammeFaculty.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblProgrammeFaculty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblProgrammeFaculty.Location = new System.Drawing.Point(16, 40);
            this.lblProgrammeFaculty.TabIndex = 2;
            this.lblProgrammeFaculty.Text = "Faculty";
            this.lblProgrammeFaculty.Name = "lblProgrammeFaculty";
            //
            // lblProgrammeFacultyStar
            //
            this.lblProgrammeFacultyStar.AutoSize = true;
            this.lblProgrammeFacultyStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblProgrammeFacultyStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblProgrammeFacultyStar.Location = new System.Drawing.Point(79, 40);
            this.lblProgrammeFacultyStar.Size = new System.Drawing.Size(14, 24);
            this.lblProgrammeFacultyStar.TabIndex = 3;
            this.lblProgrammeFacultyStar.Text = "*";
            this.lblProgrammeFacultyStar.Name = "lblProgrammeFacultyStar";
            //
            // cboProgrammeFaculty
            //
            this.cboProgrammeFaculty.AccessibleName = "Faculty of the programme (required)";
            this.cboProgrammeFaculty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProgrammeFaculty.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboProgrammeFaculty.FormattingEnabled = true;
            this.cboProgrammeFaculty.Location = new System.Drawing.Point(16, 66);
            this.cboProgrammeFaculty.Size = new System.Drawing.Size(200, 25);
            this.cboProgrammeFaculty.TabIndex = 4;
            this.cboProgrammeFaculty.Name = "cboProgrammeFaculty";
            this.cboProgrammeFaculty.Leave += new System.EventHandler(this.cboProgrammeFaculty_Leave);
            //
            // lblProgrammeFacultyError
            //
            this.lblProgrammeFacultyError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblProgrammeFacultyError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblProgrammeFacultyError.Location = new System.Drawing.Point(16, 93);
            this.lblProgrammeFacultyError.Size = new System.Drawing.Size(210, 16);
            this.lblProgrammeFacultyError.TabIndex = 5;
            this.lblProgrammeFacultyError.Text = "";
            this.lblProgrammeFacultyError.Visible = false;
            this.lblProgrammeFacultyError.Name = "lblProgrammeFacultyError";
            //
            // lblProgrammeName
            //
            this.lblProgrammeName.AutoSize = true;
            this.lblProgrammeName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblProgrammeName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblProgrammeName.Location = new System.Drawing.Point(232, 40);
            this.lblProgrammeName.TabIndex = 6;
            this.lblProgrammeName.Text = "Programme name";
            this.lblProgrammeName.Name = "lblProgrammeName";
            //
            // lblProgrammeNameStar
            //
            this.lblProgrammeNameStar.AutoSize = true;
            this.lblProgrammeNameStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblProgrammeNameStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblProgrammeNameStar.Location = new System.Drawing.Point(372, 40);
            this.lblProgrammeNameStar.Size = new System.Drawing.Size(14, 24);
            this.lblProgrammeNameStar.TabIndex = 7;
            this.lblProgrammeNameStar.Text = "*";
            this.lblProgrammeNameStar.Name = "lblProgrammeNameStar";
            //
            // txtProgrammeName
            //
            this.txtProgrammeName.AccessibleName = "Programme name (required)";
            this.txtProgrammeName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtProgrammeName.Location = new System.Drawing.Point(232, 66);
            this.txtProgrammeName.MaxLength = 100;
            this.txtProgrammeName.Size = new System.Drawing.Size(426, 25);
            this.txtProgrammeName.TabIndex = 8;
            this.txtProgrammeName.Name = "txtProgrammeName";
            this.txtProgrammeName.Leave += new System.EventHandler(this.txtProgrammeName_Leave);
            //
            // lblProgrammeNameError
            //
            this.lblProgrammeNameError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblProgrammeNameError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblProgrammeNameError.Location = new System.Drawing.Point(232, 93);
            this.lblProgrammeNameError.Size = new System.Drawing.Size(426, 16);
            this.lblProgrammeNameError.TabIndex = 9;
            this.lblProgrammeNameError.Text = "";
            this.lblProgrammeNameError.Visible = false;
            this.lblProgrammeNameError.Name = "lblProgrammeNameError";
            //
            // lblMinimumAverage
            //
            this.lblMinimumAverage.AutoSize = true;
            this.lblMinimumAverage.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblMinimumAverage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblMinimumAverage.Location = new System.Drawing.Point(16, 114);
            this.lblMinimumAverage.TabIndex = 10;
            this.lblMinimumAverage.Text = "Minimum overall average %";
            this.lblMinimumAverage.Name = "lblMinimumAverage";
            //
            // numMinimumAverage
            //
            this.numMinimumAverage.AccessibleName = "Minimum overall average percent";
            this.numMinimumAverage.DecimalPlaces = 2;
            this.numMinimumAverage.Font = new System.Drawing.Font("Consolas", 10F);
            this.numMinimumAverage.Location = new System.Drawing.Point(16, 140);
            this.numMinimumAverage.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numMinimumAverage.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numMinimumAverage.Size = new System.Drawing.Size(120, 23);
            this.numMinimumAverage.TabIndex = 11;
            this.numMinimumAverage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numMinimumAverage.Value = new decimal(new int[] { 0, 0, 0, 0 });
            this.numMinimumAverage.Name = "numMinimumAverage";
            this.numMinimumAverage.Leave += new System.EventHandler(this.numMinimumAverage_Leave);
            //
            // lblMinimumAverageError
            //
            this.lblMinimumAverageError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblMinimumAverageError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblMinimumAverageError.Location = new System.Drawing.Point(16, 167);
            this.lblMinimumAverageError.Size = new System.Drawing.Size(300, 16);
            this.lblMinimumAverageError.TabIndex = 12;
            this.lblMinimumAverageError.Text = "";
            this.lblMinimumAverageError.Visible = false;
            this.lblMinimumAverageError.Name = "lblMinimumAverageError";
            //
            // lblMinimumAverageHint
            //
            this.lblMinimumAverageHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblMinimumAverageHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblMinimumAverageHint.Location = new System.Drawing.Point(150, 140);
            this.lblMinimumAverageHint.Size = new System.Drawing.Size(270, 34);
            this.lblMinimumAverageHint.TabIndex = 13;
            this.lblMinimumAverageHint.Text = "Optional. Leave at 0.00 when the programme sets no overall average.";
            this.lblMinimumAverageHint.Name = "lblMinimumAverageHint";
            //
            // chkActive
            //
            this.chkActive.AccessibleName = "Programme is active";
            this.chkActive.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.chkActive.Location = new System.Drawing.Point(450, 140);
            this.chkActive.Size = new System.Drawing.Size(90, 24);
            this.chkActive.TabIndex = 14;
            this.chkActive.Text = "Active";
            this.chkActive.UseVisualStyleBackColor = true;
            this.chkActive.Name = "chkActive";
            //
            // pnlSummary
            //
            this.pnlSummary.Controls.Add(this.lblSummary);
            this.pnlSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.pnlSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSummary.Location = new System.Drawing.Point(16, 192);
            this.pnlSummary.Size = new System.Drawing.Size(642, 38);
            this.pnlSummary.TabIndex = 15;
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
            // UniversityInfoTab
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.Size = new System.Drawing.Size(926, 527);
            this.Controls.Add(this.lblFaculties);
            this.Controls.Add(this.lstFaculties);
            this.Controls.Add(this.lblFacultyCount);
            this.Controls.Add(this.pnlFacultyCard);
            this.Controls.Add(this.pnlGridCard);
            this.Controls.Add(this.pnlFormCard);
            this.Name = "UniversityInfoTab";
            this.pnlFacultyCard.ResumeLayout(false);
            this.pnlFacultyCard.PerformLayout();
            this.lblFacultyHeadingBar.ResumeLayout(false);
            this.lblFacultyHeadingBar.PerformLayout();
            this.pnlGridCard.ResumeLayout(false);
            this.pnlGridCard.PerformLayout();
            this.lblGridHeadingBar.ResumeLayout(false);
            this.lblGridHeadingBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProgrammes)).EndInit();
            this.pnlFormCard.ResumeLayout(false);
            this.pnlFormCard.PerformLayout();
            this.lblFormHeadingBar.ResumeLayout(false);
            this.lblFormHeadingBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinimumAverage)).EndInit();
            this.pnlSummary.ResumeLayout(false);
            this.pnlSummary.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblFaculties;
        private System.Windows.Forms.ListBox lstFaculties;
        private System.Windows.Forms.Label lblFacultyCount;
        private System.Windows.Forms.Panel pnlFacultyCard;
        private System.Windows.Forms.Panel lblFacultyHeadingBar;
        private System.Windows.Forms.Label lblFacultyHeading;
        private System.Windows.Forms.Label lblFacultyName;
        private System.Windows.Forms.Label lblFacultyNameStar;
        private System.Windows.Forms.TextBox txtFacultyName;
        private System.Windows.Forms.Label lblFacultyNameError;
        private System.Windows.Forms.Button btnAddFaculty;
        private System.Windows.Forms.Button btnRenameFaculty;
        private System.Windows.Forms.Button btnDeactivateFaculty;
        private System.Windows.Forms.Label lblFacultyHint;
        private System.Windows.Forms.Panel pnlGridCard;
        private System.Windows.Forms.Panel lblGridHeadingBar;
        private System.Windows.Forms.Label lblGridHeading;
        private System.Windows.Forms.Label lblProgrammeCount;
        private System.Windows.Forms.DataGridView dgvProgrammes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProgrammeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMinimumAverage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProgrammeId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFacultyId;
        private System.Windows.Forms.Panel pnlFormCard;
        private System.Windows.Forms.Panel lblFormHeadingBar;
        private System.Windows.Forms.Label lblFormHeading;
        private System.Windows.Forms.Label lblProgrammeFaculty;
        private System.Windows.Forms.Label lblProgrammeFacultyStar;
        private System.Windows.Forms.ComboBox cboProgrammeFaculty;
        private System.Windows.Forms.Label lblProgrammeFacultyError;
        private System.Windows.Forms.Label lblProgrammeName;
        private System.Windows.Forms.Label lblProgrammeNameStar;
        private System.Windows.Forms.TextBox txtProgrammeName;
        private System.Windows.Forms.Label lblProgrammeNameError;
        private System.Windows.Forms.Label lblMinimumAverage;
        private System.Windows.Forms.NumericUpDown numMinimumAverage;
        private System.Windows.Forms.Label lblMinimumAverageError;
        private System.Windows.Forms.Label lblMinimumAverageHint;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Label lblSummary;
    }
}
