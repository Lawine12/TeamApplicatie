using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for MatchCalendarForm.xaml
    /// </summary>
    public partial class MatchCalendarForm : Window
    {
        private DataTable _matchData;

        public delegate void DataChangedHandler();
        public event DataChangedHandler DataChanged = null;

        public MatchCalendarForm()
        {
            InitializeComponent();
            LoadMatchDataAsync();
            buttonEditMatch.IsEnabled = false;
            buttonDeleteMatch.IsEnabled = false;
            buttonEnterResults.IsEnabled = false;
        }

        protected virtual void OnDataChanged()
        {
            DataChanged?.Invoke();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void addMatch_Click(object sender, RoutedEventArgs e)
        {
            await AddMatchAsync().ConfigureAwait(true);
        }

        private async Task AddMatchAsync()
        {
            var addMatch = new AddMatchForm();
            addMatch.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            addMatch.ShowDialog();
            await LoadMatchDataAsync().ConfigureAwait(true);
        }

        private void editMatch_Click(object sender, RoutedEventArgs e)
        {
            ViewMatches();
        }

        private void deleteMatch_Click(object sender, RoutedEventArgs e)
        {
            DeleteMatch();
        }

        private void matchDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonEditMatch.IsEnabled = true;
            buttonDeleteMatch.IsEnabled = true;
            buttonEnterResults.IsEnabled = true;
        }

        private async Task LoadMatchDataAsync()
        {
            matchDataGrid.CanUserAddRows = false;
            matchDataGrid.SelectionMode = DataGridSelectionMode.Single;
            matchDataGrid.IsReadOnly = true;

            string querystring = @"SELECT MatchInfo.Id,
			Team1.Id,
			Team2.id,
            Team1.TeamName,
            Team2.TeamName,
            MatchInfo.MatchDate
                FROM dbo.MatchInfo
                INNER JOIN TeamData Team1 ON MatchInfo.Team1ID = Team1.Id
            INNER JOIN TeamData Team2 ON MatchInfo.Team2ID = Team2.Id";

            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            {
                var cmd = new SqlCommand(querystring, connection);
                var dataAdapter = new SqlDataAdapter(cmd);
                _matchData = new DataTable();
                dataAdapter.Fill(_matchData);
                matchDataGrid.DataContext = _matchData;
                matchDataGrid.ItemsSource = _matchData.DefaultView;
            }
            OnDataChanged();
        }

        private async Task DeleteMatch()
        {
            Object selectedRow = matchDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                int id = Convert.ToInt32((matchDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock)
                    .Text);

                if (MessageBox.Show("Weet u het zeker?", "", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    var itemSource = matchDataGrid.ItemsSource as DataView;
                    
                    using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
                    using (var sqlCommand = connection.CreateCommand())
                    {
                        sqlCommand.Parameters.AddWithValue("@id", id);

                        sqlCommand.CommandText =
                            $@"DELETE FROM MatchInfo WHERE Id = @Id";
                        sqlCommand.ExecuteNonQuery();
                    }

                    itemSource.Delete(matchDataGrid.SelectedIndex);
                    matchDataGrid.ItemsSource = itemSource;
                    buttonDeleteMatch.IsEnabled = false;
                    buttonEditMatch.IsEnabled = false;
                    buttonEnterResults.IsEnabled = false;
                    OnDataChanged();
                }
                else
                {
                    return;
                }

                var queryString = "SELECT * FROM dbo.MatchInfo";
                using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
                {
                    var cmd = new SqlCommand(queryString, connection);
                    var dataAdapter = new SqlDataAdapter(cmd);
                    var cmdBuilder = new SqlCommandBuilder(dataAdapter);

                    dataAdapter.Update(_matchData);
                }
            }
        }

        private void ViewMatches()
        {
            Object selectedRow = matchDataGrid.SelectedItem;
            int id = Convert.ToInt32((matchDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock)
                .Text);

            var editMatch = new EditMatchForm(id.ToString());
            editMatch.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            editMatch.ShowDialog();
            LoadMatchDataAsync();
        }

        private void enterResults_Click(object sender, RoutedEventArgs e)
        {
            Object selectedRow = matchDataGrid.SelectedItem;
            int matchId = Convert.ToInt32((matchDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock)
                .Text);
            int id1 = Convert.ToInt32((matchDataGrid.SelectedCells[1].Column.GetCellContent(selectedRow) as TextBlock)
                .Text);
            int id2 = Convert.ToInt32((matchDataGrid.SelectedCells[2].Column.GetCellContent(selectedRow) as TextBlock)
                .Text);

            var enterResults = new EnterResultsForm(id1.ToString(), id2.ToString(), matchId);
            enterResults.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            enterResults.ShowDialog();
            LoadMatchDataAsync();
        }
    }
}
