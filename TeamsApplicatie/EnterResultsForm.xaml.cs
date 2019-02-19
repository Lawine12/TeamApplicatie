using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for EnterResultsForm.xaml
    /// </summary>
    public partial class EnterResultsForm : Window
    {
        private readonly string _id;
        private DataSet _teams;
        private DataSet _doelpunten;
        private DataSet _matchInfo;

        public EnterResultsForm()
        {
            InitializeComponent();
            textNameTeam1.IsEnabled = false;
            textNameTeam2.IsEnabled = false;
            ResultsDatePicker.IsEnabled = false;
        }

        private async Task SerializeDataTableAsync(string filename, DataSet matchInfo)
        {
            string querystring = @"SELECT MatchInfo.Id,
            Team1.TeamName,
            Team2.TeamName,
            MatchInfo.MatchDate,
            MatchInfo.TotalGoalsTeam1,
            MatchInfo.TotalGoalsTeam2
                FROM dbo.MatchInfo
                INNER JOIN TeamData Team1 ON MatchInfo.Team1ID = Team1.Id
            INNER JOIN TeamData Team2 ON MatchInfo.Team2ID = Team2.Id";

            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            {
                var cmd = new SqlCommand(querystring, connection);
                var dataAdapter = new SqlDataAdapter(cmd);
                _matchInfo = new DataSet();
                dataAdapter.Fill(_matchInfo);
                TextWriter writer = new StreamWriter(filename);
                _matchInfo.WriteXml(writer);
            }
        }

        public EnterResultsForm(string id) : this()
        {
            _id = id;
            LoadData();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnterResults();
                SerializeDataTableAsync("Match Information.xml", _matchInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void LoadData()
        {
            var matchData = await GetDataTable();
            if (matchData.Rows.Count == 1)
            {
                var row = matchData.Rows[0];
                textNameTeam1.Text = row[1] as string;
                textNameTeam2.Text = row[2] as string;
                ResultsDatePicker.Text = row["MatchDate"].ToString();
                textboxDoelpuntenTeam1.Text = row["TotalGoalsTeam1"].ToString();
                textboxDoelpuntenTeam2.Text = row["TotalGoalsTeam2"].ToString();
                
            }
        }

        private async Task<DataTable> GetDataTable()
        {
           var matchData = new DataTable();
            var queryString = @"SELECT MatchInfo.Id,
			Team1.TeamName,
			Team2.TeamName,
            MatchDate,
            TotalGoalsTeam1,
            TotalGoalsTeam2
                FROM dbo.MatchInfo
				INNER JOIN TeamData team1 ON team1.Id = MatchInfo.Team1ID
				INNER JOIN TeamData team2 ON team2.Id = MatchInfo.Team2ID
            WHERE MatchInfo.Id = @id";
            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            using (var cmd = new SqlCommand(queryString, connection))
            {
                cmd.Parameters.AddWithValue("@id", _id);
                var sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(matchData);
            }
            return matchData;
        }

        private void TextBoxDoelpuntenTeam1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textboxDoelpuntenTeam1.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                textboxDoelpuntenTeam1.Text = textboxDoelpuntenTeam1.Text.Remove(textboxDoelpuntenTeam1.Text.Length - 1);
            }
        }

        private void TextBoxDoelpuntenTeam2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textboxDoelpuntenTeam2.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                textboxDoelpuntenTeam2.Text = textboxDoelpuntenTeam2.Text.Remove(textboxDoelpuntenTeam2.Text.Length - 1);
            }
        }

        private async Task EnterResults()
        {
            if (textboxDoelpuntenTeam1.Text != string.Empty && textboxDoelpuntenTeam2.Text != string.Empty)
            {
                using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
                using (var sqlCommand = connection.CreateCommand())
                {
                    var id = sqlCommand.Parameters.AddWithValue("@id", _id);
                    var team1DoelpuntenParameter = sqlCommand.Parameters.AddWithValue("@TotalGoalsTeam1", textboxDoelpuntenTeam1.Text);
                    var team2DoelpuntenParameter = sqlCommand.Parameters.AddWithValue("@TotalGoalsTeam2", textboxDoelpuntenTeam2.Text);

                    sqlCommand.CommandText =
                    $@"UPDATE [dbo].[MatchInfo]
                    SET
                    [TotalGoalsTeam1] = {team1DoelpuntenParameter.ParameterName},
                    [TotalGoalsTeam2] = {team2DoelpuntenParameter.ParameterName}
                    WHERE Id = @id";
                    sqlCommand.ExecuteNonQuery();
                }

                MessageBox.Show("Success!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
                var resultsview = new ResultsUpdateForm(buttonSave);
                resultsview.Show();
                LoadData();
            }
            else
                MessageBox.Show("Veld mag niet leeg zijn!", "Velden moeten gevuld zijn", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
