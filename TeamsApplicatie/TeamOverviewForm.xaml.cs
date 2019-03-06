﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
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
            try
            {
                LoadTeamData().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        //View Players
        private void ButtonViewPlayers_Click(object sender, RoutedEventArgs e)
        {
            ViewPlayers();
        }

        //Add Team
        private void AddTeam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddTeam();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        //Edit Team
        private void EditTeam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditTeam();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        //Delete Team
        private void DeleteTeam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteTeam().ConfigureAwait(true);
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

        //View Players
        private void ViewPlayers()
        {
            var selectedRow = teamDataGrid.SelectedItem;
            var id = Convert.ToInt32((teamDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock)?.Text);

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
            LoadTeamData().ConfigureAwait(true);
            buttonDeleteTeam.IsEnabled = false;
            buttonEditTeam.IsEnabled = false;
            buttonViewPlayers.IsEnabled = false;
        }

        //Edit Team
        private void EditTeam()
        {
            var selectedRow = teamDataGrid.SelectedItem;
            if (selectedRow == null) return;
            var id = (teamDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock)?.Text;
            var team = teamDataGrid.SelectedCells[1].Item;
            var editTeam = new EditTeamsForm(id);
            editTeam.ShowDialog();
            LoadTeamData().ConfigureAwait(true);
            buttonDeleteTeam.IsEnabled = false;
            buttonEditTeam.IsEnabled = false;
            buttonViewPlayers.IsEnabled = false;
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

                    buttonDeleteTeam.IsEnabled = false;
                    buttonEditTeam.IsEnabled = false;
                    buttonViewPlayers.IsEnabled = false;
                }
                else
                {
                    return;
                }

                var queryString = "SELECT * FROM dbo.TeamData";
                using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
                {
                    var cmd = new SqlCommand(queryString, connection);
                    var dataAdapter = new SqlDataAdapter(cmd);
                    var cmdBuilder = new SqlCommandBuilder(dataAdapter);

                    dataAdapter.Update(_teamData);
                }
            }
        }

        private async Task LoadTeamData()
        {
            teamDataGrid.CanUserAddRows = false;
            teamDataGrid.SelectionMode = DataGridSelectionMode.Single;
            teamDataGrid.IsReadOnly = true;

            var querystring = "SELECT * FROM dbo.TeamData";

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
            buttonEditTeam.IsEnabled = true;
            buttonDeleteTeam.IsEnabled = true;
            buttonViewPlayers.IsEnabled = true;
        }
    }
}
