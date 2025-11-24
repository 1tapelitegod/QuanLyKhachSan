using System;
using System.Data;
using System.Data.SqlClient;

namespace BTL1.DAL
{
    public class DataProvider
    {
        private static DataProvider instance;
        private string connectionString;

        public static DataProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (typeof(DataProvider))
                    {
                        if (instance == null)
                        {
                            instance = new DataProvider();
                        }
                    }
                }
                return instance;
            }
        }

        private DataProvider()
        {
            try
            {
                connectionString = System.Configuration.ConfigurationManager
                    .ConnectionStrings["BTL1"]?.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Connection string 'BTL1' not found!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DataProvider Init Error: {ex.Message}");
                throw;
            }
        }

        public bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            
            return dt;
        }

        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            int result = 0;
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                
                result = cmd.ExecuteNonQuery();
            }
            
            return result;
        }

        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            object result = null;
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                
                result = cmd.ExecuteScalar();
            }
            
            return result;
        }
    }
}