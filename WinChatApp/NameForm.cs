using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinChatApp
{
    public partial class NameForm : Form
    {
        public NameForm()
        {
            InitializeComponent();
        }

        private void tbName_Click(object sender, EventArgs e)
        {
            tbName.Text = String.Empty;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.yourName = String.Empty;
            Properties.Settings.Default.yourName += tbName.Text;
            Properties.Settings.Default.Save();
            MessageBox.Show("Name has been saved.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
