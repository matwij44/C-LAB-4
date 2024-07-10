using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class userView : Form
    {
        private string connectionString = "server=localhost;database=eventapp;uid=root;pwd=;";
        private DataTable eventsTable;
        private int currentEventIndex = 0;

        private string selectedParticipationType = "";
        private string selectedFoodType = "";
        private int eventId = -1;
        private int userId;

        public userView(int userId)
        {
            InitializeComponent();
            LoadEvents();
            DisplayCurrentEvent();
            CheckAdminConsent(userId,eventId);

            this.userId = userId;
        }

        private void LoadEvents()
        {
            eventsTable = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT id_wydarzenie, nazwa_wydarzenia, agenda, termin_wydarzenia FROM wydarzenia";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(eventsTable);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void CheckAdminConsent(int userId, int eventId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Zapytanie SQL, aby sprawdzić potwierdzenie admina dla danego użytkownika i wydarzenia
                    string query = "SELECT Potwierdzenie FROM logowanie_wydarzenia " +
                                   "WHERE logowanie_id_logowanie = @userId AND wydarzenia_id_wydarzenie = @eventId";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@eventId", eventId);

                    // Wykonujemy zapytanie i odczytujemy wynik
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        bool adminConsent = Convert.ToBoolean(result);
                        adminConsentInfo.Text = adminConsent ? "Zgoda !" : "Brak zgody !";
                    }
                    else
                    {
                        adminConsentInfo.Text = "---";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd przy sprawdzaniu potwierdzenia admina: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void DisplayCurrentEvent()
        {
            if (eventsTable.Rows.Count > 0)
            {
                DataRow row = eventsTable.Rows[currentEventIndex];
                lblEventName.Text = row["nazwa_wydarzenia"].ToString();
                lblAgenda.Text = row["agenda"].ToString();

                DateTime eventDateTime = (DateTime)row["termin_wydarzenia"];
                string formattedDate = eventDateTime.ToString("yyyy-MM-dd");
                lblEventDate.Text = formattedDate;

                eventId = (int)row["id_wydarzenie"];

                CheckAdminConsent(userId, eventId);
            }
        }

        private void RadioButton_Click(object sender, EventArgs e, String option)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                if (option == "pt") //participation type
                    selectedParticipationType = rb.Text;
                else if (option == "food") selectedFoodType = rb.Text;
            }
        }

        private void btnPreviousEvent_Click(object sender, EventArgs e)
        {
            if (currentEventIndex > 0)
            {
                currentEventIndex--;
                DisplayCurrentEvent();
            }
        }

        private void btnNextEvent_Click(object sender, EventArgs e)
        {
            if (currentEventIndex < eventsTable.Rows.Count - 1)
            {
                currentEventIndex++;
                DisplayCurrentEvent();
            }
        }

        private void rbListener_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton_Click(sender, e, "pt");
        }

        private void rbSponsor_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton_Click(sender, e, "pt");
        }

        private void rbAuthor_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton_Click(sender, e, "pt");
        }

        private void rbOrganiser_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton_Click(sender, e, "pt");
        }

        private void rbNopreferences_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton_Click(sender, e, "food");
        }

        private void rbVegetarian_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton_Click(sender, e, "food");
        }

        private void rbNoGluten_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton_Click(sender, e, "food");
        }

        private void btnSignUpForTheEvent_Click(object sender, EventArgs e)
        {
            // Sprawdzenie, czy wszystkie wymagane opcje zostały wybrane
            if ( eventId == -1 ||
                string.IsNullOrEmpty(selectedParticipationType) ||
                string.IsNullOrEmpty(selectedFoodType))
            {
                MessageBox.Show("Proszę wybrać wszystkie opcje przed zapisaniem na wydarzenie.");
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO logowanie_wydarzenia (logowanie_id_logowanie, wydarzenia_id_wydarzenie, typ_uczestnictwa, wyzywienie) " +
                                   "VALUES (@userId, @eventId, @participationType, @foodType)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@userId", userId); // ID zalogowanego użytkownika
                    cmd.Parameters.AddWithValue("@eventId", eventId); // ID wydarzenia
                    cmd.Parameters.AddWithValue("@participationType", selectedParticipationType);
                    cmd.Parameters.AddWithValue("@foodType", selectedFoodType);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Zapisano na wydarzenie pomyślnie, czekaj na potwierdzenie !");
                    CheckAdminConsent(userId, eventId);
                }

                catch (MySqlException ex)
                {
                    // Obsługa błędu z bazy danych
                    MessageBox.Show($"Nie możesz znów zapisać się na to samo wydarzenie","Błąd przy zapisie do bazy danych: {ex.Message}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd: " + ex.Message);
                }
            }
        }
    }
}
