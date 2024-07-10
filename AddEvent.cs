using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class AddEvent : Form
    {
        private string connectionString = "server=localhost;database=eventapp;uid=root;pwd=;";

        public AddEvent()
        {
            InitializeComponent();
        }

        private void btDodajwydarzenie_Click(object sender, EventArgs e)
        {
            string nazwaWydarzenia = tbNazwaWydarzenia.Text.Trim();
            string agenda = tbOpisWydarzenia.Text.Trim();
            DateTime terminWydarzenia;

            // Sprawdzenie długości pól
            if (nazwaWydarzenia.Length > 80 || agenda.Length > 200)
            {
                MessageBox.Show("Nazwa wydarzenie nie może przekroczyć 80 znaków, a opis 200-stu.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Sprawdzenie poprawności daty
            if (!DateTime.TryParse(tbTerminWydarzenia.Text, out terminWydarzenia))
            {
                MessageBox.Show("Nieprawidłowy format daty. Wprowadź datę w formacie dd.MM.yyyy.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO wydarzenia (nazwa_wydarzenia, agenda, termin_wydarzenia) " +
                                   "VALUES (@nazwaWydarzenia, @agenda, @terminWydarzenia)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@nazwaWydarzenia", nazwaWydarzenia);
                    cmd.Parameters.AddWithValue("@agenda", agenda);
                    cmd.Parameters.AddWithValue("@terminWydarzenia", terminWydarzenia.Date);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Wydarzenie zostało dodane pomyślnie.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //Usuwanie zawartości textbox'ów po dodaniu wydarzenia
                        tbNazwaWydarzenia.Clear();
                        tbOpisWydarzenia.Clear();
                        tbTerminWydarzenia.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się dodać wydarzenia.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas dodawania wydarzenia: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
