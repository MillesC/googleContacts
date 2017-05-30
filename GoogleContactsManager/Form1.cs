using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;

namespace GoogleContactsManager
{
    public partial class Form1 : Form
    {
        public Form1(GoogleContactsDoc d)
        {
            InitializeComponent();
            groupListBindingSource.DataSource = new GroupList(d.GroupEntriesList);
            Shown += Form1_Shown;
        }

        void Form1_Shown(object sender, EventArgs e)
        {
            try
            {              
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
