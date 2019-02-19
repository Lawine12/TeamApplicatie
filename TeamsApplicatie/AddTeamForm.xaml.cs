using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    public partial class AddTeam : Window
    {
        public AddTeam()
        {
            InitializeComponent();
            textBoxTeamName.Focus();
        }

        private void TextBox_TeamName(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TeamCoach(object sender, TextChangedEventArgs e)
        {

        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddaTeam();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public async void AddaTeam()
        {
            if (textBoxTeamName.Text != string.Empty && textBoxTeamCoach.Text != string.Empty)
            {
                using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
                using (var sqlCommand = connection.CreateCommand())
                {
                    var teamNameParameter = sqlCommand.Parameters.AddWithValue("@teamName", textBoxTeamName.Text);
                    var teamCoachParameter = sqlCommand.Parameters.AddWithValue("@teamCoach", textBoxTeamCoach.Text);

                    sqlCommand.CommandText =
                        $@"INSERT INTO [dbo].[TeamData]
                    ([TeamName], [TeamCoach])
                    VALUES ({teamNameParameter.ParameterName}, {teamCoachParameter})";
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
