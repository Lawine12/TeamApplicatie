﻿using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace TeamsApplicatie
{
    /// <summary>
    /// Interaction logic for ResultsUpdateForm.xaml
    /// </summary>
    public partial class ResultsUpdateForm : Window
    {
        private DataTable _matchData;
        private DataSet _matchInfo;

        public ResultsUpdateForm()
        {
            InitializeComponent();
            LoadMatchData();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void resultDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void LoadMatchData()
        {
            resultDataGrid.CanUserAddRows = false;
            resultDataGrid.SelectionMode = DataGridSelectionMode.Single;
            resultDataGrid.IsReadOnly = true;

            string querystring = @"SELECT MatchInfo.Id,
            Team1.TeamName,
            Team2.TeamName,
            MatchInfo.MatchDate,
            MatchInfo.TotalGoalsTeam1,
            MatchInfo.TotalGoalsTeam2
                FROM dbo.MatchInfo
                INNER JOIN TeamData Team1 ON MatchInfo.Team1ID = Team1.Id
            INNER JOIN TeamData Team2 ON MatchInfo.Team2ID = Team2.Id";

            using (var connection = await DatabaseHelper.OpenDefaultConnectionAsync())
            {
                var cmd = new SqlCommand(querystring, connection);
                var dataAdapter = new SqlDataAdapter(cmd);
                _matchData = new DataTable();
                dataAdapter.Fill(_matchData);
                resultDataGrid.DataContext = _matchData;
                resultDataGrid.ItemsSource = _matchData.DefaultView;
            }
        }

    }
}