
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    // Displays validation errors when users enter incorrect information
    // Shows which fields need correcting before continuing
    public partial class ValidationDialog : Form
    {
        // Initialise the validation dialog
        public ValidationDialog()
        {
            InitializeComponent();
        }

        // Display the validation errors with a title and correction messages
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
