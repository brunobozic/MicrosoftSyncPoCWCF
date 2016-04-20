using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MicrosoftSyncPoC.Repository.EF.Models;

namespace TestHarness
{
    public partial class TestHarness : Form
    {
        public TestHarness()
        {
            InitializeComponent();
        }

        private void btnTest1_Click(object sender, EventArgs e)
        {
            var categories = new List<Category>();

            using (var context = new northwindContext())
            {
                categories = context.Categories.Where(x => x.CategoryID != Guid.Empty).ToList();
            }
        }
    }
}