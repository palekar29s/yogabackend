using Microsoft.Data.SqlClient;

namespace yogabackend.Model
{
    public class Database
    {
        private readonly string _connectionString;


        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }
        public bool InsertLoginDetails(string username, string password)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = "INSERT INTO login (username, password) VALUES (@username, @password)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password); 

                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0; // returns true if insert was successful
                }
            }
        }
    }   
}
