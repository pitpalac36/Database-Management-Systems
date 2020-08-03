using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace sgdb_lab1
{


    public partial class Form1 : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
        SqlConnection connection;
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataSet dataset = new DataSet();
        public static int nrTextboxes = 0;

        public Form1()
        {
            connection = new SqlConnection(connectionString);
            InitializeComponent();
            dataGridView3.SelectionChanged += new System.EventHandler(dataGridView3_SelectionChanged);
        }


        private void Form1_Load_1(object sender, EventArgs e)
        {
            //int noTextboxes = int.Parse(ConfigurationManager.AppSettings["NumberOfTextboxes"]);
            List<string> labels = new List<string>(ConfigurationManager.AppSettings["ParentChildTextboxesLabels"].Split(','));
            var panel = new FlowLayoutPanel();
            panel.FlowDirection = FlowDirection.TopDown;
            panel.Dock = DockStyle.Fill;
            foreach (string label in labels)
            {
                nrTextboxes++;
                var combo = new Field("  " + label + "  ");
                panel.Controls.Add(combo);  
            }
            panel.Location = new Point(10, 50);
            panel.Visible = true;
            panel.Name = "panelParent";
            this.Controls.Add(panel);
           
        }


        private void dataGridView3_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView3.SelectedRows.Count > 0)   // completarea campului de id din parinte in textbox
            {
                TextBox textbox = (TextBox)this.Controls["panelParent"].Controls[Field.counter-1].Controls["textbox" + (Field.counter-1).ToString()];
                textbox.Text = dataGridView3.SelectedRows[0].Cells[0].Value.ToString();
            }
        }



        [Obsolete]
        private void button3_Click_1(object sender, EventArgs e)
        {
            string select = ConfigurationSettings.AppSettings["select"]; 
            adapter.SelectCommand = new SqlCommand(select, connection);
            dataset.Clear();
            adapter.Fill(dataset);
            dataGridView3.DataSource = dataset.Tables[0];
        }

        [Obsolete]
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                int contor = 0;
                string ChildTableName = ConfigurationManager.AppSettings["ChildTableName"]; 
                string ChildColumnNames = ConfigurationManager.AppSettings["ChildColumnNames"]; 
                string ColumnNamesInsertParameters = ConfigurationManager.AppSettings["ColumnNamesInsertParameters"]; 
                List<string> ColumnNamesList = new List<string>(ConfigurationManager.AppSettings["ColumnNamesInsertParameters"].Split(','));
                SqlCommand cmd = new SqlCommand("INSERT INTO " + ChildTableName + " VALUES (" + ColumnNamesInsertParameters + ")", connection);
                foreach (string column in ColumnNamesList)
                {
                        TextBox textbox = (TextBox)this.Controls["panelParent"].Controls[contor].Controls["textbox" + contor];
                        cmd.Parameters.Add(column, SqlDbType.VarChar);
                        cmd.Parameters[column].Value = textbox.Text;
                        contor++;
                   

                }
                connection.Open();
                adapter.InsertCommand = cmd;
                adapter.InsertCommand.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("s-a adaugat cu succes!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                connection.Close();
            }
        }

        [Obsolete]
        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView3.SelectedRows.Count == 1)
            {
                Form2 formRom = new Form2(dataGridView3);
                formRom.Text = ConfigurationManager.AppSettings["titluChild"];
                formRom.Show();
            }
            else
                if (dataGridView3.SelectedRows.Count > 1)
                    MessageBox.Show(ConfigurationManager.AppSettings["mesajPreaMulteSelectate"]);
                else
                    MessageBox.Show(ConfigurationManager.AppSettings["mesajNiciunSelectat"]);
        }
    }

    class Field : FlowLayoutPanel
    {
        public Label label;
        public TextBox text_box;
        public static int counter = 0;

        public Field(string label_text)
            : base()
        {
            
            AutoSize = true;
            label = new Label();
            label.Text = label_text;
            label.AutoSize = true;
            label.Anchor = AnchorStyles.Left;
            label.TextAlign = ContentAlignment.MiddleLeft;

            Controls.Add(label);

            text_box = new TextBox();
            text_box.Name = "textbox" + counter;
            Controls.Add(text_box);
            counter++;
            Console.WriteLine(text_box.Name);
        }

        public TextBox GetTextBox{ get; set; }
    }
}
