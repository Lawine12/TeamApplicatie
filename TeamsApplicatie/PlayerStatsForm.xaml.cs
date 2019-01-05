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

        public PlayerStatsForm()
        {
            InitializeComponent();
            textboxPlayerFirstName.IsEnabled = false;
            textboxPlayerLastName.IsEnabled = false;
            textboxPlayerAdress.IsEnabled = false;
            textboxPlayerCity.IsEnabled = false;
            textboxPlayerAge.IsEnabled = false;
            textboxPlayerPosition.IsEnabled = false;
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
            var playerData = GetDataTable();
            if (playerData.Rows.Count == 1)
            {
                var row = _playerInformation.Rows[0];
                textboxPlayerFirstName.Text = (string)row["First Name"];
                textboxPlayerLastName.Text = (string)row["Last Name"];
                textboxPlayerAdress.Text = (string)row["Adress"];
                textboxPlayerCity.Text = (string)row["City"];
                textboxPlayerAge.Text = row["Age"].ToString();
                textboxPlayerPosition.Text = (string)row["Position"];
            }
        }

        private DataTable GetDataTable()
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
    }
}
