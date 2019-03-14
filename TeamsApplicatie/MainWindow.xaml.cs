using System.Windows;

namespace TeamsApplicatie
{
    public partial class MainWindow : Window
    {
        private ResultsUpdateForm _resultsUpdateForm;
        private MatchCalendarForm _matchCalendarForm;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private async void Teamsbekijken_ClickAsync(object sender, RoutedEventArgs e)
        {
            var teamOverview = await TeamOverviewForm.CreateAsync();
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            };
            teamOverview.ShowDialog();
        }

        private void ShowResults()
        {
            if (_resultsUpdateForm == null)
                _resultsUpdateForm = new ResultsUpdateForm();
            _resultsUpdateForm.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void UitslagenBekijken_ClickAsync(object sender, RoutedEventArgs e)
        {
            var matchResults = await ViewResultsForm.CreateAsync();
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            };
            matchResults.Show();
        }

        private async void WedstrijdKalender_ClickAsync(object sender, RoutedEventArgs e)
        {
            var kalender = await MatchCalendarForm.CreateAsync();
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            };
            kalender.DataChanged += () => { _resultsUpdateForm?.LoadMatchData(); };
            kalender.ShowDialog();
        }

        private void UitslagenUpdated_Click(object sender, RoutedEventArgs e)
        {
            ShowResults();
        }
    }
}
