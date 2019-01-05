using System.Windows;

namespace TeamsApplicatie
{
    public partial class MainWindow : Window
    {
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

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UitslagenBekijken_Click(object sender, RoutedEventArgs e)
        {
            var matchResults = new ViewResultsForm();
            matchResults.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            matchResults.ShowDialog();
        }

        private void WedstrijdKalender_Click(object sender, RoutedEventArgs e)
        {
            var kalender = new MatchCalendarForm();
            kalender.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            kalender.ShowDialog();
        }
    }
}
