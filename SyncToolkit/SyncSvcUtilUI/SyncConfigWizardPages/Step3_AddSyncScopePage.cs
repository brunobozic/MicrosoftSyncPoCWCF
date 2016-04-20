// Copyright 2010 Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License"); 
// You may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
// INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR 
// CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
// MERCHANTABLITY OR NON-INFRINGEMENT. 

// See the Apache 2 License for the specific language governing 
// permissions and limitations under the License.

using System;
using System.Windows.Forms;
using Microsoft.Synchronization.ClientServices.Configuration;

namespace SyncSvcUtilUI.SyncConfigWizardPages
{
    public partial class Step3_AddSyncScopePage : UserControl, IWizardPage
    {
        private bool addMode;

        public Step3_AddSyncScopePage()
        {
            InitializeComponent();
        }

        private void ReadAndBindData()
        {
            syncScopeBox.Items.Clear();
            foreach (SyncScopeConfigElement scope in WizardHelper.Instance.SyncConfigSection.SyncScopes)
            {
                syncScopeBox.Items.Add(scope.Name);
            }
            syncScopeBox.SelectedIndex = -1;
            removeBtn.Enabled = true;
            editBtn.Enabled = true;
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            scopeSettingsGrp.Enabled = true;
        }

        private void syncScopeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (syncScopeBox.SelectedIndex != -1)
            {
                // When a SyncScope node is selected populate the values
                addMode = false;

                removeBtn.Enabled = true;
                editBtn.Enabled = true;

                var scope = WizardHelper.Instance.SyncConfigSection.SyncScopes.GetElementAt(syncScopeBox.SelectedIndex);
                scopeNameTxtBox.Text = scope.Name;
                schemaNameTxtBox.Text = scope.SchemaName;
                isTempScopeOption.Checked = scope.IsTemplateScope;
                enableBulkProcsOption.Checked = scope.EnableBulkApplyProcedures;
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            scopeSettingsGrp.Enabled = true;
            addMode = true;

            // Set all values to empty
            schemaNameTxtBox.Text = string.Empty;
            scopeNameTxtBox.Text = "[Enter New SyncScope Name]";
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            // Now add this to the SyncScopes list
            if (string.IsNullOrEmpty(scopeNameTxtBox.Text))
            {
                MessageBox.Show("Please enter a valid name for sync scope Name.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                scopeNameTxtBox.Focus();
            }
            if (addMode)
            {
                var scope = new SyncScopeConfigElement
                {
                    Name = scopeNameTxtBox.Text,
                    IsTemplateScope = isTempScopeOption.Checked,
                    EnableBulkApplyProcedures = enableBulkProcsOption.Checked
                };
                if (!string.IsNullOrEmpty(schemaNameTxtBox.Text))
                {
                    scope.SchemaName = schemaNameTxtBox.Text;
                }

                WizardHelper.Instance.SyncConfigSection.SyncScopes.Add(scope);
                ReadAndBindData();
            }
            else
            {
                var scope = WizardHelper.Instance.SyncConfigSection.SyncScopes.GetElementAt(syncScopeBox.SelectedIndex);
                scope.Name = scopeNameTxtBox.Text;
                if (!string.IsNullOrEmpty(schemaNameTxtBox.Text))
                {
                    scope.SchemaName = schemaNameTxtBox.Text;
                }
                scope.IsTemplateScope = isTempScopeOption.Checked;
                scope.EnableBulkApplyProcedures = enableBulkProcsOption.Checked;
            }
            scopeSettingsGrp.Enabled = false;
        }

        private void removeBtn_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show("Do you want to remove the selected SyncScope?", "Remove SyncScope Config",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                WizardHelper.Instance.SyncConfigSection.Databases.Remove(syncScopeBox.SelectedItem.ToString());
                ReadAndBindData();
            }
        }

        #region IWizardPage Members

        public bool OnMovingNext()
        {
            return true;
        }

        public void OnFinish()
        {
            // Noop for Finish
        }

        public void OnFocus()
        {
            ReadAndBindData();
            syncScopeBox.SelectedIndex = -1;
            scopeSettingsGrp.Enabled = false;
        }

        #endregion
    }
}