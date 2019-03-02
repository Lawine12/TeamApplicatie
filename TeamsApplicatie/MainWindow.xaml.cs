using System.Windows;

namespace TeamsApplicatie
{
    public partial class MainWindow : Window
    {
        private ResultsUpdateForm _ResultsUpdateForm;
        private MatchCalendarForm _MatchCalendarForm;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void teamsbekijken_Click(object sender, RoutedEventArgs e)
        {
            var teamOverview = new TeamOverviewForm();
            teamOverview.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            teamOverview.ShowDialog();
        }

        private void ShowResults()
        {
            if (_ResultsUpdateForm == null)
                _ResultsUpdateForm = new ResultsUpdateForm();
            _ResultsUpdateForm.Show();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UitslagenBekijken_Click(object sender, RoutedEventArgs e)
        {
            var matchResults = new ViewResultsForm();
            matchResults.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            matchResults.Show();
        }

        private void WedstrijdKalender_Click(object sender, RoutedEventArgs e)
        {
            var kalender = new MatchCalendarForm();
            kalender.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            kalender.DataChanged += () => { _ResultsUpdateForm?.LoadMatchData(); };
            kalender.ShowDialog();
        }

        private void UitslagenUpdated_Click(object sender, RoutedEventArgs e)
        {
            ShowResults();
        }
    }
}
