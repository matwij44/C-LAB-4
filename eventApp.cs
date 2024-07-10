using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace WindowsFormsApp1
{
    public partial class startForm : Form
    {
        private string connectionString = "server=localhost;database=eventapp;uid=root;pwd=;";
        private int failedLoginAttempts = 0;
        private int userId = -1;

        public startForm()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            tbHaslo.UseSystemPasswordChar = !tbHaslo.UseSystemPasswordChar;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Pobranie loginu i hasła
            string login = tbLogin.Text;
            string password = tbHaslo.Text;


            //Połączenie z bazą
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM logowanie WHERE login = @login AND haslo = @password";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Sprawdź czy użytkownik został autoryzowany
                        if (reader.Read())
                        {
                            userId = Convert.ToInt32(reader["id_logowanie"]);

                            string role = reader["uprawnienia"].ToString();
                            if (role == "user")
                            {
                                MessageBox.Show("Zalogowano jako zwykły user");
                                userView userView = new userView(userId);
                                userView.ShowDialog();
                            }
                            else if (role == "admin")
                            {
                                MessageBox.Show("Zalogowano jako admin");
                                AdminView adminView = new AdminView();
                                adminView.ShowDialog();
                            }

                            Hide(); // Ukryj formularz startowy po zalogowaniu
                        }
                        else
                        {
                            failedLoginAttempts++;
                            MessageBox.Show("Invalid login credentials!");

                            if (failedLoginAttempts >= 3)
                            {
                                btLogin.Enabled = false;
                                MessageBox.Show("Too many failed attempts. Try again later.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            Hide();
            registerForm registerForm = new registerForm(this);
            registerForm.ShowDialog();
        }
    }
}
