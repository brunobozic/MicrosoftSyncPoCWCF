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
using System.Collections.Generic;
using System.Windows.Forms;
using SyncSvcUtilUI.CodegenWizardPages;
using SyncSvcUtilUI.SyncConfigWizardPages;
using SyncSvcUtilUI.SyncProvisionWizardPages;

namespace SyncSvcUtilUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void genOrEdirSyncCfgLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Launch the wizard for Sync Config generation/creator
            var wizard = new SyncMasterWizard();

            // Add pages
            var pages = new List<UserControl>();
            pages.Add(new Step1_CreateOrOpenPage());
            pages.Add(new Step2_AddDatabaseInfoPage());
            pages.Add(new Step3_AddSyncScopePage());
            pages.Add(new Step4_AddSyncTablesPage());
            pages.Add(new Step5_SummaryAndFinishPage());

            // Set wizard pages
            wizard.SetPages(pages.ToArray());

            // Show wizard
            wizard.ShowDialog(this);
        }

        private void provLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Launch the wizard for Provision/Deprovision link
            var wizard = new SyncMasterWizard();

            // Add pages
            var pages = new List<UserControl>();
            pages.Add(new Setp1_GetAndOpenConfigFile());
            pages.Add(new Step2_SummaryOfProvDeProvPage());

            // Set wizard pages
            wizard.SetPages(pages.ToArray());

            // Show wizard
            wizard.ShowDialog(this);
        }

        private void codeGenLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Launch the wizard for Provision/Deprovision link
            var wizard = new SyncMasterWizard();

            // Add pages
            var pages = new List<UserControl>();
            pages.Add(new Step1_PickConfigOrCSDLModelPage());
            pages.Add(new Step2_SelectCodeGenPrams());
            pages.Add(new Step3_SummaryOfCodegenPage());

            // Set wizard pages
            wizard.SetPages(pages.ToArray());

            // Show wizard
            wizard.ShowDialog(this);
        }
    }
}