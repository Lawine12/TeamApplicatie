using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for PlayerStatsForm.xaml
    /// </summary>
    public partial class PlayerStatsForm : Window
    {
        private readonly int _id;
        private DataTable _playerInformation;
        private DataTable _teamInformation;
        private DataTable _playerStats;
        public delegate void DataChangedHandler();
        public event DataChangedHandler DataChanged = null;

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

        public PlayerStatsForm(int id) : this()
        {
            _id = id;
            try
            {
                LoadData().ConfigureAwait(true);
                LoadTeamData(id).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        protected virtual void OnDataChanged()
        {
            DataChanged?.Invoke();
        }

        public async Task LoadData()
        {
            var playerData = await GetPlayerDataTable();
            var teamData = await GetTeamDataTable();
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

        private async Task<DataTable> GetPlayerDataTable()
        {
            _playerInformation = new DataTable();
            var queryString = "SELECT * FROM Players WHERE Id=@id";
            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            using (var cmd = new SqlCommand(queryString, connection))
            {
                cmd.Parameters.AddWithValue("@id", _id);
                var sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(_playerInformation);
            }
            return _playerInformation;
        }

        private async Task<DataTable> GetTeamDataTable()
        {
            _teamInformation = new DataTable();
            var queryString = "SELECT TeamData.* FROM TeamData LEFT JOIN Players ON Players.TeamId = TeamData.Id WHERE Players.Id=@id";
            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            using (var cmd = new SqlCommand(queryString, connection))
            {
                cmd.Parameters.AddWithValue("@id", _id);
                var sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(_teamInformation);
            }
            return _teamInformation;
        }

        public async Task LoadTeamData(int id)
        {
            var selectedRow = playerStatsDatagrid.SelectedItem;
            
            var querystring = @"SELECT MatchInfo.Id,
            Team1.TeamName,
            Team2.TeamName,
            MatchInfo.MatchDate,
			Player1.PlayerGoals
                FROM dbo.MatchInfo
            INNER JOIN TeamData Team1 ON MatchInfo.Team1ID = Team1.Id
            INNER JOIN TeamData Team2 ON MatchInfo.Team2ID = Team2.Id
			INNER JOIN Players player1 ON Team1.Id = player1.TeamId OR Team2.Id = player1.TeamId
			WHERE player1.PlayerGoals > 0  AND player1.Id = @id";

            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            {
                var cmd = new SqlCommand(querystring, connection);
                cmd.Parameters.AddWithValue("@id", id);
                var dataAdapter = new SqlDataAdapter(cmd);
                _playerStats = new DataTable();
                dataAdapter.Fill(_playerStats);
                playerStatsDatagrid.DataContext = _playerStats;
                playerStatsDatagrid.ItemsSource = _playerStats.DefaultView;
            }

            playerStatsDatagrid.CanUserAddRows = false;
            playerStatsDatagrid.SelectionMode = DataGridSelectionMode.Single;
            playerStatsDatagrid.IsReadOnly = true;
            OnDataChanged();
        }
    }
}
