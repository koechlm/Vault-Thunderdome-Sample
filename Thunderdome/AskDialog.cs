using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Thunderdome.Model;
using System.Linq;

namespace Thunderdome
{
    /// <summary>
    /// Dialog for asking user about configuration updates installation
    /// </summary>
    public partial class AskDialog : Form
    {

        /// <summary>
        /// Creates instance of <c>AskDialog</c> and shows it to user
        /// </summary>
        /// <param name="model">Deployment model containing updates description</param>
        /// <returns>Value of <c>UpdateDialogResult</c> indicating that user either wants to update,
        ///  does not want to update right now, or does not want updates to be installed never</returns>
        public static UpdateDialogResult ShowDialog(DeploymentModel defaultDeployment)
        {
            using (AskDialog dialog = new AskDialog(defaultDeployment))
            {

                dialog._neverButton.Enabled = defaultDeployment!=null && defaultDeployment.ForceUpdate==false; //If admin force the deployment

                if (dialog.ShowDialog() == DialogResult.OK)
                    return dialog._updateDialogResult;

                return UpdateDialogResult.No;
            }
        }

        private UpdateDialogResult _updateDialogResult;


        private AskDialog(DeploymentModel defaultDeployment)
        {
            InitializeComponent();


                _updateListTextBox.Text = FormatDeployment(defaultDeployment);



            _updateDialogResult = UpdateDialogResult.No;
        }

        private void YesButton_Click(object sender, EventArgs e)
        {
            _updateDialogResult = UpdateDialogResult.Yes;
            DialogResult = DialogResult.OK;
        }

        private void NoButton_Click(object sender, EventArgs e)
        {
            _updateDialogResult = UpdateDialogResult.No;
            DialogResult = DialogResult.OK;
        }

        private void NeverButton_Click(object sender, EventArgs e)
        {
            _updateDialogResult = UpdateDialogResult.Never;
            DialogResult = DialogResult.OK;
        }

        private static string FormatDeployment(DeploymentModel model)
        {
            StringBuilder updateList = new StringBuilder();
            foreach (DeploymentContainer container in model.Containers)
            {
                updateList.AppendLine(container.DisplayName);
                foreach (DeploymentItem item in container.DeploymentItems)
                {
                    updateList.Append("- ").AppendLine(item.DisplayName);
                }
                updateList.AppendLine();
            }
            return updateList.ToString();
        }

    }
}
