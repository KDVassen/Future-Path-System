
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles subject and mark entry for learner advisory sessions
    public partial class MainForm
    {
        private const string SubjectsStepStatus = "Step 2 of 6 · Add every subject on your latest report, then save and continue.";
        private const string SubjectsNoDataText = "—";

        // Stores subjects captured during the current session
        private readonly BindingList<CapturedSubjectRow> subjectsRows = new BindingList<CapturedSubjectRow>();

        // Tracks whether the subject list has been loaded
        private bool subjectsLoaded;

        // Minimum number of subjects set in the system
        private int subjectsMinimumCount;

        // Keeps the mark as typed before the control adjusts its value
        private string subjectsRawMarkText = string.Empty;

        #region Presentation row

        // Stores the subject details displayed in the grid
        private sealed class CapturedSubjectRow
        {
            // ID of the captured subject
            public int SubjectId { get; set; }

            // Name of the subject
            public string SubjectName { get; set; }

            // Level of the subject
            public string SubjectLevel { get; set; }

            // Mark entered by the learner
            public decimal Mark { get; set; }

            // Achievement level calculated from the mark
            public string AchievementLevel { get; set; }
        }

        #endregion

        #region Showing and resetting

        // Load captured subjects and update the totals
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

        // Clear captured subjects for a new learner session
        private void SubjectsReset()
        {
            subjectsRows.Clear();
            cboSubject.SelectedIndex = -1;
            txtSubjectLevel.Text = string.Empty;
            numMark.Value = numMark.Minimum;
            numMark.Text = numMark.Minimum.ToString(CultureInfo.CurrentCulture);
            subjectsRawMarkText = numMark.Text;
            SubjectsClearEntryErrors();

            subjectsMinimumCount = 0;
            subjectsLoaded = false;
            SubjectsRefreshTotals();
        }

        #endregion

        #region Business Logic Layer hooks

        // Load active subjects and the minimum subject count
        private void SubjectsLoadReferenceData()
        {
            try
            {
                ReferenceDataController referenceDataController = new ReferenceDataController();
                cboSubject.DisplayMember = "SubjectName";
                cboSubject.ValueMember = "SubjectID";
                cboSubject.DataSource = referenceDataController.GetActiveSubjects();
                subjectsMinimumCount = referenceDataController.GetParameterInt(SystemSettings.MinSubjectCountCode);
                cboSubject.SelectedIndex = -1;
                txtSubjectLevel.Text = string.Empty;
                subjectsLoaded = true;
            }
            catch (Exception)
            {
                SetStatus("The subject list could not be loaded. Please go back and try again.", RedColour, Color.White);
            }
        }

        // Get the NSC achievement level for a mark
        private string SubjectsGetAchievementLevel(decimal mark)
        {
            LearnerController learnerController = new LearnerController();
            return learnerController.GetAchievementLevel(mark);
        }

        // Save the captured subjects through the controller
        private void SubjectsSave()
        {
            List<KeyValuePair<int, decimal>> subjectMarks = new List<KeyValuePair<int, decimal>>();
            foreach (CapturedSubjectRow row in subjectsRows)
            {
                subjectMarks.Add(new KeyValuePair<int, decimal>(row.SubjectId, row.Mark));
            }

            LearnerController learnerController = new LearnerController();
            learnerController.SaveSubjects(SessionId.Value, subjectMarks);
        }

        #endregion

        #region Reading the selected subject

        // Return the selected subject or null if none is chosen
        private Subject SubjectsSelectedSubject()
        {
            if (cboSubject.SelectedIndex < 0)
            {
                return null;
            }

            return cboSubject.SelectedItem as Subject;
        }

        // Check whether a subject has already been captured
        private bool SubjectsIsCaptured(int subjectId)
        {
            foreach (CapturedSubjectRow row in subjectsRows)
            {
                if (row.SubjectId == subjectId)
                {
                    return true;
                }
            }

            return false;
        }

        // Display the level of the selected subject
        private void cboSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            Subject subject = SubjectsSelectedSubject();
            if (subject == null)
            {
                txtSubjectLevel.Text = string.Empty;
            }
            else
            {
                txtSubjectLevel.Text = subject.SubjectLevel;
            }
        }

        #endregion

        #region Validation (validated again in the BLL)

        // Check for a missing or duplicate subject
        private string SubjectsSubjectProblem()
        {
            Subject subject = SubjectsSelectedSubject();
            if (subject == null)
            {
                return "Subject: choose a subject from the list.";
            }

            if (SubjectsIsCaptured(subject.SubjectID))
            {
                return cboSubject.Text + " is already captured in this session; each subject may be entered once.";
            }

            return null;
        }

        // Validate the original mark entered by the learner
        private string SubjectsMarkProblem(out decimal mark)
        {
            mark = 0;
            string text = (subjectsRawMarkText ?? string.Empty).Trim().Replace(" ", string.Empty);
            if (text.Length == 0)
            {
                return "Mark: enter the mark % from your latest report.";
            }

            // Accept decimal points and commas
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

        // Validate both subject and mark fields
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
            // Keep invalid raw text so it cannot be silently changed into an accepted mark
            return problems;
        }

        // Display validation errors and adjust the subject list layout
        private void SubjectsShowSummary(IList<string> lines)
        {
            bool show = lines.Count > 0;
            if (show)
            {
                lblSummary.Text = lines.Count + (lines.Count == 1 ? " entry" : " entries") + " to correct" +
                                  Environment.NewLine + "• " + string.Join(Environment.NewLine + "• ", lines);
            }

            // Adjust the captured subjects card to make room for errors
            pnlSummary.Visible = show;
            int bottom = pnlCapturedCard.Bottom;
            int top = show ? pnlSummary.Bottom + (pnlSummary.Top - pnlAddCard.Bottom) : pnlSummary.Top;
            pnlCapturedCard.SetBounds(pnlCapturedCard.Left, top, pnlCapturedCard.Width, bottom - top);
        }

        // Clear field errors and the validation summary
        private void SubjectsClearEntryErrors()
        {
            CheckField(true, cboSubject, lblSubject, lblSubjectError, null);
            CheckField(true, numMark, lblMark, lblMarkError, null);
            SubjectsShowSummary(new List<string>());
        }

        // Validate the subject when leaving the dropdown
        private void cboSubject_Leave(object sender, EventArgs e)
        {
            if (cboSubject.SelectedIndex >= 0 || pnlSummary.Visible)
            {
                SubjectsValidateEntry();
            }
        }

        // Validate the mark when leaving the field
        private void numMark_Leave(object sender, EventArgs e)
        {
            if (cboSubject.SelectedIndex >= 0 || pnlSummary.Visible)
            {
                SubjectsValidateEntry();
            }
        }

        // Remember the mark as the learner types
        private void numMark_TextChanged(object sender, EventArgs e)
        {
            if (numMark.ContainsFocus)
            {
                subjectsRawMarkText = numMark.Text;
            }
        }

        #endregion

        #region Add and remove

        // Validate and add the selected subject to the grid
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
            Subject subject = SubjectsSelectedSubject();
            string achievement;
            try
            {
                achievement = SubjectsGetAchievementLevel(mark);
            }
            catch (Exception)
            {
                SetStatus("This subject could not be added. Please try again.", RedColour, Color.White);
                return;
            }

            if (string.IsNullOrEmpty(achievement))
            {
                achievement = SubjectsNoDataText;
            }

            CapturedSubjectRow captured = new CapturedSubjectRow();
            captured.SubjectId = subject.SubjectID;
            captured.SubjectName = subject.SubjectName;
            captured.SubjectLevel = subject.SubjectLevel;
            captured.Mark = mark;
            captured.AchievementLevel = achievement;
            subjectsRows.Add(captured);

            // Reset the fields for another subject
            cboSubject.SelectedIndex = -1;
            txtSubjectLevel.Text = string.Empty;
            numMark.Value = numMark.Minimum;
            subjectsRawMarkText = numMark.Text;
            SubjectsClearEntryErrors();
            SubjectsRefreshTotals();
            SetStatus("Subject added. " + SubjectsStepStatus, DeepNavy, Color.White);
            cboSubject.Focus();
        }

        // Remove a subject from the captured list
        private void dgvSubjects_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= subjectsRows.Count || e.ColumnIndex != colSubjectsRemove.Index)
            {
                return;
            }

            // The subject is only saved when continuing
            CapturedSubjectRow row = subjectsRows[e.RowIndex];
            subjectsRows.RemoveAt(e.RowIndex);
            if (pnlSummary.Visible)
            {
                // Recheck duplicate errors after removing a subject
                SubjectsValidateEntry();
            }

            SubjectsRefreshTotals();
            SetStatus(row.SubjectName + " removed. " + SubjectsStepStatus, DeepNavy, Color.White);
        }

        // Clear the grid selection after loading data
        private void dgvSubjects_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvSubjects.ClearSelection();
        }

        // Update the subject count, average and save button
        private void SubjectsRefreshTotals()
        {
            int count = subjectsRows.Count;
            string countText = count + (count == 1 ? " subject" : " subjects") + " captured";
            lblFooter.Text = count == 0
                ? "No subjects captured yet"
                : countText + "     Average " + subjectsRows.Average(r => r.Mark).ToString("0.0", CultureInfo.CurrentCulture) + " %";
            if (subjectsMinimumCount > 0)
            {
                lblCapturedCount.Text = count + " of " + subjectsMinimumCount + " captured";
            }
            else
            {
                lblCapturedCount.Text = countText;
            }
            dgvSubjects.ClearSelection();

            SetPrimaryEnabled(btnSubjectsSave, count >= 1);
            SubjectsUpdateNotice();
        }

        #endregion

        #region Notice and Enter key

        // Display guidance based on the number of subjects captured
        private void SubjectsUpdateNotice()
        {
            int count = subjectsRows.Count;
            if (subjectsMinimumCount <= 0)
            {
                ShowNotice(lblSubjectsNotice, "Add each subject on your latest school report with its mark. Save and continue opens once at least " +
                           "one subject is captured.", BlueTint, InkColour);
                return;
            }

            int needed = subjectsMinimumCount - count;
            if (needed > 0)
            {
                ShowNotice(lblSubjectsNotice, needed + (needed == 1 ? " more subject" : " more subjects") + " needed. Future Path checks eligibility " +
                           "against at least " + subjectsMinimumCount + " subjects; you have captured " + count +
                           ". You can still continue, but the outcome may be less complete.", AmberTint, AmberText);
            }
            else
            {
                ShowNotice(lblSubjectsNotice, "✓ Enough subjects captured (" + count + " of " + subjectsMinimumCount + "). Add any extra subjects, " +
                           "such as Advanced Programme subjects, or save and continue.", GreenTint, GreenColour);
            }
        }

        // Use Enter to add a subject while the entry group is focused
        private void pnlAddCard_Enter(object sender, EventArgs e)
        {
            AcceptButton = btnAddSubject;
        }

        // Restore Enter to Save and continue outside the entry group
        private void pnlAddCard_Leave(object sender, EventArgs e)
        {
            AcceptButton = btnSubjectsSave;
        }

        #endregion

        #region Button click events

        // Validate and save subjects before continuing
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

                // Confirm before leaving a valid but unadded subject behind
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
            catch (ArgumentException ex)
            {
                // Display errors returned by the controller
                SetStatus(ex.Message, RedColour, Color.White);
                return;
            }
            catch (Exception)
            {
                SetStatus("Your subjects could not be saved. Please try again.", RedColour, Color.White);
                return;
            }

            ForgetResults();
            GoToStep(tabInterests, 3);
        }

        // Return to the learner profile without removing captured subjects
        private void btnSubjectsBack_Click(object sender, EventArgs e)
        {
            tcMain.SelectedTab = tabProfile;
        }

        // Cancel the session and return home
        private void btnSubjectsCancel_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        #endregion
    }
}
