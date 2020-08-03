using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace sgdb_lab1
{
    
    public partial class Form2 : Form
    {   
        public DataGridView viewParent;
        string connectionString = ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
        SqlConnection connection;
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataSet dataset = new DataSet();

        [Obsolete]
        public Form2(DataGridView parent)
        {
            viewParent = parent;
            InitializeComponent();
            connection = new SqlConnection(connectionString);
            string select = ConfigurationSettings.AppSettings["selectChildren"];
            adapter.SelectCommand = new SqlCommand(select, connection);
            adapter.SelectCommand.Parameters.Add("@parentId", SqlDbType.Int).Value = viewParent.SelectedRows[0].Cells[0].Value;
            dataset.Clear();
            adapter.Fill(dataset);
            dataGridView1.DataSource = dataset.Tables[0];

            // folosesc proprietatea SelectionChanged pt a reflecta in textboxes selectarea unei inregistrari
            dataGridView1.SelectionChanged += new System.EventHandler(dataGridView1_SelectionChanged);
        }


        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {

            }
        }

        private void dataGridView1_RowContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        [Obsolete]
        private void button1_Click(object sender, System.EventArgs e)
        {
            try
            {
                int contor = Form1.nrTextboxes;
                string ChildTableName = ConfigurationManager.AppSettings["ChildTableName"];
                string ChildColumnNames = ConfigurationManager.AppSettings["ChildColumnNames"];
                string ColumnNamesUpdateParameters = ConfigurationManager.AppSettings["ColumnNamesUpdateParameters"];
                List<string> ColumnNamesList = new List<string>(ConfigurationManager.AppSettings["ColumnNamesUpdateParameters"].Split(','));
                SqlCommand cmd = new SqlCommand(ConfigurationManager.AppSettings["updateChild"], connection);
                foreach (string column in ColumnNamesList)
                {
                    TextBox textbox = (TextBox)this.Controls["panelChild"].Controls[contor].Controls["textbox" + contor];
                    cmd.Parameters.Add(column, SqlDbType.VarChar);
                    cmd.Parameters[column].Value = textbox.Text;
                    contor++;
                }
                connection.Open();
                adapter.UpdateCommand = cmd;
                adapter.UpdateCommand.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("s-a actualizat cu succes!");
                dataset.Clear();
                adapter.Fill(dataset);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        [Obsolete]
        private void button2_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dr;
                dr = MessageBox.Show("Sigur stergeti inregistrarea?", "Confirmare", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    connection.Open();
                    string delete = ConfigurationSettings.AppSettings["deleteChild"];
                    adapter.DeleteCommand = new SqlCommand(delete, connection);
                    adapter.DeleteCommand.Parameters.Add(ConfigurationSettings.AppSettings["idChild"], SqlDbType.Int).Value = dataGridView1.SelectedRows[0].Cells[0].Value;
                    adapter.DeleteCommand.ExecuteNonQuery();
                    connection.Close();
                    MessageBox.Show("s-a sters cu succes!");
                    dataset.Clear();
                    adapter.Fill(dataset);
                    
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            List<string> labels = new List<string>(ConfigurationManager.AppSettings["ChildTextboxesLabels"].Split(','));
            var panel = new FlowLayoutPanel();
            panel.FlowDirection = FlowDirection.TopDown;
            panel.Dock = DockStyle.Fill;
            foreach (string label in labels)
            {
                var combo = new Field("  " + label + "  ");
                panel.Controls.Add(combo);
            }
            panel.Location = new Point(10, 50);
            panel.Visible = true;
            panel.Name = "panelChild";
            this.Controls.Add(panel);
        }
    }
}
