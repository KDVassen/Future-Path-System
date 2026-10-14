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
    /// UI 3 — Subjects and Marks Capture (step 2 of 6; spec 2.3.3). The graded rich dialog.
    /// The learner picks a Subject (bound to active <c>Subject</c> rows), sees its Subject level
    /// (read-only, from <c>Subject.SubjectLevel</c>, decisions.md D4) and enters a Mark % from 0 to 100
    /// with at most one decimal. <b>Add subject</b> validates the entry: a duplicate subject or a bad
    /// mark turns the label and control red and the validation summary lists every entry to fix.
    /// Captured rows are kept in a presentation list, so going Back and returning never loses them.
    /// The amber notice counts the subjects still needed against <c>MIN_SUBJECT_COUNT</c> — a warning,
    /// never a block (decisions.md §3). Leaving with invalid data raises UI 3.1a.
    /// </summary>
    public partial class MainForm
    {
        private const string SubjectsStepStatus = "Step 2 of 6 · Add every subject on your latest report, then save and continue.";
        private const string SubjectsNoDataText = "—";

        /// <summary>The rows captured in this session, shown in the grid. Kept when the learner goes Back and returns.</summary>
        private readonly BindingList<CapturedSubjectRow> subjectsRows = new BindingList<CapturedSubjectRow>();

        /// <summary>True once the subject list and the minimum subject count have been loaded for this session.</summary>
        private bool subjectsLoaded;

        /// <summary>The stored minimum subject count (SystemParameter MIN_SUBJECT_COUNT); null until loaded.</summary>
        private int? subjectsMinimumCount;

        /// <summary>
        /// The mark exactly as typed. The NumericUpDown clamps and rounds its text when it loses focus,
        /// so the raw text is remembered while the learner types and validated instead.
        /// </summary>
        private string subjectsRawMarkText = string.Empty;

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

        #region Showing and resetting

        /// <summary>
        /// Called by PageShown each time UI 3 is shown: binds the grid to the captured rows once, loads the
        /// subject list and minimum count the first time in a session, and refreshes the totals, the notice
        /// and whether Save and continue is available.
        /// </summary>
        private void SubjectsShown()
        {
            SetStatus(SubjectsStepStatus, DeepNavy, Color.White);
            if (dgvSubjects.DataSource == null)
            {
                dgvSubjects.AutoGenerateColumns = false;
                dgvSubjects.DataSource = subjectsRows;
            }

            if (!subjectsLoaded)
            {
                SubjectsLoadReferenceData();
            }

            SubjectsRefreshTotals();
        }

        /// <summary>
        /// Returns UI 3 to a brand-new session: no captured rows, an empty Add-a-subject group, no red
        /// field states or summary, the list and minimum count to be reloaded, and Enter back on Save and
        /// continue. No learner may see the previous learner's subjects (spec 2.1).
        /// </summary>
        private void SubjectsReset()
        {
            subjectsRows.Clear();
            cboSubject.SelectedIndex = -1;
            txtSubjectLevel.Text = string.Empty;
            numMark.Value = numMark.Minimum;
            numMark.Text = numMark.Minimum.ToString(CultureInfo.CurrentCulture);
            subjectsRawMarkText = numMark.Text;
            SubjectsClearEntryErrors();

            subjectsMinimumCount = null;
            subjectsLoaded = false;
            SubjectsRefreshTotals();
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>
        /// Binds the subject list to active <c>Subject</c> rows and reads the stored minimum subject
        /// count. Nothing is hard-coded: until the BLL exists the list stays empty.
        /// </summary>
        private void SubjectsLoadReferenceData()
        {
            try
            {
                // TODO BLL: cboSubject.DataSource = ReferenceDataController.GetActiveSubjects();   // SubjectID, SubjectName, SubjectLevel
                //           cboSubject.DisplayMember = "SubjectName"; cboSubject.ValueMember = "SubjectID";
                // TODO BLL: subjectsMinimumCount = ReferenceDataController.GetParameterInt("MIN_SUBJECT_COUNT");
                subjectsMinimumCount = null;
                cboSubject.SelectedIndex = -1;
                txtSubjectLevel.Text = string.Empty;
                subjectsLoaded = true;
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
        private string SubjectsAddSubject(int subjectId, decimal mark)
        {
            // TODO BLL: LearnerController.AddSubject(SessionId.Value, subjectId, mark);   // learner.addSubject(subject, mark), held until Save
            // TODO BLL: return LearnerController.GetAchievementLevel(mark);               // e.g. "6  70–79%" from NSC_LEVEL_1..7
            return SubjectsNoDataText;
        }

        /// <summary>Tells the BLL a captured subject was removed from this session.</summary>
        private void SubjectsRemoveSubject(int subjectId)
        {
            // TODO BLL: LearnerController.RemoveSubject(SessionId.Value, subjectId);
        }

        /// <summary>
        /// Writes the captured subjects as <c>LearnerSubject</c> rows for the session. Only
        /// <b>Save and continue</b> writes; Cancel and Back leave without writing anything (2.3.3).
        /// </summary>
        private void SubjectsSave()
        {
            // TODO BLL: LearnerController.SaveSubjects(SessionId.Value,
            //               subjectsRows.Select(r => new KeyValuePair<int, decimal>(r.SubjectId, r.Mark)).ToList());
            //           The BLL validates every mark again and relies on UNIQUE(SessionID, SubjectID).
        }

        #endregion

        #region Reading the selected subject

        /// <summary>
        /// Reads a named member from a bound list item, whether the list is a DataTable
        /// (DataRowView) or a list of entity objects.
        /// </summary>
        private static object SubjectsReadMember(object item, string member)
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
        private int? SubjectsSelectedSubjectId()
        {
            if (cboSubject.SelectedIndex < 0)
            {
                return null;
            }

            object value = SubjectsReadMember(cboSubject.SelectedItem, "SubjectID");
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }

        /// <summary>Fills the read-only Subject level box from the selected item's SubjectLevel.</summary>
        private void cboSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            object level = SubjectsReadMember(cboSubject.SelectedItem, "SubjectLevel");
            txtSubjectLevel.Text = level == null || level == DBNull.Value ? string.Empty : level.ToString();
        }

        #endregion

        #region Validation (validated again in the BLL)

        /// <summary>
        /// Checks the subject: one must be chosen, and it must not already be captured in this session.
        /// </summary>
        /// <returns>The message to show, or null when the subject is valid.</returns>
        private string SubjectsSubjectProblem()
        {
            int? subjectId = SubjectsSelectedSubjectId();
            if (subjectId == null)
            {
                return "Subject: choose a subject from the list.";
            }

            if (subjectsRows.Any(r => r.SubjectId == subjectId.Value))
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
        private string SubjectsMarkProblem(out decimal mark)
        {
            mark = 0;
            string text = (subjectsRawMarkText ?? string.Empty).Trim().Replace(" ", string.Empty);
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
        private List<KeyValuePair<Control, string>> SubjectsValidateEntry()
        {
            var problems = new List<KeyValuePair<Control, string>>();
            decimal mark;

            string subjectMessage = SubjectsSubjectProblem();
            string markMessage = SubjectsMarkProblem(out mark);
            CheckField(subjectMessage == null, cboSubject, lblSubject, lblSubjectError, subjectMessage);
            CheckField(markMessage == null, numMark, lblMark, lblMarkError, markMessage);
            if (subjectMessage != null) problems.Add(new KeyValuePair<Control, string>(cboSubject, subjectMessage));
            if (markMessage != null) problems.Add(new KeyValuePair<Control, string>(numMark, markMessage));

            SubjectsShowSummary(problems.Select(p => p.Value).ToList());
            // The rejected text is deliberately NOT written back into the box: a NumericUpDown clamps any text
            // assigned to it at once (150 -> 100), and a second Enter would then quietly add the clamped value.
            // The remembered raw text keeps the error standing until the learner edits the mark again.
            return problems;
        }

        /// <summary>
        /// Shows the red validation summary under the group, or hides it when there is nothing to fix.
        /// The captured-subjects card moves down to make room.
        /// </summary>
        private void SubjectsShowSummary(IList<string> lines)
        {
            bool show = lines.Count > 0;
            if (show)
            {
                lblSummary.Text = lines.Count + (lines.Count == 1 ? " entry" : " entries") + " to correct" +
                                  Environment.NewLine + "• " + string.Join(Environment.NewLine + "• ", lines);
            }

            // The card keeps its bottom edge; its top starts where the summary does, or below it with the same gap
            // as between the Add card and the summary. Read from the Designer controls, so it is right at any DPI.
            pnlSummary.Visible = show;
            int bottom = pnlCapturedCard.Bottom;
            int top = show ? pnlSummary.Bottom + (pnlSummary.Top - pnlAddCard.Bottom) : pnlSummary.Top;
            pnlCapturedCard.SetBounds(pnlCapturedCard.Left, top, pnlCapturedCard.Width, bottom - top);
        }

        /// <summary>Clears every field message and hides the summary.</summary>
        private void SubjectsClearEntryErrors()
        {
            CheckField(true, cboSubject, lblSubject, lblSubjectError, null);
            CheckField(true, numMark, lblMark, lblMarkError, null);
            SubjectsShowSummary(new List<string>());
        }

        /// <summary>Re-checks the subject on leaving it, once the learner has started an entry.</summary>
        private void cboSubject_Leave(object sender, EventArgs e)
        {
            if (cboSubject.SelectedIndex >= 0 || pnlSummary.Visible)
            {
                SubjectsValidateEntry();
            }
        }

        /// <summary>Re-checks the mark on leaving it, reading the text as typed.</summary>
        private void numMark_Leave(object sender, EventArgs e)
        {
            if (cboSubject.SelectedIndex >= 0 || pnlSummary.Visible)
            {
                SubjectsValidateEntry();
            }
        }

        /// <summary>Remembers the mark text while the learner types, before the control can clamp it.</summary>
        private void numMark_TextChanged(object sender, EventArgs e)
        {
            if (numMark.ContainsFocus)
            {
                subjectsRawMarkText = numMark.Text;
            }
        }

        #endregion

        #region Add and remove

        /// <summary>Add subject: validates, then commits the row to the grid and recalculates the totals.</summary>
        private void btnAddSubject_Click(object sender, EventArgs e)
        {
            subjectsRawMarkText = numMark.ContainsFocus ? numMark.Text : subjectsRawMarkText;
            List<KeyValuePair<Control, string>> problems = SubjectsValidateEntry();
            if (problems.Count > 0)
            {
                SetStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct before this subject can be added.",
                    RedColour, Color.White);
                problems[0].Key.Focus();
                return;
            }

            decimal mark;
            SubjectsMarkProblem(out mark);
            int subjectId = SubjectsSelectedSubjectId().Value;
            string achievement;
            try
            {
                achievement = SubjectsAddSubject(subjectId, mark);
            }
            catch (Exception)
            {
                SetStatus("This subject could not be added. Please try again.", RedColour, Color.White);
                return;
            }

            subjectsRows.Add(new CapturedSubjectRow
            {
                SubjectId = subjectId,
                SubjectName = cboSubject.Text,
                SubjectLevel = txtSubjectLevel.Text,
                Mark = mark,
                AchievementLevel = string.IsNullOrEmpty(achievement) ? SubjectsNoDataText : achievement
            });

            // Ready for the next subject.
            cboSubject.SelectedIndex = -1;
            txtSubjectLevel.Text = string.Empty;
            numMark.Value = numMark.Minimum;
            subjectsRawMarkText = numMark.Text;
            SubjectsClearEntryErrors();
            SubjectsRefreshTotals();
            SetStatus("Subject added. " + SubjectsStepStatus, DeepNavy, Color.White);
            cboSubject.Focus();
        }

        /// <summary>The Remove button in a row removes that subject and recalculates the totals.</summary>
        private void dgvSubjects_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= subjectsRows.Count || e.ColumnIndex != colSubjectsRemove.Index)
            {
                return;
            }

            CapturedSubjectRow row = subjectsRows[e.RowIndex];
            try
            {
                SubjectsRemoveSubject(row.SubjectId);
            }
            catch (Exception)
            {
                SetStatus("This subject could not be removed. Please try again.", RedColour, Color.White);
                return;
            }

            subjectsRows.RemoveAt(e.RowIndex);
            if (pnlSummary.Visible)
            {
                // A duplicate may no longer be a duplicate.
                SubjectsValidateEntry();
            }

            SubjectsRefreshTotals();
            SetStatus(row.SubjectName + " removed. " + SubjectsStepStatus, DeepNavy, Color.White);
        }

        /// <summary>The grid is for reading: no row starts highlighted after the list changes.</summary>
        private void dgvSubjects_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvSubjects.ClearSelection();
        }

        /// <summary>
        /// Recalculates the captured count and average (display only) and the "n of min captured" caption.
        /// It also refreshes the notice and whether Save and continue is available (at least one subject).
        /// </summary>
        private void SubjectsRefreshTotals()
        {
            int count = subjectsRows.Count;
            string countText = count + (count == 1 ? " subject" : " subjects") + " captured";
            lblFooter.Text = count == 0
                ? "No subjects captured yet"
                : countText + "     Average " + subjectsRows.Average(r => r.Mark).ToString("0.0", CultureInfo.CurrentCulture) + " %";
            lblCapturedCount.Text = subjectsMinimumCount.HasValue
                ? count + " of " + subjectsMinimumCount.Value + " captured"
                : countText;
            dgvSubjects.ClearSelection();

            SetPrimaryEnabled(btnSubjectsSave, count >= 1);
            SubjectsUpdateNotice();
        }

        #endregion

        #region Notice and Enter key

        /// <summary>
        /// Amber when fewer than MIN_SUBJECT_COUNT subjects are captured (a warning, never a block),
        /// green when enough are captured, blue guidance while the stored minimum is not known.
        /// </summary>
        private void SubjectsUpdateNotice()
        {
            int count = subjectsRows.Count;
            if (!subjectsMinimumCount.HasValue)
            {
                ShowNotice(lblSubjectsNotice, "Add each subject on your latest school report with its mark. Save and continue opens once at least " +
                           "one subject is captured.", BlueTint, InkColour);
                return;
            }

            int needed = subjectsMinimumCount.Value - count;
            if (needed > 0)
            {
                ShowNotice(lblSubjectsNotice, needed + (needed == 1 ? " more subject" : " more subjects") + " needed. Future Path checks eligibility " +
                           "against at least " + subjectsMinimumCount.Value + " subjects; you have captured " + count +
                           ". You can still continue, but the outcome may be less complete.", AmberTint, AmberText);
            }
            else
            {
                ShowNotice(lblSubjectsNotice, "✓ Enough subjects captured (" + count + " of " + subjectsMinimumCount.Value + "). Add any extra subjects, " +
                           "such as Advanced Programme subjects, or save and continue.", GreenTint, GreenColour);
            }
        }

        /// <summary>While focus is in the Add-a-subject group, Enter adds the subject.</summary>
        private void pnlAddCard_Enter(object sender, EventArgs e)
        {
            AcceptButton = btnAddSubject;
        }

        /// <summary>Outside the group, Enter means Save and continue again.</summary>
        private void pnlAddCard_Leave(object sender, EventArgs e)
        {
            AcceptButton = btnSubjectsSave;
        }

        #endregion

        #region Button click events

        /// <summary>
        /// Save and continue → UI 4. Invalid data in the Add-a-subject group raises UI 3.1a
        /// (Invalid or duplicate subjects) and refocuses the offending control.
        /// </summary>
        private void btnSubjectsSave_Click(object sender, EventArgs e)
        {
            subjectsRawMarkText = numMark.ContainsFocus ? numMark.Text : subjectsRawMarkText;
            if (cboSubject.SelectedIndex >= 0 || pnlSummary.Visible)
            {
                List<KeyValuePair<Control, string>> problems = SubjectsValidateEntry();
                if (problems.Count > 0)
                {
                    SetStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct before you can continue.",
                        RedColour, Color.White);
                    ShowValidation("Invalid or duplicate subjects", problems.Select(p => p.Value).ToList(), problems[0].Key);
                    return;
                }

                // A valid entry that was never added: ask rather than drop it silently.
                if (!Confirm("Subject not added",
                    cboSubject.Text + " has not been added to your captured subjects. Continue without it?", "Continue without it"))
                {
                    btnAddSubject.Focus();
                    return;
                }
            }

            if (subjectsRows.Count == 0)
            {
                ShowValidation("Invalid or duplicate subjects",
                    new List<string> { "Add at least one subject and mark before you continue." }, cboSubject);
                return;
            }

            try
            {
                SubjectsSave();
            }
            catch (Exception)
            {
                SetStatus("Your subjects could not be saved. Please try again.", RedColour, Color.White);
                return;
            }

            GoToStep(tabInterests, 3);
        }

        /// <summary>Back → UI 2. The captured subjects are kept.</summary>
        private void btnSubjectsBack_Click(object sender, EventArgs e)
        {
            tcMain.SelectedTab = tabProfile;
        }

        /// <summary>Cancel leaves the session (the Home tab asks first).</summary>
        private void btnSubjectsCancel_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        #endregion
    }
}
