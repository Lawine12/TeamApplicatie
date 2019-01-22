using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for EditTeamsForm.xaml
    /// </summary>
    public partial class EditTeamsForm : Window
    {
        private readonly string _id;
        private DataTable _teamsInformation;

        public EditTeamsForm()
        {
            InitializeComponent();
            textboxTeamName.Focus();
        }

        public EditTeamsForm(string id) : this()
        {
            _id = id;
            LoadData();
        }

        private void LoadData()
        {
            var teamsData = GetDataTable();
            if (teamsData.Rows.Count == 1)
            {
                var row = _teamsInformation.Rows[0];
                textboxTeamName.Text = (string)row["TeamName"];
                textboxTeamCoach.Text = (string)row["TeamCoach"];
                textboxPoints.Text = row["TeamGoals"].ToString();

            }
        }

        private void SaveTeam()
        {
            if (textboxTeamName.Text != string.Empty && textboxTeamCoach.Text != string.Empty)
            {
                using (var connection = DatabaseHelper.OpenDefaultConnection())
                using (var sqlCommand = connection.CreateCommand())
                {
                    var id = sqlCommand.Parameters.AddWithValue("@id", _id);
                    var teamNameParameter = sqlCommand.Parameters.AddWithValue("@teamname", textboxTeamName.Text);
                    var teamCoachParameter = sqlCommand.Parameters.AddWithValue("@teamCoach", textboxTeamCoach.Text);
                    var teamGoalsParameter = sqlCommand.Parameters.AddWithValue("@TeamGoals", textboxPoints.Text);

                    sqlCommand.CommandText =
                        $@"UPDATE [dbo].[TeamData]
                    SET
                    [TeamName] = {teamNameParameter.ParameterName},
                    [TeamCoach] = {teamCoachParameter.ParameterName},
                    [TeamGoals] = {teamGoalsParameter.ParameterName}
                    WHERE [ID] = {id.ParameterName}
                    ";
                    sqlCommand.ExecuteNonQuery();
                }

                MessageBox.Show("Success!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
                MessageBox.Show("Veld mag niet leeg zijn!", "Velden moeten gevuld zijn", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private DataTable GetDataTable()
        {
            _teamsInformation = new DataTable();
            var queryString = "SELECT * FROM TeamData WHERE Id=@id";
            using (var connection = DatabaseHelper.OpenDefaultConnection())
            using (var cmd = new SqlCommand(queryString, connection))
            {
                cmd.Parameters.AddWithValue("@id", _id);
                var sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(_teamsInformation);
            }
            return _teamsInformation;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void textboxTeamCoach_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textboxPoints_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textboxPoints.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                textboxPoints.Text = textboxPoints.Text.Remove(textboxPoints.Text.Length - 1);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveTeam();
        }
    }
}
