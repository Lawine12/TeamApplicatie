using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    /// Interaction logic for EnterResultsForm.xaml
    /// </summary>
    public partial class EnterResultsForm : Window
    {
        private readonly string _id;
        private DataSet _teams;
        private DataSet _doelpunten;

        public EnterResultsForm()
        {
            InitializeComponent();
            textNameTeam1.IsEnabled = false;
            textNameTeam2.IsEnabled = false;
            ResultsDatePicker.IsEnabled = false;
        }

        public EnterResultsForm(string id) : this()
        {
            _id = id;
            loadCombo1();
            loadCombo2();
            LoadData();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnterResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoadData()
        {
            var matchData = GetDataTable();
            if (matchData.Rows.Count == 1)
            {
                var row = matchData.Rows[0];
                textNameTeam1.Text = (string)row["TeamName1"];
                textNameTeam2.Text = (string)row["TeamName2"];
                ResultsDatePicker.Text = row["MatchDate"].ToString();
            }
        }

        private DataTable GetDataTable()
        {
            var matchData = new DataTable();
            var queryString = "SELECT * FROM MatchInfo WHERE Id=@id";
            using (var connection = DatabaseHelper.OpenDefaultConnection())
            using (var cmd = new SqlCommand(queryString, connection))
            {
                cmd.Parameters.AddWithValue("@id", _id);
                var sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(matchData);
            }
            return matchData;
        }

        public void loadCombo1()
        {
            using (var connection = DatabaseHelper.OpenDefaultConnection())
            {
                var querystring = "SELECT TeamName FROM TeamData";

                var matchData = GetDataTable();
                var row = matchData.Rows[0];
                var cmd = new SqlCommand(querystring, connection);
                var dataAdapter = new SqlDataAdapter(cmd);
                _teams = new DataSet();
                dataAdapter.Fill(_teams, "TeamData");
                textNameTeam1.Text = (string)row["TeamName1"];
            }
        }

        public void loadCombo2()
        {
            using (var connection = DatabaseHelper.OpenDefaultConnection())
            {
                var querystring = "SELECT TeamName FROM TeamData";

                var matchData = GetDataTable();
                var row = matchData.Rows[0];
                var cmd = new SqlCommand(querystring, connection);
                var dataAdapter = new SqlDataAdapter(cmd);
                _teams = new DataSet();
                dataAdapter.Fill(_teams, "TeamData");
                textNameTeam2.Text = (string)row["TeamName1"];
            }
        }

        private void TextBoxDoelpuntenTeam1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textboxDoelpuntenTeam1.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                textboxDoelpuntenTeam1.Text = textboxDoelpuntenTeam1.Text.Remove(textboxDoelpuntenTeam1.Text.Length - 1);
            }
        }

        private void TextBoxDoelpuntenTeam2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textboxDoelpuntenTeam2.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                textboxDoelpuntenTeam2.Text = textboxDoelpuntenTeam2.Text.Remove(textboxDoelpuntenTeam2.Text.Length - 1);
            }
        }

        private void EnterResults()
        {
            if (textboxDoelpuntenTeam1.Text != string.Empty && textboxDoelpuntenTeam2.Text != string.Empty)
            {
                using (var connection = DatabaseHelper.OpenDefaultConnection())
                using (var sqlCommand = connection.CreateCommand())
                {
                    var team1DoelpuntenParameter = sqlCommand.Parameters.AddWithValue("@TeamGoals", textboxDoelpuntenTeam1.Text);
                    var team2DoelpuntenParameter = sqlCommand.Parameters.AddWithValue("@TeamGoals", textboxDoelpuntenTeam2.Text);

                    sqlCommand.CommandText =
                        $@"INSERT INTO [dbo].[TeamData]
                    ([TeamGoals])
                    VALUES ({team1DoelpuntenParameter.ParameterName})";
                    sqlCommand.ExecuteNonQuery();
                }

                MessageBox.Show("Success!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
                MessageBox.Show("Veld mag niet leeg zijn!", "Velden moeten gevuld zijn", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
