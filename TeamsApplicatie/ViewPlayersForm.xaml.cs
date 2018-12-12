using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    public partial class ViewPlayersForm : Window
    {
        private DataTable _playerData;
        private readonly int _id;

        public ViewPlayersForm(int id)
        {
            _id = id;
            InitializeComponent();
            buttonDeletePlayer.IsEnabled = false;
            buttonEditPlayer.IsEnabled = false;
            LoadData();
        }

        private void playerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonEditPlayer.IsEnabled = true;
            buttonDeletePlayer.IsEnabled = true;
        }

        private void addPlayer_Click(object sender, RoutedEventArgs e)
        {
            AddPlayer();
        }

        private void deletePlayer_Click(object sender, RoutedEventArgs e)
        {
            DeletePlayer();
        }

        private void editPlayer_Click(object sender, RoutedEventArgs e)
        {
            EditPlayer();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //Add Player
        private void AddPlayer()
        {
            var addPlayer = new AddPlayerForm(_id);
            addPlayer.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            addPlayer.ShowDialog();
            LoadData();
        }

        //Edit Player
        private void EditPlayer()
        {
            Object selectedRow = playerDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                string id = (playerDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock).Text;
                var player = playerDataGrid.SelectedCells[1].Item;
                var editPlayer = new EditPlayerForm(id);
                editPlayer.ShowDialog();
                LoadData();
                buttonDeletePlayer.IsEnabled = false;
                buttonEditPlayer.IsEnabled = false;
            }
        }

        //Delete Player
        private void DeletePlayer()
        {
            Object selectedRow = playerDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                string player = (playerDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock).Text;

                if (MessageBox.Show("Weet u het zeker?", "", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    var itemSource = playerDataGrid.ItemsSource as DataView;

                    itemSource.Delete(playerDataGrid.SelectedIndex);
                    playerDataGrid.ItemsSource = itemSource;
                    buttonDeletePlayer.IsEnabled = false;
                    buttonEditPlayer.IsEnabled = false;
                }
                else
                {
                    return;
                }

                var queryString = "SELECT * FROM dbo.Players";
                using (var connection = DatabaseHelper.OpenDefaultConnection())
                {
                    var cmd = new SqlCommand(queryString, connection);
                    var dataAdapter = new SqlDataAdapter(cmd);
                    var cmdBuilder = new SqlCommandBuilder(dataAdapter);

                    dataAdapter.Update(_playerData);
                }
            }
        }

        private void LoadData()
        {
            playerDataGrid.CanUserAddRows = false;
            playerDataGrid.SelectionMode = DataGridSelectionMode.Single;
            playerDataGrid.IsReadOnly = true;

            using (var connection = DatabaseHelper.OpenDefaultConnection())
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
    }
}
