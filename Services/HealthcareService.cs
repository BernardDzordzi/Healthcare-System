using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Main healthcare service orchestrating all operations
/// Manages relationships between patients, doctors, prescriptions, and appointments
/// </summary>
public class HealthcareService
{
    private readonly PatientRepository _patientRepo;
    private readonly DoctorRepository _doctorRepo;
    private readonly PrescriptionRepository _prescriptionRepo;
    private readonly AppointmentRepository _appointmentRepo;

    // Mapping dictionaries for efficient lookups
    private readonly Dictionary<int, List<Prescription>> _patientPrescriptionsMap;
    private readonly Dictionary<int, List<Appointment>> _patientAppointmentsMap;
    private readonly Dictionary<int, List<Appointment>> _doctorAppointmentsMap;

    public HealthcareService()
    {
        _patientRepo = new PatientRepository();
        _doctorRepo = new DoctorRepository();
        _prescriptionRepo = new PrescriptionRepository();
        _appointmentRepo = new AppointmentRepository();

        _patientPrescriptionsMap = new Dictionary<int, List<Prescription>>();
        _patientAppointmentsMap = new Dictionary<int, List<Appointment>>();
        _doctorAppointmentsMap = new Dictionary<int, List<Appointment>>();
    }

    #region Patient Operations

    /// <summary>Registers a new patient</summary>
    public void AddPatient(Patient patient)
    {
        if (patient == null)
            throw new ArgumentNullException(nameof(patient));

        _patientRepo.Add(patient);
    }

    /// <summary>Retrieves a patient by ID</summary>
    public Patient? GetPatient(int patientId)
    {
        return _patientRepo.GetById(patientId);
    }

    /// <summary>Gets all patients</summary>
    public IReadOnlyList<Patient> GetAllPatients()
    {
        return _patientRepo.GetAll();
    }

    /// <summary>Updates patient information</summary>
    public bool UpdatePatient(Patient patient)
    {
        if (patient == null)
            throw new ArgumentNullException(nameof(patient));

        return _patientRepo.Update(patient);
    }

    /// <summary>Gets patients by age range</summary>
    public IReadOnlyList<Patient> GetPatientsByAgeRange(int minAge, int maxAge)
    {
        return _patientRepo.GetByAgeRange(minAge, maxAge);
    }

    #endregion

    #region Doctor Operations

    /// <summary>Adds a new doctor</summary>
    public void AddDoctor(Doctor doctor)
    {
        if (doctor == null)
            throw new ArgumentNullException(nameof(doctor));

        _doctorRepo.Add(doctor);
    }

    /// <summary>Retrieves a doctor by ID</summary>
    public Doctor? GetDoctor(int doctorId)
    {
        return _doctorRepo.GetById(doctorId);
    }

    /// <summary>Gets all active doctors</summary>
    public IReadOnlyList<Doctor> GetActiveDoctors()
    {
        return _doctorRepo.GetActiveDoctors();
    }

    /// <summary>Gets doctors by specialization</summary>
    public IReadOnlyList<Doctor> GetDoctorsBySpecialization(string specialization)
    {
        return _doctorRepo.GetBySpecialization(specialization);
    }

    #endregion

    #region Prescription Operations

    /// <summary>Issues a new prescription</summary>
    public void IssuePrescription(Prescription prescription)
    {
        if (prescription == null)
            throw new ArgumentNullException(nameof(prescription));

        if (_patientRepo.GetById(prescription.PatientId) == null)
            throw new InvalidOperationException($"Patient with ID {prescription.PatientId} not found");

        if (_doctorRepo.GetById(prescription.DoctorId) == null)
            throw new InvalidOperationException($"Doctor with ID {prescription.DoctorId} not found");

        _prescriptionRepo.Add(prescription);

        // Update mapping
        if (!_patientPrescriptionsMap.ContainsKey(prescription.PatientId))
            _patientPrescriptionsMap[prescription.PatientId] = new List<Prescription>();

        _patientPrescriptionsMap[prescription.PatientId].Add(prescription);
    }

    /// <summary>Gets prescriptions for a specific patient</summary>
    public IReadOnlyList<Prescription> GetPatientPrescriptions(int patientId)
    {
        if (_patientPrescriptionsMap.ContainsKey(patientId))
            return _patientPrescriptionsMap[patientId].AsReadOnly();

        return new List<Prescription>().AsReadOnly();
    }

    /// <summary>Gets active prescriptions for a patient</summary>
    public IReadOnlyList<Prescription> GetActivePatientPrescriptions(int patientId)
    {
        var prescriptions = GetPatientPrescriptions(patientId);
        return prescriptions.Where(p => p.Status == PrescriptionStatus.Active).ToList().AsReadOnly();
    }

    /// <summary>Updates prescription status</summary>
    public bool UpdatePrescriptionStatus(int prescriptionId, PrescriptionStatus newStatus)
    {
        var prescription = _prescriptionRepo.GetById(prescriptionId);
        if (prescription == null)
            return false;

        prescription.Status = newStatus;
        return _prescriptionRepo.Update(prescription);
    }

    #endregion

    #region Appointment Operations

