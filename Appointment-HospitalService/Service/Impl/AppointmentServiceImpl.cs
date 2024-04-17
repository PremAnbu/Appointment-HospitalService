using Appointment_HospitalService.DapperContext;
using Appointment_HospitalService.DTO.RequestDto;
using Appointment_HospitalService.DTO.ResponseDto;
using Appointment_HospitalService.Entity;
using Appointment_HospitalService.Service.Interface;
using Dapper;
using System.Net.Http;

namespace Appointment_HospitalService.Service.Impl
{
    public class AppointmentServiceImpl : IAppointment
    {
        private readonly AppointmentContext _context;
        private readonly IHttpClientFactory httpClientFactory;

        public AppointmentServiceImpl(AppointmentContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
           this.httpClientFactory =httpClientFactory;
        }

        public async Task<AppointmentResponseDto> CreateAppointment(AppointmentRequestDto appointment, int PatientID, int DoctorID)
        {
            try
            {
                string insertQuery = @"INSERT INTO Appointments (PatientName, PatientAge, Issue, DoctorName, Specialization,
                               AppointmentDate, Status, BookedWith, BookedBy)
                               VALUES (@PatientName, @PatientAge, @Issue, @DoctorName, @Specialization,
                               @AppointmentDate, @Status, @BookedWith, @BookedBy);
                               SELECT SCOPE_IDENTITY();";
                Appointment appointmentEntity= MapToEntity(appointment,getDoctorById(DoctorID),PatientID);
                /*DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("PatientName", appointmentEntity.PatientName);
                dynamicParameters.Add("PatientAge", appointmentEntity.PatientAge);
                dynamicParameters.Add("Issue", appointmentEntity.Issue);
                dynamicParameters.Add("DoctorName", appointmentEntity.DoctorName);
                dynamicParameters.Add("Specialization", appointmentEntity.Specialization);
                dynamicParameters.Add("Status", appointmentEntity.Status);
                dynamicParameters.Add("BookedWith", appointmentEntity.BookedWith);
                dynamicParameters.Add("BookedBy", appointmentEntity.BookedBy);
*/
                using (var connection = _context.CreateConnection())
                {
                    var appointmentId = await connection.ExecuteScalarAsync<int>(insertQuery,appointmentEntity);
                    // Query the newly added appointment from the database
                    string selectQuery = @"SELECT AppointmentId,PatientName,PatientAge, Issue,DoctorName,Specialization 
                                          ,AppointmentDate,Status FROM Appointments WHERE AppointmentID = @AppointmentId";
                    var addedAppointment = await connection.QueryFirstOrDefaultAsync<AppointmentResponseDto>(selectQuery, new { AppointmentId = appointmentId });
                    return addedAppointment;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding a new appointment.", ex);
            }
        }
        private Appointment MapToEntity(AppointmentRequestDto request, DoctorEntity userObject,int PatientId)
        {
            return new Appointment
            {
               PatientName= request.PatientName,PatientAge= request.PatientAge,
               Issue= request.Issue,DoctorName=userObject.DoctorName,
               Specialization=userObject.Specialization,AppointmentDate=DateTime.Now,
               Status=false,BookedWith=userObject.DoctorId,BookedBy=PatientId
            };
        }
        public DoctorEntity getDoctorById(int doctorId)
        {
            var httpclient = httpClientFactory.CreateClient("GetByDoctorId");
            var responce = httpclient.GetAsync($"GetDoctorById?doctorId={doctorId}").Result;
            if (responce.IsSuccessStatusCode)
            {
               // Console.ReadLine("1");
                return responce.Content.ReadFromJsonAsync<DoctorEntity>().Result;
            }
            throw new Exception("DoctorNotFound Create Appointment FIRST TO TRY DIFFERENT  DoctorID");
        }


        public async Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsByPatient(int patientId)
        {
            try
            {
                string selectQuery = @"SELECT * FROM Appointments WHERE BookedBy = @PatientId;";
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<AppointmentResponseDto>(selectQuery, new { PatientId = patientId });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving appointments by patient.", ex);
            }
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsByDoctor(int doctorId)
        {
            try
            {
                string selectQuery = @"SELECT * FROM Appointments WHERE BookedWith = @DoctorId;";
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<AppointmentResponseDto>(selectQuery, new { DoctorId = doctorId });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving appointments by doctor.", ex);
            }
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsById(int appointmentId)
        {
            try
            {
                string selectQuery = @"SELECT * FROM Appointments WHERE AppointmentId = @AppointmentId;";
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<AppointmentResponseDto>(selectQuery, new { AppointmentId = appointmentId });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving appointments by doctor.", ex);
            }
        }
    }

}
