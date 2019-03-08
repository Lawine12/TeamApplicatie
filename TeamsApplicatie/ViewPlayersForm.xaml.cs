using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    public partial class ViewPlayersForm : Window
    {
        private DataTable _playerData;
        private readonly int _id;
        private PlayerStatsForm _playerStatsform;
        private EditPlayerForm _editPlayerForm;
        

        public ViewPlayersForm(int id)
        {
            _id = id;
            InitializeComponent();
            buttonDeletePlayer.IsEnabled = false;
            buttonEditPlayer.IsEnabled = false;
            buttonPlayerStats.IsEnabled = false;
            try
            {
                LoadData().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void playerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonEditPlayer.IsEnabled = true;
            buttonDeletePlayer.IsEnabled = true;
            buttonPlayerStats.IsEnabled = true;
        }

        private void addPlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddPlayer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void deletePlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeletePlayer().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private async void editPlayer_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                await EditPlayer();
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

        //Add Player
        private void AddPlayer()
        {
            var addPlayer = new AddPlayerForm(_id)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            addPlayer.ShowDialog();
            LoadData().ConfigureAwait(true);
        }

        //Edit Player
        private async Task EditPlayer()
        {
            var selectedRow = playerDataGrid.SelectedItem;
            if (selectedRow == null) return;
            var id = (playerDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock)?.Text;
            var player = playerDataGrid.SelectedCells[1].Item;
            var editPlayer = new EditPlayerForm(id);
            editPlayer.ShowDialog();
            await LoadData().ConfigureAwait(true);
            if (_playerStatsform != null) await _playerStatsform.LoadData();
            buttonDeletePlayer.IsEnabled = false;
            buttonEditPlayer.IsEnabled = false;
            buttonPlayerStats.IsEnabled = false;
        }

        //Delete Player
        private async Task DeletePlayer()
        {
            var selectedRow = playerDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                var id = (playerDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock)?.Text;

                if (MessageBox.Show("Weet u het zeker?", "", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {

                    if (playerDataGrid.ItemsSource is DataView itemSource)
                    {
                        itemSource.Delete(playerDataGrid.SelectedIndex);
                        playerDataGrid.ItemsSource = itemSource;
                    }

                    buttonDeletePlayer.IsEnabled = false;
                    buttonEditPlayer.IsEnabled = false;
                    buttonPlayerStats.IsEnabled = false;

                    using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
                    using (var sqlCommand = connection.CreateCommand())
                    {
                        sqlCommand.Parameters.AddWithValue("@id", id);

                        sqlCommand.CommandText =
                            $@"DELETE FROM Players WHERE Id = @Id";
                        sqlCommand.ExecuteNonQuery();
                    }

                }
                else
                {
                    return;
                }

                var queryString = "SELECT * FROM dbo.Players";
                using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
                {
                    var cmd = new SqlCommand(queryString, connection);
                    var dataAdapter = new SqlDataAdapter(cmd);
                    var cmdBuilder = new SqlCommandBuilder(dataAdapter);

                    dataAdapter.Update(_playerData);
                }
            }
        }

        private async Task LoadData()
        {
            playerDataGrid.CanUserAddRows = false;
            playerDataGrid.SelectionMode = DataGridSelectionMode.Single;
            playerDataGrid.IsReadOnly = true;

            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            {
                var cmd = connection.CreateCommand();
                var id = cmd.Parameters.AddWithValue("@TeamId", _id);
                cmd.CommandText = "SELECT * FROM dbo.Players WHERE [TeamId] = @TeamId";

                var dataAdapter = new SqlDataAdapter(cmd);
                _playerData = new DataTable();
                dataAdapter.Fill(_playerData);
                playerDataGrid.DataContext = _playerData;
                playerDataGrid.ItemsSource = _playerData.DefaultView;
            }
            
        }

        private void playerStats_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = playerDataGrid.SelectedItem;
            if (selectedRow == null) return;
            var id = Convert.ToInt32((playerDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock)?.Text);
            _playerStatsform = new PlayerStatsForm(id);
            _playerStatsform.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _playerStatsform.Show();
        }
    }
}
