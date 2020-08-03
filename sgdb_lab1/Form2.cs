using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace sgdb_lab1
{
    
    public partial class Form2 : Form
    {   
        public DataGridView viewPirati;

        SqlConnection connection = new SqlConnection("Data Source=DESKTOP-34MD1UU; Initial Catalog=LoveRelationship;Integrated Security=True;");
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataSet dataset = new DataSet();
      

        public Form2(DataGridView pirati)
        {
            viewPirati = pirati;
            InitializeComponent();
            adapter.SelectCommand = new SqlCommand("SELECT * FROM Rom WHERE pid = @pid", connection);
            adapter.SelectCommand.Parameters.Add("@pid", SqlDbType.Int).Value = viewPirati.SelectedRows[0].Cells[0].Value;
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
                string rid = dataGridView1.SelectedRows[0].Cells[0].Value + string.Empty;
                string denumire = dataGridView1.SelectedRows[0].Cells[1].Value + string.Empty;
                string calitate = dataGridView1.SelectedRows[0].Cells[2].Value + string.Empty;
                textBox1.Text = rid;
                textBox2.Text = denumire;
                textBox3.Text = calitate;
            }
        }

        private void dataGridView1_RowContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        


        private void button1_Click(object sender, System.EventArgs e)
        {
            try
            {
                adapter.UpdateCommand = new SqlCommand("UPDATE Rom SET denumire=@d, calitate=@c WHERE rid=@r", connection);
                adapter.UpdateCommand.Parameters.AddWithValue("@d", textBox2.Text);
                adapter.UpdateCommand.Parameters.AddWithValue("@c", textBox3.Text);
                adapter.UpdateCommand.Parameters.AddWithValue("@r", dataGridView1.SelectedRows[0].Cells[0].Value);
                connection.Open();
                adapter.UpdateCommand.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("s-a actualizat cu succes!");
                dataset.Clear();
                adapter.Fill(dataset);
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dr;
                dr = MessageBox.Show("Sigur stergeti inregistrarea?", "Confirmare", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    adapter.DeleteCommand = new SqlCommand("DELETE FROM Rom WHERE rid=@r", connection);
                    adapter.DeleteCommand.Parameters.Add("@r", SqlDbType.Int).Value = dataGridView1.SelectedRows[0].Cells[0].Value;
                    connection.Open();
                    adapter.DeleteCommand.ExecuteScalar();
                    connection.Close();
                    MessageBox.Show("s-a sters cu succes!");
                    dataset.Clear();
                    adapter.Fill(dataset);
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

  
    }
}
