using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thunderdome.Model;

namespace Thunderdome
{
    public partial class _deployForm : Form
    {
        public string SelectedDeployment { get; set; }

        public _deployForm()
        {
            InitializeComponent();
        }

        public void Configure(List<DeploymentModel> deployments)
        {
            _deploymentsComboBox.Items.AddRange(deployments.Select(d => d.DisplayName).ToArray());
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _deployButton_Click(object sender, EventArgs e)
        {
            SelectedDeployment = _deploymentsComboBox.Text;
            Close();
        }
    }
}
