using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace ControlGastosApp.Web.Utils
{
    public static class DatabaseConnectionTester
    {
        public static async Task<(bool success, string message)> TestConnectionAsync(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Intentar ejecutar una consulta simple para verificar la conexión
                    using (var command = new SqlCommand("SELECT 1", connection))
                    {
                        await command.ExecuteScalarAsync();
                    }
                    
                    return (true, "Conexión exitosa a la base de datos");
                }
            }
            catch (SqlException ex)
            {
                return (false, $"Error de conexión SQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error general: {ex.Message}");
            }
        }

        public static (bool success, string message) TestConnection(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // Intentar ejecutar una consulta simple para verificar la conexión
                    using (var command = new SqlCommand("SELECT 1", connection))
                    {
                        command.ExecuteScalar();
                    }
                    
                    return (true, "Conexión exitosa a la base de datos");
                }
            }
            catch (SqlException ex)
            {
                return (false, $"Error de conexión SQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error general: {ex.Message}");
            }
        }
    }
} 