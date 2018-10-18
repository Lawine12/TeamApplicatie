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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            EditPlayer(_id);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditPlayer(string _Id)
        {
            if (textBoxFirstName.Text != string.Empty && textBoxLastName.Text != string.Empty && textBoxAdress.Text != string.Empty && textBoxCity.Text != string.Empty && textBoxAge.Text != string.Empty && textBoxPosition.Text != string.Empty)
            {
                using (var connection = DatabaseHelper.OpenDefaultConnection())
                using (var sqlCommand = connection.CreateCommand())
                {
                    var teamId = sqlCommand.Parameters.AddWithValue("@TeamId", _id);
                    var firstNameParameter = sqlCommand.Parameters.AddWithValue("@FirstName", textBoxFirstName.Text);
                    var lastNameParameter = sqlCommand.Parameters.AddWithValue("@LastName", textBoxLastName.Text);
                    var adressParameter = sqlCommand.Parameters.AddWithValue("@Adress", textBoxAdress.Text);
                    var cityParameter = sqlCommand.Parameters.AddWithValue("@City", textBoxCity.Text);
                    var ageParameter = sqlCommand.Parameters.AddWithValue("@Age", textBoxAge.Text);
                    var positionParameter = sqlCommand.Parameters.AddWithValue("@Position", textBoxPosition.Text);

                    sqlCommand.CommandText =
                        $@"UPDATE [dbo].[Players]
                    SET
                    [First Name] = {firstNameParameter}, 
                    [Last Name] = {lastNameParameter},
                    [Adress] = {adressParameter},
                    [City] =  {cityParameter},
                    [Age] = {ageParameter},
                    [Position] = {positionParameter}
                    WHERE Id = {teamId}";
                    sqlCommand.ExecuteNonQuery();
                }

                MessageBox.Show("Success!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
                MessageBox.Show("Veld mag niet leeg zijn!", "Velden moeten gevuld zijn", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
