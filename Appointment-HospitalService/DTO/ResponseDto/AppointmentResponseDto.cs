﻿namespace Appointment_HospitalService.DTO.ResponseDto
{
    public class AppointmentResponseDto
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public int PatientAge { get; set; }
        public string Issue { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool Status { get; set; }
    }
}
