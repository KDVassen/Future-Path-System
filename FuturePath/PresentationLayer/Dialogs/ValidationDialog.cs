using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// The validation dialog (spec 2.2.7), used for UI 2.1a Invalid Profile Data, UI 3.1a
    /// Invalid / Duplicate Subjects and UI 8.1a Invalid Login. It is raised when the user tries to
    /// leave a capture screen that still holds invalid data. Its single <b>Re-attempt correction</b>
    /// button closes the dialog; the calling screen then puts focus back on the first offending
    /// control, and nothing the user typed is lost.
    /// </summary>
    public partial class ValidationDialog : Form
    {
        /// <summary>Designer constructor.</summary>
        public ValidationDialog()
        {
            InitializeComponent();
        }

        /// <summary>Creates the dialog with a title and one line per entry to correct.</summary>
        /// <param name="title">E.g. "Invalid profile data".</param>
        /// <param name="lines">Messages that name the field and the correction.</param>
        public ValidationDialog(string title, IList<string> lines) : this()
        {
            Text = "Future Path — " + title;
            lblHeading.Text = title;
            if (lines == null || lines.Count == 0)
            {
                lblMessage.Text = "Some entries need correcting before you can continue.";
            }
            else
            {
                lblMessage.Text = "• " + string.Join(Environment.NewLine + "• ", lines);
            }
        }
    }
}
