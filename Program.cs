using System;
using System.Collections.Generic;

/// <summary>
/// Comprehensive demonstration of the Healthcare System
/// Shows usage of generics, collections, and the service layer
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("   Healthcare Management System Demo");
        Console.WriteLine("========================================\n");

        var healthcareService = new HealthcareService();

        // 1. Demonstrate Patient Management
        DemoPatientManagement(healthcareService);

        // 2. Demonstrate Doctor Management
        DemoDoctorManagement(healthcareService);

        // 3. Demonstrate Prescription Management
        DemoPrescriptionManagement(healthcareService);

        // 4. Demonstrate Appointment Management
        DemoAppointmentManagement(healthcareService);

        // 5. Demonstrate Generic Extensions
        DemoGenericExtensions(healthcareService);

        // 6. Demonstrate Medical Records and Statistics
        DemoReporting(healthcareService);

        Console.WriteLine("\n========================================");
        Console.WriteLine("   Demo Completed Successfully!");
        Console.WriteLine("========================================");
    }

    /// <summary>Demonstrates patient management using generic repository</summary>
    private static void DemoPatientManagement(HealthcareService service)
    {
        Console.WriteLine("\n--- PATIENT MANAGEMENT ---\n");

        // Create and register patients
        var patient1 = new Patient(1, "Alice Smith", 30, "Female", "alice@email.com", "555-0001");
        var patient2 = new Patient(2, "Bob Johnson", 45, "Male", "bob@email.com", "555-0002");
        var patient3 = new Patient(3, "Carol Davis", 28, "Female", "carol@email.com", "555-0003");
        var patient4 = new Patient(4, "David Miller", 55, "Male", "david@email.com", "555-0004");

        service.AddPatient(patient1);
        service.AddPatient(patient2);
        service.AddPatient(patient3);
        service.AddPatient(patient4);

        Console.WriteLine("✓ Registered 4 new patients\n");

        // Display all patients
        Console.WriteLine("All Registered Patients:");
        foreach (var patient in service.GetAllPatients())
        {
            Console.WriteLine($"  {patient}");
        }

        // Query patients by age range (demonstrating specialized repository)
        Console.WriteLine("\nPatients aged 25-35:");
        foreach (var patient in service.GetPatientsByAgeRange(25, 35))
        {
            Console.WriteLine($"  {patient}");
        }

        // Update patient information
        patient1.Email = "alice.smith@newmail.com";
        service.UpdatePatient(patient1);
        Console.WriteLine($"\n✓ Updated patient 1 email to: {patient1.Email}");
    }

    /// <summary>Demonstrates doctor management with specializations</summary>
    private static void DemoDoctorManagement(HealthcareService service)
    {
        Console.WriteLine("\n--- DOCTOR MANAGEMENT ---\n");

        // Create and register doctors
        var doc1 = new Doctor(101, "Dr. Sarah Wilson", "Cardiology", "LIC-001", "sarah.wilson@hospital.com", "555-1001");
        var doc2 = new Doctor(102, "Dr. Michael Brown", "Neurology", "LIC-002", "m.brown@hospital.com", "555-1002");
        var doc3 = new Doctor(103, "Dr. Emily Garcia", "Cardiology", "LIC-003", "emily.garcia@hospital.com", "555-1003");
        var doc4 = new Doctor(104, "Dr. James Lee", "Orthopedics", "LIC-004", "j.lee@hospital.com", "555-1004");

        service.AddDoctor(doc1);
        service.AddDoctor(doc2);
        service.AddDoctor(doc3);
        service.AddDoctor(doc4);

        Console.WriteLine("✓ Registered 4 new doctors\n");

        // Display all active doctors
        Console.WriteLine("All Active Doctors:");
        foreach (var doctor in service.GetActiveDoctors())
        {
            Console.WriteLine($"  {doctor}");
        }

        // Query doctors by specialization
        Console.WriteLine("\nCardiologists on Staff:");
        foreach (var doctor in service.GetDoctorsBySpecialization("Cardiology"))
        {
            Console.WriteLine($"  {doctor}");
        }
    }

    /// <summary>Demonstrates prescription management with status tracking</summary>
    private static void DemoPrescriptionManagement(HealthcareService service)
    {
        Console.WriteLine("\n--- PRESCRIPTION MANAGEMENT ---\n");

        // Issue prescriptions
        var rx1 = new Prescription(201, 1, 101, "Lisinopril", "10mg", 2, 30);
        rx1.Notes = "Once daily, with meals";

        var rx2 = new Prescription(202, 1, 101, "Aspirin", "100mg", 1, 30);
        rx2.Notes = "Daily for blood thinning";

        var rx3 = new Prescription(203, 2, 102, "Metformin", "500mg", 3, 90);
        rx3.Notes = "Three times daily with meals";

        var rx4 = new Prescription(204, 3, 103, "Atorvastatin", "20mg", 1, 60);
        rx4.Notes = "Once daily at bedtime";

        service.IssuePrescription(rx1);
        service.IssuePrescription(rx2);
        service.IssuePrescription(rx3);
        service.IssuePrescription(rx4);

        Console.WriteLine("✓ Issued 4 prescriptions\n");

        // View prescriptions for patient 1
        Console.WriteLine("Prescriptions for Patient 1 (Alice Smith):");
        foreach (var rx in service.GetPatientPrescriptions(1))
        {
            Console.WriteLine($"  {rx} - Notes: {rx.Notes}");
        }

        // Update prescription status
        service.UpdatePrescriptionStatus(201, PrescriptionStatus.Completed);
        Console.WriteLine("\n✓ Completed prescription 201");

        // Show active prescriptions for patient 1
        Console.WriteLine("\nActive Prescriptions for Patient 1:");
        foreach (var rx in service.GetActivePatientPrescriptions(1))
        {
            Console.WriteLine($"  {rx}");
        }
    }

    /// <summary>Demonstrates appointment scheduling and management</summary>
    private static void DemoAppointmentManagement(HealthcareService service)
    {
        Console.WriteLine("\n--- APPOINTMENT MANAGEMENT ---\n");

        // Schedule appointments
        var tomorrow = DateTime.Now.AddDays(1);
        var nextWeek = DateTime.Now.AddDays(7);

        var apt1 = new Appointment(301, 1, 101, tomorrow.AddHours(9), "Heart checkup");
        var apt2 = new Appointment(302, 2, 102, tomorrow.AddHours(10), "Neurological exam");
        var apt3 = new Appointment(303, 3, 101, nextWeek.AddHours(14), "Follow-up visit");

        service.ScheduleAppointment(apt1);
        service.ScheduleAppointment(apt2);
        service.ScheduleAppointment(apt3);

        Console.WriteLine("✓ Scheduled 3 appointments\n");

        // View upcoming appointments for patient 1
        Console.WriteLine("Upcoming Appointments for Patient 1:");
        foreach (var apt in service.GetUpcomingPatientAppointments(1))
        {
            Console.WriteLine($"  {apt} - Reason: {apt.Reason}");
        }

        // Complete an appointment
        service.CompleteAppointment(301, "Patient shows normal cardiac function", "No issues detected. Continue current medications.");
        Console.WriteLine("\n✓ Completed appointment 301");

        // View patient appointments (including completed)
        Console.WriteLine("\nAll Appointments for Patient 1:");
        foreach (var apt in service.GetPatientAppointments(1))
        {
            Console.WriteLine($"  {apt} - Status: {apt.Status}");
        }
    }

    /// <summary>Demonstrates generic extension methods</summary>
    private static void DemoGenericExtensions(HealthcareService service)
    {
        Console.WriteLine("\n--- GENERIC EXTENSIONS DEMONSTRATION ---\n");

        var patients = service.GetAllPatients();

        // Demonstrate DistinctBy (distinct genders)
        Console.WriteLine("Distinct Genders:");
        foreach (var gender in patients.DistinctBy(p => p.Gender).Select(p => p.Gender).Distinct())
        {
            Console.WriteLine($"  - {gender}");
        }

        // Demonstrate GroupByKey
        var patientsByGender = patients.GroupByKey(p => p.Gender);
        Console.WriteLine("\nPatients Grouped by Gender:");
        foreach (var group in patientsByGender)
        {
            Console.WriteLine($"  {group.Key}: {group.Value.Count} patients");
            foreach (var patient in group.Value)
            {
                Console.WriteLine($"    - {patient.Name}");
            }
        }

        // Demonstrate ForEachInBatches
        Console.WriteLine("\nProcessing patients in batches of 2:");
        patients.ForEachInBatches(2, batch =>
        {
            Console.WriteLine($"  Batch: {string.Join(", ", batch.Select(p => p.Name))}");
        });

        // Demonstrate SafeExecute
        Console.WriteLine("\nSafe Operation Execution:");
        var patient = service.GetPatient(1);
        if (patient != null)
        {
            var result = patient.SafeExecute(p => 
            {
                p.Age = 31;
                p.Email = "updated@email.com";
            });
            Console.WriteLine($"  Operation Result: {result}");
        }
    }

    /// <summary>Demonstrates reporting with medical records and statistics</summary>
    private static void DemoReporting(HealthcareService service)
    {
        Console.WriteLine("\n--- REPORTING & STATISTICS ---\n");

        // Patient Medical Record
        Console.WriteLine("Patient 1 - Medical Record:");
        var medicalRecord = service.GetPatientMedicalRecord(1);
        Console.WriteLine($"  Patient: {medicalRecord.Patient.Name}");
        Console.WriteLine($"  Total Visits: {medicalRecord.TotalVisits}");
        Console.WriteLine($"  Active Prescriptions: {medicalRecord.ActivePrescriptions}");
        Console.WriteLine($"  Appointments: {medicalRecord.Appointments.Count}");

        // Doctor Statistics
        Console.WriteLine("\nDoctor 1 - Statistics:");
        var doctorStats = service.GetDoctorStatistics(101);
        Console.WriteLine($"  Doctor: {doctorStats.Doctor.Name}");
        Console.WriteLine($"  Total Appointments: {doctorStats.TotalAppointments}");
        Console.WriteLine($"  Completed: {doctorStats.CompletedAppointments}");
        Console.WriteLine($"  Upcoming: {doctorStats.UpcomingAppointments}");
        Console.WriteLine($"  Cancelled: {doctorStats.CancelledAppointments}");

        // Summary statistics
        Console.WriteLine("\nSystem Summary:");
        Console.WriteLine($"  Total Patients: {service.GetAllPatients().Count}");
        Console.WriteLine($"  Total Doctors: {service.GetActiveDoctors().Count}");
    }
}
