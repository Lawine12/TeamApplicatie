using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for ViewResultsForm.xaml
    /// </summary>
    public partial class ViewResultsForm : Window
    {
        private DataTable _matchData;
        private DataSet _matchInfo;

        public ViewResultsForm()
        {
            InitializeComponent();
            try
            {
                LoadMatchData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }

            SerializeDataTableAsync("Match Information.xml", _matchInfo).ConfigureAwait(true);
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void LoadMatchData()
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
        }
    }
}
