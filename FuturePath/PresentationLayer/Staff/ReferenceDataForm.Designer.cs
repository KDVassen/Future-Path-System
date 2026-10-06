namespace FuturePath.PresentationLayer
{
    partial class ReferenceDataForm
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
            this.tabReference = new System.Windows.Forms.TabControl();
            this.tabUniversity = new System.Windows.Forms.TabPage();
            this.ucUniversityInfo = new FuturePath.PresentationLayer.UniversityInfoTab();
            this.tabRequirements = new System.Windows.Forms.TabPage();
            this.ucDegreeRequirements = new FuturePath.PresentationLayer.DegreeRequirementsTab();
            this.tabFunding = new System.Windows.Forms.TabPage();
            this.ucFunding = new FuturePath.PresentationLayer.FundingTab();
            this.tabInterests = new System.Windows.Forms.TabPage();
            this.ucInterestCategories = new FuturePath.PresentationLayer.InterestCategoriesTab();
            this.tabQuestionnaire = new System.Windows.Forms.TabPage();
            this.ucQuestionnaire = new FuturePath.PresentationLayer.QuestionnaireTab();
            this.tabUserAccounts = new System.Windows.Forms.TabPage();
            this.ucUserAccounts = new FuturePath.PresentationLayer.UserAccountsTab();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblBadge = new System.Windows.Forms.Label();
            this.pnlTitleRule = new System.Windows.Forms.Panel();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.pnlActionRule = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDeactivate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
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
            this.tabReference.SuspendLayout();
            this.tabUniversity.SuspendLayout();
            this.tabRequirements.SuspendLayout();
            this.tabFunding.SuspendLayout();
            this.tabInterests.SuspendLayout();
            this.tabQuestionnaire.SuspendLayout();
            this.tabUserAccounts.SuspendLayout();
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
            this.pnlContent.Controls.Add(this.tabReference);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(20, 12);
            this.pnlContent.Size = new System.Drawing.Size(934, 563);
            this.pnlContent.TabIndex = 0;
            this.pnlContent.Name = "pnlContent";
            //
            // tabReference
            //
            this.tabReference.Controls.Add(this.tabUniversity);
            this.tabReference.Controls.Add(this.tabRequirements);
            this.tabReference.Controls.Add(this.tabFunding);
            this.tabReference.Controls.Add(this.tabInterests);
            this.tabReference.Controls.Add(this.tabQuestionnaire);
            this.tabReference.Controls.Add(this.tabUserAccounts);
            this.tabReference.AccessibleName = "Reference data tabs";
            this.tabReference.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabReference.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tabReference.Location = new System.Drawing.Point(0, 0);
            this.tabReference.Padding = new System.Drawing.Point(6, 6);
            this.tabReference.Size = new System.Drawing.Size(934, 563);
            this.tabReference.TabIndex = 0;
            this.tabReference.Name = "tabReference";
            this.tabReference.SelectedIndexChanged += new System.EventHandler(this.tabReference_SelectedIndexChanged);
            //
            // tabUniversity
            //
            this.tabUniversity.Controls.Add(this.ucUniversityInfo);
            this.tabUniversity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.tabUniversity.Location = new System.Drawing.Point(4, 32);
            this.tabUniversity.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tabUniversity.Size = new System.Drawing.Size(926, 527);
            this.tabUniversity.TabIndex = 0;
            this.tabUniversity.Text = "University info \u2014 13.1";
            this.tabUniversity.Name = "tabUniversity";
            //
            // ucUniversityInfo
            //
            this.ucUniversityInfo.AccessibleName = "University info \u2014 13.1";
            this.ucUniversityInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucUniversityInfo.Location = new System.Drawing.Point(0, 0);
            this.ucUniversityInfo.Size = new System.Drawing.Size(926, 527);
            this.ucUniversityInfo.TabIndex = 0;
            this.ucUniversityInfo.Name = "ucUniversityInfo";
            //
            // tabRequirements
            //
            this.tabRequirements.Controls.Add(this.ucDegreeRequirements);
            this.tabRequirements.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.tabRequirements.Location = new System.Drawing.Point(4, 32);
            this.tabRequirements.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tabRequirements.Size = new System.Drawing.Size(926, 527);
            this.tabRequirements.TabIndex = 1;
            this.tabRequirements.Text = "Degree requirements \u2014 13.2";
            this.tabRequirements.Name = "tabRequirements";
            //
            // ucDegreeRequirements
            //
            this.ucDegreeRequirements.AccessibleName = "Degree requirements \u2014 13.2";
            this.ucDegreeRequirements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucDegreeRequirements.Location = new System.Drawing.Point(0, 0);
            this.ucDegreeRequirements.Size = new System.Drawing.Size(926, 527);
            this.ucDegreeRequirements.TabIndex = 0;
            this.ucDegreeRequirements.Name = "ucDegreeRequirements";
            //
            // tabFunding
            //
            this.tabFunding.Controls.Add(this.ucFunding);
            this.tabFunding.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.tabFunding.Location = new System.Drawing.Point(4, 32);
            this.tabFunding.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tabFunding.Size = new System.Drawing.Size(926, 527);
            this.tabFunding.TabIndex = 2;
            this.tabFunding.Text = "Funding \u2014 13.3";
            this.tabFunding.Name = "tabFunding";
            //
            // ucFunding
            //
            this.ucFunding.AccessibleName = "Funding \u2014 13.3";
            this.ucFunding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucFunding.Location = new System.Drawing.Point(0, 0);
            this.ucFunding.Size = new System.Drawing.Size(926, 527);
            this.ucFunding.TabIndex = 0;
            this.ucFunding.Name = "ucFunding";
            //
            // tabInterests
            //
            this.tabInterests.Controls.Add(this.ucInterestCategories);
            this.tabInterests.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.tabInterests.Location = new System.Drawing.Point(4, 32);
            this.tabInterests.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tabInterests.Size = new System.Drawing.Size(926, 527);
            this.tabInterests.TabIndex = 3;
            this.tabInterests.Text = "Interest categories \u2014 13.4";
            this.tabInterests.Name = "tabInterests";
            //
            // ucInterestCategories
            //
            this.ucInterestCategories.AccessibleName = "Interest categories \u2014 13.4";
            this.ucInterestCategories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucInterestCategories.Location = new System.Drawing.Point(0, 0);
            this.ucInterestCategories.Size = new System.Drawing.Size(926, 527);
            this.ucInterestCategories.TabIndex = 0;
            this.ucInterestCategories.Name = "ucInterestCategories";
            //
            // tabQuestionnaire
            //
            this.tabQuestionnaire.Controls.Add(this.ucQuestionnaire);
            this.tabQuestionnaire.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.tabQuestionnaire.Location = new System.Drawing.Point(4, 32);
            this.tabQuestionnaire.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tabQuestionnaire.Size = new System.Drawing.Size(926, 527);
            this.tabQuestionnaire.TabIndex = 4;
            this.tabQuestionnaire.Text = "Questionnaire \u2014 13.5";
            this.tabQuestionnaire.Name = "tabQuestionnaire";
            //
            // ucQuestionnaire
            //
            this.ucQuestionnaire.AccessibleName = "Questionnaire \u2014 13.5";
            this.ucQuestionnaire.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucQuestionnaire.Location = new System.Drawing.Point(0, 0);
            this.ucQuestionnaire.Size = new System.Drawing.Size(926, 527);
            this.ucQuestionnaire.TabIndex = 0;
            this.ucQuestionnaire.Name = "ucQuestionnaire";
            //
            // tabUserAccounts
            //
            this.tabUserAccounts.Controls.Add(this.ucUserAccounts);
            this.tabUserAccounts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.tabUserAccounts.Location = new System.Drawing.Point(4, 32);
            this.tabUserAccounts.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tabUserAccounts.Size = new System.Drawing.Size(926, 527);
            this.tabUserAccounts.TabIndex = 5;
            this.tabUserAccounts.Text = "User accounts \u2014 13.6";
            this.tabUserAccounts.Name = "tabUserAccounts";
            //
            // ucUserAccounts
            //
            this.ucUserAccounts.AccessibleName = "User accounts \u2014 13.6";
            this.ucUserAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucUserAccounts.Location = new System.Drawing.Point(0, 0);
            this.ucUserAccounts.Size = new System.Drawing.Size(926, 527);
            this.ucUserAccounts.TabIndex = 0;
            this.ucUserAccounts.Name = "ucUserAccounts";
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
            this.lblTitle.Text = "Reference data";
            this.lblTitle.Name = "lblTitle";
            //
            // lblSubtitle
            //
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.lblSubtitle.Location = new System.Drawing.Point(24, 54);
            this.lblSubtitle.Size = new System.Drawing.Size(914, 18);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Maintain the data the recommendation engine reads \u2014 Administrators only";
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
            this.lblBadge.Text = "ADMINISTRATOR";
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
            this.pnlActions.Controls.Add(this.btnAdd);
            this.pnlActions.Controls.Add(this.btnUpdate);
            this.pnlActions.Controls.Add(this.btnDeactivate);
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
            // btnAdd
            //
            this.btnAdd.AccessibleName = "Add requirement";
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(754, 12);
            this.btnAdd.Size = new System.Drawing.Size(200, 40);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add requirement";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // btnUpdate
            //
            this.btnUpdate.AccessibleName = "Update requirement";
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.BackColor = System.Drawing.Color.White;
            this.btnUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnUpdate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnUpdate.Location = new System.Drawing.Point(544, 12);
            this.btnUpdate.Size = new System.Drawing.Size(200, 40);
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Text = "Update requirement";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            //
            // btnDeactivate
            //
            this.btnDeactivate.AccessibleName = "Deactivate";
            this.btnDeactivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeactivate.BackColor = System.Drawing.Color.White;
            this.btnDeactivate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeactivate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeactivate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDeactivate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnDeactivate.Location = new System.Drawing.Point(404, 12);
            this.btnDeactivate.Size = new System.Drawing.Size(130, 40);
            this.btnDeactivate.TabIndex = 3;
            this.btnDeactivate.Text = "Deactivate";
            this.btnDeactivate.UseVisualStyleBackColor = false;
            this.btnDeactivate.Name = "btnDeactivate";
            this.btnDeactivate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.btnDeactivate.Click += new System.EventHandler(this.btnDeactivate_Click);
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
            this.btnCancel.Location = new System.Drawing.Point(264, 12);
            this.btnCancel.Size = new System.Drawing.Size(130, 40);
            this.btnCancel.TabIndex = 4;
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
            this.lblActionHint.Size = new System.Drawing.Size(224, 40);
            this.lblActionHint.TabIndex = 5;
            this.lblActionHint.Text = "Changes apply to new advisory sessions only";
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
            this.btnNavReferenceData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(64)))), ((int)(((byte)(104)))));
            this.btnNavReferenceData.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavReferenceData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavReferenceData.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnNavReferenceData.ForeColor = System.Drawing.Color.White;
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
            // ReferenceDataForm
            //
            this.AcceptButton = this.btnAdd;
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
            this.Text = "Future Path \u2014 Reference data";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlTitle);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlRail);
            this.Controls.Add(this.pnlStatus);
            this.Name = "ReferenceDataForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ReferenceDataForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ReferenceDataForm_FormClosed);
            this.Shown += new System.EventHandler(this.ReferenceDataForm_Shown);
            this.pnlBody.ResumeLayout(false);
            this.pnlBody.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.tabReference.ResumeLayout(false);
            this.tabReference.PerformLayout();
            this.tabUniversity.ResumeLayout(false);
            this.tabUniversity.PerformLayout();
            this.tabRequirements.ResumeLayout(false);
            this.tabRequirements.PerformLayout();
            this.tabFunding.ResumeLayout(false);
            this.tabFunding.PerformLayout();
            this.tabInterests.ResumeLayout(false);
            this.tabInterests.PerformLayout();
            this.tabQuestionnaire.ResumeLayout(false);
            this.tabQuestionnaire.PerformLayout();
            this.tabUserAccounts.ResumeLayout(false);
            this.tabUserAccounts.PerformLayout();
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
        private System.Windows.Forms.TabControl tabReference;
        private System.Windows.Forms.TabPage tabUniversity;
        private FuturePath.PresentationLayer.UniversityInfoTab ucUniversityInfo;
        private System.Windows.Forms.TabPage tabRequirements;
        private FuturePath.PresentationLayer.DegreeRequirementsTab ucDegreeRequirements;
        private System.Windows.Forms.TabPage tabFunding;
        private FuturePath.PresentationLayer.FundingTab ucFunding;
        private System.Windows.Forms.TabPage tabInterests;
        private FuturePath.PresentationLayer.InterestCategoriesTab ucInterestCategories;
        private System.Windows.Forms.TabPage tabQuestionnaire;
        private FuturePath.PresentationLayer.QuestionnaireTab ucQuestionnaire;
        private System.Windows.Forms.TabPage tabUserAccounts;
        private FuturePath.PresentationLayer.UserAccountsTab ucUserAccounts;
        private System.Windows.Forms.Panel pnlTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblBadge;
        private System.Windows.Forms.Panel pnlTitleRule;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Panel pnlActionRule;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDeactivate;
        private System.Windows.Forms.Button btnCancel;
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
