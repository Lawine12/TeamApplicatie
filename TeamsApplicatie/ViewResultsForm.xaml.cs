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

        public ViewResultsForm()
        {
            InitializeComponent();
            LoadMatchData();
            SerializeDataTable("test.xml");
        }

        private void SerializeDataTable(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
            DataSet set = new DataSet();
            DataTable table = new DataTable("MatchInfo");
            DataColumn t1 = new DataColumn("Team1ID");
            DataColumn t2 = new DataColumn("Team2ID");
            table.Columns.Add(t1);
            table.Columns.Add(t2);
            set.Tables.Add(table);
            DataRow row;
            for (int i = 0; i < 20; i++)
            {
                row = table.NewRow();
                row[0] = " " + i;
                table.Rows.Add(row);
            }

            TextWriter writer = new StreamWriter(filename);
            serializer.Serialize(writer, set);
            writer.Close();
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
