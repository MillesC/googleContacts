using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleContactsManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string x = GoogleContactsManagerClientId.ProjectId;
            Application.Run(new DocumentListForm());
            //Application.Run(new ConsoleForm());
        }
    }
}
