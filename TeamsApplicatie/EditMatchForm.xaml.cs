using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for EditMatchForm.xaml
    /// </summary>
    public partial class EditMatchForm : Window
    {
        private readonly string _id;
        private DataTable _matchData;
        private DataSet _teams;

        public EditMatchForm()
        {
            InitializeComponent();
        }

        public EditMatchForm(string id) : this()
        {
            _id = id;
            loadCombo1();
            loadCombo2();
            LoadData();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBoxEditTeam2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LoadData()
        {
            var matchData = GetDataTable();
            if (matchData.Rows.Count == 1)
            {
                var row = _matchData.Rows[0];
                comboBoxEditTeam1.SelectedItem = (string)row["TeamName1"];
                comboBoxEditTeam2.SelectedItem = (string)row["TeamName2"];
                EditMatchDatePicker.Text = row["MatchDate"].ToString();
            }
        }

        private DataTable GetDataTable()
        {
            _matchData = new DataTable();
            var queryString = "SELECT * FROM MatchInfo WHERE Id=@id";
            using (var connection = DatabaseHelper.OpenDefaultConnection())
            using (var cmd = new SqlCommand(queryString, connection))
            {
                cmd.Parameters.AddWithValue("@id", _id);
                var sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(_matchData);
            }
            return _matchData;
        }

        public void loadCombo1()
        {
            using (var connection = DatabaseHelper.OpenDefaultConnection())
            {
                var querystring = "SELECT TeamName FROM TeamData";

                var cmd = new SqlCommand(querystring, connection);
                var dataAdapter = new SqlDataAdapter(cmd);
                _teams = new DataSet();
                dataAdapter.Fill(_teams, "TeamData");
                comboBoxEditTeam1.ItemsSource = _teams.Tables[0].DefaultView.ToStringList();
            }
        }


        public void loadCombo2()
        {
            using (var connection = DatabaseHelper.OpenDefaultConnection())
            {
                var querystring = "SELECT TeamName FROM TeamData";

                var cmd = new SqlCommand(querystring, connection);
                var dataAdapter = new SqlDataAdapter(cmd);
                _teams = new DataSet();
                dataAdapter.Fill(_teams, "TeamData");
                comboBoxEditTeam2.ItemsSource = _teams.Tables[0].DefaultView.ToStringList();
            }
        }
    }
}
