using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Modyfikacja_Eventow : Form
    {
        private string connectionString = "server=localhost;database=eventapp;uid=root;pwd=;";
        private DataTable eventData; // DataTable do przechowywania danych wydarzeń
        private int currentEventIndex;

        public Modyfikacja_Eventow()
        {
            InitializeComponent();
            eventData = new DataTable(); // Inicjalizacja DataTable
            LoadAllEvents(); // Ładowanie danych wydarzeń po inicjalizacji formularza
            DisplayEventData(currentEventIndex); // Wyświetlenie pierwszego wydarzenia
        }

        private void LoadAllEvents()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT id_wydarzenie, nazwa_wydarzenia, agenda, termin_wydarzenia FROM wydarzenia";
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
                tbNazwa.Text = row["nazwa_wydarzenia"].ToString();
                tbTermin.Text = row["termin_wydarzenia"].ToString();
                tbAgenda.Text = row["agenda"].ToString();
            }
        }

        private void btnPreviousEvent_Click_1(object sender, EventArgs e)
        {
            currentEventIndex--;
            if (currentEventIndex < 0)
            {
                currentEventIndex = eventData.Rows.Count - 1; // Przewinięcie do ostatniego wydarzenia po osiągnięciu początku
            }
            DisplayEventData(currentEventIndex);
        }

        private void btnNextEvent_Click_1(object sender, EventArgs e)
        {
            currentEventIndex++;
            if (currentEventIndex >= eventData.Rows.Count)
            {
                currentEventIndex = 0; // Przewinięcie do pierwszego wydarzenia po osiągnięciu końca
            }
            DisplayEventData(currentEventIndex);
        }

        private void btModifyEvent_Click(object sender, EventArgs e)
        {
            {
                string nazwaWydarzenia = tbNazwa.Text;
                string agenda = tbAgenda.Text;
                DateTime terminWydarzenia;

                if (!DateTime.TryParse(tbTermin.Text, out terminWydarzenia))
                {
                    MessageBox.Show("Nieprawidłowy format daty.");
                    return;
                }

                if (currentEventIndex < 0 || currentEventIndex >= eventData.Rows.Count)
                {
                    MessageBox.Show("Nie wybrano poprawnego wydarzenia.");
                    return;
                }

                DataRow row = eventData.Rows[currentEventIndex];
                int idWydarzenie = Convert.ToInt32(row["id_wydarzenie"]);

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "UPDATE wydarzenia SET nazwa_wydarzenia = @nazwaWydarzenia, agenda = @agenda, termin_wydarzenia = @terminWydarzenia WHERE id_wydarzenie = @idWydarzenie";
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@nazwaWydarzenia", nazwaWydarzenia);
                        cmd.Parameters.AddWithValue("@agenda", agenda);
                        cmd.Parameters.AddWithValue("@terminWydarzenia", terminWydarzenia);
                        cmd.Parameters.AddWithValue("@idWydarzenie", idWydarzenie);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Wydarzenie zostało pomyślnie zaktualizowane.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadAllEvents(); // Odświeżenie danych po aktualizacji
                            DisplayEventData(currentEventIndex); // Wyświetlenie zaktualizowanego wydarzenia
                        }
                        else
                        {
                            MessageBox.Show("Nie udało się zaktualizować wydarzenia.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Błąd podczas aktualizacji wydarzenia: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
