namespace FuturePath.PresentationLayer
{
    partial class AdminDashboardForm
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
            this.pnlBody = new System.Windows.Forms.Panel();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblIntro = new System.Windows.Forms.Label();
            this.pnlReferenceDataCard = new System.Windows.Forms.Panel();
            this.lblReferenceDataHeadingBar = new System.Windows.Forms.Panel();
            this.lblReferenceDataHeading = new System.Windows.Forms.Label();
            this.lblReferenceDataText = new System.Windows.Forms.Label();
            this.btnMaintainReferenceData = new System.Windows.Forms.Button();
            this.pnlReportsCard = new System.Windows.Forms.Panel();
            this.lblReportsHeadingBar = new System.Windows.Forms.Panel();
            this.lblReportsHeading = new System.Windows.Forms.Label();
            this.lblReportsText = new System.Windows.Forms.Label();
            this.btnGenerateReports = new System.Windows.Forms.Button();
            this.lblNote = new System.Windows.Forms.Label();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblBadge = new System.Windows.Forms.Label();
            this.pnlTitleRule = new System.Windows.Forms.Panel();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.pnlActionRule = new System.Windows.Forms.Panel();
            this.lblActionHint = new System.Windows.Forms.Label();
            this.pnlRail = new System.Windows.Forms.Panel();
            this.lblBrand = new System.Windows.Forms.Label();
            this.pnlBrandRule = new System.Windows.Forms.Panel();
            this.btnNavDashboard = new System.Windows.Forms.Button();
            this.btnNavSessions = new System.Windows.Forms.Button();
            this.btnNavReferenceData = new System.Windows.Forms.Button();
            this.btnNavReports = new System.Windows.Forms.Button();
            this.btnNavSignOut = new System.Windows.Forms.Button();
            this.lblRailFooter = new System.Windows.Forms.Label();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlBody.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlReferenceDataCard.SuspendLayout();
            this.lblReferenceDataHeadingBar.SuspendLayout();
            this.pnlReportsCard.SuspendLayout();
            this.lblReportsHeadingBar.SuspendLayout();
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
            this.pnlContent.Controls.Add(this.lblWelcome);
            this.pnlContent.Controls.Add(this.lblIntro);
            this.pnlContent.Controls.Add(this.pnlReferenceDataCard);
            this.pnlContent.Controls.Add(this.pnlReportsCard);
            this.pnlContent.Controls.Add(this.lblNote);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(20, 12);
            this.pnlContent.Size = new System.Drawing.Size(734, 483);
            this.pnlContent.TabIndex = 0;
            this.pnlContent.Name = "pnlContent";
            //
            // lblWelcome
            //
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblWelcome.Location = new System.Drawing.Point(0, 0);
            this.lblWelcome.Size = new System.Drawing.Size(734, 28);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Welcome";
            this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblWelcome.Name = "lblWelcome";
            //
            // lblIntro
            //
            this.lblIntro.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblIntro.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblIntro.Location = new System.Drawing.Point(0, 32);
            this.lblIntro.Size = new System.Drawing.Size(734, 44);
            this.lblIntro.TabIndex = 1;
            this.lblIntro.Text = "Choose what you want to do. Reference data drives every eligibility check, so each change is confirmed and applies to new advisory sessions only.";
            this.lblIntro.Name = "lblIntro";
            //
            // pnlReferenceDataCard
            //
            this.pnlReferenceDataCard.Controls.Add(this.lblReferenceDataHeadingBar);
            this.pnlReferenceDataCard.Controls.Add(this.lblReferenceDataHeading);
            this.pnlReferenceDataCard.Controls.Add(this.lblReferenceDataText);
            this.pnlReferenceDataCard.Controls.Add(this.btnMaintainReferenceData);
            this.pnlReferenceDataCard.BackColor = System.Drawing.Color.White;
            this.pnlReferenceDataCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlReferenceDataCard.Location = new System.Drawing.Point(0, 84);
            this.pnlReferenceDataCard.Size = new System.Drawing.Size(357, 230);
            this.pnlReferenceDataCard.TabIndex = 2;
            this.pnlReferenceDataCard.Name = "pnlReferenceDataCard";
            //
            // lblReferenceDataHeadingBar
            //
            this.lblReferenceDataHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblReferenceDataHeadingBar.Location = new System.Drawing.Point(16, 18);
            this.lblReferenceDataHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblReferenceDataHeadingBar.TabIndex = 0;
            this.lblReferenceDataHeadingBar.Name = "lblReferenceDataHeadingBar";
            //
            // lblReferenceDataHeading
            //
            this.lblReferenceDataHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblReferenceDataHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblReferenceDataHeading.Location = new System.Drawing.Point(26, 14);
            this.lblReferenceDataHeading.Size = new System.Drawing.Size(300, 28);
            this.lblReferenceDataHeading.TabIndex = 1;
            this.lblReferenceDataHeading.Text = "Reference data";
            this.lblReferenceDataHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblReferenceDataHeading.Name = "lblReferenceDataHeading";
            //
            // lblReferenceDataText
            //
            this.lblReferenceDataText.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblReferenceDataText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblReferenceDataText.Location = new System.Drawing.Point(20, 52);
            this.lblReferenceDataText.Size = new System.Drawing.Size(317, 110);
            this.lblReferenceDataText.TabIndex = 2;
            this.lblReferenceDataText.Text = "Maintain faculties and programmes, degree requirements, funding options, interest categories, the questionnaire and staff user accounts.";
            this.lblReferenceDataText.Name = "lblReferenceDataText";
            //
            // btnMaintainReferenceData
            //
            this.btnMaintainReferenceData.AccessibleName = "Maintain reference data";
            this.btnMaintainReferenceData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.btnMaintainReferenceData.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMaintainReferenceData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaintainReferenceData.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnMaintainReferenceData.ForeColor = System.Drawing.Color.White;
            this.btnMaintainReferenceData.Location = new System.Drawing.Point(20, 172);
            this.btnMaintainReferenceData.Size = new System.Drawing.Size(240, 40);
            this.btnMaintainReferenceData.TabIndex = 3;
            this.btnMaintainReferenceData.Text = "Maintain reference data";
            this.btnMaintainReferenceData.UseVisualStyleBackColor = false;
            this.btnMaintainReferenceData.Name = "btnMaintainReferenceData";
            this.btnMaintainReferenceData.FlatAppearance.BorderSize = 0;
            this.btnMaintainReferenceData.Click += new System.EventHandler(this.btnMaintainReferenceData_Click);
            //
            // pnlReportsCard
            //
            this.pnlReportsCard.Controls.Add(this.lblReportsHeadingBar);
            this.pnlReportsCard.Controls.Add(this.lblReportsHeading);
            this.pnlReportsCard.Controls.Add(this.lblReportsText);
            this.pnlReportsCard.Controls.Add(this.btnGenerateReports);
            this.pnlReportsCard.BackColor = System.Drawing.Color.White;
            this.pnlReportsCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlReportsCard.Location = new System.Drawing.Point(377, 84);
            this.pnlReportsCard.Size = new System.Drawing.Size(357, 230);
            this.pnlReportsCard.TabIndex = 3;
            this.pnlReportsCard.Name = "pnlReportsCard";
            //
            // lblReportsHeadingBar
            //
            this.lblReportsHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblReportsHeadingBar.Location = new System.Drawing.Point(16, 18);
            this.lblReportsHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblReportsHeadingBar.TabIndex = 0;
            this.lblReportsHeadingBar.Name = "lblReportsHeadingBar";
            //
            // lblReportsHeading
            //
            this.lblReportsHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblReportsHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblReportsHeading.Location = new System.Drawing.Point(26, 14);
            this.lblReportsHeading.Size = new System.Drawing.Size(300, 28);
            this.lblReportsHeading.TabIndex = 1;
            this.lblReportsHeading.Text = "Reports";
            this.lblReportsHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblReportsHeading.Name = "lblReportsHeading";
            //
            // lblReportsText
            //
            this.lblReportsText.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblReportsText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblReportsText.Location = new System.Drawing.Point(20, 52);
            this.lblReportsText.Size = new System.Drawing.Size(317, 110);
            this.lblReportsText.TabIndex = 2;
            this.lblReportsText.Text = "Generate the Degree Interest and Eligibility Report (FP-DIE) and the Learners Not Currently Eligible exception report (FP-LER), or regenerate a learner's FP-LSR.";
            this.lblReportsText.Name = "lblReportsText";
            //
            // btnGenerateReports
            //
            this.btnGenerateReports.AccessibleName = "Generate reports";
            this.btnGenerateReports.BackColor = System.Drawing.Color.White;
            this.btnGenerateReports.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerateReports.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerateReports.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnGenerateReports.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnGenerateReports.Location = new System.Drawing.Point(20, 172);
            this.btnGenerateReports.Size = new System.Drawing.Size(200, 40);
            this.btnGenerateReports.TabIndex = 3;
            this.btnGenerateReports.Text = "Generate reports";
            this.btnGenerateReports.UseVisualStyleBackColor = false;
            this.btnGenerateReports.Name = "btnGenerateReports";
            this.btnGenerateReports.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.btnGenerateReports.Click += new System.EventHandler(this.btnGenerateReports_Click);
            //
            // lblNote
            //
            this.lblNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblNote.Location = new System.Drawing.Point(0, 334);
            this.lblNote.Size = new System.Drawing.Size(734, 36);
            this.lblNote.TabIndex = 4;
            this.lblNote.Text = "Records are deactivated, never deleted, so past advisory sessions and reports stay reproducible. Staff passwords are reset on UI 13.6 User accounts and stored only as salted hashes.";
            this.lblNote.Name = "lblNote";
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
            this.lblTitle.Text = "Administrator dashboard";
            this.lblTitle.Name = "lblTitle";
            //
            // lblSubtitle
            //
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.lblSubtitle.Location = new System.Drawing.Point(24, 54);
            this.lblSubtitle.Size = new System.Drawing.Size(714, 18);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Maintain reference data and generate reports";
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
            this.lblBadge.Text = "ADMINISTRATOR";
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
            // lblActionHint
            //
            this.lblActionHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblActionHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblActionHint.Location = new System.Drawing.Point(20, 12);
            this.lblActionHint.Size = new System.Drawing.Size(724, 40);
            this.lblActionHint.TabIndex = 1;
            this.lblActionHint.Text = "Sign out from the rail when you finish. Learners never log in.";
            this.lblActionHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblActionHint.Name = "lblActionHint";
            //
            // pnlRail
            //
            this.pnlRail.Controls.Add(this.lblBrand);
            this.pnlRail.Controls.Add(this.pnlBrandRule);
            this.pnlRail.Controls.Add(this.btnNavDashboard);
            this.pnlRail.Controls.Add(this.btnNavSessions);
            this.pnlRail.Controls.Add(this.btnNavReferenceData);
            this.pnlRail.Controls.Add(this.btnNavReports);
            this.pnlRail.Controls.Add(this.btnNavSignOut);
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
            // btnNavDashboard
            //
            this.btnNavDashboard.AccessibleName = "Dashboard";
            this.btnNavDashboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(64)))), ((int)(((byte)(104)))));
            this.btnNavDashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavDashboard.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnNavDashboard.ForeColor = System.Drawing.Color.White;
            this.btnNavDashboard.Location = new System.Drawing.Point(0, 84);
            this.btnNavDashboard.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.btnNavDashboard.Size = new System.Drawing.Size(210, 36);
            this.btnNavDashboard.TabIndex = 2;
            this.btnNavDashboard.Text = "Dashboard";
            this.btnNavDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavDashboard.UseVisualStyleBackColor = false;
            this.btnNavDashboard.Name = "btnNavDashboard";
            this.btnNavDashboard.FlatAppearance.BorderSize = 0;
            this.btnNavDashboard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(64)))), ((int)(((byte)(104)))));
            this.btnNavDashboard.Click += new System.EventHandler(this.btnNavDashboard_Click);
            //
            // btnNavSessions
            //
            this.btnNavSessions.AccessibleName = "Sessions";
            this.btnNavSessions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.btnNavSessions.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavSessions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavSessions.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNavSessions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.btnNavSessions.Location = new System.Drawing.Point(0, 120);
            this.btnNavSessions.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.btnNavSessions.Size = new System.Drawing.Size(210, 36);
            this.btnNavSessions.TabIndex = 3;
            this.btnNavSessions.Text = "Sessions";
            this.btnNavSessions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavSessions.UseVisualStyleBackColor = false;
            this.btnNavSessions.Name = "btnNavSessions";
            this.btnNavSessions.FlatAppearance.BorderSize = 0;
            this.btnNavSessions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(64)))), ((int)(((byte)(104)))));
            this.btnNavSessions.Click += new System.EventHandler(this.btnNavSessions_Click);
            //
            // btnNavReferenceData
            //
            this.btnNavReferenceData.AccessibleName = "Reference data";
            this.btnNavReferenceData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.btnNavReferenceData.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavReferenceData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavReferenceData.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNavReferenceData.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.btnNavReferenceData.Location = new System.Drawing.Point(0, 120);
            this.btnNavReferenceData.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.btnNavReferenceData.Size = new System.Drawing.Size(210, 36);
            this.btnNavReferenceData.TabIndex = 4;
            this.btnNavReferenceData.Text = "Reference data";
            this.btnNavReferenceData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavReferenceData.UseVisualStyleBackColor = false;
            this.btnNavReferenceData.Name = "btnNavReferenceData";
            this.btnNavReferenceData.FlatAppearance.BorderSize = 0;
            this.btnNavReferenceData.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(64)))), ((int)(((byte)(104)))));
            this.btnNavReferenceData.Click += new System.EventHandler(this.btnNavReferenceData_Click);
            //
            // btnNavReports
            //
            this.btnNavReports.AccessibleName = "Reports";
            this.btnNavReports.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.btnNavReports.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavReports.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavReports.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNavReports.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.btnNavReports.Location = new System.Drawing.Point(0, 156);
            this.btnNavReports.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.btnNavReports.Size = new System.Drawing.Size(210, 36);
            this.btnNavReports.TabIndex = 5;
            this.btnNavReports.Text = "Reports";
            this.btnNavReports.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavReports.UseVisualStyleBackColor = false;
            this.btnNavReports.Name = "btnNavReports";
            this.btnNavReports.FlatAppearance.BorderSize = 0;
            this.btnNavReports.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(64)))), ((int)(((byte)(104)))));
            this.btnNavReports.Click += new System.EventHandler(this.btnNavReports_Click);
            //
            // btnNavSignOut
            //
            this.btnNavSignOut.AccessibleName = "Sign out";
            this.btnNavSignOut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.btnNavSignOut.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavSignOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavSignOut.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNavSignOut.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.btnNavSignOut.Location = new System.Drawing.Point(0, 192);
            this.btnNavSignOut.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.btnNavSignOut.Size = new System.Drawing.Size(210, 36);
            this.btnNavSignOut.TabIndex = 6;
            this.btnNavSignOut.Text = "Sign out";
            this.btnNavSignOut.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavSignOut.UseVisualStyleBackColor = false;
            this.btnNavSignOut.Name = "btnNavSignOut";
            this.btnNavSignOut.FlatAppearance.BorderSize = 0;
            this.btnNavSignOut.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(64)))), ((int)(((byte)(104)))));
            this.btnNavSignOut.Click += new System.EventHandler(this.btnNavSignOut_Click);
            //
            // lblRailFooter
            //
            this.lblRailFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblRailFooter.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblRailFooter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.lblRailFooter.Location = new System.Drawing.Point(0, 575);
            this.lblRailFooter.Padding = new System.Windows.Forms.Padding(16, 0, 8, 0);
            this.lblRailFooter.Size = new System.Drawing.Size(210, 80);
            this.lblRailFooter.TabIndex = 7;
            this.lblRailFooter.Text = "";
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
            // AdminDashboardForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(984, 681);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Future Path \u2014 Administrator dashboard";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlTitle);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlRail);
            this.Controls.Add(this.pnlStatus);
            this.Name = "AdminDashboardForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AdminDashboardForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AdminDashboardForm_FormClosed);
            this.pnlBody.ResumeLayout(false);
            this.pnlBody.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.pnlReferenceDataCard.ResumeLayout(false);
            this.pnlReferenceDataCard.PerformLayout();
            this.lblReferenceDataHeadingBar.ResumeLayout(false);
            this.lblReferenceDataHeadingBar.PerformLayout();
            this.pnlReportsCard.ResumeLayout(false);
            this.pnlReportsCard.PerformLayout();
            this.lblReportsHeadingBar.ResumeLayout(false);
            this.lblReportsHeadingBar.PerformLayout();
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
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblIntro;
        private System.Windows.Forms.Panel pnlReferenceDataCard;
        private System.Windows.Forms.Panel lblReferenceDataHeadingBar;
        private System.Windows.Forms.Label lblReferenceDataHeading;
        private System.Windows.Forms.Label lblReferenceDataText;
        private System.Windows.Forms.Button btnMaintainReferenceData;
        private System.Windows.Forms.Panel pnlReportsCard;
        private System.Windows.Forms.Panel lblReportsHeadingBar;
        private System.Windows.Forms.Label lblReportsHeading;
        private System.Windows.Forms.Label lblReportsText;
        private System.Windows.Forms.Button btnGenerateReports;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.Panel pnlTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblBadge;
        private System.Windows.Forms.Panel pnlTitleRule;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Panel pnlActionRule;
        private System.Windows.Forms.Label lblActionHint;
        private System.Windows.Forms.Panel pnlRail;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.Panel pnlBrandRule;
        private System.Windows.Forms.Button btnNavDashboard;
        private System.Windows.Forms.Button btnNavSessions;
        private System.Windows.Forms.Button btnNavReferenceData;
        private System.Windows.Forms.Button btnNavReports;
        private System.Windows.Forms.Button btnNavSignOut;
        private System.Windows.Forms.Label lblRailFooter;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblStatus;
    }
}
