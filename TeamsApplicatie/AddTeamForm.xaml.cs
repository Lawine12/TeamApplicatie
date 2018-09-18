using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for AddTeam.xaml
    /// </summary>
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

        private void TextBox_TeamDriver(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TeamCoach(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_Points(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBoxPoints.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                textBoxPoints.Text = textBoxPoints.Text.Remove(textBoxPoints.Text.Length - 1);
            }
        }

        private void AddTeamAndDriver()
        {
            if (textBoxTeamName.Text != string.Empty && textBoxTeamDriver.Text != string.Empty && textBoxTeamCoach.Text != string.Empty)
            {
                using (var connection = DatabaseHelper.OpenDefaultConnection())
                using (var sqlCommand = connection.CreateCommand())
                {
                    var teamNameParameter = sqlCommand.Parameters.AddWithValue("@teamName", textBoxTeamName.Text);
                    var teamDriverParameter = sqlCommand.Parameters.AddWithValue("@teamDriver", textBoxTeamDriver.Text);
                    var teamCoachParameter = sqlCommand.Parameters.AddWithValue("@teamCoach", textBoxTeamCoach.Text);
                    var pointsParameter = sqlCommand.Parameters.AddWithValue("@Points", textBoxPoints.Text);

                    sqlCommand.CommandText =
                        $@"INSERT INTO [dbo].[TeamData]
                    ([TeamName], [TeamDriver], [TeamCoach], [Points])
                    VALUES ({teamNameParameter.ParameterName}, {teamDriverParameter.ParameterName}, {teamCoachParameter}, {pointsParameter})";
                    sqlCommand.ExecuteNonQuery();
                }

                MessageBox.Show("Success!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
                MessageBox.Show("Veld mag niet leeg zijn!", "Velden moeten gevuld zijn", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddTeamAndDriver();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
