using System;
using System.Configuration;
using System.Windows.Forms;

namespace sgdb_lab1
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
            Form1 fParent = new Form1();
            fParent.Text = ConfigurationManager.AppSettings["titluParent"];
            Application.Run(fParent);
        }
    }
}
