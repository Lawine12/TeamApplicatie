using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            try
            {
                var win = await TeamOverviewForm.CreateAsync();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error on startup", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
