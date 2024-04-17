using Appointment_HospitalService.DTO.RequestDto;
using Appointment_HospitalService.DTO.ResponseDto;
using Appointment_HospitalService.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace Appointment_HospitalService.Service.Interface
{
    public interface IAppointment
    {
        Task<AppointmentResponseDto> CreateAppointment(AppointmentRequestDto appointment, int PatientID, int DoctorID);
       // public Task<bool> DeleteAppointment(int appointmentId, int UserID);
        public Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsByDoctor(int doctorId);
        public Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsByPatient(int patientId);
        Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsById(int appointmentId);

    }
}
