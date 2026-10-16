using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 6.2b — Funding Options (step 5 of 6; spec 2.3.8).
    /// Nothing is typed. The learner sets the Faculty and Funding type filters (both bound to stored
    /// reference data), may limit the list to options that are still open, and selects a row to fill
    /// the read-only detail card. Only <b>active</b> funding linked to the faculty or programme is
    /// listed. Funding is labelled as sample information to verify with the funder. There is no
    /// validation. <b>Add to Summary</b> sets <c>FundingInfoRequested = 1</c> and opens UI 7.
    /// </summary>
    public partial class MainForm
    {
        private const string FundingReadyStatus = "Step 5 of 6 · Select a funding option to see its details.";

        /// <summary>The UI 5 faculty the Faculty filter was last defaulted to.</summary>
        private int? fundingDefaultedFacultyId;

        /// <summary>True once both filters have been bound for this session.</summary>
        private bool fundingListsLoaded;

        #region Shown, button clicks and reset

        /// <summary>
        /// Called by PageShown each time UI 6.2b appears: shows the ready status, binds the filters once
        /// per session and lists the funding for the UI 5 faculty.
        /// </summary>
        private void FundingShown()
        {
            SetStatus(FundingReadyStatus, DeepNavy, Color.White);
            if (!fundingListsLoaded)
            {
                FundingLoadLists();
            }

            FundingApplyDefaultFaculty();
        }

        /// <summary>Add to Summary → sets FundingInfoRequested = 1, then opens UI 7 (Back there returns here).</summary>
        private void btnFundingAdd_Click(object sender, EventArgs e)
        {
            try
            {
                FundingRequestInSummary();
            }
            catch (Exception)
            {
                SetStatus("Funding could not be added to your summary. Please try again.", AmberColour, InkColour);
                return;
            }

            OpenSummary(tabFunding);
        }

        /// <summary>Back → UI 6 (Eligibility results).</summary>
        private void btnFundingBack_Click(object sender, EventArgs e)
        {
            tcResults.SelectedTab = tabOutcome;
        }

        /// <summary>
        /// Returns UI 6.2b to a brand-new state for the next learner (spec 2.1): no faculty or funding
        /// type chosen, "still open" unticked, an empty list and count, the default heading, an empty
        /// detail card, and the filters are bound again the next time the screen is shown.
        /// </summary>
        private void FundingReset()
        {
            fundingListsLoaded = false;          // first, so unticking below does not list funding
            fundingDefaultedFacultyId = null;

            cboFundFaculty.SelectedIndex = -1;
            cboFundingType.SelectedIndex = -1;
            chkOpenOnly.Checked = false;

            dgvFunding.AutoGenerateColumns = false;
            dgvFunding.DataSource = null;
            lblFundingCount.Text = string.Empty;
            lblFundingHeading.Text = "Funding linked to your faculty";
            FundingClearDetail();
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds both filters to stored reference data (never literal Items).</summary>
        private void FundingLoadLists()
        {
            try
            {
                ReferenceDataController referenceDataController = new ReferenceDataController();
                cboFundFaculty.DisplayMember = "FacultyName";
                cboFundFaculty.ValueMember = "FacultyID";
                cboFundFaculty.DataSource = referenceDataController.GetActiveFaculties();
                cboFundFaculty.SelectedIndex = -1;

                // SystemParameter rows FUNDING_TYPE_1..4 after the BLL's "All funding types" row (FundingType "").
                FundingController fundingController = new FundingController();
                cboFundingType.DisplayMember = "ParameterValue";
                cboFundingType.ValueMember = "FundingType";
                cboFundingType.DataSource = fundingController.GetFundingTypes(true);
                cboFundingType.SelectedIndex = 0;
                fundingListsLoaded = true;
            }
            catch (Exception)
            {
                SetStatus("The faculty and funding type lists could not be loaded. Go back and try again.", AmberColour, InkColour);
            }
        }

        /// <summary>Lists the active funding for the chosen faculty (and the selected programme) using the filters.</summary>
        private void FundingLoad()
        {
            FundingClearDetail();
            FundingUpdateHeading();
            try
            {
                // 0 and "" mean "no filter" to the BLL.
                int facultyId = 0;
                if (cboFundFaculty.SelectedIndex >= 0)
                {
                    facultyId = (int)cboFundFaculty.SelectedValue;
                }

                int programmeId = 0;
                if (SelectedProgrammeId.HasValue)
                {
                    programmeId = SelectedProgrammeId.Value;
                }

                string fundingType = string.Empty;
                if (cboFundingType.SelectedIndex >= 0)
                {
                    fundingType = Convert.ToString(cboFundingType.SelectedValue, CultureInfo.InvariantCulture);
                }

                // Only active funding linked to the faculty or the programme, nearest closing date first.
                FundingController fundingController = new FundingController();
                dgvFunding.DataSource = fundingController.GetActiveFunding(facultyId, programmeId, fundingType, chkOpenOnly.Checked);
            }
            catch (Exception)
            {
                SetStatus("Funding options could not be loaded. Please try again.", AmberColour, InkColour);
            }
        }

        /// <summary>Records that the learner wants funding in the summary (FundingInfoRequested = 1).</summary>
        private void FundingRequestInSummary()
        {
            FundingController fundingController = new FundingController();
            fundingController.RequestFundingInfo(SessionId.Value);
        }

        #endregion

        #region Filters

        /// <summary>Defaults the Faculty filter to the UI 5 faculty whenever that choice has changed.</summary>
        private void FundingApplyDefaultFaculty()
        {
            if (fundingDefaultedFacultyId == PreferredFacultyId && dgvFunding.DataSource != null)
            {
                return;
            }

            if (PreferredFacultyId.HasValue && cboFundFaculty.Items.Count > 0)
            {
                cboFundFaculty.SelectedValue = PreferredFacultyId.Value;
            }

            fundingDefaultedFacultyId = PreferredFacultyId;
            FundingLoad();
        }

        /// <summary>A different faculty relists the funding.</summary>
        private void cboFundFaculty_SelectionChangeCommitted(object sender, EventArgs e) { FundingLoad(); }

        /// <summary>A different funding type relists the funding.</summary>
        private void cboFundingType_SelectionChangeCommitted(object sender, EventArgs e) { FundingLoad(); }

        /// <summary>
        /// "Show only options that are still open" relists the funding — once the filters are loaded, so
        /// unticking the box in <see cref="FundingReset"/> does not list anything.
        /// </summary>
        private void chkOpenOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (fundingListsLoaded)
            {
                FundingLoad();
            }
        }

        /// <summary>"Funding linked to the Faculty of Science" — follows the Faculty filter.</summary>
        private void FundingUpdateHeading()
        {
            string faculty = cboFundFaculty.SelectedIndex >= 0 ? cboFundFaculty.Text : string.Empty;
            if (faculty.Length == 0)
            {
                lblFundingHeading.Text = "Funding linked to your faculty";
            }
            else if (faculty.StartsWith("Faculty", StringComparison.OrdinalIgnoreCase))
            {
                lblFundingHeading.Text = "Funding linked to the " + faculty;
            }
            else
            {
                lblFundingHeading.Text = "Funding linked to the Faculty of " + faculty;
            }
        }

        #endregion

        #region Grid and detail card (display only)

        /// <summary>Selecting a row fills the read-only detail card.</summary>
        private void dgvFunding_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewRow row = dgvFunding.CurrentRow;
            if (row == null)
            {
                FundingClearDetail();
                return;
            }

            lblDetailHeading.Text = FundingCellText(row, colFundingName);
            txtEligibility.Text = FundingCellText(row, colFundingEligibility);
            txtDocuments.Text = FundingCellText(row, colFundingDocuments);
            txtContact.Text = FundingCellText(row, colFundingContact);
        }

        /// <summary>Shows the first option's details as soon as the list is filled.</summary>
        private void dgvFunding_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            int count = dgvFunding.Rows.Count;
            lblFundingCount.Text = count == 1 ? "1 funding option shown" : count + " funding options shown";
            if (count > 0)
            {
                dgvFunding.CurrentCell = dgvFunding.Rows[0].Cells[colFundingName.Index];
            }
            else
            {
                FundingClearDetail();
            }
        }

        /// <summary>
        /// Status chip derived from the stored ClosingDate against today's date (display only):
        /// "✓ Open" on green, or "– Closed" in grey. No closing date counts as open.
        /// </summary>
        private void dgvFunding_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colFundingStatus.Index)
            {
                return;
            }

            object closing = dgvFunding.Rows[e.RowIndex].Cells[colFundingClosingDate.Index].Value;
            bool open = closing == null || closing == DBNull.Value
                        || Convert.ToDateTime(closing, CultureInfo.InvariantCulture).Date >= DateTime.Today;
            e.Value = open ? "✓ Open" : "– Closed";
            e.CellStyle.Font = ChipFont;
            e.CellStyle.ForeColor = open ? GreenColour : MutedColour;
            e.CellStyle.BackColor = open ? GreenTint : RuleColour;
            e.FormattingApplied = true;
        }

        /// <summary>A cell's text for the detail card, or a dash when it is empty.</summary>
        private static string FundingCellText(DataGridViewRow row, DataGridViewColumn column)
        {
            object value = row.Cells[column.Index].Value;
            string text = value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value, CultureInfo.CurrentCulture);
            return text.Length == 0 ? "—" : text;
        }

        /// <summary>Empties the detail card and restores its prompt.</summary>
        private void FundingClearDetail()
        {
            lblDetailHeading.Text = "Select a funding option to see its details";
            txtEligibility.Text = string.Empty;
            txtDocuments.Text = string.Empty;
            txtContact.Text = string.Empty;
        }

        #endregion
    }
}
