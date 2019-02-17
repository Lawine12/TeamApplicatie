using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;
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
            LoadMatchData();
            SerializeDataTableAsync("Match Information.xml", _matchInfo);
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

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void resultDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
