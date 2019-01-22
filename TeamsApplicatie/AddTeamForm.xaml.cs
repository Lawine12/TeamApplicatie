using System;
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
            textBoxPoints.Text = "0";
        }

        private void TextBox_TeamName(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TeamCoach(object sender, TextChangedEventArgs e)
        {

        }

        private void textBoxPoints_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBoxPoints.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                textBoxPoints.Text = textBoxPoints.Text.Remove(textBoxPoints.Text.Length - 1);
            }
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

        private void AddaTeam()
        {
            if (textBoxTeamName.Text != string.Empty && textBoxTeamCoach.Text != string.Empty)
            {
                using (var connection = DatabaseHelper.OpenDefaultConnection())
                using (var sqlCommand = connection.CreateCommand())
                {
                    var teamNameParameter = sqlCommand.Parameters.AddWithValue("@teamName", textBoxTeamName.Text);
                    var teamCoachParameter = sqlCommand.Parameters.AddWithValue("@teamCoach", textBoxTeamCoach.Text);
                    var pointsParameter = sqlCommand.Parameters.AddWithValue("@TeamGoals", textBoxPoints.Text);

                    sqlCommand.CommandText =
                        $@"INSERT INTO [dbo].[TeamData]
                    ([TeamName], [TeamCoach], [TeamGoals])
                    VALUES ({teamNameParameter.ParameterName}, {teamCoachParameter}, {pointsParameter})";
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
