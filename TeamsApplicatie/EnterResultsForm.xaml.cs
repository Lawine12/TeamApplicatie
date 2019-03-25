using System;
using System.Collections.Generic;
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
        private readonly string _idTeam1;
        private readonly string _idTeam2;
        private readonly int _matchId;
        private DataSet _teams;
        private DataSet _doelpunten;
        private DataSet _matchInfo;
        private DataSet _playersTeam1;
        private DataSet _playersTeam2;

        public EnterResultsForm()
        {
            InitializeComponent();
            textNameTeam1.IsEnabled = false;
            textNameTeam2.IsEnabled = false;
            ResultsDatePicker.IsEnabled = false;
        }

        private async Task SerializeDataTableAsync(string filename, DataSet matchInfo)
        {
            var querystring = @"SELECT MatchInfo.Id,
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

        private EnterResultsForm(string id1, string id2, int matchId) : this()
        {
            _matchId = matchId;
            _idTeam1 = id1;
            _idTeam2 = id2;

        }

        private async Task LoadAllData()
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }

            await LoadCombo1();
            await LoadCombo2();
            textboxDoelpuntenTeam1.IsEnabled = false;
            textboxDoelpuntenTeam2.IsEnabled = false;
        }

        public static async Task<EnterResultsForm> CreateAsync(string id1, string id2, int matchId)
        {
            var form = new EnterResultsForm(id1, id2, matchId);
            await form.LoadAllData();
            return form;
        }

        private async void Save_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                await EnterResults();
                await SerializeDataTableAsync("Match Information.xml", _matchInfo);
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

        public async Task LoadCombo1()
        {
            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            {
                var querystring = "SELECT id, [Last Name] as LastName FROM Players WHERE TeamId = @Id";

                var cmd = new SqlCommand(querystring, connection);
                cmd.Parameters.AddWithValue("@id", _idTeam1);

                var dataAdapter = new SqlDataAdapter(cmd);
                _playersTeam1 = new DataSet();
                dataAdapter.Fill(_playersTeam1, "Players");
                var list = new List<Player>();

                foreach (DataRow player in _playersTeam1.Tables[0].Rows)
                {
                    var id = player.Field<int>("id");
                    var playerName = player.Field<string>("LastName");

                    list.Add(new Player { Id = id, PlayerName = playerName });
                }
                comboBoxPlayersTeam1.ItemsSource = list;
            }
        }

        public async Task LoadCombo2()
        {
            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            {
                var querystring = "SELECT id, [Last Name] as LastName FROM Players WHERE TeamId = @Id";

                var cmd = new SqlCommand(querystring, connection);
                cmd.Parameters.AddWithValue("@id", _idTeam2);

                var dataAdapter = new SqlDataAdapter(cmd);
                _playersTeam2 = new DataSet();
                dataAdapter.Fill(_playersTeam2, "Players");
                var list = new List<Player>();

                foreach (DataRow player in _playersTeam2.Tables[0].Rows)
                {
                    var id = player.Field<int>("id");
                    var playerName = player.Field<string>("LastName");

                    list.Add(new Player { Id = id, PlayerName = playerName });
                }
                comboBoxPlayersTeam2.ItemsSource = list;
            }
        }

        private async void LoadData()
        {
            var matchData = await GetDataTable();
            if (matchData.Rows.Count != 1) return;
            var row = matchData.Rows[0];
            textNameTeam1.Text = row[1] as string;
            textNameTeam2.Text = row[2] as string;
            ResultsDatePicker.Text = row["MatchDate"].ToString();
            textboxDoelpuntenTeam1.Text = row["TotalGoalsTeam1"].ToString();
            textboxDoelpuntenTeam2.Text = row["TotalGoalsTeam2"].ToString();
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
                cmd.Parameters.AddWithValue("@id", _matchId);
                var sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(matchData);
            }
            return matchData;
        }

        private void TextBoxDoelpuntenTeam1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(textboxDoelpuntenTeam1.Text, "[^0-9]")) return;
            MessageBox.Show("Please enter only numbers.");
            textboxDoelpuntenTeam1.Text = textboxDoelpuntenTeam1.Text.Remove(textboxDoelpuntenTeam1.Text.Length - 1);
        }

        private void TextBoxDoelpuntenTeam2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(textboxDoelpuntenTeam2.Text, "[^0-9]")) return;
            MessageBox.Show("Please enter only numbers.");
            textboxDoelpuntenTeam2.Text = textboxDoelpuntenTeam2.Text.Remove(textboxDoelpuntenTeam2.Text.Length - 1);
        }

        private async Task EnterResults()
        {
            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            using (var sqlCommand = connection.CreateCommand())
            {
                var id = sqlCommand.Parameters.AddWithValue("@id", _matchId);
                var team1DoelpuntenParameter = sqlCommand.Parameters.AddWithValue("@TotalGoalsTeam1", Convert.ToInt16(textboxDoelpuntenTeam1.Text));
                var team2DoelpuntenParameter = sqlCommand.Parameters.AddWithValue("@TotalGoalsTeam2", Convert.ToInt16(textboxDoelpuntenTeam2.Text));

                sqlCommand.CommandText =
                $@"UPDATE [dbo].[MatchInfo]
                    SET
                    [TotalGoalsTeam1] = {team1DoelpuntenParameter.Value},
                    [TotalGoalsTeam2] = {team2DoelpuntenParameter.Value}
                    WHERE Id = @id";
                sqlCommand.ExecuteNonQuery();
            }

            MessageBox.Show("Success!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private async Task AddPlayerPoints1()
        {
            if (comboBoxPlayersTeam1.SelectedValue != null)
            {
                int doelpunten = short.Parse(textboxDoelpuntenTeam1.Text);

                textboxDoelpuntenTeam1.Text = (doelpunten + 1).ToString();

                using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
                using (var sqlCommand = connection.CreateCommand())
                {
                    var id = sqlCommand.Parameters.AddWithValue("@id", comboBoxPlayersTeam1.SelectedValue);

                    sqlCommand.CommandText =
                        @"UPDATE [dbo].[Players]
                    SET
                    PlayerGoals = PlayerGoals + 1
                    WHERE Id = @id";
                    sqlCommand.ExecuteNonQuery();
                }
                MessageBox.Show("Added a Goal for this player! Don't forget to press the Save to save the total scores for both teams!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show("Selecteer AUB een speler", "", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private async Task AddPlayerPoints2()
        {
            if (comboBoxPlayersTeam2.SelectedValue != null)
            {
                int doelpunten = short.Parse(textboxDoelpuntenTeam2.Text);

                textboxDoelpuntenTeam2.Text = (doelpunten + 1).ToString();

                using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
                using (var sqlCommand = connection.CreateCommand())
                {

                    var id = sqlCommand.Parameters.AddWithValue("@id", comboBoxPlayersTeam2.SelectedValue);

                    sqlCommand.CommandText =
                        @"UPDATE [dbo].[Players]
                    SET
                    PlayerGoals = PlayerGoals + 1
                    WHERE Id = @id";
                    sqlCommand.ExecuteNonQuery();
                }
                MessageBox.Show("Added a Goal for this player! Don't forget to press the Save to save!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show("Selecteer AUB een speler", "", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        public class Player
        {
            public int Id { get; set; }
            public string PlayerName { get; set; }
        }

        private async void ButtonDoelpuntTeam1toevoegen_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                await AddPlayerPoints1();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private async void ButtonDoelpuntTeam2toevoegen_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                await AddPlayerPoints2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
    }
}
