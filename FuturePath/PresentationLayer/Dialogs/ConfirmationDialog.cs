using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// A confirmation request (spec Section 7 and 2.2.7), raised before submitting a session,
    /// exporting a report, deactivating reference data, abandoning a session or signing out.
    /// Cancel is the safe default (the form's CancelButton).
    /// </summary>
    public partial class ConfirmationDialog : Form
    {
        /// <summary>Designer constructor.</summary>
        public ConfirmationDialog()
        {
            InitializeComponent();
        }

        /// <summary>Creates a confirmation request.</summary>
        /// <param name="title">What is being confirmed, e.g. "Export advisory summary".</param>
        /// <param name="message">The consequence, in plain words.</param>
        /// <param name="confirmText">The confirm button label, e.g. "Export".</param>
        public ConfirmationDialog(string title, string message, string confirmText) : this()
        {
            Text = "Future Path — " + title;
            lblHeading.Text = title;
            lblMessage.Text = message;
            if (!string.IsNullOrWhiteSpace(confirmText))
            {
                btnConfirm.Text = confirmText;
                btnConfirm.AccessibleName = confirmText;
            }
        }
    }
}
