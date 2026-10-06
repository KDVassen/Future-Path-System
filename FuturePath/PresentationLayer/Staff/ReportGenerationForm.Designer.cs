namespace FuturePath.PresentationLayer
{
    partial class ReportGenerationForm
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
            this.pnlReportCard = new System.Windows.Forms.Panel();
            this.lblReportHeadingBar = new System.Windows.Forms.Panel();
            this.lblReportHeading = new System.Windows.Forms.Label();
            this.rdoLearnerSummary = new System.Windows.Forms.RadioButton();
            this.lblLearnerSummaryNote = new System.Windows.Forms.Label();
            this.rdoDegreeInterest = new System.Windows.Forms.RadioButton();
            this.lblDegreeInterestNote = new System.Windows.Forms.Label();
            this.rdoNotEligible = new System.Windows.Forms.RadioButton();
            this.lblNotEligibleNote = new System.Windows.Forms.Label();
            this.pnlParameterCard = new System.Windows.Forms.Panel();
            this.lblParameterHeadingBar = new System.Windows.Forms.Panel();
            this.lblParameterHeading = new System.Windows.Forms.Label();
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblFromStar = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblFromError = new System.Windows.Forms.Label();
            this.lblTo = new System.Windows.Forms.Label();
            this.lblToStar = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblPeriodNote = new System.Windows.Forms.Label();
            this.lblFaculty = new System.Windows.Forms.Label();
            this.cboFaculty = new System.Windows.Forms.ComboBox();
            this.lblFacultyNote = new System.Windows.Forms.Label();
            this.lblGrade = new System.Windows.Forms.Label();
            this.cboGrade = new System.Windows.Forms.ComboBox();
            this.lblGradeNote = new System.Windows.Forms.Label();
            this.lblOutcome = new System.Windows.Forms.Label();
            this.cboOutcome = new System.Windows.Forms.ComboBox();
            this.lblOutcomeNote = new System.Windows.Forms.Label();
            this.lblSessionNumber = new System.Windows.Forms.Label();
            this.lblSessionNumberStar = new System.Windows.Forms.Label();
            this.txtSessionNumber = new System.Windows.Forms.TextBox();
            this.lblSessionNumberNote = new System.Windows.Forms.Label();
            this.lblSessionNumberError = new System.Windows.Forms.Label();
            this.chkIncludeInactive = new System.Windows.Forms.CheckBox();
            this.lblIncludeInactiveNote = new System.Windows.Forms.Label();
            this.pnlOutputCard = new System.Windows.Forms.Panel();
            this.lblOutputHeadingBar = new System.Windows.Forms.Panel();
            this.lblOutputHeading = new System.Windows.Forms.Label();
            this.lblOutputFormat = new System.Windows.Forms.Label();
            this.lblOutputFormatStar = new System.Windows.Forms.Label();
            this.cboOutputFormat = new System.Windows.Forms.ComboBox();
            this.lblOutputFormatError = new System.Windows.Forms.Label();
            this.lblSaveTo = new System.Windows.Forms.Label();
            this.lblSaveToStar = new System.Windows.Forms.Label();
            this.txtSaveTo = new System.Windows.Forms.TextBox();
            this.lblSaveToError = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.pnlReadiness = new System.Windows.Forms.Panel();
            this.lblReadyReport = new System.Windows.Forms.Label();
            this.lblReadyScope = new System.Windows.Forms.Label();
            this.lblReadyChip = new System.Windows.Forms.Label();
            this.pnlReadinessBar = new System.Windows.Forms.Panel();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblBadge = new System.Windows.Forms.Label();
            this.pnlTitleRule = new System.Windows.Forms.Panel();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.pnlActionRule = new System.Windows.Forms.Panel();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
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
            this.pnlReportCard.SuspendLayout();
            this.lblReportHeadingBar.SuspendLayout();
            this.pnlParameterCard.SuspendLayout();
            this.lblParameterHeadingBar.SuspendLayout();
            this.pnlOutputCard.SuspendLayout();
            this.lblOutputHeadingBar.SuspendLayout();
            this.pnlReadiness.SuspendLayout();
            this.pnlReadinessBar.SuspendLayout();
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
            this.pnlContent.Controls.Add(this.pnlReportCard);
            this.pnlContent.Controls.Add(this.pnlParameterCard);
            this.pnlContent.Controls.Add(this.pnlOutputCard);
            this.pnlContent.Controls.Add(this.pnlReadiness);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(20, 12);
            this.pnlContent.Size = new System.Drawing.Size(734, 483);
            this.pnlContent.TabIndex = 0;
            this.pnlContent.Name = "pnlContent";
            //
            // pnlReportCard
            //
            this.pnlReportCard.Controls.Add(this.lblReportHeadingBar);
            this.pnlReportCard.Controls.Add(this.lblReportHeading);
            this.pnlReportCard.Controls.Add(this.rdoLearnerSummary);
            this.pnlReportCard.Controls.Add(this.lblLearnerSummaryNote);
            this.pnlReportCard.Controls.Add(this.rdoDegreeInterest);
            this.pnlReportCard.Controls.Add(this.lblDegreeInterestNote);
            this.pnlReportCard.Controls.Add(this.rdoNotEligible);
            this.pnlReportCard.Controls.Add(this.lblNotEligibleNote);
            this.pnlReportCard.BackColor = System.Drawing.Color.White;
            this.pnlReportCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlReportCard.Location = new System.Drawing.Point(0, 0);
            this.pnlReportCard.Size = new System.Drawing.Size(734, 120);
            this.pnlReportCard.TabIndex = 0;
            this.pnlReportCard.Name = "pnlReportCard";
            //
            // lblReportHeadingBar
            //
            this.lblReportHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblReportHeadingBar.Location = new System.Drawing.Point(16, 8);
            this.lblReportHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblReportHeadingBar.TabIndex = 0;
            this.lblReportHeadingBar.Name = "lblReportHeadingBar";
            //
            // lblReportHeading
            //
            this.lblReportHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblReportHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblReportHeading.Location = new System.Drawing.Point(26, 4);
            this.lblReportHeading.Size = new System.Drawing.Size(500, 28);
            this.lblReportHeading.TabIndex = 1;
            this.lblReportHeading.Text = "Which report do you want?";
            this.lblReportHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblReportHeading.Name = "lblReportHeading";
            //
            // rdoLearnerSummary
            //
            this.rdoLearnerSummary.AccessibleName = "Learner Advisory Summary \u2014 FP-LSR (UI 11.1a)";
            this.rdoLearnerSummary.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.rdoLearnerSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.rdoLearnerSummary.Location = new System.Drawing.Point(20, 34);
            this.rdoLearnerSummary.Size = new System.Drawing.Size(375, 24);
            this.rdoLearnerSummary.TabIndex = 2;
            this.rdoLearnerSummary.Text = "Learner Advisory Summary \u2014 FP-LSR (UI 11.1a)";
            this.rdoLearnerSummary.UseVisualStyleBackColor = true;
            this.rdoLearnerSummary.Name = "rdoLearnerSummary";
            this.rdoLearnerSummary.CheckedChanged += new System.EventHandler(this.ReportChoice_CheckedChanged);
            //
            // lblLearnerSummaryNote
            //
            this.lblLearnerSummaryNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblLearnerSummaryNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblLearnerSummaryNote.Location = new System.Drawing.Point(400, 37);
            this.lblLearnerSummaryNote.Size = new System.Drawing.Size(322, 18);
            this.lblLearnerSummaryNote.TabIndex = 3;
            this.lblLearnerSummaryNote.Text = "Detailed: one completed advisory session, as exported on UI 7.";
            this.lblLearnerSummaryNote.Name = "lblLearnerSummaryNote";
            //
            // rdoDegreeInterest
            //
            this.rdoDegreeInterest.AccessibleName = "Degree Interest and Eligibility Report \u2014 FP-DIE (UI 11.2b)";
            this.rdoDegreeInterest.Checked = true;
            this.rdoDegreeInterest.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.rdoDegreeInterest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.rdoDegreeInterest.Location = new System.Drawing.Point(20, 62);
            this.rdoDegreeInterest.Size = new System.Drawing.Size(375, 24);
            this.rdoDegreeInterest.TabIndex = 4;
            this.rdoDegreeInterest.TabStop = true;
            this.rdoDegreeInterest.Text = "Degree Interest and Eligibility Report \u2014 FP-DIE (UI 11.2b)";
            this.rdoDegreeInterest.UseVisualStyleBackColor = true;
            this.rdoDegreeInterest.Name = "rdoDegreeInterest";
            this.rdoDegreeInterest.CheckedChanged += new System.EventHandler(this.ReportChoice_CheckedChanged);
            //
            // lblDegreeInterestNote
            //
            this.lblDegreeInterestNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblDegreeInterestNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblDegreeInterestNote.Location = new System.Drawing.Point(400, 65);
            this.lblDegreeInterestNote.Size = new System.Drawing.Size(322, 18);
            this.lblDegreeInterestNote.TabIndex = 5;
            this.lblDegreeInterestNote.Text = "Summary for a period: demand, outcome counts, subject gaps.";
            this.lblDegreeInterestNote.Name = "lblDegreeInterestNote";
            //
            // rdoNotEligible
            //
            this.rdoNotEligible.AccessibleName = "Learners Not Currently Eligible \u2014 FP-LER (UI 11.3c)";
            this.rdoNotEligible.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.rdoNotEligible.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.rdoNotEligible.Location = new System.Drawing.Point(20, 90);
            this.rdoNotEligible.Size = new System.Drawing.Size(375, 24);
            this.rdoNotEligible.TabIndex = 6;
            this.rdoNotEligible.Text = "Learners Not Currently Eligible \u2014 FP-LER (UI 11.3c)";
            this.rdoNotEligible.UseVisualStyleBackColor = true;
            this.rdoNotEligible.Name = "rdoNotEligible";
            this.rdoNotEligible.CheckedChanged += new System.EventHandler(this.ReportChoice_CheckedChanged);
            //
            // lblNotEligibleNote
            //
            this.lblNotEligibleNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblNotEligibleNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblNotEligibleNote.Location = new System.Drawing.Point(400, 93);
            this.lblNotEligibleNote.Size = new System.Drawing.Size(322, 18);
            this.lblNotEligibleNote.TabIndex = 7;
            this.lblNotEligibleNote.Text = "Exceptions: Not Currently Eligible, More Information Needed.";
            this.lblNotEligibleNote.Name = "lblNotEligibleNote";
            //
            // pnlParameterCard
            //
            this.pnlParameterCard.Controls.Add(this.lblParameterHeadingBar);
            this.pnlParameterCard.Controls.Add(this.lblParameterHeading);
            this.pnlParameterCard.Controls.Add(this.lblFrom);
            this.pnlParameterCard.Controls.Add(this.lblFromStar);
            this.pnlParameterCard.Controls.Add(this.dtpFrom);
            this.pnlParameterCard.Controls.Add(this.lblFromError);
            this.pnlParameterCard.Controls.Add(this.lblTo);
            this.pnlParameterCard.Controls.Add(this.lblToStar);
            this.pnlParameterCard.Controls.Add(this.dtpTo);
            this.pnlParameterCard.Controls.Add(this.lblPeriodNote);
            this.pnlParameterCard.Controls.Add(this.lblFaculty);
            this.pnlParameterCard.Controls.Add(this.cboFaculty);
            this.pnlParameterCard.Controls.Add(this.lblFacultyNote);
            this.pnlParameterCard.Controls.Add(this.lblGrade);
            this.pnlParameterCard.Controls.Add(this.cboGrade);
            this.pnlParameterCard.Controls.Add(this.lblGradeNote);
            this.pnlParameterCard.Controls.Add(this.lblOutcome);
            this.pnlParameterCard.Controls.Add(this.cboOutcome);
            this.pnlParameterCard.Controls.Add(this.lblOutcomeNote);
            this.pnlParameterCard.Controls.Add(this.lblSessionNumber);
            this.pnlParameterCard.Controls.Add(this.lblSessionNumberStar);
            this.pnlParameterCard.Controls.Add(this.txtSessionNumber);
            this.pnlParameterCard.Controls.Add(this.lblSessionNumberNote);
            this.pnlParameterCard.Controls.Add(this.lblSessionNumberError);
            this.pnlParameterCard.Controls.Add(this.chkIncludeInactive);
            this.pnlParameterCard.Controls.Add(this.lblIncludeInactiveNote);
            this.pnlParameterCard.BackColor = System.Drawing.Color.White;
            this.pnlParameterCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlParameterCard.Location = new System.Drawing.Point(0, 128);
            this.pnlParameterCard.Size = new System.Drawing.Size(734, 180);
            this.pnlParameterCard.TabIndex = 1;
            this.pnlParameterCard.Name = "pnlParameterCard";
            //
            // lblParameterHeadingBar
            //
            this.lblParameterHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblParameterHeadingBar.Location = new System.Drawing.Point(16, 8);
            this.lblParameterHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblParameterHeadingBar.TabIndex = 0;
            this.lblParameterHeadingBar.Name = "lblParameterHeadingBar";
            //
            // lblParameterHeading
            //
            this.lblParameterHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblParameterHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblParameterHeading.Location = new System.Drawing.Point(26, 4);
            this.lblParameterHeading.Size = new System.Drawing.Size(500, 28);
            this.lblParameterHeading.TabIndex = 1;
            this.lblParameterHeading.Text = "Report parameters";
            this.lblParameterHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblParameterHeading.Name = "lblParameterHeading";
            //
            // lblFrom
            //
            this.lblFrom.AutoSize = true;
            this.lblFrom.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFrom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFrom.Location = new System.Drawing.Point(20, 34);
            this.lblFrom.TabIndex = 2;
            this.lblFrom.Text = "Sessions from";
            this.lblFrom.Name = "lblFrom";
            //
            // lblFromStar
            //
            this.lblFromStar.AutoSize = true;
            this.lblFromStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFromStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblFromStar.Location = new System.Drawing.Point(136, 34);
            this.lblFromStar.Size = new System.Drawing.Size(14, 24);
            this.lblFromStar.TabIndex = 3;
            this.lblFromStar.Text = "*";
            this.lblFromStar.Name = "lblFromStar";
            //
            // dtpFrom
            //
            this.dtpFrom.AccessibleName = "Sessions from (required)";
            this.dtpFrom.CustomFormat = "dd/MM/yyyy";
            this.dtpFrom.Font = new System.Drawing.Font("Consolas", 10F);
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom.Location = new System.Drawing.Point(20, 60);
            this.dtpFrom.Size = new System.Drawing.Size(150, 23);
            this.dtpFrom.TabIndex = 4;
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.ValueChanged += new System.EventHandler(this.Parameter_Changed);
            //
            // lblFromError
            //
            this.lblFromError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFromError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblFromError.Location = new System.Drawing.Point(20, 86);
            this.lblFromError.Size = new System.Drawing.Size(330, 16);
            this.lblFromError.TabIndex = 5;
            this.lblFromError.Text = "";
            this.lblFromError.Visible = false;
            this.lblFromError.Name = "lblFromError";
            //
            // lblTo
            //
            this.lblTo.AutoSize = true;
            this.lblTo.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblTo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblTo.Location = new System.Drawing.Point(190, 34);
            this.lblTo.TabIndex = 6;
            this.lblTo.Text = "Sessions to";
            this.lblTo.Name = "lblTo";
            //
            // lblToStar
            //
            this.lblToStar.AutoSize = true;
            this.lblToStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblToStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblToStar.Location = new System.Drawing.Point(286, 34);
            this.lblToStar.Size = new System.Drawing.Size(14, 24);
            this.lblToStar.TabIndex = 7;
            this.lblToStar.Text = "*";
            this.lblToStar.Name = "lblToStar";
            //
            // dtpTo
            //
            this.dtpTo.AccessibleName = "Sessions to (required)";
            this.dtpTo.CustomFormat = "dd/MM/yyyy";
            this.dtpTo.Font = new System.Drawing.Font("Consolas", 10F);
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTo.Location = new System.Drawing.Point(190, 60);
            this.dtpTo.Size = new System.Drawing.Size(150, 23);
            this.dtpTo.TabIndex = 8;
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.ValueChanged += new System.EventHandler(this.Parameter_Changed);
            //
            // lblPeriodNote
            //
            this.lblPeriodNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblPeriodNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblPeriodNote.Location = new System.Drawing.Point(20, 86);
            this.lblPeriodNote.Size = new System.Drawing.Size(330, 16);
            this.lblPeriodNote.TabIndex = 9;
            this.lblPeriodNote.Text = "";
            this.lblPeriodNote.Name = "lblPeriodNote";
            //
            // lblFaculty
            //
            this.lblFaculty.AutoSize = true;
            this.lblFaculty.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblFaculty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblFaculty.Location = new System.Drawing.Point(360, 34);
            this.lblFaculty.TabIndex = 10;
            this.lblFaculty.Text = "Faculty";
            this.lblFaculty.Name = "lblFaculty";
            //
            // cboFaculty
            //
            this.cboFaculty.AccessibleName = "Faculty (optional filter)";
            this.cboFaculty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFaculty.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboFaculty.FormattingEnabled = true;
            this.cboFaculty.Location = new System.Drawing.Point(360, 60);
            this.cboFaculty.Size = new System.Drawing.Size(200, 25);
            this.cboFaculty.TabIndex = 11;
            this.cboFaculty.Name = "cboFaculty";
            this.cboFaculty.SelectedIndexChanged += new System.EventHandler(this.Parameter_Changed);
            //
            // lblFacultyNote
            //
            this.lblFacultyNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFacultyNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblFacultyNote.Location = new System.Drawing.Point(360, 86);
            this.lblFacultyNote.Size = new System.Drawing.Size(210, 16);
            this.lblFacultyNote.TabIndex = 12;
            this.lblFacultyNote.Text = "";
            this.lblFacultyNote.Name = "lblFacultyNote";
            //
            // lblGrade
            //
            this.lblGrade.AutoSize = true;
            this.lblGrade.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblGrade.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblGrade.Location = new System.Drawing.Point(580, 34);
            this.lblGrade.TabIndex = 13;
            this.lblGrade.Text = "Grade";
            this.lblGrade.Name = "lblGrade";
            //
            // cboGrade
            //
            this.cboGrade.AccessibleName = "Grade (optional filter)";
            this.cboGrade.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGrade.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboGrade.FormattingEnabled = true;
            this.cboGrade.Location = new System.Drawing.Point(580, 60);
            this.cboGrade.Size = new System.Drawing.Size(130, 25);
            this.cboGrade.TabIndex = 14;
            this.cboGrade.Name = "cboGrade";
            this.cboGrade.SelectedIndexChanged += new System.EventHandler(this.Parameter_Changed);
            //
            // lblGradeNote
            //
            this.lblGradeNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblGradeNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblGradeNote.Location = new System.Drawing.Point(580, 86);
            this.lblGradeNote.Size = new System.Drawing.Size(140, 16);
            this.lblGradeNote.TabIndex = 15;
            this.lblGradeNote.Text = "";
            this.lblGradeNote.Name = "lblGradeNote";
            //
            // lblOutcome
            //
            this.lblOutcome.AutoSize = true;
            this.lblOutcome.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblOutcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblOutcome.Location = new System.Drawing.Point(20, 108);
            this.lblOutcome.TabIndex = 16;
            this.lblOutcome.Text = "Advisory outcome";
            this.lblOutcome.Name = "lblOutcome";
            //
            // cboOutcome
            //
            this.cboOutcome.AccessibleName = "Advisory outcome (optional filter)";
            this.cboOutcome.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOutcome.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboOutcome.FormattingEnabled = true;
            this.cboOutcome.Location = new System.Drawing.Point(20, 134);
            this.cboOutcome.Size = new System.Drawing.Size(320, 25);
            this.cboOutcome.TabIndex = 17;
            this.cboOutcome.Name = "cboOutcome";
            this.cboOutcome.SelectedIndexChanged += new System.EventHandler(this.Parameter_Changed);
            //
            // lblOutcomeNote
            //
            this.lblOutcomeNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblOutcomeNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblOutcomeNote.Location = new System.Drawing.Point(20, 160);
            this.lblOutcomeNote.Size = new System.Drawing.Size(330, 16);
            this.lblOutcomeNote.TabIndex = 18;
            this.lblOutcomeNote.Text = "";
            this.lblOutcomeNote.Name = "lblOutcomeNote";
            //
            // lblSessionNumber
            //
            this.lblSessionNumber.AutoSize = true;
            this.lblSessionNumber.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSessionNumber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblSessionNumber.Location = new System.Drawing.Point(360, 108);
            this.lblSessionNumber.TabIndex = 19;
            this.lblSessionNumber.Text = "Session number";
            this.lblSessionNumber.Name = "lblSessionNumber";
            //
            // lblSessionNumberStar
            //
            this.lblSessionNumberStar.AutoSize = true;
            this.lblSessionNumberStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSessionNumberStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblSessionNumberStar.Location = new System.Drawing.Point(490, 108);
            this.lblSessionNumberStar.Size = new System.Drawing.Size(14, 24);
            this.lblSessionNumberStar.TabIndex = 20;
            this.lblSessionNumberStar.Text = "*";
            this.lblSessionNumberStar.Name = "lblSessionNumberStar";
            //
            // txtSessionNumber
            //
            this.txtSessionNumber.AccessibleName = "Session number, yyyy-NNNN (required for FP-LSR)";
            this.txtSessionNumber.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtSessionNumber.Location = new System.Drawing.Point(360, 134);
            this.txtSessionNumber.MaxLength = 9;
            this.txtSessionNumber.Size = new System.Drawing.Size(140, 25);
            this.txtSessionNumber.TabIndex = 21;
            this.txtSessionNumber.Name = "txtSessionNumber";
            this.txtSessionNumber.TextChanged += new System.EventHandler(this.Parameter_Changed);
            this.txtSessionNumber.Leave += new System.EventHandler(this.txtSessionNumber_Leave);
            //
            // lblSessionNumberNote
            //
            this.lblSessionNumberNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSessionNumberNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblSessionNumberNote.Location = new System.Drawing.Point(360, 160);
            this.lblSessionNumberNote.Size = new System.Drawing.Size(150, 16);
            this.lblSessionNumberNote.TabIndex = 22;
            this.lblSessionNumberNote.Text = "";
            this.lblSessionNumberNote.Name = "lblSessionNumberNote";
            //
            // lblSessionNumberError
            //
            this.lblSessionNumberError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSessionNumberError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblSessionNumberError.Location = new System.Drawing.Point(360, 160);
            this.lblSessionNumberError.Size = new System.Drawing.Size(155, 16);
            this.lblSessionNumberError.TabIndex = 23;
            this.lblSessionNumberError.Text = "";
            this.lblSessionNumberError.Visible = false;
            this.lblSessionNumberError.Name = "lblSessionNumberError";
            //
            // chkIncludeInactive
            //
            this.chkIncludeInactive.AccessibleName = "Include inactive programmes";
            this.chkIncludeInactive.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkIncludeInactive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.chkIncludeInactive.Location = new System.Drawing.Point(520, 134);
            this.chkIncludeInactive.Size = new System.Drawing.Size(200, 24);
            this.chkIncludeInactive.TabIndex = 24;
            this.chkIncludeInactive.Text = "Include inactive programmes";
            this.chkIncludeInactive.UseVisualStyleBackColor = true;
            this.chkIncludeInactive.Name = "chkIncludeInactive";
            //
            // lblIncludeInactiveNote
            //
            this.lblIncludeInactiveNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblIncludeInactiveNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblIncludeInactiveNote.Location = new System.Drawing.Point(520, 160);
            this.lblIncludeInactiveNote.Size = new System.Drawing.Size(200, 16);
            this.lblIncludeInactiveNote.TabIndex = 25;
            this.lblIncludeInactiveNote.Text = "";
            this.lblIncludeInactiveNote.Name = "lblIncludeInactiveNote";
            //
            // pnlOutputCard
            //
            this.pnlOutputCard.Controls.Add(this.lblOutputHeadingBar);
            this.pnlOutputCard.Controls.Add(this.lblOutputHeading);
            this.pnlOutputCard.Controls.Add(this.lblOutputFormat);
            this.pnlOutputCard.Controls.Add(this.lblOutputFormatStar);
            this.pnlOutputCard.Controls.Add(this.cboOutputFormat);
            this.pnlOutputCard.Controls.Add(this.lblOutputFormatError);
            this.pnlOutputCard.Controls.Add(this.lblSaveTo);
            this.pnlOutputCard.Controls.Add(this.lblSaveToStar);
            this.pnlOutputCard.Controls.Add(this.txtSaveTo);
            this.pnlOutputCard.Controls.Add(this.lblSaveToError);
            this.pnlOutputCard.Controls.Add(this.btnBrowse);
            this.pnlOutputCard.BackColor = System.Drawing.Color.White;
            this.pnlOutputCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlOutputCard.Location = new System.Drawing.Point(0, 316);
            this.pnlOutputCard.Size = new System.Drawing.Size(734, 104);
            this.pnlOutputCard.TabIndex = 2;
            this.pnlOutputCard.Name = "pnlOutputCard";
            //
            // lblOutputHeadingBar
            //
            this.lblOutputHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblOutputHeadingBar.Location = new System.Drawing.Point(16, 8);
            this.lblOutputHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblOutputHeadingBar.TabIndex = 0;
            this.lblOutputHeadingBar.Name = "lblOutputHeadingBar";
            //
            // lblOutputHeading
            //
            this.lblOutputHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblOutputHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblOutputHeading.Location = new System.Drawing.Point(26, 4);
            this.lblOutputHeading.Size = new System.Drawing.Size(500, 28);
            this.lblOutputHeading.TabIndex = 1;
            this.lblOutputHeading.Text = "Output";
            this.lblOutputHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblOutputHeading.Name = "lblOutputHeading";
            //
            // lblOutputFormat
            //
            this.lblOutputFormat.AutoSize = true;
            this.lblOutputFormat.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblOutputFormat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblOutputFormat.Location = new System.Drawing.Point(20, 34);
            this.lblOutputFormat.TabIndex = 2;
            this.lblOutputFormat.Text = "Output format";
            this.lblOutputFormat.Name = "lblOutputFormat";
            //
            // lblOutputFormatStar
            //
            this.lblOutputFormatStar.AutoSize = true;
            this.lblOutputFormatStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblOutputFormatStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblOutputFormatStar.Location = new System.Drawing.Point(128, 34);
            this.lblOutputFormatStar.Size = new System.Drawing.Size(14, 24);
            this.lblOutputFormatStar.TabIndex = 3;
            this.lblOutputFormatStar.Text = "*";
            this.lblOutputFormatStar.Name = "lblOutputFormatStar";
            //
            // cboOutputFormat
            //
            this.cboOutputFormat.AccessibleName = "Output format (required)";
            this.cboOutputFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOutputFormat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboOutputFormat.FormattingEnabled = true;
            this.cboOutputFormat.Location = new System.Drawing.Point(20, 60);
            this.cboOutputFormat.Size = new System.Drawing.Size(240, 25);
            this.cboOutputFormat.TabIndex = 4;
            this.cboOutputFormat.Name = "cboOutputFormat";
            this.cboOutputFormat.SelectedIndexChanged += new System.EventHandler(this.Parameter_Changed);
            this.cboOutputFormat.Leave += new System.EventHandler(this.cboOutputFormat_Leave);
            //
            // lblOutputFormatError
            //
            this.lblOutputFormatError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblOutputFormatError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblOutputFormatError.Location = new System.Drawing.Point(20, 86);
            this.lblOutputFormatError.Size = new System.Drawing.Size(250, 16);
            this.lblOutputFormatError.TabIndex = 5;
            this.lblOutputFormatError.Text = "";
            this.lblOutputFormatError.Visible = false;
            this.lblOutputFormatError.Name = "lblOutputFormatError";
            //
            // lblSaveTo
            //
            this.lblSaveTo.AutoSize = true;
            this.lblSaveTo.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSaveTo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblSaveTo.Location = new System.Drawing.Point(280, 34);
            this.lblSaveTo.TabIndex = 6;
            this.lblSaveTo.Text = "Save to";
            this.lblSaveTo.Name = "lblSaveTo";
            //
            // lblSaveToStar
            //
            this.lblSaveToStar.AutoSize = true;
            this.lblSaveToStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblSaveToStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblSaveToStar.Location = new System.Drawing.Point(343, 34);
            this.lblSaveToStar.Size = new System.Drawing.Size(14, 24);
            this.lblSaveToStar.TabIndex = 7;
            this.lblSaveToStar.Text = "*";
            this.lblSaveToStar.Name = "lblSaveToStar";
            //
            // txtSaveTo
            //
            this.txtSaveTo.AccessibleName = "Save to folder (required)";
            this.txtSaveTo.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtSaveTo.Location = new System.Drawing.Point(280, 60);
            this.txtSaveTo.MaxLength = 248;
            this.txtSaveTo.Size = new System.Drawing.Size(300, 25);
            this.txtSaveTo.TabIndex = 8;
            this.txtSaveTo.Name = "txtSaveTo";
            this.txtSaveTo.TextChanged += new System.EventHandler(this.Parameter_Changed);
            this.txtSaveTo.Leave += new System.EventHandler(this.txtSaveTo_Leave);
            //
            // lblSaveToError
            //
            this.lblSaveToError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSaveToError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblSaveToError.Location = new System.Drawing.Point(280, 86);
            this.lblSaveToError.Size = new System.Drawing.Size(430, 16);
            this.lblSaveToError.TabIndex = 9;
            this.lblSaveToError.Text = "";
            this.lblSaveToError.Visible = false;
            this.lblSaveToError.Name = "lblSaveToError";
            //
            // btnBrowse
            //
            this.btnBrowse.AccessibleName = "Browse";
            this.btnBrowse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.btnBrowse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnBrowse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnBrowse.Location = new System.Drawing.Point(594, 58);
            this.btnBrowse.Size = new System.Drawing.Size(120, 30);
            this.btnBrowse.TabIndex = 10;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.FlatAppearance.BorderSize = 0;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            //
            // pnlReadiness
            //
            this.pnlReadiness.Controls.Add(this.lblReadyReport);
            this.pnlReadiness.Controls.Add(this.lblReadyScope);
            this.pnlReadiness.Controls.Add(this.lblReadyChip);
            this.pnlReadiness.Controls.Add(this.pnlReadinessBar);
            this.pnlReadiness.BackColor = System.Drawing.Color.White;
            this.pnlReadiness.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlReadiness.Location = new System.Drawing.Point(0, 428);
            this.pnlReadiness.Size = new System.Drawing.Size(734, 55);
            this.pnlReadiness.TabIndex = 3;
            this.pnlReadiness.Name = "pnlReadiness";
            //
            // lblReadyReport
            //
            this.lblReadyReport.AccessibleName = "Report";
            this.lblReadyReport.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblReadyReport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblReadyReport.Location = new System.Drawing.Point(18, 5);
            this.lblReadyReport.Size = new System.Drawing.Size(520, 22);
            this.lblReadyReport.TabIndex = 0;
            this.lblReadyReport.Text = "";
            this.lblReadyReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblReadyReport.Name = "lblReadyReport";
            //
            // lblReadyScope
            //
            this.lblReadyScope.AccessibleName = "Report scope";
            this.lblReadyScope.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblReadyScope.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblReadyScope.Location = new System.Drawing.Point(18, 28);
            this.lblReadyScope.Size = new System.Drawing.Size(540, 18);
            this.lblReadyScope.TabIndex = 1;
            this.lblReadyScope.Text = "";
            this.lblReadyScope.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblReadyScope.Name = "lblReadyScope";
            //
            // lblReadyChip
            //
            this.lblReadyChip.AccessibleName = "Readiness";
            this.lblReadyChip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(243)))), ((int)(((byte)(224)))));
            this.lblReadyChip.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReadyChip.Font = new System.Drawing.Font("Segoe UI Semibold", 8F);
            this.lblReadyChip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblReadyChip.Location = new System.Drawing.Point(560, 13);
            this.lblReadyChip.Size = new System.Drawing.Size(156, 26);
            this.lblReadyChip.TabIndex = 2;
            this.lblReadyChip.Text = "";
            this.lblReadyChip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblReadyChip.Name = "lblReadyChip";
            //
            // pnlReadinessBar
            //
            this.pnlReadinessBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.pnlReadinessBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlReadinessBar.Location = new System.Drawing.Point(0, 0);
            this.pnlReadinessBar.Size = new System.Drawing.Size(5, 53);
            this.pnlReadinessBar.TabIndex = 3;
            this.pnlReadinessBar.Name = "pnlReadinessBar";
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
            this.lblTitle.Text = "Reports";
            this.lblTitle.Name = "lblTitle";
            //
            // lblSubtitle
            //
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(235)))));
            this.lblSubtitle.Location = new System.Drawing.Point(24, 54);
            this.lblSubtitle.Size = new System.Drawing.Size(714, 18);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Choose a report, set its parameters, then preview or generate it";
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
            this.lblBadge.Text = "CAREER ADVISOR";
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
            this.pnlActions.Controls.Add(this.btnGenerate);
            this.pnlActions.Controls.Add(this.btnPreview);
            this.pnlActions.Controls.Add(this.btnCancel);
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
            // btnGenerate
            //
            this.btnGenerate.AccessibleName = "Generate report";
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.btnGenerate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerate.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.Location = new System.Drawing.Point(574, 12);
            this.btnGenerate.Size = new System.Drawing.Size(180, 40);
            this.btnGenerate.TabIndex = 1;
            this.btnGenerate.Text = "Generate report";
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.FlatAppearance.BorderSize = 0;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            //
            // btnPreview
            //
            this.btnPreview.AccessibleName = "Preview";
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreview.BackColor = System.Drawing.Color.White;
            this.btnPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPreview.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnPreview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.btnPreview.Location = new System.Drawing.Point(434, 12);
            this.btnPreview.Size = new System.Drawing.Size(130, 40);
            this.btnPreview.TabIndex = 2;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
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
            this.btnCancel.Location = new System.Drawing.Point(294, 12);
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
            this.lblActionHint.Size = new System.Drawing.Size(254, 40);
            this.lblActionHint.TabIndex = 4;
            this.lblActionHint.Text = "Career Advisors reach these three reports from UI 10.3c";
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
            this.btnNavReports.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(64)))), ((int)(((byte)(104)))));
            this.btnNavReports.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavReports.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavReports.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnNavReports.ForeColor = System.Drawing.Color.White;
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
            // ReportGenerationForm
            //
            this.AcceptButton = this.btnGenerate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(984, 681);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Future Path \u2014 Reports";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlTitle);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlRail);
            this.Controls.Add(this.pnlStatus);
            this.Name = "ReportGenerationForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ReportGenerationForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ReportGenerationForm_FormClosed);
            this.pnlBody.ResumeLayout(false);
            this.pnlBody.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.pnlReportCard.ResumeLayout(false);
            this.pnlReportCard.PerformLayout();
            this.lblReportHeadingBar.ResumeLayout(false);
            this.lblReportHeadingBar.PerformLayout();
            this.pnlParameterCard.ResumeLayout(false);
            this.pnlParameterCard.PerformLayout();
            this.lblParameterHeadingBar.ResumeLayout(false);
            this.lblParameterHeadingBar.PerformLayout();
            this.pnlOutputCard.ResumeLayout(false);
            this.pnlOutputCard.PerformLayout();
            this.lblOutputHeadingBar.ResumeLayout(false);
            this.lblOutputHeadingBar.PerformLayout();
            this.pnlReadiness.ResumeLayout(false);
            this.pnlReadiness.PerformLayout();
            this.pnlReadinessBar.ResumeLayout(false);
            this.pnlReadinessBar.PerformLayout();
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
        private System.Windows.Forms.Panel pnlReportCard;
        private System.Windows.Forms.Panel lblReportHeadingBar;
        private System.Windows.Forms.Label lblReportHeading;
        private System.Windows.Forms.RadioButton rdoLearnerSummary;
        private System.Windows.Forms.Label lblLearnerSummaryNote;
        private System.Windows.Forms.RadioButton rdoDegreeInterest;
        private System.Windows.Forms.Label lblDegreeInterestNote;
        private System.Windows.Forms.RadioButton rdoNotEligible;
        private System.Windows.Forms.Label lblNotEligibleNote;
        private System.Windows.Forms.Panel pnlParameterCard;
        private System.Windows.Forms.Panel lblParameterHeadingBar;
        private System.Windows.Forms.Label lblParameterHeading;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblFromStar;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label lblFromError;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Label lblToStar;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label lblPeriodNote;
        private System.Windows.Forms.Label lblFaculty;
        private System.Windows.Forms.ComboBox cboFaculty;
        private System.Windows.Forms.Label lblFacultyNote;
        private System.Windows.Forms.Label lblGrade;
        private System.Windows.Forms.ComboBox cboGrade;
        private System.Windows.Forms.Label lblGradeNote;
        private System.Windows.Forms.Label lblOutcome;
        private System.Windows.Forms.ComboBox cboOutcome;
        private System.Windows.Forms.Label lblOutcomeNote;
        private System.Windows.Forms.Label lblSessionNumber;
        private System.Windows.Forms.Label lblSessionNumberStar;
        private System.Windows.Forms.TextBox txtSessionNumber;
        private System.Windows.Forms.Label lblSessionNumberNote;
        private System.Windows.Forms.Label lblSessionNumberError;
        private System.Windows.Forms.CheckBox chkIncludeInactive;
        private System.Windows.Forms.Label lblIncludeInactiveNote;
        private System.Windows.Forms.Panel pnlOutputCard;
        private System.Windows.Forms.Panel lblOutputHeadingBar;
        private System.Windows.Forms.Label lblOutputHeading;
        private System.Windows.Forms.Label lblOutputFormat;
        private System.Windows.Forms.Label lblOutputFormatStar;
        private System.Windows.Forms.ComboBox cboOutputFormat;
        private System.Windows.Forms.Label lblOutputFormatError;
        private System.Windows.Forms.Label lblSaveTo;
        private System.Windows.Forms.Label lblSaveToStar;
        private System.Windows.Forms.TextBox txtSaveTo;
        private System.Windows.Forms.Label lblSaveToError;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Panel pnlReadiness;
        private System.Windows.Forms.Label lblReadyReport;
        private System.Windows.Forms.Label lblReadyScope;
        private System.Windows.Forms.Label lblReadyChip;
        private System.Windows.Forms.Panel pnlReadinessBar;
        private System.Windows.Forms.Panel pnlTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblBadge;
        private System.Windows.Forms.Panel pnlTitleRule;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Panel pnlActionRule;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnPreview;
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
