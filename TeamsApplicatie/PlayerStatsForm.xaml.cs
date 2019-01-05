using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for PlayerStatsForm.xaml
    /// </summary>
    public partial class PlayerStatsForm : Window
    {
        private readonly string _id;
        private DataTable _playerInformation;
        private DataTable _teamInformation;

        public PlayerStatsForm()
        {
            InitializeComponent();
            textboxPlayerFirstName.IsEnabled = false;
            textboxPlayerLastName.IsEnabled = false;
            textboxPlayerAdress.IsEnabled = false;
            textboxPlayerCity.IsEnabled = false;
            textboxPlayerAge.IsEnabled = false;
            textboxPlayerPosition.IsEnabled = false;
            textboxCurrentTeam.IsEnabled = false;
        }

        public PlayerStatsForm(string id) : this()
        {
            _id = id;
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void LoadData()
        {
            var playerData = GetPlayerDataTable();
            var teamData = GetTeamDataTable();
            if (playerData.Rows.Count == 1)
            {
                var teamRow = _teamInformation.Rows[0];
                var playerRow = _playerInformation.Rows[0];
                textboxPlayerFirstName.Text = (string)playerRow["First Name"];
                textboxPlayerLastName.Text = (string)playerRow["Last Name"];
                textboxPlayerAdress.Text = (string)playerRow["Adress"];
                textboxPlayerCity.Text = (string)playerRow["City"];
                textboxPlayerAge.Text = playerRow["Age"].ToString();
                textboxPlayerPosition.Text = (string)playerRow["Position"];
                textboxCurrentTeam.Text = (string) teamRow["TeamName"];
            }
        }

        private DataTable GetPlayerDataTable()
        {
            _playerInformation = new DataTable();
            var queryString = "SELECT * FROM Players WHERE Id=@id";
            using (var connection = DatabaseHelper.OpenDefaultConnection())
            using (var cmd = new SqlCommand(queryString, connection))
            {
                cmd.Parameters.AddWithValue("@id", _id);
                var sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(_playerInformation);
            }
            return _playerInformation;
        }

        private DataTable GetTeamDataTable()
        {
            _teamInformation = new DataTable();
            var queryString = "SELECT * FROM TeamData WHERE Id=@id";
            using (var connection = DatabaseHelper.OpenDefaultConnection())
            using (var cmd = new SqlCommand(queryString, connection))
            {
                cmd.Parameters.AddWithValue("@id", _id);
                var sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(_teamInformation);
            }
            return _teamInformation;
        }
    }
}
