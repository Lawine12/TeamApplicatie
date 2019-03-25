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
    /// Interaction logic for ResultsUpdateForm.xaml
    /// </summary>
    public partial class ResultsUpdateForm : Window
    {
        private DataTable _matchData;
        private DataSet _matchInfo;

        private ResultsUpdateForm()
        {
            InitializeComponent();
        }

        public static async Task<ResultsUpdateForm> CreateAsync()
        {
            try
            {
                var form = new ResultsUpdateForm();
                await form.LoadMatchData();
                return form;
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

        public async Task LoadMatchData()
        {
            resultDataGrid.CanUserAddRows = false;
            resultDataGrid.SelectionMode = DataGridSelectionMode.Single;
            resultDataGrid.IsReadOnly = true;

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
                _matchData = new DataTable();
                dataAdapter.Fill(_matchData);
                resultDataGrid.DataContext = _matchData;
                resultDataGrid.ItemsSource = _matchData.DefaultView;
            }
            await SerializeDataTableAsync("Match Information.xml");
        }

        public static async Task SerializeDataTableAsync(string filename)
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
                var matchInfo = new DataSet();
                dataAdapter.Fill(matchInfo);
                TextWriter writer = new StreamWriter(filename);
                matchInfo.WriteXml(writer);
            }
        }

    }
}
