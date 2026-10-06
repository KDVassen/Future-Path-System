namespace FuturePath.PresentationLayer
{
    partial class SessionsDashboardForm
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
            this.tabSessions = new System.Windows.Forms.TabControl();
            this.tpLearnerProfile = new System.Windows.Forms.TabPage();
            this.learnerProfileTab = new FuturePath.PresentationLayer.LearnerProfileTab();
            this.tpAdvisorySession = new System.Windows.Forms.TabPage();
            this.advisorySessionTab = new FuturePath.PresentationLayer.AdvisorySessionTab();
            this.tpAdvisoryReports = new System.Windows.Forms.TabPage();
            this.advisoryReportsTab = new FuturePath.PresentationLayer.AdvisoryReportsTab();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblBadge = new System.Windows.Forms.Label();
            this.pnlTitleRule = new System.Windows.Forms.Panel();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.pnlActionRule = new System.Windows.Forms.Panel();
            this.btnPrimaryAction = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
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
            this.tabSessions.SuspendLayout();
            this.tpLearnerProfile.SuspendLayout();
            this.tpAdvisorySession.SuspendLayout();
            this.tpAdvisoryReports.SuspendLayout();
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
            this.pnlBody.Size = new System.Drawing.Size(974, 587);
            this.pnlBody.TabIndex = 0;
            this.pnlBody.Name = "pnlBody";
            //
            // pnlContent
            //
            this.pnlContent.Controls.Add(this.tabSessions);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(20, 12);
            this.pnlContent.Size = new System.Drawing.Size(934, 563);
            this.pnlContent.TabIndex = 0;
            this.pnlContent.Name = "pnlContent";
            //
            // tabSessions
            //
            this.tabSessions.Controls.Add(this.tpLearnerProfile);
            this.tabSessions.Controls.Add(this.tpAdvisorySession);
            this.tabSessions.Controls.Add(this.tpAdvisoryReports);
            this.tabSessions.AccessibleName = "Sessions dashboard tabs";
            this.tabSessions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSessions.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.tabSessions.ItemSize = new System.Drawing.Size(210, 30);
            this.tabSessions.Location = new System.Drawing.Point(0, 0);
            this.tabSessions.Size = new System.Drawing.Size(934, 563);
            this.tabSessions.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabSessions.TabIndex = 0;
            this.tabSessions.Name = "tabSessions";
            this.tabSessions.SelectedIndexChanged += new System.EventHandler(this.tabSessions_SelectedIndexChanged);
            //
            // tpLearnerProfile
            //
            this.tpLearnerProfile.Controls.Add(this.learnerProfileTab);
            this.tpLearnerProfile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.tpLearnerProfile.Location = new System.Drawing.Point(4, 34);
            this.tpLearnerProfile.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tpLearnerProfile.Size = new System.Drawing.Size(926, 527);
            this.tpLearnerProfile.TabIndex = 0;
            this.tpLearnerProfile.Text = "Learner profile \u2014 10.1a";
            this.tpLearnerProfile.UseVisualStyleBackColor = false;
            this.tpLearnerProfile.Name = "tpLearnerProfile";
            //
            // learnerProfileTab
            //
            this.learnerProfileTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.learnerProfileTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.learnerProfileTab.Location = new System.Drawing.Point(0, 0);
            this.learnerProfileTab.Size = new System.Drawing.Size(926, 527);
            this.learnerProfileTab.TabIndex = 0;
            this.learnerProfileTab.Name = "learnerProfileTab";
            //
            // tpAdvisorySession
            //
            this.tpAdvisorySession.Controls.Add(this.advisorySessionTab);
            this.tpAdvisorySession.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.tpAdvisorySession.Location = new System.Drawing.Point(4, 34);
            this.tpAdvisorySession.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tpAdvisorySession.Size = new System.Drawing.Size(926, 527);
            this.tpAdvisorySession.TabIndex = 1;
            this.tpAdvisorySession.Text = "Advisory session \u2014 10.2b";
            this.tpAdvisorySession.UseVisualStyleBackColor = false;
            this.tpAdvisorySession.Name = "tpAdvisorySession";
            //
            // advisorySessionTab
            //
            this.advisorySessionTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.advisorySessionTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advisorySessionTab.Location = new System.Drawing.Point(0, 0);
            this.advisorySessionTab.Size = new System.Drawing.Size(926, 527);
            this.advisorySessionTab.TabIndex = 0;
            this.advisorySessionTab.Name = "advisorySessionTab";
            //
            // tpAdvisoryReports
            //
            this.tpAdvisoryReports.Controls.Add(this.advisoryReportsTab);
            this.tpAdvisoryReports.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.tpAdvisoryReports.Location = new System.Drawing.Point(4, 34);
            this.tpAdvisoryReports.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tpAdvisoryReports.Size = new System.Drawing.Size(926, 527);
            this.tpAdvisoryReports.TabIndex = 2;
            this.tpAdvisoryReports.Text = "Advisory reports \u2014 10.3c";
            this.tpAdvisoryReports.UseVisualStyleBackColor = false;
            this.tpAdvisoryReports.Name = "tpAdvisoryReports";
            //
            // advisoryReportsTab
            //
            this.advisoryReportsTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.advisoryReportsTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advisoryReportsTab.Location = new System.Drawing.Point(0, 0);
            this.advisoryReportsTab.Size = new System.Drawing.Size(926, 527);
            this.advisoryReportsTab.TabIndex = 0;
            this.advisoryReportsTab.Name = "advisoryReportsTab";
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
            this.lblTitle.Text = "Sessions dashboard";
            this.lblTitle.Name = "lblTitle";
            //
            // lblSubtitle
            //
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.lblSubtitle.Location = new System.Drawing.Point(24, 54);
            this.lblSubtitle.Size = new System.Drawing.Size(914, 18);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Find an advisory session and open or export its summary";
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
            this.lblBadge.Text = "CAREER ADVISOR";
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
            this.pnlActions.Controls.Add(this.btnPrimaryAction);
            this.pnlActions.Controls.Add(this.btnClose);
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
            // btnPrimaryAction
            //
            this.btnPrimaryAction.AccessibleName = "Open advisory summary";
            this.btnPrimaryAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrimaryAction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.btnPrimaryAction.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrimaryAction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrimaryAction.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnPrimaryAction.ForeColor = System.Drawing.Color.White;
            this.btnPrimaryAction.Location = new System.Drawing.Point(724, 12);
            this.btnPrimaryAction.Size = new System.Drawing.Size(230, 40);
            this.btnPrimaryAction.TabIndex = 1;
            this.btnPrimaryAction.Text = "Open advisory summary";
            this.btnPrimaryAction.UseVisualStyleBackColor = false;
            this.btnPrimaryAction.Name = "btnPrimaryAction";
            this.btnPrimaryAction.FlatAppearance.BorderSize = 0;
            this.btnPrimaryAction.Click += new System.EventHandler(this.btnPrimaryAction_Click);
            //
            // btnClose
            //
            this.btnClose.AccessibleName = "Close";
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.White;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnClose.Location = new System.Drawing.Point(584, 12);
            this.btnClose.Size = new System.Drawing.Size(130, 40);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Name = "btnClose";
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // lblActionHint
            //
            this.lblActionHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblActionHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblActionHint.Location = new System.Drawing.Point(20, 12);
            this.lblActionHint.Size = new System.Drawing.Size(544, 40);
            this.lblActionHint.TabIndex = 3;
            this.lblActionHint.Text = "Read-only \u00B7 search on any field, none are required.";
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
            // btnNavDashboard
            //
            this.btnNavDashboard.AccessibleName = "Dashboard";
            this.btnNavDashboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.btnNavDashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavDashboard.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNavDashboard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
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
            this.btnNavSessions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(64)))), ((int)(((byte)(104)))));
            this.btnNavSessions.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavSessions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavSessions.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnNavSessions.ForeColor = System.Drawing.Color.White;
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
            this.lblRailFooter.Location = new System.Drawing.Point(0, 655);
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
            // SessionsDashboardForm
            //
            this.AcceptButton = this.btnPrimaryAction;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Future Path \u2014 Sessions dashboard";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlTitle);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlRail);
            this.Controls.Add(this.pnlStatus);
            this.Name = "SessionsDashboardForm";
            this.Load += new System.EventHandler(this.SessionsDashboardForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SessionsDashboardForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SessionsDashboardForm_FormClosed);
            this.pnlBody.ResumeLayout(false);
            this.pnlBody.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.tabSessions.ResumeLayout(false);
            this.tabSessions.PerformLayout();
            this.tpLearnerProfile.ResumeLayout(false);
            this.tpLearnerProfile.PerformLayout();
            this.tpAdvisorySession.ResumeLayout(false);
            this.tpAdvisorySession.PerformLayout();
            this.tpAdvisoryReports.ResumeLayout(false);
            this.tpAdvisoryReports.PerformLayout();
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
        private System.Windows.Forms.TabControl tabSessions;
        private System.Windows.Forms.TabPage tpLearnerProfile;
        private FuturePath.PresentationLayer.LearnerProfileTab learnerProfileTab;
        private System.Windows.Forms.TabPage tpAdvisorySession;
        private FuturePath.PresentationLayer.AdvisorySessionTab advisorySessionTab;
        private System.Windows.Forms.TabPage tpAdvisoryReports;
        private FuturePath.PresentationLayer.AdvisoryReportsTab advisoryReportsTab;
        private System.Windows.Forms.Panel pnlTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblBadge;
        private System.Windows.Forms.Panel pnlTitleRule;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Panel pnlActionRule;
        private System.Windows.Forms.Button btnPrimaryAction;
        private System.Windows.Forms.Button btnClose;
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
