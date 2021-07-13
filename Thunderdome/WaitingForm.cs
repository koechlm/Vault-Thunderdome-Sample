using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Thunderdome
{
    public partial class WaitingForm : Form
    {
        public WaitingForm(Form parent, int nbSteps)
        {
            InitializeComponent();

            this.Location = new System.Drawing.Point(parent.Location.X + (parent.Width - this.Width) / 2, parent.Location.Y + (parent.Height - this.Height) / 2);

            this._progressBar.Maximum = nbSteps;
        }


        public void SetProgress(string message, int currentStepId)
        {
            this._progressLabel.Text = message;
            this._progressBar.Value = currentStepId;
            //this._progressBar.Refresh();

            //this.Refresh();

            //Application.DoEvents();
        }
    }
}
