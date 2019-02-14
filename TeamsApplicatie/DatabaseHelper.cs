using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace TeamsApplicatie
{
    class DatabaseHelper
    {
        public static async Task<SqlConnection> OpenDefaultConnectionAsync()
        {
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