    /// <summary>Schedules a new appointment</summary>
    public void ScheduleAppointment(Appointment appointment)
    {
        if (appointment == null)
            throw new ArgumentNullException(nameof(appointment));

        if (_patientRepo.GetById(appointment.PatientId) == null)
            throw new InvalidOperationException($"Patient with ID {appointment.PatientId} not found");

        if (_doctorRepo.GetById(appointment.DoctorId) == null)
            throw new InvalidOperationException($"Doctor with ID {appointment.DoctorId} not found");

        if (appointment.AppointmentDate < DateTime.Now)
            throw new InvalidOperationException("Cannot schedule appointment in the past");

        _appointmentRepo.Add(appointment);

        // Update patient appointments mapping
        if (!_patientAppointmentsMap.ContainsKey(appointment.PatientId))
            _patientAppointmentsMap[appointment.PatientId] = new List<Appointment>();
        _patientAppointmentsMap[appointment.PatientId].Add(appointment);

        // Update doctor appointments mapping
        if (!_doctorAppointmentsMap.ContainsKey(appointment.DoctorId))
            _doctorAppointmentsMap[appointment.DoctorId] = new List<Appointment>();
        _doctorAppointmentsMap[appointment.DoctorId].Add(appointment);
    }

    /// <summary>Gets appointments for a patient</summary>
    public IReadOnlyList<Appointment> GetPatientAppointments(int patientId)
    {
        if (_patientAppointmentsMap.ContainsKey(patientId))
            return _patientAppointmentsMap[patientId].AsReadOnly();

        return new List<Appointment>().AsReadOnly();
    }

    /// <summary>Gets appointments for a doctor</summary>
    public IReadOnlyList<Appointment> GetDoctorAppointments(int doctorId)
    {
        if (_doctorAppointmentsMap.ContainsKey(doctorId))
            return _doctorAppointmentsMap[doctorId].AsReadOnly();

        return new List<Appointment>().AsReadOnly();
    }

    /// <summary>Gets upcoming appointments for a patient</summary>
    public IReadOnlyList<Appointment> GetUpcomingPatientAppointments(int patientId)
    {
        var appointments = GetPatientAppointments(patientId);
        return appointments
            .Where(a => a.AppointmentDate > DateTime.Now && a.Status == AppointmentStatus.Scheduled)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>Updates appointment status</summary>
    public bool UpdateAppointmentStatus(int appointmentId, AppointmentStatus newStatus)
    {
        var appointment = _appointmentRepo.GetById(appointmentId);
        if (appointment == null)
            return false;

        appointment.Status = newStatus;
        return _appointmentRepo.Update(appointment);
    }

    /// <summary>Completes an appointment with diagnosis notes</summary>
    public bool CompleteAppointment(int appointmentId, string diagnosis, string notes)
    {
        var appointment = _appointmentRepo.GetById(appointmentId);
        if (appointment == null)
            return false;

        appointment.Status = AppointmentStatus.Completed;
        appointment.Diagnosis = diagnosis;
        appointment.Notes = notes;
        appointment.ActualCheckOutTime = DateTime.Now;

        return _appointmentRepo.Update(appointment);
    }

    #endregion

    #region Reporting

    /// <summary>Gets a patient's medical summary (appointments and prescriptions)</summary>
    public PatientMedicalRecord GetPatientMedicalRecord(int patientId)
    {
        var patient = _patientRepo.GetById(patientId);
        if (patient == null)
            throw new InvalidOperationException($"Patient with ID {patientId} not found");

        var appointments = GetPatientAppointments(patientId);
        var prescriptions = GetPatientPrescriptions(patientId);

        return new PatientMedicalRecord
        {
            Patient = patient,
            Appointments = appointments,
            Prescriptions = prescriptions,
            TotalVisits = appointments.Count,
            ActivePrescriptions = prescriptions.Count(p => p.Status == PrescriptionStatus.Active)
        };
    }

    /// <summary>Gets statistics about a doctor</summary>
    public DoctorStatistics GetDoctorStatistics(int doctorId)
    {
        var doctor = _doctorRepo.GetById(doctorId);
        if (doctor == null)
            throw new InvalidOperationException($"Doctor with ID {doctorId} not found");

        var appointments = GetDoctorAppointments(doctorId);

        return new DoctorStatistics
        {
            Doctor = doctor,
            TotalAppointments = appointments.Count,
            CompletedAppointments = appointments.Count(a => a.Status == AppointmentStatus.Completed),
            UpcomingAppointments = appointments.Count(a => a.Status == AppointmentStatus.Scheduled && a.AppointmentDate > DateTime.Now),
            CancelledAppointments = appointments.Count(a => a.Status == AppointmentStatus.Cancelled)
        };
    }

    #endregion
}

/// <summary>Medical record for a patient</summary>
public class PatientMedicalRecord
{
    public Patient Patient { get; set; }
    public IReadOnlyList<Appointment> Appointments { get; set; }
    public IReadOnlyList<Prescription> Prescriptions { get; set; }
    public int TotalVisits { get; set; }
    public int ActivePrescriptions { get; set; }
}

/// <summary>Statistics for a doctor</summary>
public class DoctorStatistics
{
    public Doctor Doctor { get; set; }
    public int TotalAppointments { get; set; }
    public int CompletedAppointments { get; set; }
    public int UpcomingAppointments { get; set; }
    public int CancelledAppointments { get; set; }
}
