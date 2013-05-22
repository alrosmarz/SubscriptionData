using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SubscriptionData
{
    public partial class frmTabCorp : Form
    {
        public frmTabCorp()
        {
            InitializeComponent();
        }

        public void setText(DataTable dt)
        {
            //dataGridView1.DataSource = null;
            dataGridView1.AutoGenerateColumns = true;
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.DataSource = dt;

            dataGridView1.Columns["Field20"].DisplayIndex = 4;
            dataGridView1.Columns["Field21"].DisplayIndex = 5;
        }
    }
}
