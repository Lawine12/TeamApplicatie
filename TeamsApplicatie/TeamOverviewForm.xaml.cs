using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for TeamOverviewForm.xaml
    /// </summary>
    public partial class TeamOverviewForm : Window
    {
        private DataTable _teamData;

        public TeamOverviewForm()
        {
            InitializeComponent();
            buttonDeleteTeam.IsEnabled = false;
            buttonEditTeam.IsEnabled = false;
            LoadData();
        }

        //Add Team
        private void addTeam_Click(object sender, RoutedEventArgs e)
        {
            var addTeam = new AddTeam();
            addTeam.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            addTeam.ShowDialog();
            LoadData();
            buttonDeleteTeam.IsEnabled = false;
            buttonEditTeam.IsEnabled = false;
        }

        //Edit Team
        private void editTeam_Click(object sender, RoutedEventArgs e)
        {
            Object selectedRow = teamDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                string id = (teamDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock).Text;
                var team = teamDataGrid.SelectedCells[1].Item;
                var editTeam = new EditTeamsForm(id);
                editTeam.ShowDialog();
                LoadData();
                buttonDeleteTeam.IsEnabled = false;
                buttonEditTeam.IsEnabled = false;
            }
        }


        //Delete Team
        private void deleteTeam_Click(object sender, RoutedEventArgs e)
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

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadData()
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
        }
    }
}
