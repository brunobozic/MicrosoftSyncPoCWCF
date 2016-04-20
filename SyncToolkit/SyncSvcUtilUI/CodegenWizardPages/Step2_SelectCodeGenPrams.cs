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

namespace SyncSvcUtilUI.CodegenWizardPages
{
    public partial class Step2_SelectCodeGenPrams : UserControl, IWizardPage
    {
        public Step2_SelectCodeGenPrams()
        {
            InitializeComponent();

            folderBrowserDialog1.Description = "Pick a location for generated files";
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyDocuments;
            folderBrowserDialog1.ShowNewFolderButton = true;

            langBox.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                outputDirTxtBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        #region IWizardPage Members

        public void OnFocus()
        {
            outputDirTxtBox.Text = folderBrowserDialog1.SelectedPath;
            nsTxtBox.Text = WizardHelper.Instance.CodeGenWizardHelper[WizardHelper.SELECTED_CONFIG_NAME];
            targetBox.Items.Clear();
            if (WizardHelper.Instance.CodeGenWizardHelper[WizardHelper.SELECTED_CODEGEN_SOURCE] ==
                WizardHelper.CONFIG_FILE_CODEGEN_SOURCE)
            {
                targetBox.Items.AddRange(new[] {"Server", "IsolatedStore Client", "Generic Client"});
            }
            else
            {
                targetBox.Items.AddRange(new[] {"IsolatedStore Client", "Generic Client"});
            }
        }

        public bool OnMovingNext()
        {
            if (targetBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please specify a target for which to generate files.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrEmpty(outputDirTxtBox.Text))
            {
                MessageBox.Show("Please specify a output folder name for generated files.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrEmpty(nsTxtBox.Text))
            {
                MessageBox.Show("Please specify a non empty namespace for generated files.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            WizardHelper.Instance.CodeGenWizardHelper[WizardHelper.CODEGEN_LANGUAGE] = langBox.SelectedItem.ToString();
            WizardHelper.Instance.CodeGenWizardHelper[WizardHelper.CODEGEN_NAMESPACE] = nsTxtBox.Text;
            WizardHelper.Instance.CodeGenWizardHelper[WizardHelper.CODEGEN_OUTPREFIX] = outprefixTxtBox.Text ??
                                                                                        string.Empty;
            WizardHelper.Instance.CodeGenWizardHelper[WizardHelper.CODEGEN_OUTDIRECTORY] = outputDirTxtBox.Text;
            switch (targetBox.SelectedItem.ToString())
            {
                case "Server":
                    WizardHelper.Instance.CodeGenWizardHelper[WizardHelper.CODEGEN_TARGET] = "server";
                    break;
                case "Generic Client":
                    WizardHelper.Instance.CodeGenWizardHelper[WizardHelper.CODEGEN_TARGET] = "client";
                    break;
                default:
                    WizardHelper.Instance.CodeGenWizardHelper[WizardHelper.CODEGEN_TARGET] = "isclient";
                    break;
            }

            return true;
        }

        public void OnFinish()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}