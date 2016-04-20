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

namespace SyncSvcUtilUI
{
    public partial class SyncMasterWizard : Form
    {
        private const string FinishText = "&Finish";
        private const string NextText = "&Next";
        // Represents the current page the wizard is on.
        private int curPage = -1;
        // Represents the pages that makes this wizard
        private UserControl[] wizardPages;

        public SyncMasterWizard()
        {
            InitializeComponent();
            wizardPages = new UserControl[0];
        }

        public object WizardHelper { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            MoveForward();
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            MoveBack();
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            MoveForward();
        }

        private void MoveForward()
        {
            if (nextBtn.Text.Equals(NextText))
            {
                if (curPage >= 0)
                {
                    // Invoke the OnMovingNext callback
                    if (!((IWizardPage) wizardPages[curPage]).OnMovingNext())
                    {
                        return;
                    }
                }

                curPage++;

                if (wizardPages.Length > curPage)
                {
                    ShowWizardPageAtIndex();
                }
            }
            else
            {
                try
                {
                    // Call the finish callback
                    ((IWizardPage) wizardPages[curPage]).OnFinish();
                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception exp)
                {
                    // If exception then show it to user.
                    MessageBox.Show(exp.Message, "Error in finishing wizard workflow.", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            SetButtonLabels();
        }

        private void ShowWizardPageAtIndex()
        {
            // Remove and add current page to the wizard
            wizardPagePanel.Controls.Clear();
            wizardPagePanel.Controls.Add(wizardPages[curPage]);
            wizardPagePanel.Controls[0].Dock = DockStyle.Fill;
            ((IWizardPage) wizardPages[curPage]).OnFocus();
        }

        private void MoveBack()
        {
            if (curPage != 0)
            {
                curPage--;
            }

            if (curPage >= 0)
            {
                ShowWizardPageAtIndex();
            }

            SetButtonLabels();
        }

        private void SetButtonLabels()
        {
            backBtn.Enabled = curPage > 0;
            nextBtn.Text = (curPage >= wizardPages.Length - 1) ? FinishText : NextText;
        }

        internal void SetPages(UserControl[] pages)
        {
            if (pages == null)
            {
                throw new ArgumentNullException("pages");
            }
            wizardPages = pages;
        }
    }
}