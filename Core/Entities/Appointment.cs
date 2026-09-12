using System;

/// <summary>
/// Represents an appointment between a patient and doctor
/// </summary>
public class Appointment : IEntity
{
    public int Id { get; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime? ActualCheckInTime { get; set; }
    public DateTime? ActualCheckOutTime { get; set; }
    public string Reason { get; set; }
    public string Diagnosis { get; set; }
    public AppointmentStatus Status { get; set; }
    public string Notes { get; set; }

    public Appointment(int id, int patientId, int doctorId, DateTime appointmentDate, string reason = "")
    {
        Id = id;
        PatientId = patientId;
        DoctorId = doctorId;
        AppointmentDate = appointmentDate;
        Reason = reason;
        Status = AppointmentStatus.Scheduled;
        Notes = "";
        Diagnosis = "";
    }

    public override string ToString()
    {
        return $"Appointment[ID: {Id}, PatientID: {PatientId}, DoctorID: {DoctorId}, " +
               $"Date: {AppointmentDate:g}, Status: {Status}]";
    }

    public override bool Equals(object? obj)
    {
        if (obj is Appointment other)
            return Id == other.Id;
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

public enum AppointmentStatus
{
    Scheduled,
    CheckedIn,
    Completed,
    Cancelled,
    NoShow
}
