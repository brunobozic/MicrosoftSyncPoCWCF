using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MicrosoftSyncPoC.Client.WPF.Properties;
using MicrosoftSyncPoC.EF.Client.Models;

namespace MicrosoftSyncPoC.Client.WPF
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Create a BackgroundWorker object to synchronize without blocking
        // the UI thread
        private readonly BackgroundWorker backgroundWorker1;

        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker1 = new BackgroundWorker {WorkerReportsProgress = true};
            // Register the various BackgroundWorker events
            backgroundWorker1.DoWork
                += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged
                += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted
                += backgroundWorker1_RunWorkerCompleted;
        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            // Check if sync is already in progress
            if (!backgroundWorker1.IsBusy)
            {
                msg.AppendText(
                    "Starting Data Synchronization Process...\r\n Please wait till the process compeletes.\r\n");
                Application.Current.MainWindow.Cursor = Cursors.Wait;

                var syncHelper = new SynchronizationHelper();
                // Start synchronization
                backgroundWorker1.RunWorkerAsync(syncHelper);
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            // Insert a test value in local CE database
            using (var con =
                new SqlConnection(Settings.Default.DbConnection))
            {
                var cmd =
                    new SqlCommand("INSERT INTO [EmployeeSales] ([Id],[ProductId],[EmployeeId],[Unit],[Date])"
                                   + " Values (@Id,@ProductId,@EmployeeId,@Unit,@Date)");
                cmd.Parameters.Add("@Id", Guid.NewGuid());
                //Hardcoded ProductId
                cmd.Parameters.Add("@ProductId", "F4FC70E6-5DC7-4982-95F0-32E806E21D84");
                //Hardcoded EmployeeId
                cmd.Parameters.Add("@EmployeeId", "0A5F6AD1-F345-48D4-B6D6-0850A7C930C9");
                // Hardcoded Units
                cmd.Parameters.Add("@Unit", 10);
                cmd.Parameters.Add("@Date", DateTime.Now);
                cmd.Connection = con;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                catch (Exception ed)
                {
                    MessageBox.Show("Error Occurred while inserting.\r\n" + ed);
                }
            }
        }

        // Method to start syncthonization in background
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker;
            worker = (BackgroundWorker) sender;
            var syncHelper = (SynchronizationHelper) e.Argument;
            syncHelper.SynchronizeAsync(worker, e);
        }

        //Method to report synchronization progress
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var results = (SyncResults) e.UserState;
            if (results != null)
            {
                DisplayStats(results);
            }
        }

        //Method runs when synchtonization compeletes
        private void backgroundWorker1_RunWorkerCompleted(
            object sender, RunWorkerCompletedEventArgs e)
        {
            Application.Current.MainWindow.Cursor = Cursors.Arrow;
            if (e.Error != null)
            {
                msg.AppendText(
                    "An Error has occurred. Please try synchronization later.\r\nThe error:" + e.Error.Message);
                MessageBox.Show("Error: " + e.Error.Message);
            }
            else
            {
                msg.AppendText("Synchronization Finished Successfully\r\n");
                MessageBox.Show("Finished Synchronization");
            }
        }

        // Method to format the SyncResults for display
        private void DisplayStats(SyncResults results)
        {
            var diff =
                results.Stats.SyncEndTime.Subtract(results.Stats.SyncStartTime);

            msg.AppendText(
                string.Format(
                    "{4}:  - Total Time To Synchronize = {0}:{1}:{2}:{3}\r\nTotal Records Uploaded: {5}  Total Records Downloaded: {6}\r\n",
                    diff.Hours, diff.Minutes, diff.Seconds,
                    diff.Milliseconds, results.Message,
                    results.Stats.UploadChangesTotal,
                    results.Stats.DownloadChangesTotal));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<Customer> cust = new List<Customer>();
            using (var ctx = new northwind_guid_clientContext())
            {
                cust.AddRange(ctx.Customers.Where(item => item != null));
            }
        }
    }
}