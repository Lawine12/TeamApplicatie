using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
        
        private EditMatchForm(string id)
        {
            _id = id;
        }

        private async Task LoadMatchDataAsync()
        {
            await LoadCombo1Async();
            await LoadCombo2Async();
            try
            {
                await LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public static async Task<EditMatchForm> CreateAsync(string id)
        {
            var form = new EditMatchForm(id);
            await form.LoadMatchDataAsync();
            return form;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async Task LoadData()
        {
            var matchData = await GetDataTableAsync();
            if (matchData.Rows.Count != 1) return;
            var row = _matchData.Rows[0];
            comboBoxEditTeam1.SelectedItem = comboBoxEditTeam1.ItemsSource.Cast<Team>().FirstOrDefault(x => x.Id == (int)row["Team1ID"]);
            comboBoxEditTeam2.SelectedItem = comboBoxEditTeam2.ItemsSource.Cast<Team>().FirstOrDefault(x => x.Id == (int)row["Team2ID"]);
            EditMatchDatePicker.Text = row["MatchDate"].ToString();
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

        public async Task LoadCombo1Async()
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

        public async Task LoadCombo2Async()
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
            if (comboBoxEditTeam1.Text == string.Empty || EditMatchDatePicker.Text == string.Empty ||
                comboBoxEditTeam2.Text == string.Empty) return;
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
