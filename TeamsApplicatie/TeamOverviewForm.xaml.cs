using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    public partial class TeamOverviewForm : Window, INotifyDataReload
    {
        private DataTable _teamData;
        private ResultsUpdateForm _resultsUpdateForm;

        public TeamOverviewForm()
        {
            InitializeComponent();
            UpdateButtonState();
        }

        public static async Task<TeamOverviewForm> CreateAsync()
        {
            var form = new TeamOverviewForm();
            await form.LoadTeamDataAsync();
            return form;
        }

        private async Task LoadTeamDataAsync()
        {
            try
            {
                await LoadTeamData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        //View Players
        private async void ButtonViewPlayers_ClickAsync(object sender, RoutedEventArgs e)
        {
            await ViewPlayersAsync();
        }

        //Add Team
        private async void AddTeam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await AddTeam();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        //Edit Team
        private async void EditTeam_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                await EditTeamAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        //Delete Team
        private async void DeleteTeam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await DeleteTeam();
            }
            catch (SqlException ex) when(ex.Number == 547)
            {
                MessageBox.Show("Het team bevat nog spelers en/of er zijn nog geplande wedstrijden met dit team", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //View Players
        private async Task ViewPlayersAsync()
        {
            var selectedRow = teamDataGrid.SelectedItem;
            var id = Convert.ToInt32((teamDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock)?.Text);

            var playerOverview = await ViewPlayersForm.CreateAsync(id);
            playerOverview.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            playerOverview.ShowDialog();
            UpdateButtonState();
        }

        //Add Team
        private async Task AddTeam()
        {
            var addTeam = new AddTeam();
            addTeam.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            addTeam.ShowDialog();
            await LoadTeamData();
            UpdateButtonState();
        }

        //Edit Team
        private async Task EditTeamAsync()
        {
            var selectedRow = teamDataGrid.SelectedItem;
            if (selectedRow == null) return;
            var id = (teamDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock)?.Text;
            var team = teamDataGrid.SelectedCells[1].Item;
            var editTeam = await EditTeamsForm.CreateAsync(id);
            editTeam.ShowDialog();
            await LoadTeamData();
            UpdateButtonState();
        }

        //Delete Team
        private async Task DeleteTeam()
        {
            var selectedRow = teamDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                var id = Convert.ToInt32((teamDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock)?.Text);

                if (MessageBox.Show("Weet u het zeker?", "", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    var itemSource = teamDataGrid.ItemsSource as DataView;

                    using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
                    using (var sqlCommand = connection.CreateCommand())
                    {
                        sqlCommand.Parameters.AddWithValue("@id", id);

                        sqlCommand.CommandText =
                            $@"DELETE FROM TeamData WHERE Id = @Id";
                        sqlCommand.ExecuteNonQuery();
                    }

                    if (itemSource != null)
                    {
                        itemSource.Delete(teamDataGrid.SelectedIndex);
                        teamDataGrid.ItemsSource = itemSource;
                    }
                }
                UpdateButtonState();
            }
        }

        private void UpdateButtonState()
        {
            buttonDeleteTeam.IsEnabled = teamDataGrid.SelectedItem != null;
            buttonEditTeam.IsEnabled = teamDataGrid.SelectedItem != null;
            buttonViewPlayers.IsEnabled = teamDataGrid.SelectedItem != null;
        }

        private async Task LoadTeamData()
        {
            teamDataGrid.CanUserAddRows = false;
            teamDataGrid.SelectionMode = DataGridSelectionMode.Single;
            teamDataGrid.IsReadOnly = true;

            var querystring = "SELECT Id, TeamName, TeamCoach FROM dbo.TeamData";

            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
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
            UpdateButtonState();
        }

        private async void ButtonViewResults_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_resultsUpdateForm == null)
                {
                    _resultsUpdateForm = await ResultsUpdateForm.CreateAsync();
                    _resultsUpdateForm.Closed += OnResultsFormClosed;
                }
                _resultsUpdateForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnResultsFormClosed(object sender, EventArgs e)
        {
            if (_resultsUpdateForm != null)
            {
                _resultsUpdateForm.Closed -= OnResultsFormClosed;
            }

            _resultsUpdateForm = null;
        }


        private async void ButtonEditResults_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                var form = await MatchCalendarForm.CreateAsync(this);
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task ReloadData()
        {
            if (_resultsUpdateForm != null)
            {
                await _resultsUpdateForm.LoadMatchData();
            }
        }
    }
}
