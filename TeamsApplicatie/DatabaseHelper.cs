using System.Configuration;
using System.Data.SqlClient;

namespace TeamsApplicatie
{
    class DatabaseHelper
    {
        public static SqlConnection OpenDefaultConnection()
        {
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
