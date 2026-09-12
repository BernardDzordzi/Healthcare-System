using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Specialized repository for Patient entities with domain-specific queries
/// Demonstrates extending generic repository for specific use cases
/// </summary>
public class PatientRepository : Repository<Patient>
{
    /// <summary>Gets patients by age range</summary>
    public IReadOnlyList<Patient> GetByAgeRange(int minAge, int maxAge)
    {
        return GetWhere(p => p.Age >= minAge && p.Age <= maxAge);
    }

    /// <summary>Gets patients by gender</summary>
    public IReadOnlyList<Patient> GetByGender(string gender)
    {
        return GetWhere(p => p.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Gets patients registered after a specific date</summary>
    public IReadOnlyList<Patient> GetRegisteredAfter(DateTime date)
    {
        return GetWhere(p => p.RegistrationDate > date);
    }
}

/// <summary>
/// Specialized repository for Prescription entities with domain-specific queries
/// </summary>
public class PrescriptionRepository : Repository<Prescription>
{
    /// <summary>Gets active prescriptions only</summary>
    public IReadOnlyList<Prescription> GetActivePrescriptions()
    {
        return GetWhere(p => p.Status == PrescriptionStatus.Active);
    }

    /// <summary>Gets expired prescriptions</summary>
    public IReadOnlyList<Prescription> GetExpiredPrescriptions()
    {
        return GetWhere(p => p.DateExpires.HasValue && p.DateExpires < DateTime.Now);
    }

    /// <summary>Gets prescriptions by medication name</summary>
    public IReadOnlyList<Prescription> GetByMedication(string medicationName)
    {
        return GetWhere(p => p.MedicationName.Contains(medicationName, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Specialized repository for Doctor entities with domain-specific queries
/// </summary>
public class DoctorRepository : Repository<Doctor>
{
    /// <summary>Gets active doctors only</summary>
    public IReadOnlyList<Doctor> GetActiveDoctors()
    {
        return GetWhere(d => d.IsActive);
    }

    /// <summary>Gets doctors by specialization</summary>
    public IReadOnlyList<Doctor> GetBySpecialization(string specialization)
    {
        return GetWhere(d => d.Specialization.Equals(specialization, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Specialized repository for Appointment entities with domain-specific queries
/// </summary>
public class AppointmentRepository : Repository<Appointment>
{
    /// <summary>Gets upcoming appointments</summary>
    public IReadOnlyList<Appointment> GetUpcomingAppointments()
    {
        return GetWhere(a => a.AppointmentDate > DateTime.Now && a.Status == AppointmentStatus.Scheduled);
    }

    /// <summary>Gets completed appointments</summary>
    public IReadOnlyList<Appointment> GetCompletedAppointments()
    {
        return GetWhere(a => a.Status == AppointmentStatus.Completed);
    }

    /// <summary>Gets appointments for a specific date</summary>
    public IReadOnlyList<Appointment> GetByDate(DateTime date)
    {
        return GetWhere(a => a.AppointmentDate.Date == date.Date);
    }
}
