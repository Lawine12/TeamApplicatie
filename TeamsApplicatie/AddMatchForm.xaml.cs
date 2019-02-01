using System.Data;
using System.Data.SqlClient;
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
            loadCombo1();
            loadCombo2();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void loadCombo1()
        {
            using (var connection = DatabaseHelper.OpenDefaultConnection())
            {
                var querystring = "SELECT id, TeamName FROM TeamData";

                var cmd = new SqlCommand(querystring, connection);
                var dataAdapter = new SqlDataAdapter(cmd);
                _teams = new DataSet();
                dataAdapter.Fill(_teams, "TeamData");
                //comboBoxTeam1.DataContext = _teams.Tables[0].DefaultView;
                comboBoxTeam1.ItemsSource = new[]
                {
                    new Team
                        { Id = 1,
                          TeamName = "Ajax"
                        },
                    new Team
                    {
                        Id = 2,
                        TeamName = "PVC"
                    }
                };
            }
        }

        public void loadCombo2()
        {
            using (var connection = DatabaseHelper.OpenDefaultConnection())
            {
                var querystring = "SELECT id, TeamName FROM TeamData";

                var cmd = new SqlCommand(querystring, connection);
                var dataAdapter = new SqlDataAdapter(cmd);
                _teams = new DataSet();
                dataAdapter.Fill(_teams, "TeamData");
                comboBoxTeam2.ItemsSource = new[]
                {
                    new Team
                        { Id = 1,
                          TeamName = "Ajax"
                        },
                    new Team
                    {
                        Id = 2,
                        TeamName = "PVC"
                    }
                };

            }
        }
        
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxTeam1.Text != string.Empty && MatchDatePicker.Text != string.Empty &&
                comboBoxTeam2.Text != string.Empty)
            {
                using (var connection = DatabaseHelper.OpenDefaultConnection())
                using (var sqlCommand = connection.CreateCommand())
                {
                    var team1 = sqlCommand.Parameters.AddWithValue("@Team1ID", comboBoxTeam1.SelectedValue);
                    var team2 = sqlCommand.Parameters.AddWithValue("@Team2ID", comboBoxTeam2.SelectedIndex);
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
    }

    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
    }
}
