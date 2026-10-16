using System.Data;
using Microsoft.Data.SqlClient; // Adjust if using another provider
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    // Simple wrapper to obtain IDbConnection instances for Dapper queries.
    public sealed class DapperContext
    {
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                 ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
