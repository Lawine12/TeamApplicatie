using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    public partial class EditPlayerForm : Window
    {
        private readonly string _id;
        private DataTable _playerInformation;

        public EditPlayerForm()
        {
            InitializeComponent();
        }

        public EditPlayerForm(string id) : this()
        {
            _id = id;
            try
            { 
            LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void TextBox_Age(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBoxAge.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                textBoxAge.Text = textBoxAge.Text.Remove(textBoxAge.Text.Length - 1);
            }
        }

        private void LoadData()
        {
            var playerData = GetDataTable();
            if (playerData.Rows.Count == 1)
            {
                var row = _playerInformation.Rows[0];
                textBoxFirstName.Text = (string)row["First Name"];
                textBoxLastName.Text = (string)row["Last Name"];
                textBoxAdress.Text = (string)row["Adress"];
                textBoxCity.Text = (string)row["City"];
                textBoxAge.Text = row["Age"].ToString();
                textBoxPosition.Text = (string)row["Position"];
            }
        }

        private DataTable GetDataTable()
        {
            _playerInformation = new DataTable();
            var queryString = "SELECT * FROM Players WHERE Id=@id";
            using (var connection = DatabaseHelper.OpenDefaultConnection())
            using (var cmd = new SqlCommand(queryString, connection))
            {
                cmd.Parameters.AddWithValue("@id", _id);
                var sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(_playerInformation);
            }
            return _playerInformation;
        }
    }
}
