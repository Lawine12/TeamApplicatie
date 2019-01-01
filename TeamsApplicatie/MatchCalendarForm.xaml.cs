using System;
using System.Data;
using System.Data.SqlClient;
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

        public MatchCalendarForm()
        {
            InitializeComponent();
            LoadMatchData();
            buttonEditMatch.IsEnabled = false;
            buttonDeleteMatch.IsEnabled = false;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void addMatch_Click(object sender, RoutedEventArgs e)
        {
            var addMatch = new AddMatchForm();
            addMatch.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            addMatch.ShowDialog();
            LoadMatchData();
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
        }

        private void LoadMatchData()
        {
            matchDataGrid.CanUserAddRows = false;
            matchDataGrid.SelectionMode = DataGridSelectionMode.Single;
            matchDataGrid.IsReadOnly = true;

            string querystring = "SELECT * FROM dbo.MatchInfo";

            using (var connection = DatabaseHelper.OpenDefaultConnection())
            {
                var cmd = new SqlCommand(querystring, connection);
                var dataAdapter = new SqlDataAdapter(cmd);
                _matchData = new DataTable();
                dataAdapter.Fill(_matchData);
                matchDataGrid.DataContext = _matchData;
                matchDataGrid.ItemsSource = _matchData.DefaultView;
            }
        }

        private void DeleteMatch()
        {
            Object selectedRow = matchDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                string team = (matchDataGrid.SelectedCells[0].Column.GetCellContent(selectedRow) as TextBlock).Text;

                if (MessageBox.Show("Weet u het zeker?", "", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    var itemSource = matchDataGrid.ItemsSource as DataView;

                    itemSource.Delete(matchDataGrid.SelectedIndex);
                    matchDataGrid.ItemsSource = itemSource;
                    buttonDeleteMatch.IsEnabled = false;
                    buttonEditMatch.IsEnabled = false;
                }
                else
                {
                    return;
                }

                var queryString = "SELECT * FROM dbo.MatchInfo";
                using (var connection = DatabaseHelper.OpenDefaultConnection())
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
        }
    }
}
