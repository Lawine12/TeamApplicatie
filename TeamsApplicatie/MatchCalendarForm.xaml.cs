using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for MatchCalendarForm.xaml
    /// </summary>
    public partial class MatchCalendarForm : Window
    {
        public MatchCalendarForm()
        {
            InitializeComponent();
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
        }

        private void editMatch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteMatch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void matchDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
