using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class DeleteEvent : Form
    {
        private string connectionString = "server=localhost;database=eventapp;uid=root;pwd=;";
        private DataTable eventData; // DataTable do przechowywania danych wydarzeń
        private int currentEventIndex;

        public DeleteEvent()
        {
            InitializeComponent();
            eventData = new DataTable();
            LoadAllEvents();
            DisplayEventData(currentEventIndex);
        }

        private void LoadAllEvents()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT nazwa_wydarzenia, termin_wydarzenia FROM wydarzenia";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        eventData.Clear(); // Wyczyszczenie danych przed wypełnieniem
                        adapter.Fill(eventData); // Wypełnienie DataTable danymi z bazy danych
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas pobierania danych wydarzeń: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DisplayEventData(int index)
        {
            if (index >= 0 && index < eventData.Rows.Count)
            {
                DataRow row = eventData.Rows[index];
                lbNazwaZBazy.Text = row["nazwa_wydarzenia"].ToString();
                lbTerminZBazy.Text = row["termin_wydarzenia"].ToString();
            }
        }

        private void btnPreviousEvent_Click(object sender, EventArgs e)
        {
            currentEventIndex--;
            if (currentEventIndex < 0)
            {
                currentEventIndex = eventData.Rows.Count - 1; // Przewinięcie do ostatniego wydarzenia po osiągnięciu początku
            }
            DisplayEventData(currentEventIndex);
        }

        private void btnNextEvent_Click(object sender, EventArgs e)
        {
            currentEventIndex++;
            if (currentEventIndex >= eventData.Rows.Count)
            {
                currentEventIndex = 0; // Przewinięcie do pierwszego wydarzenia po osiągnięciu końca
            }
            DisplayEventData(currentEventIndex);
        }

        private void btDeleteEvent_Click(object sender, EventArgs e)
        {
            string nazwaWydarzenia = lbNazwaZBazy.Text;
            DateTime terminWydarzenia;

            if (!DateTime.TryParse(lbTerminZBazy.Text, out terminWydarzenia))
            {
                MessageBox.Show("Nieprawidłowy format daty.");
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM wydarzenia WHERE nazwa_wydarzenia = @nazwaWydarzenia AND termin_wydarzenia = @terminWydarzenia";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@nazwaWydarzenia", nazwaWydarzenia);
                    cmd.Parameters.AddWithValue("@terminWydarzenia", terminWydarzenia);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Wydarzenie zostało pomyślnie usunięte.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAllEvents();
                        currentEventIndex = 0; // Przewinięcie do pierwszego wydarzenia po usunięciu
                        DisplayEventData(currentEventIndex);
                    }
                    else
                    {
                        MessageBox.Show("Nie znaleziono wydarzenia o podanych danych.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas usuwania wydarzenia: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
