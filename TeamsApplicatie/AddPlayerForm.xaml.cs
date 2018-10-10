using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for AddPlayerForm.xaml
    /// </summary>
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
            if (System.Text.RegularExpressions.Regex.IsMatch(textBoxAge.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                textBoxAge.Text = textBoxAge.Text.Remove(textBoxAge.Text.Length - 1);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddaPlayer(_id);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddaPlayer(int _Id)
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
                        $@"INSERT INTO [dbo].[Players]
                    ([TeamId], [First Name], [Last Name], [Adress], [City], [Age], [Position])
                    VALUES ({teamId}, {firstNameParameter}, {lastNameParameter}, {adressParameter}, {cityParameter}, {ageParameter}, {positionParameter})";
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
