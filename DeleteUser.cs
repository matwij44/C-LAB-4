﻿using MySql.Data.MySqlClient;
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
    public partial class DeleteUser : Form
    {
        private string connectionString = "server=localhost;database=eventapp;uid=root;pwd=;";
        private DataTable userData;
        private int currentUserIndex;
        public DeleteUser()
        {
            InitializeComponent();
            userData = new DataTable();
            LoadAllUsers();
            DisplayUserData(currentUserIndex);
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
                lblImieZBazy.Text = row["imie"].ToString();
                lblNazwiskoZBazy.Text = row["nazwisko"].ToString();
                lblLoginZBazy.Text = row["login"].ToString();
            }
        }

        private void BtnNextEvent_Click(object sender, EventArgs e)
        {
            currentUserIndex++;
            if (currentUserIndex >= userData.Rows.Count)
            {
                currentUserIndex = 0;
            }
            DisplayUserData(currentUserIndex);
        }

        private void BtnPreviousEvent_Click(object sender, EventArgs e)
        {
            currentUserIndex--;
            if (currentUserIndex < 0)
            {
                currentUserIndex = userData.Rows.Count - 1; 
            }
            DisplayUserData(currentUserIndex);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Pobranie imienia, nazwiska i loginu z etykiet
            string imie = lblImieZBazy.Text;
            string nazwisko = lblNazwiskoZBazy.Text;
            string login = lblLoginZBazy.Text;

            // Sprawdzenie czy dane nie są puste
            if (string.IsNullOrEmpty(imie) || string.IsNullOrEmpty(nazwisko) || string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Nie można usunąć użytkownika. Dane użytkownika są niekompletne.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Usuwanie użytkownika na podstawie imienia, nazwiska i loginu
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM logowanie WHERE imie = @imie AND nazwisko = @nazwisko AND login = @login";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@imie", imie);
                    cmd.Parameters.AddWithValue("@nazwisko", nazwisko);
                    cmd.Parameters.AddWithValue("@login", login);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Użytkownik został pomyślnie usunięty.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAllUsers();
                        currentUserIndex = 0; // Przewinięcie do pierwszego użytkownika po usunięciu
                        DisplayUserData(currentUserIndex);
                    }
                    else
                    {
                        MessageBox.Show("Nie znaleziono użytkownika o podanych danych.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas usuwania użytkownika: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
