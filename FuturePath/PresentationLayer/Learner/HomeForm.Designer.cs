namespace FuturePath.PresentationLayer
{
    partial class HomeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomeForm));
            this.pnlBody = new System.Windows.Forms.Panel();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblIntro = new System.Windows.Forms.Label();
            this.pnlLearnerCard = new System.Windows.Forms.Panel();
            this.lblLearnerHeadingBar = new System.Windows.Forms.Panel();
            this.lblLearnerHeading = new System.Windows.Forms.Label();
            this.lblLearnerText = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.pnlStaffCard = new System.Windows.Forms.Panel();
            this.lblStaffHeadingBar = new System.Windows.Forms.Panel();
            this.lblStaffHeading = new System.Windows.Forms.Label();
            this.lblStaffText = new System.Windows.Forms.Label();
            this.btnStaffLogin = new System.Windows.Forms.Button();
            this.lblDisclaimer = new System.Windows.Forms.Label();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.pnlTitleRule = new System.Windows.Forms.Panel();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.pnlActionRule = new System.Windows.Forms.Panel();
            this.lblActionHint = new System.Windows.Forms.Label();
            this.pnlRail = new System.Windows.Forms.Panel();
            this.lblBrand = new System.Windows.Forms.Label();
            this.pnlBrandRule = new System.Windows.Forms.Panel();
            this.lblRailFooter = new System.Windows.Forms.Label();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlBody.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlLearnerCard.SuspendLayout();
            this.pnlStaffCard.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.pnlRail.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBody
            // 
            this.pnlBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.pnlBody.Controls.Add(this.pnlContent);
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Location = new System.Drawing.Point(262, 105);
            this.pnlBody.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new System.Windows.Forms.Padding(25, 15, 25, 15);
            this.pnlBody.Size = new System.Drawing.Size(968, 634);
            this.pnlBody.TabIndex = 0;
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.lblIntro);
            this.pnlContent.Controls.Add(this.pnlLearnerCard);
            this.pnlContent.Controls.Add(this.pnlStaffCard);
            this.pnlContent.Controls.Add(this.lblDisclaimer);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(25, 15);
            this.pnlContent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(918, 604);
            this.pnlContent.TabIndex = 0;
            // 
            // lblIntro
            // 
            this.lblIntro.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblIntro.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblIntro.Location = new System.Drawing.Point(0, 0);
            this.lblIntro.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIntro.Name = "lblIntro";
            this.lblIntro.Size = new System.Drawing.Size(918, 80);
            this.lblIntro.TabIndex = 0;
            this.lblIntro.Text = resources.GetString("lblIntro.Text");
            // 
            // pnlLearnerCard
            // 
            this.pnlLearnerCard.BackColor = System.Drawing.Color.White;
            this.pnlLearnerCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLearnerCard.Controls.Add(this.lblLearnerHeadingBar);
            this.pnlLearnerCard.Controls.Add(this.lblLearnerHeading);
            this.pnlLearnerCard.Controls.Add(this.lblLearnerText);
            this.pnlLearnerCard.Controls.Add(this.btnStart);
            this.pnlLearnerCard.Location = new System.Drawing.Point(0, 105);
            this.pnlLearnerCard.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlLearnerCard.Name = "pnlLearnerCard";
            this.pnlLearnerCard.Size = new System.Drawing.Size(446, 324);
            this.pnlLearnerCard.TabIndex = 1;
            // 
            // lblLearnerHeadingBar
            // 
            this.lblLearnerHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblLearnerHeadingBar.Location = new System.Drawing.Point(20, 22);
            this.lblLearnerHeadingBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lblLearnerHeadingBar.Name = "lblLearnerHeadingBar";
            this.lblLearnerHeadingBar.Size = new System.Drawing.Size(5, 25);
            this.lblLearnerHeadingBar.TabIndex = 0;
            // 
            // lblLearnerHeading
            // 
            this.lblLearnerHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblLearnerHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblLearnerHeading.Location = new System.Drawing.Point(32, 18);
            this.lblLearnerHeading.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLearnerHeading.Name = "lblLearnerHeading";
            this.lblLearnerHeading.Size = new System.Drawing.Size(375, 35);
            this.lblLearnerHeading.TabIndex = 1;
            this.lblLearnerHeading.Text = "I am a learner";
            this.lblLearnerHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLearnerText
            // 
            this.lblLearnerText.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblLearnerText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblLearnerText.Location = new System.Drawing.Point(25, 65);
            this.lblLearnerText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLearnerText.Name = "lblLearnerText";
            this.lblLearnerText.Size = new System.Drawing.Size(396, 162);
            this.lblLearnerText.TabIndex = 2;
            this.lblLearnerText.Text = resources.GetString("lblLearnerText.Text");
            // 
            // btnStart
            // 
            this.btnStart.AccessibleName = "Start advisory session";
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.btnStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStart.FlatAppearance.BorderSize = 0;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(25, 245);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(275, 50);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start advisory session";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pnlStaffCard
            // 
            this.pnlStaffCard.BackColor = System.Drawing.Color.White;
            this.pnlStaffCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlStaffCard.Controls.Add(this.lblStaffHeadingBar);
            this.pnlStaffCard.Controls.Add(this.lblStaffHeading);
            this.pnlStaffCard.Controls.Add(this.lblStaffText);
            this.pnlStaffCard.Controls.Add(this.btnStaffLogin);
            this.pnlStaffCard.Location = new System.Drawing.Point(471, 105);
            this.pnlStaffCard.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlStaffCard.Name = "pnlStaffCard";
            this.pnlStaffCard.Size = new System.Drawing.Size(446, 324);
            this.pnlStaffCard.TabIndex = 2;
            // 
            // lblStaffHeadingBar
            // 
            this.lblStaffHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblStaffHeadingBar.Location = new System.Drawing.Point(20, 22);
            this.lblStaffHeadingBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lblStaffHeadingBar.Name = "lblStaffHeadingBar";
            this.lblStaffHeadingBar.Size = new System.Drawing.Size(5, 25);
            this.lblStaffHeadingBar.TabIndex = 0;
            // 
            // lblStaffHeading
            // 
            this.lblStaffHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblStaffHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblStaffHeading.Location = new System.Drawing.Point(32, 18);
            this.lblStaffHeading.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStaffHeading.Name = "lblStaffHeading";
            this.lblStaffHeading.Size = new System.Drawing.Size(375, 35);
            this.lblStaffHeading.TabIndex = 1;
            this.lblStaffHeading.Text = "I am a staff member";
            this.lblStaffHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaffText
            // 
            this.lblStaffText.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStaffText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblStaffText.Location = new System.Drawing.Point(25, 65);
            this.lblStaffText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStaffText.Name = "lblStaffText";
            this.lblStaffText.Size = new System.Drawing.Size(396, 162);
            this.lblStaffText.TabIndex = 2;
            this.lblStaffText.Text = "Career Advisors log in to review advisory sessions and generate reports. System A" +
    "dministrators also maintain faculties, programmes, requirements, funding, the qu" +
    "estionnaire and user accounts.";
            // 
            // btnStaffLogin
            // 
            this.btnStaffLogin.AccessibleName = "Staff login";
            this.btnStaffLogin.BackColor = System.Drawing.Color.White;
            this.btnStaffLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStaffLogin.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.btnStaffLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStaffLogin.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnStaffLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnStaffLogin.Location = new System.Drawing.Point(25, 245);
            this.btnStaffLogin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStaffLogin.Name = "btnStaffLogin";
            this.btnStaffLogin.Size = new System.Drawing.Size(200, 50);
            this.btnStaffLogin.TabIndex = 3;
            this.btnStaffLogin.Text = "Staff login";
            this.btnStaffLogin.UseVisualStyleBackColor = false;
            this.btnStaffLogin.Click += new System.EventHandler(this.btnStaffLogin_Click);
            // 
            // lblDisclaimer
            // 
            this.lblDisclaimer.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblDisclaimer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblDisclaimer.Location = new System.Drawing.Point(0, 455);
            this.lblDisclaimer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDisclaimer.Name = "lblDisclaimer";
            this.lblDisclaimer.Size = new System.Drawing.Size(918, 45);
            this.lblDisclaimer.TabIndex = 3;
            this.lblDisclaimer.Text = "Preliminary guidance only — not an admission decision. Requirement and funding da" +
    "ta are locally stored sample data; check them against the official source before" +
    " you apply.";
            // 
            // pnlTitle
            // 
            this.pnlTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Controls.Add(this.lblSubtitle);
            this.pnlTitle.Controls.Add(this.pnlTitleRule);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(262, 0);
            this.pnlTitle.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(968, 105);
            this.pnlTitle.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(25, 12);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(700, 50);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Welcome to Future Path";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.lblSubtitle.Location = new System.Drawing.Point(30, 68);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(892, 22);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Degree, alternative programme and funding guidance for Grade 11 and 12 learners";
            // 
            // pnlTitleRule
            // 
            this.pnlTitleRule.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.pnlTitleRule.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTitleRule.Location = new System.Drawing.Point(0, 101);
            this.pnlTitleRule.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlTitleRule.Name = "pnlTitleRule";
            this.pnlTitleRule.Size = new System.Drawing.Size(968, 4);
            this.pnlTitleRule.TabIndex = 2;
            // 
            // pnlActions
            // 
            this.pnlActions.BackColor = System.Drawing.Color.White;
            this.pnlActions.Controls.Add(this.pnlActionRule);
            this.pnlActions.Controls.Add(this.lblActionHint);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlActions.Location = new System.Drawing.Point(262, 739);
            this.pnlActions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(968, 80);
            this.pnlActions.TabIndex = 2;
            // 
            // pnlActionRule
            // 
            this.pnlActionRule.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            this.pnlActionRule.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlActionRule.Location = new System.Drawing.Point(0, 0);
            this.pnlActionRule.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlActionRule.Name = "pnlActionRule";
            this.pnlActionRule.Size = new System.Drawing.Size(968, 1);
            this.pnlActionRule.TabIndex = 0;
            // 
            // lblActionHint
            // 
            this.lblActionHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblActionHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblActionHint.Location = new System.Drawing.Point(25, 15);
            this.lblActionHint.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblActionHint.Name = "lblActionHint";
            this.lblActionHint.Size = new System.Drawing.Size(905, 50);
            this.lblActionHint.TabIndex = 1;
            this.lblActionHint.Text = "Preliminary guidance only — not an admission decision.";
            this.lblActionHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlRail
            // 
            this.pnlRail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.pnlRail.Controls.Add(this.lblBrand);
            this.pnlRail.Controls.Add(this.pnlBrandRule);
            this.pnlRail.Controls.Add(this.lblRailFooter);
            this.pnlRail.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlRail.Location = new System.Drawing.Point(0, 0);
            this.pnlRail.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlRail.Name = "pnlRail";
            this.pnlRail.Size = new System.Drawing.Size(262, 819);
            this.pnlRail.TabIndex = 3;
            // 
            // lblBrand
            // 
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblBrand.ForeColor = System.Drawing.Color.White;
            this.lblBrand.Location = new System.Drawing.Point(0, 0);
            this.lblBrand.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(262, 82);
            this.lblBrand.TabIndex = 0;
            this.lblBrand.Text = "Future Path";
            this.lblBrand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlBrandRule
            // 
            this.pnlBrandRule.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.pnlBrandRule.Location = new System.Drawing.Point(30, 82);
            this.pnlBrandRule.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlBrandRule.Name = "pnlBrandRule";
            this.pnlBrandRule.Size = new System.Drawing.Size(202, 2);
            this.pnlBrandRule.TabIndex = 1;
            // 
            // lblRailFooter
            // 
            this.lblRailFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblRailFooter.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblRailFooter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.lblRailFooter.Location = new System.Drawing.Point(0, 719);
            this.lblRailFooter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRailFooter.Name = "lblRailFooter";
            this.lblRailFooter.Padding = new System.Windows.Forms.Padding(20, 0, 10, 0);
            this.lblRailFooter.Size = new System.Drawing.Size(262, 100);
            this.lblRailFooter.TabIndex = 2;
            this.lblRailFooter.Text = "Works offline\r\nLocal data only";
            this.lblRailFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlStatus
            // 
            this.pnlStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(31)))), ((int)(((byte)(58)))));
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatus.Location = new System.Drawing.Point(0, 819);
            this.pnlStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(1230, 32);
            this.pnlStatus.TabIndex = 4;
            // 
            // lblStatus
            // 
            this.lblStatus.AccessibleName = "Status";
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(0, 0);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.lblStatus.Size = new System.Drawing.Size(1230, 32);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // HomeForm
            // 
            this.AcceptButton = this.btnStart;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1230, 851);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlTitle);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlRail);
            this.Controls.Add(this.pnlStatus);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "HomeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Future Path — Homepage";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HomeForm_FormClosing);
            this.Load += new System.EventHandler(this.HomeForm_Load);
            this.VisibleChanged += new System.EventHandler(this.HomeForm_VisibleChanged);
            this.pnlBody.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.pnlLearnerCard.ResumeLayout(false);
            this.pnlStaffCard.ResumeLayout(false);
            this.pnlTitle.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.pnlRail.ResumeLayout(false);
            this.pnlStatus.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlBody;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Label lblIntro;
        private System.Windows.Forms.Panel pnlLearnerCard;
        private System.Windows.Forms.Panel lblLearnerHeadingBar;
        private System.Windows.Forms.Label lblLearnerHeading;
        private System.Windows.Forms.Label lblLearnerText;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Panel pnlStaffCard;
        private System.Windows.Forms.Panel lblStaffHeadingBar;
        private System.Windows.Forms.Label lblStaffHeading;
        private System.Windows.Forms.Label lblStaffText;
        private System.Windows.Forms.Button btnStaffLogin;
        private System.Windows.Forms.Label lblDisclaimer;
        private System.Windows.Forms.Panel pnlTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Panel pnlTitleRule;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Panel pnlActionRule;
        private System.Windows.Forms.Label lblActionHint;
        private System.Windows.Forms.Panel pnlRail;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.Panel pnlBrandRule;
        private System.Windows.Forms.Label lblRailFooter;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblStatus;
    }
}
