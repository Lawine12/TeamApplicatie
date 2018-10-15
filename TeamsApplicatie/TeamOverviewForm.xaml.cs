using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    public partial class TeamOverviewForm : Window
    {
        private DataTable _teamData;

        public TeamOverviewForm()
        {
            InitializeComponent();
            buttonDeleteTeam.IsEnabled = false;
            buttonEditTeam.IsEnabled = false;
            buttonViewPlayers.IsEnabled = false;
            LoadTeamData();
        }

        //View Players
        private void buttonViewPlayers_Click(object sender, RoutedEventArgs e)
        {
            ViewPlayers();
        }

        //Add Team
        private void addTeam_Click(object sender, RoutedEventArgs e)
        {
            AddTeam();
        }

        //Edit Team
        private void editTeam_Click(object sender, RoutedEventArgs e)
        {
            EditTeam();
        }

        //Delete Team
        private void deleteTeam_Click(object sender, RoutedEventArgs e)
        {
            DeleteTeam();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //View Players
        private void ViewPlayers()
        {
            Object selectedRow = teamDataGrid.SelectedItem;
            int id = Convert.ToInt32((teamDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock)
                .Text);

            var playerOverview = new ViewPlayersForm(id);
            playerOverview.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            playerOverview.ShowDialog();
        }

        //Add Team
        private void AddTeam()
        {
            var addTeam = new AddTeam();
            addTeam.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            addTeam.ShowDialog();
            LoadTeamData();
            buttonDeleteTeam.IsEnabled = false;
            buttonEditTeam.IsEnabled = false;
            buttonViewPlayers.IsEnabled = false;
        }

        //Edit Team
        private void EditTeam()
        {
            Object selectedRow = teamDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                string id = (teamDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock).Text;
                var team = teamDataGrid.SelectedCells[1].Item;
                var editTeam = new EditTeamsForm(id);
                editTeam.ShowDialog();
                LoadTeamData();
                buttonDeleteTeam.IsEnabled = false;
                buttonEditTeam.IsEnabled = false;
                buttonViewPlayers.IsEnabled = false;
            }
        }

        //Delete Team
        private void DeleteTeam()
        {
            Object selectedRow = teamDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                string team = (teamDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock).Text;

                if (MessageBox.Show("Weet u het zeker?", "", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    var itemSource = teamDataGrid.ItemsSource as DataView;

                    itemSource.Delete(teamDataGrid.SelectedIndex);
                    teamDataGrid.ItemsSource = itemSource;
                    buttonDeleteTeam.IsEnabled = false;
                    buttonEditTeam.IsEnabled = false;
                    buttonViewPlayers.IsEnabled = false;
                }
                else
                {
                    return;
                }

                var queryString = "SELECT * FROM dbo.TeamData";
                using (var connection = DatabaseHelper.OpenDefaultConnection())
                {
                    var cmd = new SqlCommand(queryString, connection);
                    var dataAdapter = new SqlDataAdapter(cmd);
                    var cmdBuilder = new SqlCommandBuilder(dataAdapter);

                    dataAdapter.Update(_teamData);
                }
            }
        }

        private void LoadTeamData()
        {
            teamDataGrid.CanUserAddRows = false;
            teamDataGrid.SelectionMode = DataGridSelectionMode.Single;
            teamDataGrid.IsReadOnly = true;

            string querystring = "SELECT * FROM dbo.TeamData";

            using (var connection = DatabaseHelper.OpenDefaultConnection())
            {
                var cmd = new SqlCommand(querystring, connection);
                var dataAdapter = new SqlDataAdapter(cmd);
                _teamData = new DataTable();
                dataAdapter.Fill(_teamData);
                teamDataGrid.DataContext = _teamData;
                teamDataGrid.ItemsSource = _teamData.DefaultView;
            }
        }

        private void teamDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonEditTeam.IsEnabled = true;
            buttonDeleteTeam.IsEnabled = true;
            buttonViewPlayers.IsEnabled = true;
        }
    }
}
