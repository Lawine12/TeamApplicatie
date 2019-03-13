using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for AddMatchForm.xaml
    /// </summary>
    public partial class AddMatchForm : Window
    {
        private DataSet _teams;

        public AddMatchForm()
        {
            InitializeComponent();
            LoadCombo1().ConfigureAwait(true);
            LoadCombo2().ConfigureAwait(true);
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public async Task LoadCombo1()
        {
            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            {
                var querystring = "SELECT id, TeamName FROM TeamData";

                var cmd = new SqlCommand(querystring, connection);
                var dataAdapter = new SqlDataAdapter(cmd);
                _teams = new DataSet();
                dataAdapter.Fill(_teams, "TeamData");
                comboBoxTeam1.DataContext = _teams.Tables[0].DefaultView;
                var list = new List<Team>();

                foreach (DataRow team in _teams.Tables[0].Rows)
                {
                    var id = team.Field<int>("id");
                    var teamName = team.Field<string>("TeamName");

                    list.Add(new Team{Id = id, TeamName = teamName});
                }
                comboBoxTeam1.ItemsSource = list;
            }
        }

        public async Task LoadCombo2()
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

                comboBoxTeam2.ItemsSource = list; // .Where(x => x.Id != (int)comboBoxTeam1.SelectedValue);
            }
        }

        public async void Add_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxTeam1.Text == string.Empty || MatchDatePicker.Text == string.Empty ||
                comboBoxTeam2.Text == string.Empty) return;
            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            using (var sqlCommand = connection.CreateCommand())
            {
                var team1 = sqlCommand.Parameters.AddWithValue("@Team1ID", comboBoxTeam1.SelectedValue);
                var team2 = sqlCommand.Parameters.AddWithValue("@Team2ID", comboBoxTeam2.SelectedValue);
                var matchDate = sqlCommand.Parameters.AddWithValue("@MatchDate", MatchDatePicker.Text);

                sqlCommand.CommandText =
                    $@"INSERT INTO [dbo].[MatchInfo]
                    ([Team1ID], [Team2ID], [MatchDate])
                    VALUES ({team1}, {team2}, {matchDate})";
                sqlCommand.ExecuteNonQuery();
            }
            Close();

        }
    }

    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
    }
}
