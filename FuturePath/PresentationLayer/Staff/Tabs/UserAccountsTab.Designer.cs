namespace FuturePath.PresentationLayer
{
    partial class UserAccountsTab
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
            System.Windows.Forms.DataGridViewCellStyle grdAccountsAltStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle grdAccountsHeaderStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle grdAccountsCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlAccountsCard = new System.Windows.Forms.Panel();
            this.lblAccountsHeadingBar = new System.Windows.Forms.Panel();
            this.lblAccountsHeading = new System.Windows.Forms.Label();
            this.lblAccountCount = new System.Windows.Forms.Label();
            this.grdAccounts = new System.Windows.Forms.DataGridView();
            this.colUserId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUsername = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFullName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRole = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAccountStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlAccountFormCard = new System.Windows.Forms.Panel();
            this.lblAccountFormHeadingBar = new System.Windows.Forms.Panel();
            this.lblAccountFormHeading = new System.Windows.Forms.Label();
            this.lblAccountRequiredNote = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblUsernameStar = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUsernameError = new System.Windows.Forms.Label();
            this.lblFullName = new System.Windows.Forms.Label();
            this.lblFullNameStar = new System.Windows.Forms.Label();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.lblFullNameError = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.lblRoleStar = new System.Windows.Forms.Label();
            this.cboRole = new System.Windows.Forms.ComboBox();
            this.lblRoleError = new System.Windows.Forms.Label();
            this.chkAccountActive = new System.Windows.Forms.CheckBox();
            this.pnlPassword = new System.Windows.Forms.Panel();
            this.lblPasswordHeading = new System.Windows.Forms.Label();
            this.lblNewPassword = new System.Windows.Forms.Label();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.lblNewPasswordError = new System.Windows.Forms.Label();
            this.lblConfirmPassword = new System.Windows.Forms.Label();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.lblConfirmPasswordError = new System.Windows.Forms.Label();
            this.lblPasswordHint = new System.Windows.Forms.Label();
            this.pnlAccountSummary = new System.Windows.Forms.Panel();
            this.lblAccountSummary = new System.Windows.Forms.Label();
            this.pnlAccountsCard.SuspendLayout();
            this.lblAccountsHeadingBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdAccounts)).BeginInit();
            this.pnlAccountFormCard.SuspendLayout();
            this.lblAccountFormHeadingBar.SuspendLayout();
            this.pnlPassword.SuspendLayout();
            this.pnlAccountSummary.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlAccountsCard
            //
            this.pnlAccountsCard.Controls.Add(this.lblAccountsHeadingBar);
            this.pnlAccountsCard.Controls.Add(this.lblAccountsHeading);
            this.pnlAccountsCard.Controls.Add(this.lblAccountCount);
            this.pnlAccountsCard.Controls.Add(this.grdAccounts);
            this.pnlAccountsCard.BackColor = System.Drawing.Color.White;
            this.pnlAccountsCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAccountsCard.Location = new System.Drawing.Point(0, 0);
            this.pnlAccountsCard.Size = new System.Drawing.Size(926, 236);
            this.pnlAccountsCard.TabIndex = 0;
            this.pnlAccountsCard.Name = "pnlAccountsCard";
            //
            // lblAccountsHeadingBar
            //
            this.lblAccountsHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblAccountsHeadingBar.Location = new System.Drawing.Point(16, 12);
            this.lblAccountsHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblAccountsHeadingBar.TabIndex = 0;
            this.lblAccountsHeadingBar.Name = "lblAccountsHeadingBar";
            //
            // lblAccountsHeading
            //
            this.lblAccountsHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblAccountsHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblAccountsHeading.Location = new System.Drawing.Point(26, 8);
            this.lblAccountsHeading.Size = new System.Drawing.Size(400, 28);
            this.lblAccountsHeading.TabIndex = 1;
            this.lblAccountsHeading.Text = "Stored user accounts";
            this.lblAccountsHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAccountsHeading.Name = "lblAccountsHeading";
            //
            // lblAccountCount
            //
            this.lblAccountCount.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblAccountCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblAccountCount.Location = new System.Drawing.Point(560, 12);
            this.lblAccountCount.Size = new System.Drawing.Size(348, 20);
            this.lblAccountCount.TabIndex = 2;
            this.lblAccountCount.Text = "";
            this.lblAccountCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblAccountCount.Name = "lblAccountCount";
            //
            // grdAccounts
            //
            this.grdAccounts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colUserId,
            this.colUsername,
            this.colFullName,
            this.colRole,
            this.colAccountStatus});
            grdAccountsAltStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.grdAccounts.AlternatingRowsDefaultCellStyle = grdAccountsAltStyle;
            grdAccountsHeaderStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grdAccountsHeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            grdAccountsHeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            grdAccountsHeaderStyle.ForeColor = System.Drawing.Color.White;
            grdAccountsHeaderStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            grdAccountsHeaderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            grdAccountsHeaderStyle.SelectionForeColor = System.Drawing.Color.White;
            grdAccountsHeaderStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdAccounts.ColumnHeadersDefaultCellStyle = grdAccountsHeaderStyle;
            grdAccountsCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grdAccountsCellStyle.BackColor = System.Drawing.Color.White;
            grdAccountsCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            grdAccountsCellStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            grdAccountsCellStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            grdAccountsCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            grdAccountsCellStyle.SelectionForeColor = System.Drawing.Color.White;
            grdAccountsCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grdAccounts.DefaultCellStyle = grdAccountsCellStyle;
            this.grdAccounts.AccessibleName = "Stored user accounts";
            this.grdAccounts.AllowUserToAddRows = false;
            this.grdAccounts.AllowUserToDeleteRows = false;
            this.grdAccounts.AllowUserToResizeRows = false;
            this.grdAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdAccounts.BackgroundColor = System.Drawing.Color.White;
            this.grdAccounts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.grdAccounts.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grdAccounts.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.grdAccounts.ColumnHeadersHeight = 34;
            this.grdAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdAccounts.EnableHeadersVisualStyles = false;
            this.grdAccounts.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            this.grdAccounts.Location = new System.Drawing.Point(16, 44);
            this.grdAccounts.MultiSelect = false;
            this.grdAccounts.ReadOnly = true;
            this.grdAccounts.RowHeadersVisible = false;
            this.grdAccounts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdAccounts.Size = new System.Drawing.Size(892, 176);
            this.grdAccounts.TabIndex = 3;
            this.grdAccounts.Name = "grdAccounts";
            this.grdAccounts.RowTemplate.Height = 30;
            this.grdAccounts.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.grdAccounts_CellFormatting);
            this.grdAccounts.SelectionChanged += new System.EventHandler(this.grdAccounts_SelectionChanged);
            this.grdAccounts.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.grdAccounts_DataBindingComplete);
            //
            // colUserId
            //
            this.colUserId.DataPropertyName = "UserID";
            this.colUserId.FillWeight = 5F;
            this.colUserId.HeaderText = "User ID";
            this.colUserId.ReadOnly = true;
            this.colUserId.Visible = false;
            this.colUserId.Name = "colUserId";
            //
            // colUsername
            //
            this.colUsername.DataPropertyName = "Username";
            this.colUsername.FillWeight = 24F;
            this.colUsername.HeaderText = "Username";
            this.colUsername.ReadOnly = true;
            this.colUsername.Name = "colUsername";
            //
            // colFullName
            //
            this.colFullName.DataPropertyName = "FullName";
            this.colFullName.FillWeight = 40F;
            this.colFullName.HeaderText = "Full name";
            this.colFullName.ReadOnly = true;
            this.colFullName.Name = "colFullName";
            //
            // colRole
            //
            this.colRole.DataPropertyName = "Role";
            this.colRole.FillWeight = 20F;
            this.colRole.HeaderText = "Role";
            this.colRole.ReadOnly = true;
            this.colRole.Name = "colRole";
            //
            // colAccountStatus
            //
            this.colAccountStatus.DataPropertyName = "IsActive";
            this.colAccountStatus.FillWeight = 14F;
            this.colAccountStatus.HeaderText = "Status";
            this.colAccountStatus.ReadOnly = true;
            this.colAccountStatus.Name = "colAccountStatus";
            //
            // pnlAccountFormCard
            //
            this.pnlAccountFormCard.Controls.Add(this.lblAccountFormHeadingBar);
            this.pnlAccountFormCard.Controls.Add(this.lblAccountFormHeading);
            this.pnlAccountFormCard.Controls.Add(this.lblAccountRequiredNote);
            this.pnlAccountFormCard.Controls.Add(this.lblUsername);
            this.pnlAccountFormCard.Controls.Add(this.lblUsernameStar);
            this.pnlAccountFormCard.Controls.Add(this.txtUsername);
            this.pnlAccountFormCard.Controls.Add(this.lblUsernameError);
            this.pnlAccountFormCard.Controls.Add(this.lblFullName);
            this.pnlAccountFormCard.Controls.Add(this.lblFullNameStar);
            this.pnlAccountFormCard.Controls.Add(this.txtFullName);
            this.pnlAccountFormCard.Controls.Add(this.lblFullNameError);
            this.pnlAccountFormCard.Controls.Add(this.lblRole);
            this.pnlAccountFormCard.Controls.Add(this.lblRoleStar);
            this.pnlAccountFormCard.Controls.Add(this.cboRole);
            this.pnlAccountFormCard.Controls.Add(this.lblRoleError);
            this.pnlAccountFormCard.Controls.Add(this.chkAccountActive);
            this.pnlAccountFormCard.Controls.Add(this.pnlPassword);
            this.pnlAccountFormCard.Controls.Add(this.pnlAccountSummary);
            this.pnlAccountFormCard.BackColor = System.Drawing.Color.White;
            this.pnlAccountFormCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAccountFormCard.Location = new System.Drawing.Point(0, 248);
            this.pnlAccountFormCard.Size = new System.Drawing.Size(926, 279);
            this.pnlAccountFormCard.TabIndex = 1;
            this.pnlAccountFormCard.Name = "pnlAccountFormCard";
            //
            // lblAccountFormHeadingBar
            //
            this.lblAccountFormHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblAccountFormHeadingBar.Location = new System.Drawing.Point(16, 12);
            this.lblAccountFormHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblAccountFormHeadingBar.TabIndex = 0;
            this.lblAccountFormHeadingBar.Name = "lblAccountFormHeadingBar";
            //
            // lblAccountFormHeading
            //
            this.lblAccountFormHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblAccountFormHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblAccountFormHeading.Location = new System.Drawing.Point(26, 8);
            this.lblAccountFormHeading.Size = new System.Drawing.Size(400, 28);
            this.lblAccountFormHeading.TabIndex = 1;
            this.lblAccountFormHeading.Text = "Add or edit a user account";
            this.lblAccountFormHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAccountFormHeading.Name = "lblAccountFormHeading";
            //
            // lblAccountRequiredNote
            //
            this.lblAccountRequiredNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblAccountRequiredNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblAccountRequiredNote.Location = new System.Drawing.Point(700, 12);
            this.lblAccountRequiredNote.Size = new System.Drawing.Size(206, 18);
            this.lblAccountRequiredNote.TabIndex = 2;
            this.lblAccountRequiredNote.Text = "* indicates a required field.";
            this.lblAccountRequiredNote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblAccountRequiredNote.Name = "lblAccountRequiredNote";
            //
            // lblUsername
            //
            this.lblUsername.AutoSize = true;
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblUsername.Location = new System.Drawing.Point(20, 44);
            this.lblUsername.TabIndex = 3;
            this.lblUsername.Text = "Username";
            this.lblUsername.Name = "lblUsername";
            //
            // lblUsernameStar
            //
            this.lblUsernameStar.AutoSize = true;
            this.lblUsernameStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblUsernameStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblUsernameStar.Location = new System.Drawing.Point(102, 44);
            this.lblUsernameStar.Size = new System.Drawing.Size(14, 24);
            this.lblUsernameStar.TabIndex = 4;
            this.lblUsernameStar.Text = "*";
            this.lblUsernameStar.Name = "lblUsernameStar";
            //
            // txtUsername
            //
            this.txtUsername.AccessibleName = "Username (required, up to 50 characters, unique)";
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUsername.Location = new System.Drawing.Point(20, 70);
            this.txtUsername.MaxLength = 50;
            this.txtUsername.Size = new System.Drawing.Size(200, 25);
            this.txtUsername.TabIndex = 5;
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Leave += new System.EventHandler(this.txtUsername_Leave);
            //
            // lblUsernameError
            //
            this.lblUsernameError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblUsernameError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblUsernameError.Location = new System.Drawing.Point(20, 97);
            this.lblUsernameError.Size = new System.Drawing.Size(210, 16);
            this.lblUsernameError.TabIndex = 6;
            this.lblUsernameError.Text = "";
            this.lblUsernameError.Visible = false;
            this.lblUsernameError.Name = "lblUsernameError";
            //
            // lblFullName
            //
            this.lblFullName.AutoSize = true;
            this.lblFullName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFullName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFullName.Location = new System.Drawing.Point(240, 44);
            this.lblFullName.TabIndex = 7;
            this.lblFullName.Text = "Full name";
            this.lblFullName.Name = "lblFullName";
            //
            // lblFullNameStar
            //
            this.lblFullNameStar.AutoSize = true;
            this.lblFullNameStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFullNameStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblFullNameStar.Location = new System.Drawing.Point(323, 44);
            this.lblFullNameStar.Size = new System.Drawing.Size(14, 24);
            this.lblFullNameStar.TabIndex = 8;
            this.lblFullNameStar.Text = "*";
            this.lblFullNameStar.Name = "lblFullNameStar";
            //
            // txtFullName
            //
            this.txtFullName.AccessibleName = "Full name (required, up to 100 characters)";
            this.txtFullName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtFullName.Location = new System.Drawing.Point(240, 70);
            this.txtFullName.MaxLength = 100;
            this.txtFullName.Size = new System.Drawing.Size(280, 25);
            this.txtFullName.TabIndex = 9;
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Leave += new System.EventHandler(this.txtFullName_Leave);
            //
            // lblFullNameError
            //
            this.lblFullNameError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFullNameError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblFullNameError.Location = new System.Drawing.Point(240, 97);
            this.lblFullNameError.Size = new System.Drawing.Size(290, 16);
            this.lblFullNameError.TabIndex = 10;
            this.lblFullNameError.Text = "";
            this.lblFullNameError.Visible = false;
            this.lblFullNameError.Name = "lblFullNameError";
            //
            // lblRole
            //
            this.lblRole.AutoSize = true;
            this.lblRole.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblRole.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblRole.Location = new System.Drawing.Point(540, 44);
            this.lblRole.TabIndex = 11;
            this.lblRole.Text = "Role";
            this.lblRole.Name = "lblRole";
            //
            // lblRoleStar
            //
            this.lblRoleStar.AutoSize = true;
            this.lblRoleStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblRoleStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblRoleStar.Location = new System.Drawing.Point(579, 44);
            this.lblRoleStar.Size = new System.Drawing.Size(14, 24);
            this.lblRoleStar.TabIndex = 12;
            this.lblRoleStar.Text = "*";
            this.lblRoleStar.Name = "lblRoleStar";
            //
            // cboRole
            //
            this.cboRole.AccessibleName = "Role (required)";
            this.cboRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRole.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboRole.FormattingEnabled = true;
            this.cboRole.Location = new System.Drawing.Point(540, 70);
            this.cboRole.Size = new System.Drawing.Size(200, 25);
            this.cboRole.TabIndex = 13;
            this.cboRole.Name = "cboRole";
            this.cboRole.Leave += new System.EventHandler(this.cboRole_Leave);
            //
            // lblRoleError
            //
            this.lblRoleError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblRoleError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblRoleError.Location = new System.Drawing.Point(540, 97);
            this.lblRoleError.Size = new System.Drawing.Size(200, 16);
            this.lblRoleError.TabIndex = 14;
            this.lblRoleError.Text = "";
            this.lblRoleError.Visible = false;
            this.lblRoleError.Name = "lblRoleError";
            //
            // chkAccountActive
            //
            this.chkAccountActive.AccessibleName = "Active (unticked hides it from new advisory sessions)";
            this.chkAccountActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAccountActive.Checked = true;
            this.chkAccountActive.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkAccountActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.chkAccountActive.Location = new System.Drawing.Point(760, 70);
            this.chkAccountActive.Size = new System.Drawing.Size(140, 24);
            this.chkAccountActive.TabIndex = 15;
            this.chkAccountActive.Text = "Active";
            this.chkAccountActive.UseVisualStyleBackColor = true;
            this.chkAccountActive.Name = "chkAccountActive";
            //
            // pnlPassword
            //
            this.pnlPassword.Controls.Add(this.lblPasswordHeading);
            this.pnlPassword.Controls.Add(this.lblNewPassword);
            this.pnlPassword.Controls.Add(this.txtNewPassword);
            this.pnlPassword.Controls.Add(this.lblNewPasswordError);
            this.pnlPassword.Controls.Add(this.lblConfirmPassword);
            this.pnlPassword.Controls.Add(this.txtConfirmPassword);
            this.pnlPassword.Controls.Add(this.lblConfirmPasswordError);
            this.pnlPassword.Controls.Add(this.lblPasswordHint);
            this.pnlPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.pnlPassword.Location = new System.Drawing.Point(20, 118);
            this.pnlPassword.Size = new System.Drawing.Size(560, 150);
            this.pnlPassword.TabIndex = 16;
            this.pnlPassword.Name = "pnlPassword";
            //
            // lblPasswordHeading
            //
            this.lblPasswordHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblPasswordHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblPasswordHeading.Location = new System.Drawing.Point(12, 6);
            this.lblPasswordHeading.Size = new System.Drawing.Size(536, 20);
            this.lblPasswordHeading.TabIndex = 0;
            this.lblPasswordHeading.Text = "Password \u2014 required for a new account; optional on update (resets it)";
            this.lblPasswordHeading.Name = "lblPasswordHeading";
            //
            // lblNewPassword
            //
            this.lblNewPassword.AutoSize = true;
            this.lblNewPassword.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblNewPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblNewPassword.Location = new System.Drawing.Point(12, 30);
            this.lblNewPassword.TabIndex = 1;
            this.lblNewPassword.Text = "New password";
            this.lblNewPassword.Name = "lblNewPassword";
            //
            // txtNewPassword
            //
            this.txtNewPassword.AccessibleName = "New password";
            this.txtNewPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNewPassword.Location = new System.Drawing.Point(12, 56);
            this.txtNewPassword.Size = new System.Drawing.Size(250, 25);
            this.txtNewPassword.TabIndex = 2;
            this.txtNewPassword.UseSystemPasswordChar = true;
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.Leave += new System.EventHandler(this.txtNewPassword_Leave);
            //
            // lblNewPasswordError
            //
            this.lblNewPasswordError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblNewPasswordError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblNewPasswordError.Location = new System.Drawing.Point(12, 83);
            this.lblNewPasswordError.Size = new System.Drawing.Size(260, 16);
            this.lblNewPasswordError.TabIndex = 3;
            this.lblNewPasswordError.Text = "";
            this.lblNewPasswordError.Visible = false;
            this.lblNewPasswordError.Name = "lblNewPasswordError";
            //
            // lblConfirmPassword
            //
            this.lblConfirmPassword.AutoSize = true;
            this.lblConfirmPassword.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblConfirmPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblConfirmPassword.Location = new System.Drawing.Point(282, 30);
            this.lblConfirmPassword.TabIndex = 4;
            this.lblConfirmPassword.Text = "Confirm password";
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            //
            // txtConfirmPassword
            //
            this.txtConfirmPassword.AccessibleName = "Confirm new password";
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtConfirmPassword.Location = new System.Drawing.Point(282, 56);
            this.txtConfirmPassword.Size = new System.Drawing.Size(250, 25);
            this.txtConfirmPassword.TabIndex = 5;
            this.txtConfirmPassword.UseSystemPasswordChar = true;
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.Leave += new System.EventHandler(this.txtConfirmPassword_Leave);
            //
            // lblConfirmPasswordError
            //
            this.lblConfirmPasswordError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblConfirmPasswordError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblConfirmPasswordError.Location = new System.Drawing.Point(282, 83);
            this.lblConfirmPasswordError.Size = new System.Drawing.Size(270, 16);
            this.lblConfirmPasswordError.TabIndex = 6;
            this.lblConfirmPasswordError.Text = "";
            this.lblConfirmPasswordError.Visible = false;
            this.lblConfirmPasswordError.Name = "lblConfirmPasswordError";
            //
            // lblPasswordHint
            //
            this.lblPasswordHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblPasswordHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblPasswordHint.Location = new System.Drawing.Point(12, 104);
            this.lblPasswordHint.Size = new System.Drawing.Size(536, 36);
            this.lblPasswordHint.TabIndex = 7;
            this.lblPasswordHint.Text = "At least 8 characters. Leave both boxes blank on update to keep the current password. Passwords are stored only as a salted hash and are never shown.";
            this.lblPasswordHint.Name = "lblPasswordHint";
            //
            // pnlAccountSummary
            //
            this.pnlAccountSummary.Controls.Add(this.lblAccountSummary);
            this.pnlAccountSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.pnlAccountSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAccountSummary.Location = new System.Drawing.Point(600, 118);
            this.pnlAccountSummary.Size = new System.Drawing.Size(306, 150);
            this.pnlAccountSummary.TabIndex = 17;
            this.pnlAccountSummary.Visible = false;
            this.pnlAccountSummary.Name = "pnlAccountSummary";
            //
            // lblAccountSummary
            //
            this.lblAccountSummary.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAccountSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblAccountSummary.Location = new System.Drawing.Point(12, 6);
            this.lblAccountSummary.Size = new System.Drawing.Size(280, 138);
            this.lblAccountSummary.TabIndex = 0;
            this.lblAccountSummary.Text = "";
            this.lblAccountSummary.Name = "lblAccountSummary";
            //
            // UserAccountsTab
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.Size = new System.Drawing.Size(926, 527);
            this.Controls.Add(this.pnlAccountsCard);
            this.Controls.Add(this.pnlAccountFormCard);
            this.Name = "UserAccountsTab";
            this.pnlAccountsCard.ResumeLayout(false);
            this.pnlAccountsCard.PerformLayout();
            this.lblAccountsHeadingBar.ResumeLayout(false);
            this.lblAccountsHeadingBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdAccounts)).EndInit();
            this.pnlAccountFormCard.ResumeLayout(false);
            this.pnlAccountFormCard.PerformLayout();
            this.lblAccountFormHeadingBar.ResumeLayout(false);
            this.lblAccountFormHeadingBar.PerformLayout();
            this.pnlPassword.ResumeLayout(false);
            this.pnlPassword.PerformLayout();
            this.pnlAccountSummary.ResumeLayout(false);
            this.pnlAccountSummary.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlAccountsCard;
        private System.Windows.Forms.Panel lblAccountsHeadingBar;
        private System.Windows.Forms.Label lblAccountsHeading;
        private System.Windows.Forms.Label lblAccountCount;
        private System.Windows.Forms.DataGridView grdAccounts;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUserId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUsername;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFullName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRole;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAccountStatus;
        private System.Windows.Forms.Panel pnlAccountFormCard;
        private System.Windows.Forms.Panel lblAccountFormHeadingBar;
        private System.Windows.Forms.Label lblAccountFormHeading;
        private System.Windows.Forms.Label lblAccountRequiredNote;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblUsernameStar;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblUsernameError;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.Label lblFullNameStar;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.Label lblFullNameError;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Label lblRoleStar;
        private System.Windows.Forms.ComboBox cboRole;
        private System.Windows.Forms.Label lblRoleError;
        private System.Windows.Forms.CheckBox chkAccountActive;
        private System.Windows.Forms.Panel pnlPassword;
        private System.Windows.Forms.Label lblPasswordHeading;
        private System.Windows.Forms.Label lblNewPassword;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Label lblNewPasswordError;
        private System.Windows.Forms.Label lblConfirmPassword;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.Label lblConfirmPasswordError;
        private System.Windows.Forms.Label lblPasswordHint;
        private System.Windows.Forms.Panel pnlAccountSummary;
        private System.Windows.Forms.Label lblAccountSummary;
    }
}
