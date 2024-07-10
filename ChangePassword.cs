using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ChangePassword : Form
    {
        private string connectionString = "server=localhost;database=eventapp;uid=root;pwd=;";
        private DataTable userData;
        private int currentUserIndex;

        public ChangePassword()
        {
            InitializeComponent();
            userData = new DataTable();
            LoadAllUsers();
            DisplayUserData(currentUserIndex);
            FormClosing += ChangePassword_FormClosing;
        }

        private void ChangePassword_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; // Anulowanie zamknięcia formularza
            this.Hide();     // Ukrycie formularza
        }

        private void LoadAllUsers()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT imie, nazwisko, login FROM logowanie";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        userData.Clear();
                        adapter.Fill(userData);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas pobierania danych użytkowników: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DisplayUserData(int index)
        {
            if (index >= 0 && index < userData.Rows.Count)
            {
                DataRow row = userData.Rows[index];
                lblImie.Text = row["imie"].ToString();
                lblNazwisko.Text = row["nazwisko"].ToString();
                lblLogin.Text = row["login"].ToString();
            }
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            // Pobranie imienia, nazwiska i loginu z etykiet
            string imie = lblImie.Text;
            string nazwisko = lblNazwisko.Text;
            string login = lblLogin.Text;
            string newPassword = tbNewPassword.Text;

            // Sprawdzenie czy dane nie są puste
            if (string.IsNullOrEmpty(imie) || string.IsNullOrEmpty(nazwisko) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Nie można zmienić hasła użytkownika. Dane użytkownika lub nowe hasło są niekompletne.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Sprawdzenie długości nowego hasła
            if (newPassword.Length < 5)
            {
                MessageBox.Show("Hasło musi mieć conajmniej 5 znaków.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Zmiana hasła użytkownika na podstawie imienia, nazwiska i loginu
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE logowanie SET haslo = @newPassword WHERE imie = @imie AND nazwisko = @nazwisko AND login = @login";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@newPassword", newPassword);
                    cmd.Parameters.AddWithValue("@imie", imie);
                    cmd.Parameters.AddWithValue("@nazwisko", nazwisko);
                    cmd.Parameters.AddWithValue("@login", login);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Hasło użytkownika zostało pomyślnie zmienione.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Nie znaleziono użytkownika o podanych danych.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas zmiany hasła użytkownika: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btChangePassword_Click(object sender, EventArgs e)
        {
            // Pobranie imienia, nazwiska i loginu z etykiet
            string imie = lblImie.Text;
            string nazwisko = lblNazwisko.Text;
            string login = lblLogin.Text;
            string newPassword = tbNewPassword.Text;

            // Sprawdzenie czy dane są niepuste
            if (string.IsNullOrEmpty(imie) || string.IsNullOrEmpty(nazwisko) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Nie można zmienić hasła użytkownika. Dane użytkownika lub nowe hasło są niekompletne.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Sprawdzenie długości nowego hasła
            if (newPassword.Length < 5)
            {
                MessageBox.Show("Hasło musi mieć conajmniej 5 znaków.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Zmiana hasła użytkownika na podstawie imienia, nazwiska i loginu
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE logowanie SET haslo = @newPassword WHERE imie = @imie AND nazwisko = @nazwisko AND login = @login";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@newPassword", newPassword);
                    cmd.Parameters.AddWithValue("@imie", imie);
                    cmd.Parameters.AddWithValue("@nazwisko", nazwisko);
                    cmd.Parameters.AddWithValue("@login", login);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Hasło użytkownika zostało pomyślnie zmienione.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Nie znaleziono użytkownika o podanych danych.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas zmiany hasła użytkownika: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnPreviousUser_Click(object sender, EventArgs e)
        {
            currentUserIndex++;
            if (currentUserIndex >= userData.Rows.Count)
            {
                currentUserIndex = 0; // Przewinięcie do pierwszego użytkownika po osiągnięciu końca listy
            }
            DisplayUserData(currentUserIndex);
        }

        private void btnNextUser_Click(object sender, EventArgs e)
        {
            currentUserIndex--;
            if (currentUserIndex < 0)
            {
                currentUserIndex = userData.Rows.Count - 1; // Przewinięcie do ostatniego użytkownika po osiągnięciu początku listy
            }
            DisplayUserData(currentUserIndex);
        }
    }
}
