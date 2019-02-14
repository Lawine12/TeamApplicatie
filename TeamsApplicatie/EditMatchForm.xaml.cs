using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for EditMatchForm.xaml
    /// </summary>
    public partial class EditMatchForm : Window
    {
        private readonly string _id;
        private DataTable _matchData;
        private DataSet _teams;

        public EditMatchForm()
        {
            InitializeComponent();
        }

        public EditMatchForm(string id) : this()
        {
            _id = id;
            loadCombo1Async();
            loadCombo2();
            LoadData();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void LoadData()
        {
            var matchData = await GetDataTableAsync();
            if (matchData.Rows.Count == 1)
            {
                var row = _matchData.Rows[0];
                comboBoxEditTeam1.SelectedItem = comboBoxEditTeam1.ItemsSource.Cast<Team>().FirstOrDefault(x => x.Id == (int)row["Team1ID"]);
                comboBoxEditTeam2.SelectedItem = comboBoxEditTeam2.ItemsSource.Cast<Team>().FirstOrDefault(x => x.Id == (int)row["Team2ID"]);
                EditMatchDatePicker.Text = row["MatchDate"].ToString();
            }
        }

        private async Task<DataTable> GetDataTableAsync()
        {
            _matchData = new DataTable();
            var queryString = @"SELECT MatchInfo.Id,
            Team1ID,
            Team2ID,
            MatchDate,
            TotalGoalsTeam1,
            TotalGoalsTeam2
                FROM dbo.MatchInfo
            WHERE MatchInfo.Id = @id";
            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            using (var cmd = new SqlCommand(queryString, connection))
            {
                cmd.Parameters.AddWithValue("@id", _id);
                var sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(_matchData);
            }
            return _matchData;
        }

        public async Task loadCombo1Async()
        {
            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            {
                var querystring = "SELECT id, TeamName FROM TeamData";

                var cmd = new SqlCommand(querystring, connection);
                var dataAdapter = new SqlDataAdapter(cmd);
                _teams = new DataSet();
                dataAdapter.Fill(_teams, "TeamData");
                comboBoxEditTeam1.DataContext = _teams.Tables[0].DefaultView;
                var list = new List<Team>();

                foreach (DataRow team in _teams.Tables[0].Rows)
                {
                    var id = team.Field<int>("id");
                    var teamName = team.Field<string>("TeamName");

                    list.Add(new Team { Id = id, TeamName = teamName });
                }

                comboBoxEditTeam1.ItemsSource = list;
            }
        }

        public async Task loadCombo2()
        {
            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            {
                var querystring = "SELECT id, TeamName FROM TeamData";

                var cmd = new SqlCommand(querystring, connection);
                var dataAdapter = new SqlDataAdapter(cmd);
                _teams = new DataSet();
                dataAdapter.Fill(_teams, "TeamData");
                var list = new List<Team>();

                foreach (DataRow team in _teams.Tables[0].Rows)
                {
                    var id = team.Field<int>("id");
                    var teamName = team.Field<string>("TeamName");

                    list.Add(new Team { Id = id, TeamName = teamName });
                }

                comboBoxEditTeam2.ItemsSource = list;
            }
        }

        public async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxEditTeam1.Text != string.Empty && EditMatchDatePicker.Text != string.Empty &&
                comboBoxEditTeam2.Text != string.Empty)
            {
                using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
                using (var sqlCommand = connection.CreateCommand())
                {
                    var id = sqlCommand.Parameters.AddWithValue("@id", _id);
                    var team1 = sqlCommand.Parameters.AddWithValue("@Team1ID", comboBoxEditTeam1.SelectedValue);
                    var team2 = sqlCommand.Parameters.AddWithValue("@Team2ID", comboBoxEditTeam2.SelectedValue);
                    var matchDate = sqlCommand.Parameters.AddWithValue("@MatchDate", EditMatchDatePicker.Text);

                    sqlCommand.CommandText =
                    $@"UPDATE [dbo].[MatchInfo]
                    SET
                    [Team1ID] = {team1},
                    [Team2ID] = {team2},
                    [MatchDate] = {matchDate}
                    WHERE [ID] = {id}";
                    sqlCommand.ExecuteNonQuery();
                }
                Close();
            }
        }
    }
}
