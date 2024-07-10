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
    public partial class AdminView : Form
    {
        private registerForm registerForm;

        public AdminView()
        {
            InitializeComponent();
        }

        private void btDodawanieUzytkownika_Click(object sender, EventArgs e)
        {
            Hide();
            registerForm = new registerForm(this);
            registerForm.ShowDialog();
        }

        private void btUsuwanieUzytkownika_Click(object sender, EventArgs e)
        {
            DeleteUser DeleteUser = new DeleteUser();
            DeleteUser.ShowDialog();
        }

        private void btResetHasla_Click(object sender, EventArgs e)
        {
            ChangePassword changePassword = new ChangePassword();
            changePassword.ShowDialog();
        }

        private void btDodawanieWydarzenia_Click(object sender, EventArgs e)
        {
            AddEvent addevent = new AddEvent();
            addevent.ShowDialog();
        }

        private void btUsuwanieWydarzenia_Click(object sender, EventArgs e)
        {
            DeleteEvent deleteEvent = new DeleteEvent();
            deleteEvent.ShowDialog();
        }

        private void brModyfikacjaWydarzenia_Click(object sender, EventArgs e)
        {
            Modyfikacja_Eventow modyfikacja_Eventow = new Modyfikacja_Eventow();
            modyfikacja_Eventow.ShowDialog();
        }

        private void btPotwierdzanieZapisow_Click(object sender, EventArgs e)
        {
            PotwierdzanieZapisu potwierdzanieZapisu = new PotwierdzanieZapisu();
            potwierdzanieZapisu.ShowDialog();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit();
        }
    }
}
