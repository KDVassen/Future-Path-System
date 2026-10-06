namespace FuturePath.PresentationLayer
{
    partial class QuestionnaireTab
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
            System.Windows.Forms.DataGridViewCellStyle grdQuestionsAltStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle grdQuestionsHeaderStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle grdQuestionsCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colDisplayOrderStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlScopeCard = new System.Windows.Forms.Panel();
            this.lblScopeHeadingBar = new System.Windows.Forms.Panel();
            this.lblScopeHeading = new System.Windows.Forms.Label();
            this.lblScopeCategory = new System.Windows.Forms.Label();
            this.cboScopeCategory = new System.Windows.Forms.ComboBox();
            this.lnkShowAll = new System.Windows.Forms.LinkLabel();
            this.lblScaleNote = new System.Windows.Forms.Label();
            this.lblScopeNote = new System.Windows.Forms.Label();
            this.pnlQuestionsCard = new System.Windows.Forms.Panel();
            this.lblQuestionsHeadingBar = new System.Windows.Forms.Panel();
            this.lblQuestionsHeading = new System.Windows.Forms.Label();
            this.lblQuestionCount = new System.Windows.Forms.Label();
            this.grdQuestions = new System.Windows.Forms.DataGridView();
            this.colQuestionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuestionCategoryId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDisplayOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuestionText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuestionCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuestionStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlQuestionFormCard = new System.Windows.Forms.Panel();
            this.lblQuestionFormHeadingBar = new System.Windows.Forms.Panel();
            this.lblQuestionFormHeading = new System.Windows.Forms.Label();
            this.lblStatementLength = new System.Windows.Forms.Label();
            this.lblStatement = new System.Windows.Forms.Label();
            this.lblStatementStar = new System.Windows.Forms.Label();
            this.txtStatement = new System.Windows.Forms.TextBox();
            this.lblStatementError = new System.Windows.Forms.Label();
            this.lblQuestionCategory = new System.Windows.Forms.Label();
            this.lblQuestionCategoryStar = new System.Windows.Forms.Label();
            this.cboQuestionCategory = new System.Windows.Forms.ComboBox();
            this.lblQuestionCategoryError = new System.Windows.Forms.Label();
            this.lblDisplayOrder = new System.Windows.Forms.Label();
            this.lblDisplayOrderStar = new System.Windows.Forms.Label();
            this.numDisplayOrder = new System.Windows.Forms.NumericUpDown();
            this.lblDisplayOrderError = new System.Windows.Forms.Label();
            this.chkQuestionActive = new System.Windows.Forms.CheckBox();
            this.pnlQuestionSummary = new System.Windows.Forms.Panel();
            this.lblQuestionSummary = new System.Windows.Forms.Label();
            this.pnlScopeCard.SuspendLayout();
            this.lblScopeHeadingBar.SuspendLayout();
            this.pnlQuestionsCard.SuspendLayout();
            this.lblQuestionsHeadingBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdQuestions)).BeginInit();
            this.pnlQuestionFormCard.SuspendLayout();
            this.lblQuestionFormHeadingBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDisplayOrder)).BeginInit();
            this.pnlQuestionSummary.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlScopeCard
            //
            this.pnlScopeCard.Controls.Add(this.lblScopeHeadingBar);
            this.pnlScopeCard.Controls.Add(this.lblScopeHeading);
            this.pnlScopeCard.Controls.Add(this.lblScopeCategory);
            this.pnlScopeCard.Controls.Add(this.cboScopeCategory);
            this.pnlScopeCard.Controls.Add(this.lnkShowAll);
            this.pnlScopeCard.Controls.Add(this.lblScaleNote);
            this.pnlScopeCard.Controls.Add(this.lblScopeNote);
            this.pnlScopeCard.BackColor = System.Drawing.Color.White;
            this.pnlScopeCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlScopeCard.Location = new System.Drawing.Point(0, 0);
            this.pnlScopeCard.Size = new System.Drawing.Size(200, 527);
            this.pnlScopeCard.TabIndex = 0;
            this.pnlScopeCard.Name = "pnlScopeCard";
            //
            // lblScopeHeadingBar
            //
            this.lblScopeHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblScopeHeadingBar.Location = new System.Drawing.Point(12, 12);
            this.lblScopeHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblScopeHeadingBar.TabIndex = 0;
            this.lblScopeHeadingBar.Name = "lblScopeHeadingBar";
            //
            // lblScopeHeading
            //
            this.lblScopeHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblScopeHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblScopeHeading.Location = new System.Drawing.Point(22, 8);
            this.lblScopeHeading.Size = new System.Drawing.Size(170, 28);
            this.lblScopeHeading.TabIndex = 1;
            this.lblScopeHeading.Text = "Scope";
            this.lblScopeHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblScopeHeading.Name = "lblScopeHeading";
            //
            // lblScopeCategory
            //
            this.lblScopeCategory.AutoSize = true;
            this.lblScopeCategory.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblScopeCategory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblScopeCategory.Location = new System.Drawing.Point(16, 44);
            this.lblScopeCategory.TabIndex = 2;
            this.lblScopeCategory.Text = "Category";
            this.lblScopeCategory.Name = "lblScopeCategory";
            //
            // cboScopeCategory
            //
            this.cboScopeCategory.AccessibleName = "Show questions in this interest category";
            this.cboScopeCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboScopeCategory.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboScopeCategory.FormattingEnabled = true;
            this.cboScopeCategory.Location = new System.Drawing.Point(16, 70);
            this.cboScopeCategory.Size = new System.Drawing.Size(166, 25);
            this.cboScopeCategory.TabIndex = 3;
            this.cboScopeCategory.Name = "cboScopeCategory";
            this.cboScopeCategory.SelectionChangeCommitted += new System.EventHandler(this.cboScopeCategory_SelectionChangeCommitted);
            //
            // lnkShowAll
            //
            this.lnkShowAll.AccessibleName = "Show questions in all categories";
            this.lnkShowAll.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.lnkShowAll.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lnkShowAll.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.lnkShowAll.Location = new System.Drawing.Point(16, 102);
            this.lnkShowAll.Size = new System.Drawing.Size(166, 22);
            this.lnkShowAll.TabIndex = 4;
            this.lnkShowAll.TabStop = true;
            this.lnkShowAll.Text = "Show all categories";
            this.lnkShowAll.Name = "lnkShowAll";
            this.lnkShowAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkShowAll_LinkClicked);
            //
            // lblScaleNote
            //
            this.lblScaleNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblScaleNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.lblScaleNote.Location = new System.Drawing.Point(16, 140);
            this.lblScaleNote.Size = new System.Drawing.Size(170, 150);
            this.lblScaleNote.TabIndex = 5;
            this.lblScaleNote.Text = "Learners answer every active question on UI 4 on a 1\u20135 scale, from Strongly disagree (1) to Strongly agree (5). Questions appear in display order, with their category beneath.";
            this.lblScaleNote.Name = "lblScaleNote";
            //
            // lblScopeNote
            //
            this.lblScopeNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblScopeNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblScopeNote.Location = new System.Drawing.Point(16, 300);
            this.lblScopeNote.Size = new System.Drawing.Size(170, 70);
            this.lblScopeNote.TabIndex = 6;
            this.lblScopeNote.Text = "Inactive questions stay listed here for administrators and are never shown to learners.";
            this.lblScopeNote.Name = "lblScopeNote";
            //
            // pnlQuestionsCard
            //
            this.pnlQuestionsCard.Controls.Add(this.lblQuestionsHeadingBar);
            this.pnlQuestionsCard.Controls.Add(this.lblQuestionsHeading);
            this.pnlQuestionsCard.Controls.Add(this.lblQuestionCount);
            this.pnlQuestionsCard.Controls.Add(this.grdQuestions);
            this.pnlQuestionsCard.BackColor = System.Drawing.Color.White;
            this.pnlQuestionsCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlQuestionsCard.Location = new System.Drawing.Point(212, 0);
            this.pnlQuestionsCard.Size = new System.Drawing.Size(714, 232);
            this.pnlQuestionsCard.TabIndex = 1;
            this.pnlQuestionsCard.Name = "pnlQuestionsCard";
            //
            // lblQuestionsHeadingBar
            //
            this.lblQuestionsHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblQuestionsHeadingBar.Location = new System.Drawing.Point(16, 12);
            this.lblQuestionsHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblQuestionsHeadingBar.TabIndex = 0;
            this.lblQuestionsHeadingBar.Name = "lblQuestionsHeadingBar";
            //
            // lblQuestionsHeading
            //
            this.lblQuestionsHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblQuestionsHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblQuestionsHeading.Location = new System.Drawing.Point(26, 8);
            this.lblQuestionsHeading.Size = new System.Drawing.Size(400, 28);
            this.lblQuestionsHeading.TabIndex = 1;
            this.lblQuestionsHeading.Text = "Stored questions";
            this.lblQuestionsHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblQuestionsHeading.Name = "lblQuestionsHeading";
            //
            // lblQuestionCount
            //
            this.lblQuestionCount.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblQuestionCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblQuestionCount.Location = new System.Drawing.Point(380, 12);
            this.lblQuestionCount.Size = new System.Drawing.Size(316, 20);
            this.lblQuestionCount.TabIndex = 2;
            this.lblQuestionCount.Text = "";
            this.lblQuestionCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblQuestionCount.Name = "lblQuestionCount";
            //
            // grdQuestions
            //
            this.grdQuestions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colQuestionId,
            this.colQuestionCategoryId,
            this.colDisplayOrder,
            this.colQuestionText,
            this.colQuestionCategory,
            this.colQuestionStatus});
            grdQuestionsAltStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.grdQuestions.AlternatingRowsDefaultCellStyle = grdQuestionsAltStyle;
            grdQuestionsHeaderStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grdQuestionsHeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            grdQuestionsHeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            grdQuestionsHeaderStyle.ForeColor = System.Drawing.Color.White;
            grdQuestionsHeaderStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            grdQuestionsHeaderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            grdQuestionsHeaderStyle.SelectionForeColor = System.Drawing.Color.White;
            grdQuestionsHeaderStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdQuestions.ColumnHeadersDefaultCellStyle = grdQuestionsHeaderStyle;
            grdQuestionsCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grdQuestionsCellStyle.BackColor = System.Drawing.Color.White;
            grdQuestionsCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            grdQuestionsCellStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            grdQuestionsCellStyle.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            grdQuestionsCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            grdQuestionsCellStyle.SelectionForeColor = System.Drawing.Color.White;
            grdQuestionsCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grdQuestions.DefaultCellStyle = grdQuestionsCellStyle;
            this.grdQuestions.AccessibleName = "Stored questionnaire questions";
            this.grdQuestions.AllowUserToAddRows = false;
            this.grdQuestions.AllowUserToDeleteRows = false;
            this.grdQuestions.AllowUserToResizeRows = false;
            this.grdQuestions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdQuestions.BackgroundColor = System.Drawing.Color.White;
            this.grdQuestions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.grdQuestions.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grdQuestions.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.grdQuestions.ColumnHeadersHeight = 34;
            this.grdQuestions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdQuestions.EnableHeadersVisualStyles = false;
            this.grdQuestions.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            this.grdQuestions.Location = new System.Drawing.Point(16, 44);
            this.grdQuestions.MultiSelect = false;
            this.grdQuestions.ReadOnly = true;
            this.grdQuestions.RowHeadersVisible = false;
            this.grdQuestions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdQuestions.Size = new System.Drawing.Size(680, 172);
            this.grdQuestions.TabIndex = 3;
            this.grdQuestions.Name = "grdQuestions";
            this.grdQuestions.RowTemplate.Height = 30;
            this.grdQuestions.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.grdQuestions_CellFormatting);
            this.grdQuestions.SelectionChanged += new System.EventHandler(this.grdQuestions_SelectionChanged);
            this.grdQuestions.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.grdQuestions_DataBindingComplete);
            //
            // colQuestionId
            //
            this.colQuestionId.DataPropertyName = "QuestionID";
            this.colQuestionId.FillWeight = 5F;
            this.colQuestionId.HeaderText = "Question ID";
            this.colQuestionId.ReadOnly = true;
            this.colQuestionId.Visible = false;
            this.colQuestionId.Name = "colQuestionId";
            //
            // colQuestionCategoryId
            //
            this.colQuestionCategoryId.DataPropertyName = "CategoryID";
            this.colQuestionCategoryId.FillWeight = 5F;
            this.colQuestionCategoryId.HeaderText = "Category ID";
            this.colQuestionCategoryId.ReadOnly = true;
            this.colQuestionCategoryId.Visible = false;
            this.colQuestionCategoryId.Name = "colQuestionCategoryId";
            //
            // colDisplayOrder
            //
            colDisplayOrderStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            colDisplayOrderStyle.Font = new System.Drawing.Font("Consolas", 10F);
            this.colDisplayOrder.DefaultCellStyle = colDisplayOrderStyle;
            this.colDisplayOrder.DataPropertyName = "DisplayOrder";
            this.colDisplayOrder.FillWeight = 10F;
            this.colDisplayOrder.HeaderText = "Order";
            this.colDisplayOrder.ReadOnly = true;
            this.colDisplayOrder.Name = "colDisplayOrder";
            //
            // colQuestionText
            //
            this.colQuestionText.DataPropertyName = "QuestionText";
            this.colQuestionText.FillWeight = 60F;
            this.colQuestionText.HeaderText = "Statement";
            this.colQuestionText.ReadOnly = true;
            this.colQuestionText.Name = "colQuestionText";
            //
            // colQuestionCategory
            //
            this.colQuestionCategory.DataPropertyName = "CategoryName";
            this.colQuestionCategory.FillWeight = 22F;
            this.colQuestionCategory.HeaderText = "Category";
            this.colQuestionCategory.ReadOnly = true;
            this.colQuestionCategory.Name = "colQuestionCategory";
            //
            // colQuestionStatus
            //
            this.colQuestionStatus.DataPropertyName = "IsActive";
            this.colQuestionStatus.FillWeight = 14F;
            this.colQuestionStatus.HeaderText = "Status";
            this.colQuestionStatus.ReadOnly = true;
            this.colQuestionStatus.Name = "colQuestionStatus";
            //
            // pnlQuestionFormCard
            //
            this.pnlQuestionFormCard.Controls.Add(this.lblQuestionFormHeadingBar);
            this.pnlQuestionFormCard.Controls.Add(this.lblQuestionFormHeading);
            this.pnlQuestionFormCard.Controls.Add(this.lblStatementLength);
            this.pnlQuestionFormCard.Controls.Add(this.lblStatement);
            this.pnlQuestionFormCard.Controls.Add(this.lblStatementStar);
            this.pnlQuestionFormCard.Controls.Add(this.txtStatement);
            this.pnlQuestionFormCard.Controls.Add(this.lblStatementError);
            this.pnlQuestionFormCard.Controls.Add(this.lblQuestionCategory);
            this.pnlQuestionFormCard.Controls.Add(this.lblQuestionCategoryStar);
            this.pnlQuestionFormCard.Controls.Add(this.cboQuestionCategory);
            this.pnlQuestionFormCard.Controls.Add(this.lblQuestionCategoryError);
            this.pnlQuestionFormCard.Controls.Add(this.lblDisplayOrder);
            this.pnlQuestionFormCard.Controls.Add(this.lblDisplayOrderStar);
            this.pnlQuestionFormCard.Controls.Add(this.numDisplayOrder);
            this.pnlQuestionFormCard.Controls.Add(this.lblDisplayOrderError);
            this.pnlQuestionFormCard.Controls.Add(this.chkQuestionActive);
            this.pnlQuestionFormCard.Controls.Add(this.pnlQuestionSummary);
            this.pnlQuestionFormCard.BackColor = System.Drawing.Color.White;
            this.pnlQuestionFormCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlQuestionFormCard.Location = new System.Drawing.Point(212, 244);
            this.pnlQuestionFormCard.Size = new System.Drawing.Size(714, 283);
            this.pnlQuestionFormCard.TabIndex = 2;
            this.pnlQuestionFormCard.Name = "pnlQuestionFormCard";
            //
            // lblQuestionFormHeadingBar
            //
            this.lblQuestionFormHeadingBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(137)))), ((int)(((byte)(43)))));
            this.lblQuestionFormHeadingBar.Location = new System.Drawing.Point(16, 12);
            this.lblQuestionFormHeadingBar.Size = new System.Drawing.Size(4, 20);
            this.lblQuestionFormHeadingBar.TabIndex = 0;
            this.lblQuestionFormHeadingBar.Name = "lblQuestionFormHeadingBar";
            //
            // lblQuestionFormHeading
            //
            this.lblQuestionFormHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblQuestionFormHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblQuestionFormHeading.Location = new System.Drawing.Point(26, 8);
            this.lblQuestionFormHeading.Size = new System.Drawing.Size(400, 28);
            this.lblQuestionFormHeading.TabIndex = 1;
            this.lblQuestionFormHeading.Text = "Add or edit a question";
            this.lblQuestionFormHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblQuestionFormHeading.Name = "lblQuestionFormHeading";
            //
            // lblStatementLength
            //
            this.lblStatementLength.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblStatementLength.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(96)))), ((int)(((byte)(108)))));
            this.lblStatementLength.Location = new System.Drawing.Point(560, 50);
            this.lblStatementLength.Size = new System.Drawing.Size(134, 18);
            this.lblStatementLength.TabIndex = 2;
            this.lblStatementLength.Text = "0 / 255";
            this.lblStatementLength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblStatementLength.Name = "lblStatementLength";
            //
            // lblStatement
            //
            this.lblStatement.AutoSize = true;
            this.lblStatement.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblStatement.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblStatement.Location = new System.Drawing.Point(20, 44);
            this.lblStatement.TabIndex = 3;
            this.lblStatement.Text = "Statement";
            this.lblStatement.Name = "lblStatement";
            //
            // lblStatementStar
            //
            this.lblStatementStar.AutoSize = true;
            this.lblStatementStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblStatementStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblStatementStar.Location = new System.Drawing.Point(103, 44);
            this.lblStatementStar.Size = new System.Drawing.Size(14, 24);
            this.lblStatementStar.TabIndex = 4;
            this.lblStatementStar.Text = "*";
            this.lblStatementStar.Name = "lblStatementStar";
            //
            // txtStatement
            //
            this.txtStatement.AccessibleName = "Statement (required, up to 255 characters)";
            this.txtStatement.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtStatement.Location = new System.Drawing.Point(20, 70);
            this.txtStatement.MaxLength = 255;
            this.txtStatement.Multiline = true;
            this.txtStatement.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatement.Size = new System.Drawing.Size(674, 50);
            this.txtStatement.TabIndex = 5;
            this.txtStatement.Name = "txtStatement";
            this.txtStatement.Leave += new System.EventHandler(this.txtStatement_Leave);
            this.txtStatement.TextChanged += new System.EventHandler(this.txtStatement_TextChanged);
            //
            // lblStatementError
            //
            this.lblStatementError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblStatementError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblStatementError.Location = new System.Drawing.Point(20, 122);
            this.lblStatementError.Size = new System.Drawing.Size(674, 16);
            this.lblStatementError.TabIndex = 6;
            this.lblStatementError.Text = "";
            this.lblStatementError.Visible = false;
            this.lblStatementError.Name = "lblStatementError";
            //
            // lblQuestionCategory
            //
            this.lblQuestionCategory.AutoSize = true;
            this.lblQuestionCategory.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblQuestionCategory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblQuestionCategory.Location = new System.Drawing.Point(20, 142);
            this.lblQuestionCategory.TabIndex = 7;
            this.lblQuestionCategory.Text = "Category";
            this.lblQuestionCategory.Name = "lblQuestionCategory";
            //
            // lblQuestionCategoryStar
            //
            this.lblQuestionCategoryStar.AutoSize = true;
            this.lblQuestionCategoryStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblQuestionCategoryStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblQuestionCategoryStar.Location = new System.Drawing.Point(92, 142);
            this.lblQuestionCategoryStar.Size = new System.Drawing.Size(14, 24);
            this.lblQuestionCategoryStar.TabIndex = 8;
            this.lblQuestionCategoryStar.Text = "*";
            this.lblQuestionCategoryStar.Name = "lblQuestionCategoryStar";
            //
            // cboQuestionCategory
            //
            this.cboQuestionCategory.AccessibleName = "Interest category of the statement (required)";
            this.cboQuestionCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboQuestionCategory.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboQuestionCategory.FormattingEnabled = true;
            this.cboQuestionCategory.Location = new System.Drawing.Point(20, 168);
            this.cboQuestionCategory.Size = new System.Drawing.Size(260, 25);
            this.cboQuestionCategory.TabIndex = 9;
            this.cboQuestionCategory.Name = "cboQuestionCategory";
            this.cboQuestionCategory.Leave += new System.EventHandler(this.cboQuestionCategory_Leave);
            //
            // lblQuestionCategoryError
            //
            this.lblQuestionCategoryError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblQuestionCategoryError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblQuestionCategoryError.Location = new System.Drawing.Point(20, 195);
            this.lblQuestionCategoryError.Size = new System.Drawing.Size(260, 16);
            this.lblQuestionCategoryError.TabIndex = 10;
            this.lblQuestionCategoryError.Text = "";
            this.lblQuestionCategoryError.Visible = false;
            this.lblQuestionCategoryError.Name = "lblQuestionCategoryError";
            //
            // lblDisplayOrder
            //
            this.lblDisplayOrder.AutoSize = true;
            this.lblDisplayOrder.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblDisplayOrder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.lblDisplayOrder.Location = new System.Drawing.Point(300, 142);
            this.lblDisplayOrder.TabIndex = 11;
            this.lblDisplayOrder.Text = "Display order";
            this.lblDisplayOrder.Name = "lblDisplayOrder";
            //
            // lblDisplayOrderStar
            //
            this.lblDisplayOrderStar.AutoSize = true;
            this.lblDisplayOrderStar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblDisplayOrderStar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblDisplayOrderStar.Location = new System.Drawing.Point(407, 142);
            this.lblDisplayOrderStar.Size = new System.Drawing.Size(14, 24);
            this.lblDisplayOrderStar.TabIndex = 12;
            this.lblDisplayOrderStar.Text = "*";
            this.lblDisplayOrderStar.Name = "lblDisplayOrderStar";
            //
            // numDisplayOrder
            //
            this.numDisplayOrder.AccessibleName = "Display order, 1 to 99 (required)";
            this.numDisplayOrder.Font = new System.Drawing.Font("Consolas", 10F);
            this.numDisplayOrder.Location = new System.Drawing.Point(300, 168);
            this.numDisplayOrder.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            this.numDisplayOrder.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numDisplayOrder.Size = new System.Drawing.Size(110, 23);
            this.numDisplayOrder.TabIndex = 13;
            this.numDisplayOrder.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.numDisplayOrder.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.numDisplayOrder.Name = "numDisplayOrder";
            this.numDisplayOrder.Leave += new System.EventHandler(this.numDisplayOrder_Leave);
            //
            // lblDisplayOrderError
            //
            this.lblDisplayOrderError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblDisplayOrderError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblDisplayOrderError.Location = new System.Drawing.Point(300, 195);
            this.lblDisplayOrderError.Size = new System.Drawing.Size(240, 16);
            this.lblDisplayOrderError.TabIndex = 14;
            this.lblDisplayOrderError.Text = "";
            this.lblDisplayOrderError.Visible = false;
            this.lblDisplayOrderError.Name = "lblDisplayOrderError";
            //
            // chkQuestionActive
            //
            this.chkQuestionActive.AccessibleName = "Active (unticked hides it from new advisory sessions)";
            this.chkQuestionActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkQuestionActive.Checked = true;
            this.chkQuestionActive.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkQuestionActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.chkQuestionActive.Location = new System.Drawing.Point(430, 168);
            this.chkQuestionActive.Size = new System.Drawing.Size(120, 24);
            this.chkQuestionActive.TabIndex = 15;
            this.chkQuestionActive.Text = "Active";
            this.chkQuestionActive.UseVisualStyleBackColor = true;
            this.chkQuestionActive.Name = "chkQuestionActive";
            //
            // pnlQuestionSummary
            //
            this.pnlQuestionSummary.Controls.Add(this.lblQuestionSummary);
            this.pnlQuestionSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.pnlQuestionSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlQuestionSummary.Location = new System.Drawing.Point(20, 212);
            this.pnlQuestionSummary.Size = new System.Drawing.Size(674, 64);
            this.pnlQuestionSummary.TabIndex = 16;
            this.pnlQuestionSummary.Visible = false;
            this.pnlQuestionSummary.Name = "pnlQuestionSummary";
            //
            // lblQuestionSummary
            //
            this.lblQuestionSummary.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblQuestionSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblQuestionSummary.Location = new System.Drawing.Point(12, 6);
            this.lblQuestionSummary.Size = new System.Drawing.Size(648, 52);
            this.lblQuestionSummary.TabIndex = 0;
            this.lblQuestionSummary.Text = "";
            this.lblQuestionSummary.Name = "lblQuestionSummary";
            //
            // QuestionnaireTab
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.Size = new System.Drawing.Size(926, 527);
            this.Controls.Add(this.pnlScopeCard);
            this.Controls.Add(this.pnlQuestionsCard);
            this.Controls.Add(this.pnlQuestionFormCard);
            this.Name = "QuestionnaireTab";
            this.pnlScopeCard.ResumeLayout(false);
            this.pnlScopeCard.PerformLayout();
            this.lblScopeHeadingBar.ResumeLayout(false);
            this.lblScopeHeadingBar.PerformLayout();
            this.pnlQuestionsCard.ResumeLayout(false);
            this.pnlQuestionsCard.PerformLayout();
            this.lblQuestionsHeadingBar.ResumeLayout(false);
            this.lblQuestionsHeadingBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdQuestions)).EndInit();
            this.pnlQuestionFormCard.ResumeLayout(false);
            this.pnlQuestionFormCard.PerformLayout();
            this.lblQuestionFormHeadingBar.ResumeLayout(false);
            this.lblQuestionFormHeadingBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDisplayOrder)).EndInit();
            this.pnlQuestionSummary.ResumeLayout(false);
            this.pnlQuestionSummary.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlScopeCard;
        private System.Windows.Forms.Panel lblScopeHeadingBar;
        private System.Windows.Forms.Label lblScopeHeading;
        private System.Windows.Forms.Label lblScopeCategory;
        private System.Windows.Forms.ComboBox cboScopeCategory;
        private System.Windows.Forms.LinkLabel lnkShowAll;
        private System.Windows.Forms.Label lblScaleNote;
        private System.Windows.Forms.Label lblScopeNote;
        private System.Windows.Forms.Panel pnlQuestionsCard;
        private System.Windows.Forms.Panel lblQuestionsHeadingBar;
        private System.Windows.Forms.Label lblQuestionsHeading;
        private System.Windows.Forms.Label lblQuestionCount;
        private System.Windows.Forms.DataGridView grdQuestions;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuestionId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuestionCategoryId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisplayOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuestionText;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuestionCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuestionStatus;
        private System.Windows.Forms.Panel pnlQuestionFormCard;
        private System.Windows.Forms.Panel lblQuestionFormHeadingBar;
        private System.Windows.Forms.Label lblQuestionFormHeading;
        private System.Windows.Forms.Label lblStatementLength;
        private System.Windows.Forms.Label lblStatement;
        private System.Windows.Forms.Label lblStatementStar;
        private System.Windows.Forms.TextBox txtStatement;
        private System.Windows.Forms.Label lblStatementError;
        private System.Windows.Forms.Label lblQuestionCategory;
        private System.Windows.Forms.Label lblQuestionCategoryStar;
        private System.Windows.Forms.ComboBox cboQuestionCategory;
        private System.Windows.Forms.Label lblQuestionCategoryError;
        private System.Windows.Forms.Label lblDisplayOrder;
        private System.Windows.Forms.Label lblDisplayOrderStar;
        private System.Windows.Forms.NumericUpDown numDisplayOrder;
        private System.Windows.Forms.Label lblDisplayOrderError;
        private System.Windows.Forms.CheckBox chkQuestionActive;
        private System.Windows.Forms.Panel pnlQuestionSummary;
        private System.Windows.Forms.Label lblQuestionSummary;
    }
}
