using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
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
            try
            {
                LoadData().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private async Task LoadData()
        {
            var teamsData = await GetDataTable();
            if (teamsData.Rows.Count == 1)
            {
                var row = _teamsInformation.Rows[0];
                textboxTeamName.Text = (string)row["TeamName"];
                textboxTeamCoach.Text = (string)row["TeamCoach"];

            }
        }

        private async Task SaveTeam()
        {
            if (textboxTeamName.Text != string.Empty && textboxTeamCoach.Text != string.Empty)
            {
                using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
                using (var sqlCommand = connection.CreateCommand())
                {
                    var id = sqlCommand.Parameters.AddWithValue("@id", _id);
                    var teamNameParameter = sqlCommand.Parameters.AddWithValue("@teamname", textboxTeamName.Text);
                    var teamCoachParameter = sqlCommand.Parameters.AddWithValue("@teamCoach", textboxTeamCoach.Text);

                    sqlCommand.CommandText =
                        $@"UPDATE [dbo].[TeamData]
                    SET
                    [TeamName] = {teamNameParameter.ParameterName},
                    [TeamCoach] = {teamCoachParameter.ParameterName}
                    WHERE [ID] = {id.ParameterName}
                    ";
                    await sqlCommand.ExecuteNonQueryAsync();
                }

                MessageBox.Show("Success!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
                MessageBox.Show("Veld mag niet leeg zijn!", "Velden moeten gevuld zijn", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task<DataTable> GetDataTable()
        {
            _teamsInformation = new DataTable();
            var queryString = "SELECT * FROM TeamData WHERE Id=@id";
            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
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

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            await SaveTeam();
        }
    }
}
