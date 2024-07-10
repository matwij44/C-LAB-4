using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class registerForm : Form
    {
        private string connectionString = "server=localhost;database=eventapp;uid=root;pwd=;";
        private startForm parentForm;
        private AdminView parentFormAdmin;

        // Konstruktor dla startForm
        public registerForm(startForm parentForm)
        {
            InitializeComponent();
            this.parentForm = parentForm;
        }

        // Konstruktor dla AdminView
        public registerForm(AdminView parentFormAdmin)
        {
            InitializeComponent();
            this.parentFormAdmin = parentFormAdmin;
        }


        private void registerForm_Load(object sender, EventArgs e)
        {
            // Dodanie obrazka
            PictureBox pb1 = new PictureBox();
            pb1.ImageLocation = "login.png";
            pb1.SizeMode = PictureBoxSizeMode.AutoSize;
            this.Controls.Add(pb1);
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        private bool IsLettersOnly(string str)
        {
            return Regex.IsMatch(str, @"^[a-zA-Z\s]+$");
        }

        private void btnRegister_Click_1(object sender, EventArgs e)
        {
            string firstName = tbImie.Text.Trim();
            string lastName = tbNazwisko.Text.Trim();
            string login = tbLogin.Text.Trim();
            string password = tbHaslo.Text;
            string confirmPassword = tbPotwierdzHaslo.Text;
            string email = tbEmail.Text.Trim();

            // Walidacja danych przy rejestracji
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            if (firstName.Length > 50)
            {
                MessageBox.Show("First name must be less than 50 characters.");
                return;
            }

            if (lastName.Length > 50)
            {
                MessageBox.Show("Last name must be less than 50 characters.");
                return;
            }

            if (login.Length > 50)
            {
                MessageBox.Show("Login must be less than 50 characters.");
                return;
            }

            if (password.Length > 50)
            {
                MessageBox.Show("Password must be less than 50 characters.");
                return;
            }

            if (email.Length > 50)
            {
                MessageBox.Show("Email must be less than 50 characters.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match!");
                return;
            }

            if (password.Length < 5)
            {
                MessageBox.Show("Password must be at least 5 characters long.");
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Invalid email format.");
                return;
            }

            if (!IsLettersOnly(firstName))
            {
                MessageBox.Show("First name can only contain letters and spaces.");
                return;
            }

            if (!IsLettersOnly(lastName))
            {
                MessageBox.Show("Last name can only contain letters and spaces.");
                return;
            }

            // Dodawanie danych do bazy danych
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    //sprawdzenie czy użytkownik o danym loginie już istnieje
                    string checkQuery = "SELECT COUNT(*) FROM logowanie WHERE login = @login";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
                    checkCmd.Parameters.AddWithValue("@login", login);
                    int userExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (userExists > 0)
                    {
                        MessageBox.Show("User with this login already exists.");
                        return;
                    }

                    //Dodawanie danych do bazy danych
                    string query = "INSERT INTO logowanie (imie, nazwisko, login, haslo, email, uprawnienia, data_rejestracji) " +
                                   "VALUES (@firstName, @lastName, @login, @password, @email, 'user', @registrationDate)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@registrationDate", DateTime.Now);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Registered successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btReturnToLoginForm_Click(object sender, EventArgs e)
        {
            Hide();

            if (parentForm != null)
            {
                parentForm.Show();
            }
            else if (parentFormAdmin != null)
            {
                parentFormAdmin.Show();
            }
        }

    }
}
