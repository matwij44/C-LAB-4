using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class PotwierdzanieZapisu : Form
    {
        private string connectionString = "server=localhost;database=eventapp;uid=root;pwd=;";
        private DataTable eventData;
        private int currentIndex;

        public PotwierdzanieZapisu()
        {
            InitializeComponent();
            eventData = new DataTable(); // Inicjalizacja obiektu DataTable
            LoadAllData(); // Ładowanie danych po uruchomieniu formularza
            currentIndex = 0; // Inicjalizacja indeksu
            DisplayCurrentData(); // Wyświetlenie danych dla pierwszego wiersza po uruchomieniu formularza
        }

        private void LoadAllData()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            l.imie, 
                            l.nazwisko, 
                            l.login, 
                            w.nazwa_wydarzenia,
                            lw.wydarzenia_id_wydarzenie 
                        FROM 
                            logowanie_wydarzenia lw
                        JOIN 
                            logowanie l ON lw.logowanie_id_logowanie = l.id_logowanie
                        JOIN 
                            wydarzenia w ON lw.wydarzenia_id_wydarzenie = w.id_wydarzenie";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        eventData.Clear();
                        adapter.Fill(eventData);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas pobierania danych: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DisplayCurrentData()
        {
            if (currentIndex >= 0 && currentIndex < eventData.Rows.Count)
            {
                DataRow row = eventData.Rows[currentIndex];
                lbImieZBazy.Text = row["imie"].ToString();
                lbNazwiskoZBazy.Text = row["nazwisko"].ToString();
                lbLoginZBazy.Text = row["login"].ToString();
                lbWydarzenieZBazy.Text = row["nazwa_wydarzenia"].ToString();
            }
        }

        private void btnZgoda_Click(object sender, EventArgs e)
        {
            UpdateConfirmationStatus(true);
        }

        private void btnOdmowa_Click(object sender, EventArgs e)
        {
            UpdateConfirmationStatus(false);
        }

        private void btnPrevRow_Click(object sender, EventArgs e)
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = eventData.Rows.Count - 1; // Przewinięcie do ostatniego wiersza po osiągnięciu początku
            }
            DisplayCurrentData();
        }

        private void btnNextRow_Click(object sender, EventArgs e)
        {
            currentIndex++;
            if (currentIndex >= eventData.Rows.Count)
            {
                currentIndex = 0; // Przewinięcie do pierwszego wiersza po osiągnięciu końca
            }
            DisplayCurrentData();
        }

        private void UpdateConfirmationStatus(bool confirmed)
        {
            if (currentIndex < 0 || currentIndex >= eventData.Rows.Count)
            {
                MessageBox.Show("Nie wybrano poprawnego wiersza.");
                return;
            }

            DataRow row = eventData.Rows[currentIndex];
            int eventId = Convert.ToInt32(row["wydarzenia_id_wydarzenie"]);
            string login = row["login"].ToString();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE logowanie_wydarzenia SET Potwierdzenie = @Potwierdzenie WHERE wydarzenia_id_wydarzenie = @eventId AND logowanie_id_logowanie IN (SELECT id_logowanie FROM logowanie WHERE login = @login)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Potwierdzenie", confirmed ? 1 : 0);
                    cmd.Parameters.AddWithValue("@eventId", eventId);
                    cmd.Parameters.AddWithValue("@login", login);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Status potwierdzenia został zaktualizowany.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAllData(); // Odświeżenie danych
                        DisplayCurrentData(); // Wyświetlenie danych dla bieżącego wiersza
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się zaktualizować statusu potwierdzenia.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas aktualizacji statusu potwierdzenia: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
