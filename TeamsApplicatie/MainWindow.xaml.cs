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

        private void Teamsbekijken_Click(object sender, RoutedEventArgs e)
        {
            var teamOverview = new TeamOverviewForm {WindowStartupLocation = WindowStartupLocation.CenterScreen};
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

        private void UitslagenBekijken_Click(object sender, RoutedEventArgs e)
        {
            var matchResults = new ViewResultsForm {WindowStartupLocation = WindowStartupLocation.CenterScreen};
            matchResults.Show();
        }

        private void WedstrijdKalender_Click(object sender, RoutedEventArgs e)
        {
            var kalender = new MatchCalendarForm {WindowStartupLocation = WindowStartupLocation.CenterScreen};
            kalender.DataChanged += () => { _resultsUpdateForm?.LoadMatchData(); };
            kalender.ShowDialog();
        }

        private void UitslagenUpdated_Click(object sender, RoutedEventArgs e)
        {
            ShowResults();
        }
    }
}
