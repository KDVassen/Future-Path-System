
namespace FuturePath.PresentationLayer
{
    partial class ValidationDialog
    {
        // Designer components
        private System.ComponentModel.IContainer components = null;

        // Dispose of resources used by the form
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        // Initialise the form controls
        private void InitializeComponent()
        {
            this.pnlBody = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.pnlRedBar = new System.Windows.Forms.Panel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblHeading = new System.Windows.Forms.Label();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.btnReattempt = new System.Windows.Forms.Button();
            this.pnlBody.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBody
            // 
            this.pnlBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.pnlBody.Controls.Add(this.lblMessage);
            this.pnlBody.Controls.Add(this.pnlRedBar);
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Location = new System.Drawing.Point(0, 70);
            this.pnlBody.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Size = new System.Drawing.Size(630, 176);
            this.pnlBody.TabIndex = 0;
            // 
            // lblMessage
            // 
            this.lblMessage.AccessibleName = "Entries to correct";
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.lblMessage.Location = new System.Drawing.Point(8, 0);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Padding = new System.Windows.Forms.Padding(18, 18, 18, 10);
            this.lblMessage.Size = new System.Drawing.Size(622, 176);
            this.lblMessage.TabIndex = 0;
            // 
            // pnlRedBar
            // 
            this.pnlRedBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(38)))), ((int)(((byte)(44)))));
            this.pnlRedBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlRedBar.Location = new System.Drawing.Point(0, 0);
            this.pnlRedBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlRedBar.Name = "pnlRedBar";
            this.pnlRedBar.Size = new System.Drawing.Size(8, 176);
            this.pnlRedBar.TabIndex = 1;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.pnlHeader.Controls.Add(this.lblHeading);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(630, 70);
            this.pnlHeader.TabIndex = 1;
            // 
            // lblHeading
            // 
            this.lblHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblHeading.ForeColor = System.Drawing.Color.White;
            this.lblHeading.Location = new System.Drawing.Point(0, 0);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.lblHeading.Size = new System.Drawing.Size(630, 70);
            this.lblHeading.TabIndex = 0;
            this.lblHeading.Text = "Entries to correct";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlActions
            // 
            this.pnlActions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.pnlActions.Controls.Add(this.btnReattempt);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlActions.Location = new System.Drawing.Point(0, 246);
            this.pnlActions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(630, 80);
            this.pnlActions.TabIndex = 2;
            // 
            // btnReattempt
            // 
            this.btnReattempt.AccessibleName = "Re-attempt correction";
            this.btnReattempt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.btnReattempt.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReattempt.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnReattempt.FlatAppearance.BorderSize = 0;
            this.btnReattempt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReattempt.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnReattempt.ForeColor = System.Drawing.Color.White;
            this.btnReattempt.Location = new System.Drawing.Point(380, 15);
            this.btnReattempt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnReattempt.Name = "btnReattempt";
            this.btnReattempt.Size = new System.Drawing.Size(225, 50);
            this.btnReattempt.TabIndex = 0;
            this.btnReattempt.Text = "Try again";
            this.btnReattempt.UseVisualStyleBackColor = false;
            // 
            // ValidationDialog
            // 
            this.AcceptButton = this.btnReattempt;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnReattempt;
            this.ClientSize = new System.Drawing.Size(630, 326);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlActions);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(36)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ValidationDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Future Path — Entries to correct";
            this.pnlBody.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlBody;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Panel pnlRedBar;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Button btnReattempt;
    }
}
