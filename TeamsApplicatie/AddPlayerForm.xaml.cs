using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace TeamsApplicatie
{
    public partial class AddPlayerForm : Window
    {
        private readonly int _id;

        public AddPlayerForm(int id)
        {
            _id = id;
            InitializeComponent();
        }

        private void TextBox_Age(object sender, TextChangedEventArgs e)
        {
            if (!Regex.IsMatch(textBoxAge.Text, "[^0-9]")) return;
            MessageBox.Show("Please enter only numbers.");
            textBoxAge.Text = textBoxAge.Text.Remove(textBoxAge.Text.Length - 1);
        }

        private async void Add_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            { 
            await AddaPlayer(_id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async Task AddaPlayer(int _Id)
        {
            if (textBoxFirstName.Text != string.Empty && textBoxLastName.Text != string.Empty && textBoxAdress.Text != string.Empty && textBoxCity.Text != string.Empty && textBoxAge.Text != string.Empty && textBoxPosition.Text != string.Empty)
            {
                using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
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
                        $@"INSERT INTO [dbo].[Players]
                    ([TeamId], [First Name], [Last Name], [Adress], [City], [Age], [Position])
                    VALUES ({teamId}, {firstNameParameter}, {lastNameParameter}, {adressParameter}, {cityParameter}, {ageParameter}, {positionParameter})";
                    await sqlCommand.ExecuteNonQueryAsync();
                }

                MessageBox.Show("Success!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
                MessageBox.Show("Veld mag niet leeg zijn!", "Velden moeten gevuld zijn", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
