using System.Windows;

namespace TeamsApplicatie
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void teamsbekijken_Click(object sender, RoutedEventArgs e)
        {
            var teamOverview = new TeamOverviewForm();
            teamOverview.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            teamOverview.ShowDialog();
        }
        
        private void deleteTeams_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
