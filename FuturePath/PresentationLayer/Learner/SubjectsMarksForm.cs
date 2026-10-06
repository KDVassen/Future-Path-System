using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 3 — Subjects and Marks Capture (wide, step 2 of 6; spec 2.3.3). The graded rich dialog.
    /// The learner picks a Subject (bound to active <c>Subject</c> rows), sees its Subject level
    /// (read-only, from <c>Subject.SubjectLevel</c>, decisions.md D4) and enters a Mark % from 0 to 100
    /// with at most one decimal. <b>Add subject</b> validates the entry: a duplicate subject or a bad
    /// mark turns the label and control red and the validation summary lists every entry to fix.
    /// Captured rows are kept in a presentation list, so going Back and returning never loses them.
    /// The amber notice counts the subjects still needed against <c>MIN_SUBJECT_COUNT</c> — a warning,
    /// never a block (decisions.md §3). Leaving with invalid data raises UI 3.1a.
    /// </summary>
    public partial class SubjectsMarksForm : Form
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color RedTint = Color.FromArgb(250, 234, 235);
        private static readonly Color BlueColour = Color.FromArgb(31, 95, 168);
        private static readonly Color BlueTint = Color.FromArgb(232, 238, 246);
        private static readonly Color AmberColour = Color.FromArgb(200, 137, 43);
        private static readonly Color AmberText = Color.FromArgb(138, 93, 24);
        private static readonly Color AmberTint = Color.FromArgb(252, 243, 224);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color GreenTint = Color.FromArgb(230, 242, 234);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);

        // Layout of the captured-subjects card with and without the validation summary above it.
        private const int CapturedTopPlain = 126;
        private const int CapturedTopWithSummary = 206;
        private const int ContentBottom = 459;

        private const string StepStatus = "Step 2 of 6 · Add every subject on your latest report, then save and continue.";
        private const string NoDataText = "—";

        private readonly Form homeForm;
        private readonly Form previousForm;
        private InterestQuestionnaireForm questionnaireForm;

        /// <summary>The rows captured in this session, shown in the grid. Kept while the form is only hidden.</summary>
        private readonly BindingList<CapturedSubjectRow> capturedRows = new BindingList<CapturedSubjectRow>();

        /// <summary>The stored minimum subject count (SystemParameter MIN_SUBJECT_COUNT); null until loaded.</summary>
        private int? minimumSubjectCount;

        /// <summary>
        /// The mark exactly as typed. The NumericUpDown clamps and rounds its text when it loses focus,
        /// so the raw text is remembered while the learner types and validated instead.
        /// </summary>
        private string rawMarkText = string.Empty;

        /// <summary>Creates the subjects and marks screen. Only stores its arguments.</summary>
        /// <param name="homeForm">UI 1, shown again when the session is left.</param>
        /// <param name="previousForm">UI 2, shown again by Back.</param>
        public SubjectsMarksForm(Form homeForm, Form previousForm)
        {
            InitializeComponent();
            this.homeForm = homeForm;
            this.previousForm = previousForm;

            dgvSubjects.AutoGenerateColumns = false;
            dgvSubjects.DataSource = capturedRows;
            rawMarkText = numMark.Text;

            SetStatus(StepStatus, DeepNavy, Color.White);
            LoadReferenceData();
            RefreshTotals();
        }

        #region Session values handed to the next screens

        /// <summary>AdvisorySession.SessionID, set by UI 2 before this screen is shown.</summary>
        public int? SessionId { get; set; }

        /// <summary>Learner.LearnerID, set by UI 2 before this screen is shown.</summary>
        public int? LearnerId { get; set; }

        /// <summary>The learner context strip text (name · grade · school · province · session).</summary>
        public string LearnerContext { get; set; }

        #endregion

        #region Presentation row

        /// <summary>
        /// One captured subject as the grid shows it. Presentation only: the BLL keeps the
        /// <c>LearnerSubject</c> rows; this list lets Back and return keep what was entered.
        /// </summary>
        private sealed class CapturedSubjectRow
        {
            /// <summary>Subject.SubjectID of the captured subject.</summary>
            public int SubjectId { get; set; }

            /// <summary>Subject.SubjectName, shown in the Subject column.</summary>
            public string SubjectName { get; set; }

            /// <summary>Subject.SubjectLevel, shown in the Level column.</summary>
            public string SubjectLevel { get; set; }

            /// <summary>LearnerSubject.Mark, DECIMAL(5,1), shown in Consolas right-aligned.</summary>
            public decimal Mark { get; set; }

            /// <summary>NSC achievement level worked out by the BLL from NSC_LEVEL_1..7.</summary>
            public string AchievementLevel { get; set; }
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>
        /// Binds the subject list to active <c>Subject</c> rows and reads the stored minimum subject
        /// count. Nothing is hard-coded: until the BLL exists the list stays empty.
        /// </summary>
        private void LoadReferenceData()
        {
            try
            {
                // TODO BLL: cboSubject.DataSource = ReferenceDataController.GetActiveSubjects();   // SubjectID, SubjectName, SubjectLevel
                //           cboSubject.DisplayMember = "SubjectName"; cboSubject.ValueMember = "SubjectID";
                // TODO BLL: minimumSubjectCount = ReferenceDataController.GetParameterInt("MIN_SUBJECT_COUNT");
                minimumSubjectCount = null;
                cboSubject.SelectedIndex = -1;
                txtSubjectLevel.Text = string.Empty;
            }
            catch (Exception)
            {
                SetStatus("The subject list could not be loaded. Please go back and try again.", RedColour, Color.White);
            }
        }

        /// <summary>
        /// Asks the BLL to accept a subject for this session. The BLL re-checks the mark range
        /// (LearnerSubject.validateMarkRange) and duplicates (UNIQUE(SessionID, SubjectID)), and
        /// returns the NSC achievement level for the mark from the NSC_LEVEL_n parameters.
        /// </summary>
        /// <returns>The achievement level to show, or an em dash until the BLL exists.</returns>
        private string AddSubject(int subjectId, decimal mark)
        {
            // TODO BLL: LearnerController.AddSubject(SessionId.Value, subjectId, mark);   // learner.addSubject(subject, mark), held until Save
            // TODO BLL: return LearnerController.GetAchievementLevel(mark);               // e.g. "6  70–79%" from NSC_LEVEL_1..7
            return NoDataText;
        }

        /// <summary>Tells the BLL a captured subject was removed from this session.</summary>
        private void RemoveSubject(int subjectId)
        {
            // TODO BLL: LearnerController.RemoveSubject(SessionId.Value, subjectId);
        }

        /// <summary>
        /// Writes the captured subjects as <c>LearnerSubject</c> rows for the session. Only
        /// <b>Save and continue</b> writes; Cancel and Back leave without writing anything (2.3.3).
        /// </summary>
        private void SaveSubjects()
        {
            // TODO BLL: LearnerController.SaveSubjects(SessionId.Value,
            //               capturedRows.Select(r => new KeyValuePair<int, decimal>(r.SubjectId, r.Mark)).ToList());
            //           The BLL validates every mark again and relies on UNIQUE(SessionID, SubjectID).
        }

        #endregion

        #region Reading the selected subject

        /// <summary>
        /// Reads a named member from a bound list item, whether the list is a DataTable
        /// (DataRowView) or a list of entity objects.
        /// </summary>
        private static object ReadMember(object item, string member)
        {
            if (item == null)
            {
                return null;
            }

            var rowView = item as DataRowView;
            if (rowView != null)
            {
                return rowView.Row.Table.Columns.Contains(member) ? rowView[member] : null;
            }

            var property = TypeDescriptor.GetProperties(item).Find(member, true);
            return property == null ? null : property.GetValue(item);
        }

        /// <summary>The SubjectID of the selected subject, or null when nothing usable is selected.</summary>
        private int? SelectedSubjectId()
        {
            if (cboSubject.SelectedIndex < 0)
            {
                return null;
            }

            object value = ReadMember(cboSubject.SelectedItem, "SubjectID");
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }

        /// <summary>Fills the read-only Subject level box from the selected item's SubjectLevel.</summary>
        private void cboSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            object level = ReadMember(cboSubject.SelectedItem, "SubjectLevel");
            txtSubjectLevel.Text = level == null || level == DBNull.Value ? string.Empty : level.ToString();
        }

        #endregion

        #region Validation (validated again in the BLL)

        /// <summary>
        /// Checks the subject: one must be chosen, and it must not already be captured in this session.
        /// </summary>
        /// <returns>The message to show, or null when the subject is valid.</returns>
        private string SubjectProblem()
        {
            int? subjectId = SelectedSubjectId();
            if (subjectId == null)
            {
                return "Subject: choose a subject from the list.";
            }

            if (capturedRows.Any(r => r.SubjectId == subjectId.Value))
            {
                return cboSubject.Text + " is already captured in this session; each subject may be entered once.";
            }

            return null;
        }

        /// <summary>
        /// Checks the mark as typed (not the clamped value): numbers only, 0 to 100, at most one decimal.
        /// </summary>
        /// <param name="mark">The parsed mark when valid.</param>
        /// <returns>The message to show, or null when the mark is valid.</returns>
        private string MarkProblem(out decimal mark)
        {
            mark = 0;
            string text = (rawMarkText ?? string.Empty).Trim().Replace(" ", string.Empty);
            if (text.Length == 0)
            {
                return "Mark: enter the mark % from your latest report.";
            }

            // Accept either decimal separator, whatever the regional setting.
            string normalised = text.Replace(',', '.');
            if (!decimal.TryParse(normalised, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture, out mark))
            {
                return "Mark: use numbers only, for example 67 or 67.5.";
            }

            if (mark < 0 || mark > 100)
            {
                return "Mark must be a number from 0 to 100 (you entered " + text + ").";
            }

            int point = normalised.IndexOf('.');
            if (point >= 0 && normalised.Length - point - 1 > 1)
            {
                return "Mark: use at most one decimal place, for example 67.5.";
            }

            return null;
        }

        /// <summary>
        /// Validates the Add-a-subject group, marks every offending label and control red, and shows
        /// the validation summary listing every entry to fix.
        /// </summary>
        /// <returns>The problems found, each with the control to refocus.</returns>
        private List<KeyValuePair<Control, string>> ValidateEntry()
        {
            var problems = new List<KeyValuePair<Control, string>>();
            decimal mark;

            string subjectMessage = SubjectProblem();
            string markMessage = MarkProblem(out mark);
            MarkField(subjectMessage, cboSubject, lblSubject, lblSubjectError);
            MarkField(markMessage, numMark, lblMark, lblMarkError);
            if (subjectMessage != null) problems.Add(new KeyValuePair<Control, string>(cboSubject, subjectMessage));
            if (markMessage != null) problems.Add(new KeyValuePair<Control, string>(numMark, markMessage));

            ShowSummary(problems.Select(p => p.Value).ToList());
            if (markMessage != null)
            {
                // Put back what was typed, so the learner can see the value being questioned.
                numMark.Text = rawMarkText;
            }

            return problems;
        }

        /// <summary>
        /// Shows or clears a field message (spec 2.2.7): red caption under the control with a warning
        /// glyph, red-tint control and red label.
        /// </summary>
        private static void MarkField(string message, Control input, Label fieldLabel, Label messageLabel)
        {
            bool valid = message == null;
            input.BackColor = valid ? SystemColors.Window : RedTint;
            fieldLabel.ForeColor = valid ? InkColour : RedColour;
            messageLabel.Text = valid ? string.Empty : "⚠ " + message;
            messageLabel.Visible = !valid;
            input.AccessibleDescription = message;
        }

        /// <summary>
        /// Shows the red validation summary under the group, or hides it when there is nothing to fix.
        /// The captured-subjects card moves down to make room.
        /// </summary>
        private void ShowSummary(IList<string> lines)
        {
            bool show = lines.Count > 0;
            if (show)
            {
                lblSummary.Text = lines.Count + (lines.Count == 1 ? " entry" : " entries") + " to correct" +
                                  Environment.NewLine + "• " + string.Join(Environment.NewLine + "• ", lines);
            }

            pnlSummary.Visible = show;
            int top = show ? CapturedTopWithSummary : CapturedTopPlain;
            pnlCapturedCard.SetBounds(0, top, pnlCapturedCard.Width, ContentBottom - top);
        }

        /// <summary>Clears every field message and hides the summary.</summary>
        private void ClearEntryErrors()
        {
            MarkField(null, cboSubject, lblSubject, lblSubjectError);
            MarkField(null, numMark, lblMark, lblMarkError);
            ShowSummary(new List<string>());
        }

        /// <summary>Re-checks the subject on leaving it, once the learner has started an entry.</summary>
        private void cboSubject_Leave(object sender, EventArgs e)
        {
            if (cboSubject.SelectedIndex >= 0 || pnlSummary.Visible)
            {
                ValidateEntry();
            }
        }

        /// <summary>Re-checks the mark on leaving it, reading the text as typed.</summary>
        private void numMark_Leave(object sender, EventArgs e)
        {
            if (cboSubject.SelectedIndex >= 0 || pnlSummary.Visible)
            {
                ValidateEntry();
            }
        }

        /// <summary>Remembers the mark text while the learner types, before the control can clamp it.</summary>
        private void numMark_TextChanged(object sender, EventArgs e)
        {
            if (numMark.ContainsFocus)
            {
                rawMarkText = numMark.Text;
            }
        }

        #endregion

        #region Add and remove

        /// <summary>Add subject: validates, then commits the row to the grid and recalculates the totals.</summary>
        private void btnAddSubject_Click(object sender, EventArgs e)
        {
            rawMarkText = numMark.ContainsFocus ? numMark.Text : rawMarkText;
            List<KeyValuePair<Control, string>> problems = ValidateEntry();
            if (problems.Count > 0)
            {
                SetStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct before this subject can be added.",
                    RedColour, Color.White);
                problems[0].Key.Focus();
                return;
            }

            decimal mark;
            MarkProblem(out mark);
            int subjectId = SelectedSubjectId().Value;
            string achievement;
            try
            {
                achievement = AddSubject(subjectId, mark);
            }
            catch (Exception)
            {
                SetStatus("This subject could not be added. Please try again.", RedColour, Color.White);
                return;
            }

            capturedRows.Add(new CapturedSubjectRow
            {
                SubjectId = subjectId,
                SubjectName = cboSubject.Text,
                SubjectLevel = txtSubjectLevel.Text,
                Mark = mark,
                AchievementLevel = string.IsNullOrEmpty(achievement) ? NoDataText : achievement
            });

            // Ready for the next subject.
            cboSubject.SelectedIndex = -1;
            txtSubjectLevel.Text = string.Empty;
            numMark.Value = numMark.Minimum;
            rawMarkText = numMark.Text;
            ClearEntryErrors();
            RefreshTotals();
            SetStatus("Subject added. " + StepStatus, DeepNavy, Color.White);
            cboSubject.Focus();
        }

        /// <summary>The Remove button in a row removes that subject and recalculates the totals.</summary>
        private void dgvSubjects_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= capturedRows.Count || e.ColumnIndex != colRemove.Index)
            {
                return;
            }

            CapturedSubjectRow row = capturedRows[e.RowIndex];
            try
            {
                RemoveSubject(row.SubjectId);
            }
            catch (Exception)
            {
                SetStatus("This subject could not be removed. Please try again.", RedColour, Color.White);
                return;
            }

            capturedRows.RemoveAt(e.RowIndex);
            if (pnlSummary.Visible)
            {
                // A duplicate may no longer be a duplicate.
                ValidateEntry();
            }

            RefreshTotals();
            SetStatus(row.SubjectName + " removed. " + StepStatus, DeepNavy, Color.White);
        }

        /// <summary>The grid is for reading: no row starts highlighted after the list changes.</summary>
        private void dgvSubjects_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvSubjects.ClearSelection();
        }

        /// <summary>
        /// Recalculates the captured count and average (display only), the "n of min captured" caption,
        /// the notice panel and whether Save and continue is available (at least one subject).
        /// </summary>
        private void RefreshTotals()
        {
            int count = capturedRows.Count;
            string countText = count + (count == 1 ? " subject" : " subjects") + " captured";
            lblFooter.Text = count == 0
                ? "No subjects captured yet"
                : countText + "     Average " + capturedRows.Average(r => r.Mark).ToString("0.0", CultureInfo.CurrentCulture) + " %";
            lblCapturedCount.Text = minimumSubjectCount.HasValue
                ? count + " of " + minimumSubjectCount.Value + " captured"
                : countText;
            btnSaveContinue.Enabled = count >= 1;
            dgvSubjects.ClearSelection();
            UpdateNotice();
        }

        #endregion

        #region Notice panel and status band

        /// <summary>
        /// Amber when fewer than MIN_SUBJECT_COUNT subjects are captured (a warning, never a block),
        /// green when enough are captured, blue guidance while the stored minimum is not known.
        /// </summary>
        private void UpdateNotice()
        {
            int count = capturedRows.Count;
            if (!minimumSubjectCount.HasValue)
            {
                SetNotice("Add each subject on your latest school report with its mark. Save and continue opens once at least " +
                          "one subject is captured.", BlueTint, BlueColour, InkColour);
                return;
            }

            int needed = minimumSubjectCount.Value - count;
            if (needed > 0)
            {
                SetNotice(needed + (needed == 1 ? " more subject" : " more subjects") + " needed. Future Path checks eligibility " +
                          "against at least " + minimumSubjectCount.Value + " subjects; you have captured " + count +
                          ". You can still continue, but the outcome may be less complete.", AmberTint, AmberColour, AmberText);
            }
            else
            {
                SetNotice("✓ Enough subjects captured (" + count + " of " + minimumSubjectCount.Value + "). Add any extra subjects, " +
                          "such as Advanced Programme subjects, or save and continue.", GreenTint, GreenColour, GreenColour);
            }
        }

        private void SetNotice(string text, Color back, Color bar, Color fore)
        {
            pnlNotice.BackColor = back;
            pnlNoticeBar.BackColor = bar;
            lblNotice.ForeColor = fore;
            lblNotice.Text = text;
        }

        private void SetStatus(string message, Color back, Color fore)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = fore;
            lblStatus.Text = message;
        }

        /// <summary>While focus is in the Add-a-subject group, Enter adds the subject.</summary>
        private void pnlAddCard_Enter(object sender, EventArgs e)
        {
            AcceptButton = btnAddSubject;
        }

        /// <summary>Outside the group, Enter means Save and continue again.</summary>
        private void pnlAddCard_Leave(object sender, EventArgs e)
        {
            AcceptButton = btnSaveContinue;
        }

        #endregion

        #region Buttons and navigation

        /// <summary>
        /// Save and continue → UI 4. Invalid data in the Add-a-subject group raises UI 3.1a
        /// (Invalid or duplicate subjects) and refocuses the offending control.
        /// </summary>
        private void btnSaveContinue_Click(object sender, EventArgs e)
        {
            rawMarkText = numMark.ContainsFocus ? numMark.Text : rawMarkText;
            if (cboSubject.SelectedIndex >= 0 || pnlSummary.Visible)
            {
                List<KeyValuePair<Control, string>> problems = ValidateEntry();
                if (problems.Count > 0)
                {
                    SetStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct before you can continue.",
                        RedColour, Color.White);
                    using (var dialog = new ValidationDialog("Invalid or duplicate subjects", problems.Select(p => p.Value).ToList()))
                    {
                        dialog.ShowDialog(this);
                    }

                    problems[0].Key.Focus();
                    return;
                }

                // A valid entry that was never added: ask rather than drop it silently.
                using (var dialog = new ConfirmationDialog("Subject not added",
                    cboSubject.Text + " has not been added to your captured subjects. Continue without it?", "Continue without it"))
                {
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        btnAddSubject.Focus();
                        return;
                    }
                }
            }

            if (capturedRows.Count == 0)
            {
                using (var dialog = new ValidationDialog("Invalid or duplicate subjects",
                    new List<string> { "Add at least one subject and mark before you continue." }))
                {
                    dialog.ShowDialog(this);
                }

                cboSubject.Focus();
                return;
            }

            try
            {
                SaveSubjects();
            }
            catch (Exception)
            {
                SetStatus("Your subjects could not be saved. Please try again.", RedColour, Color.White);
                return;
            }

            if (questionnaireForm == null || questionnaireForm.IsDisposed)
            {
                questionnaireForm = new InterestQuestionnaireForm(homeForm, this);
            }

            questionnaireForm.SessionId = SessionId;
            questionnaireForm.LearnerId = LearnerId;
            questionnaireForm.LearnerContext = LearnerContext;
            questionnaireForm.Show();
            Hide();
        }

        /// <summary>Back → UI 2. The captured rows stay in this (hidden) form.</summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            previousForm.Show();
            Hide();
        }

        /// <summary>Cancel abandons the session after confirmation.</summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            LeaveSession();
        }

        /// <summary>Home on the rail leaves the session after confirmation.</summary>
        private void btnNavHome_Click(object sender, EventArgs e)
        {
            LeaveSession();
        }

        /// <summary>
        /// Asks for confirmation, returns to UI 1 and closes every learner screen. Anything already
        /// saved stays on record as an "In progress" session (decisions.md D3).
        /// </summary>
        private void LeaveSession()
        {
            using (var dialog = new ConfirmationDialog("Leave this advisory session",
                "Return to the homepage? Subjects not yet saved are discarded. Anything already saved stays on record as an " +
                "\"In progress\" session.", "Leave session"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }

            homeForm.Show();
            foreach (Form screen in Application.OpenForms.Cast<Form>().ToList())
            {
                if (screen != homeForm)
                {
                    screen.Close();
                }
            }
        }

        #endregion

        #region Showing and closing

        /// <summary>Shows the learner context strip each time the screen is shown.</summary>
        private void SubjectsMarksForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                lblContext.Text = LearnerContext ?? string.Empty;
                SetStatus(StepStatus, DeepNavy, Color.White);
            }
        }

        /// <summary>The caption-bar X asks first, because it closes Future Path.</summary>
        private void SubjectsMarksForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing || homeForm.Visible)
            {
                return;
            }

            using (var dialog = new ConfirmationDialog("Close Future Path",
                "Close Future Path? Anything already saved stays on record as an \"In progress\" session.", "Close"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>Ends the application when this screen was closed with the caption-bar X.</summary>
        private void SubjectsMarksForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!homeForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
