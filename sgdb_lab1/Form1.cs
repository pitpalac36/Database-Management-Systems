using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace sgdb_lab1
{
    public partial class Form1 : Form
    {
        SqlConnection connection = new SqlConnection("Data Source=DESKTOP-34MD1UU; Initial Catalog=LoveRelationship;Integrated Security=True;");
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataSet dataset = new DataSet();

        public Form1()
        {
            InitializeComponent();
        }


        private void button3_Click_1(object sender, EventArgs e)
        {
            adapter.SelectCommand = new SqlCommand("SELECT * FROM Pirati", connection);
            dataset.Clear();
            adapter.Fill(dataset);
            dataGridView3.DataSource = dataset.Tables[0];
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                adapter.InsertCommand = new SqlCommand("INSERT INTO Rom(denumire,calitate,pid) VALUES (@d,@c,@p)", connection);
                adapter.InsertCommand.Parameters.Add("@d", SqlDbType.VarChar).Value = textBox5.Text;
                adapter.InsertCommand.Parameters.Add("@c", SqlDbType.VarChar).Value = textBox6.Text;
                adapter.InsertCommand.Parameters.Add("@p", SqlDbType.Int).Value = dataGridView3.SelectedRows[0].Cells[0].Value;

                connection.Open();
                adapter.InsertCommand.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("s-a adaugat cu succes!");
                textBox5.Text = "";
                textBox6.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connection.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView3.SelectedRows.Count == 1)
            {
                Form2 formRom = new Form2(dataGridView3);
                formRom.Text = "ROM";
                formRom.Show();
            }
            else
                if (dataGridView3.SelectedRows.Count > 1)
                    MessageBox.Show("Selectati un singur pirat!");
                else
                    MessageBox.Show("Selectati mai intai un pirat!");
        }
    }
}
