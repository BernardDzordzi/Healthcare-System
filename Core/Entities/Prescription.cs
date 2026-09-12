using System;

/// <summary>
/// Represents a prescription issued to a patient
/// </summary>
public class Prescription : IEntity
{
    public int Id { get; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string MedicationName { get; set; }
    public string Dosage { get; set; }
    public int Frequency { get; set; }
    public int DurationDays { get; set; }
    public DateTime DateIssued { get; set; }
    public DateTime? DateExpires { get; set; }
    public string Notes { get; set; }
    public PrescriptionStatus Status { get; set; }

    public Prescription(int id, int patientId, int doctorId, string medicationName, 
                       string dosage, int frequency, int durationDays)
    {
        Id = id;
        PatientId = patientId;
        DoctorId = doctorId;
        MedicationName = medicationName;
        Dosage = dosage;
        Frequency = frequency;
        DurationDays = durationDays;
        DateIssued = DateTime.Now;
        DateExpires = DateTime.Now.AddDays(durationDays);
        Status = PrescriptionStatus.Active;
        Notes = "";
    }

    public override string ToString()
    {
        return $"Prescription[ID: {Id}, PatientID: {PatientId}, Medication: {MedicationName}, " +
               $"Dosage: {Dosage}, Status: {Status}]";
    }

    public override bool Equals(object? obj)
    {
        if (obj is Prescription other)
            return Id == other.Id;
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

public enum PrescriptionStatus
{
    Active,
    Completed,
    Cancelled,
    Expired
}
