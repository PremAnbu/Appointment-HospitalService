using Microsoft.Data.SqlClient;
using System.Data;

namespace Appointment_HospitalService.DapperContext
{
    public class AppointmentContext
    {

        private readonly IConfiguration _configuration;

        private readonly string _connectionString;

        public AppointmentContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("AppointmentConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);


    }
}
