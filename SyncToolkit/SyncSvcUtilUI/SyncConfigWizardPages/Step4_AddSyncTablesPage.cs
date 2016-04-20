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
    public partial class Step4_AddSyncTablesPage : UserControl, IWizardPage
    {
        public Step4_AddSyncTablesPage()
        {
            InitializeComponent();
        }

        private void ReadAndBindData()
        {
            syncTablesBox.Items.Clear();
            scopeComboBox.Items.Clear();
            foreach (SyncScopeConfigElement scope in WizardHelper.Instance.SyncConfigSection.SyncScopes)
            {
                scopeComboBox.Items.Add(scope.Name);
            }
            scopeComboBox.SelectedIndex = -1;
            selectTablesGrp.Enabled = false;
            upBtn.Enabled = false;
            downBtn.Enabled = false;
        }

        private void delBtn_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show("Do you want to remove the selected SyncTable?", "Remove SyncTable Config",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                WizardHelper.Instance.SyncConfigSection.SyncScopes.GetElementAt(scopeComboBox.SelectedIndex)
                    .SyncTables.Remove(syncTablesBox.SelectedItem.ToString());
                ReadAndBindTablesData();
            }
        }

        private void scopeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (scopeComboBox.SelectedIndex > -1)
            {
                ReadAndBindTablesData();
                selectTablesGrp.Enabled = true;
            }
        }

        private void ReadAndBindTablesData()
        {
            var selectedScope =
                WizardHelper.Instance.SyncConfigSection.SyncScopes.GetElementAt(scopeComboBox.SelectedIndex);
            syncTablesBox.Items.Clear();
            foreach (SyncTableConfigElement table in selectedScope.SyncTables)
            {
                syncTablesBox.Items.Add(table.Name);
            }
            syncTablesBox.SelectedIndex = -1;
            delBtn.Enabled = syncTablesBox.Items.Count > 0;
        }

        private void syncTablesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (syncTablesBox.SelectedIndex > -1)
            {
                var selectedScope =
                    WizardHelper.Instance.SyncConfigSection.SyncScopes.GetElementAt(scopeComboBox.SelectedIndex);

                var table = selectedScope.SyncTables.GetElementAt(syncTablesBox.SelectedIndex);
                if (string.IsNullOrEmpty(table.FilterClause))
                {
                    filterColTxtBox.Text = table.FilterClause;
                }

                allColsIncludedOption.Checked = table.IncludeAllColumns;
                filterColTxtBox.Text = table.FilterClause;
            }
            // Allow reordering the SyncTable orders.
            upBtn.Enabled = syncTablesBox.SelectedIndex > 0;
            downBtn.Enabled = syncTablesBox.SelectedIndex != syncTablesBox.Items.Count - 1;
        }

        private void upBtn_Click(object sender, EventArgs e)
        {
            MoveTable(syncTablesBox.SelectedIndex, syncTablesBox.SelectedIndex - 1);
        }

        private void downBtn_Click(object sender, EventArgs e)
        {
            MoveTable(syncTablesBox.SelectedIndex, syncTablesBox.SelectedIndex + 1);
        }

        private void MoveTable(int currentIndex, int newIndex)
        {
            var selectedScope =
                WizardHelper.Instance.SyncConfigSection.SyncScopes.GetElementAt(scopeComboBox.SelectedIndex);
            var table = selectedScope.SyncTables.GetElementAt(currentIndex);

            // Remove element from current index
            selectedScope.SyncTables.RemoveAt(syncTablesBox.SelectedIndex);
            if (newIndex == selectedScope.SyncTables.Count)
            {
                // It means the element should go to the end of the list. Just call add
                selectedScope.SyncTables.Add(table);
                newIndex = selectedScope.SyncTables.Count - 1;
            }
            else
            {
                // Re-Add the element at the new index
                selectedScope.SyncTables.Add(newIndex, table);
            }

            // Rebind
            ReadAndBindTablesData();
            syncTablesBox.SelectedIndex = newIndex;
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            using (var dialog = new Step4a_ReadTableInfoFromDbPage(scopeComboBox.SelectedIndex))
            {
                dialog.ShowDialog(this);
            }
            ReadAndBindTablesData();
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
        }

        #endregion
    }
}