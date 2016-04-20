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
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Synchronization.ClientServices.Configuration;

namespace SyncSvcUtilUI.SyncConfigWizardPages
{
    public partial class Step2_AddDatabaseInfoPage : UserControl, IWizardPage
    {
        private bool addMode;

        public Step2_AddDatabaseInfoPage()
        {
            InitializeComponent();
        }

        private void ReadAndBindData()
        {
            dbListBox.Items.Clear();
            foreach (TargetDatabaseConfigElement dbs in WizardHelper.Instance.SyncConfigSection.Databases)
            {
                dbListBox.Items.Add(dbs.Name);
            }
            dbListBox.SelectedIndex = -1;
            removeBtn.Enabled = true;
            editBtn.Enabled = true;
        }

        private void useIntAuthRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            SetVisibilityForSqlLoginFields();
        }

        private void SetVisibilityForSqlLoginFields()
        {
            unameTxtBox.Enabled = useSqlAuthRadioBtn.Checked;
            pwdTextBox.Enabled = useSqlAuthRadioBtn.Checked;
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            dbSettingsGrp.Enabled = true;
        }

        private void useSqlAuthRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            SetVisibilityForSqlLoginFields();
        }

        private void dbListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dbListBox.SelectedIndex != -1)
            {
                // When a TargetDatabase node is selected populate the values
                addMode = false;

                removeBtn.Enabled = true;
                editBtn.Enabled = true;

                var db = WizardHelper.Instance.SyncConfigSection.Databases.GetElementAt(dbListBox.SelectedIndex);
                useIntAuthRadioBtn.Checked = db.UseIntegratedAuth;
                useSqlAuthRadioBtn.Checked = !db.UseIntegratedAuth;
                cfgNameTxtBox.Text = db.Name;
                useIntAuthRadioBtn.Checked = db.UseIntegratedAuth;
                unameTxtBox.Text = db.UserName;
                pwdTextBox.Text = db.Password;
                dbServerTxtBox.Text = db.DbServer;
                dbNameTxtBox.Text = db.DbName;
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            dbSettingsGrp.Enabled = true;
            addMode = true;

            // Set all values to empty
            dbServerTxtBox.Text = dbNameTxtBox.Text =
                unameTxtBox.Text = pwdTextBox.Text = string.Empty;
            cfgNameTxtBox.Text = "[Enter New Target Database]";
            useIntAuthRadioBtn.Checked = true;
        }

        private void testBtn_Click(object sender, EventArgs e)
        {
            if (TestConnection())
            {
                MessageBox.Show("Connection successful.", "Connection test.", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            // First test connection
            if (TestConnection())
            {
                // Now add this to the Target database list
                if (string.IsNullOrEmpty(cfgNameTxtBox.Text))
                {
                    MessageBox.Show("Please enter a valid name for Config Name.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    cfgNameTxtBox.Focus();
                }
                if (addMode)
                {
                    var db = new TargetDatabaseConfigElement
                    {
                        Name = cfgNameTxtBox.Text,
                        UseIntegratedAuth = useIntAuthRadioBtn.Checked,
                        UserName = unameTxtBox.Text,
                        Password = pwdTextBox.Text,
                        DbServer = dbServerTxtBox.Text,
                        DbName = dbNameTxtBox.Text
                    };
                    WizardHelper.Instance.SyncConfigSection.Databases.Add(db);
                    ReadAndBindData();
                }
                else
                {
                    var db = WizardHelper.Instance.SyncConfigSection.Databases.GetElementAt(dbListBox.SelectedIndex);
                    db.Name = cfgNameTxtBox.Text;
                    db.UseIntegratedAuth = useIntAuthRadioBtn.Checked;
                    db.UserName = unameTxtBox.Text;
                    db.Password = pwdTextBox.Text;
                    db.DbServer = dbServerTxtBox.Text;
                    db.DbName = dbNameTxtBox.Text;
                }
                dbSettingsGrp.Enabled = false;
            }
        }

        private bool TestConnection()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = dbServerTxtBox.Text;
            builder.InitialCatalog = dbNameTxtBox.Text;
            builder.IntegratedSecurity = useIntAuthRadioBtn.Checked;
            if (!builder.IntegratedSecurity)
            {
                builder.UserID = unameTxtBox.Text;
                builder.Password = pwdTextBox.Text;
            }

            try
            {
                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                }
                return true;
            }
            catch (SqlException sqlE)
            {
                MessageBox.Show("Connection to database failed. " + sqlE.Message, "Connection test.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void removeBtn_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show("Do you want to remove the Database config info?", "Remove Database Config",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                WizardHelper.Instance.SyncConfigSection.Databases.Remove(dbListBox.SelectedItem.ToString());
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
            SetVisibilityForSqlLoginFields();
            dbListBox.SelectedIndex = -1;
            dbSettingsGrp.Enabled = false;
        }

        #endregion
    }
}