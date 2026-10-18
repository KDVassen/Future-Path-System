
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    // Displays confirmation messages before important actions
    // Allows users to confirm or cancel an operation
    public partial class ConfirmationDialog : Form
    {
        // Initialise the confirmation dialog
        public ConfirmationDialog()
        {
            InitializeComponent();
        }

        // Set the dialog title, message and confirmation button text
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
