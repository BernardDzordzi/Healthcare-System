using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Unit test examples for the Healthcare System
/// Demonstrates testing generic repositories, services, and extensions
/// </summary>
public class HealthcareSystemTests
{
    // Repository Tests

    public static void TestRepositoryAdd()
    {
        var repo = new Repository<Patient>();
        var patient = new Patient(1, "Test Patient", 30, "Male");

        repo.Add(patient);

        assert(repo.Count() == 1, "Repository should contain 1 patient");
        assert(repo.GetById(1) != null, "GetById should find the patient");
    }

    public static void TestRepositoryDuplicateId()
    {
        var repo = new Repository<Patient>();
        var patient1 = new Patient(1, "Patient 1", 30, "Male");
        var patient2 = new Patient(1, "Patient 2", 25, "Female");

        repo.Add(patient1);
        try
        {
            repo.Add(patient2);
            assert(false, "Should throw InvalidOperationException for duplicate ID");
        }
        catch (InvalidOperationException)
        {
            assert(true, "Correctly threw exception for duplicate ID");
        }
    }

    public static void TestRepositoryUpdate()
    {
        var repo = new Repository<Patient>();
        var patient = new Patient(1, "Original Name", 30, "Male");
        repo.Add(patient);

        patient.Name = "Updated Name";
        bool updated = repo.Update(patient);

        assert(updated, "Update should return true");
        assert(repo.GetById(1)?.Name == "Updated Name", "Patient name should be updated");
    }

    public static void TestRepositoryGetWhere()
    {
        var repo = new PatientRepository();
        repo.Add(new Patient(1, "Alice", 30, "Female"));
        repo.Add(new Patient(2, "Bob", 45, "Male"));
        repo.Add(new Patient(3, "Carol", 28, "Female"));

        var females = repo.GetWhere(p => p.Gender == "Female");

        assert(females.Count == 2, "Should find 2 females");
    }

    public static void TestSpecializedRepositoryQueries()
    {
        var repo = new PatientRepository();
        repo.Add(new Patient(1, "Young Patient", 25, "Female"));
        repo.Add(new Patient(2, "Middle Patient", 45, "Male"));
        repo.Add(new Patient(3, "Old Patient", 65, "Female"));

        var ageRange = repo.GetByAgeRange(20, 50);
        assert(ageRange.Count == 2, "Should find 2 patients in age range 20-50");

        var females = repo.GetByGender("Female");
        assert(females.Count == 2, "Should find 2 female patients");
    }

    // Service Tests

    public static void TestHealthcareServiceAddPatient()
    {
        var service = new HealthcareService();
        var patient = new Patient(1, "Test Patient", 30, "Male");

        service.AddPatient(patient);
        var retrieved = service.GetPatient(1);

        assert(retrieved != null, "Patient should be retrievable");
        assert(retrieved.Name == "Test Patient", "Patient name should match");
    }

    public static void TestHealthcareServicePrescriptionIssuing()
    {
        var service = new HealthcareService();
        service.AddPatient(new Patient(1, "Patient", 30, "Male"));
        service.AddDoctor(new Doctor(101, "Dr. Smith", "Cardiology", "LIC-001"));

        var prescription = new Prescription(201, 1, 101, "Aspirin", "100mg", 1, 30);
        service.IssuePrescription(prescription);

        var prescriptions = service.GetPatientPrescriptions(1);
        assert(prescriptions.Count == 1, "Patient should have 1 prescription");
    }

    public static void TestHealthcareServiceInvalidPrescription()
    {
        var service = new HealthcareService();
        var prescription = new Prescription(201, 999, 999, "Aspirin", "100mg", 1, 30);

        try
        {
            service.IssuePrescription(prescription);
            assert(false, "Should throw InvalidOperationException for non-existent patient");
        }
        catch (InvalidOperationException)
        {
            assert(true, "Correctly threw exception for invalid patient ID");
        }
    }

    public static void TestHealthcareServiceAppointmentScheduling()
    {
        var service = new HealthcareService();
        service.AddPatient(new Patient(1, "Patient", 30, "Male"));
        service.AddDoctor(new Doctor(101, "Dr. Smith", "Cardiology", "LIC-001"));

        var appointment = new Appointment(301, 1, 101, DateTime.Now.AddDays(1), "Checkup");
        service.ScheduleAppointment(appointment);

        var appointments = service.GetPatientAppointments(1);
        assert(appointments.Count == 1, "Patient should have 1 appointment");
        assert(appointments[0].Status == AppointmentStatus.Scheduled, "Appointment should be scheduled");
    }

    public static void TestHealthcareServiceCompleteAppointment()
    {
        var service = new HealthcareService();
        service.AddPatient(new Patient(1, "Patient", 30, "Male"));
        service.AddDoctor(new Doctor(101, "Dr. Smith", "Cardiology", "LIC-001"));
        service.ScheduleAppointment(new Appointment(301, 1, 101, DateTime.Now.AddDays(1), "Checkup"));

        bool completed = service.CompleteAppointment(301, "Patient is healthy", "No issues found");

        assert(completed, "Appointment should be completed");
        var appointment = service.GetPatientAppointments(1)[0];
        assert(appointment.Status == AppointmentStatus.Completed, "Status should be completed");
        assert(appointment.Diagnosis == "Patient is healthy", "Diagnosis should be set");
    }

    // Generic Extensions Tests

    public static void TestGroupByKeyExtension()
    {
        var patients = new List<Patient>
        {
            new Patient(1, "Alice", 30, "Female"),
            new Patient(2, "Bob", 45, "Male"),
            new Patient(3, "Carol", 28, "Female")
        };

        var grouped = patients.GroupByKey(p => p.Gender);

        assert(grouped.Count == 2, "Should have 2 groups");
        assert(grouped["Female"].Count == 2, "Should have 2 females");
        assert(grouped["Male"].Count == 1, "Should have 1 male");
    }

    public static void TestDistinctByExtension()
    {
        var patients = new List<Patient>
        {
            new Patient(1, "Alice", 30, "Female"),
            new Patient(2, "Bob", 45, "Male"),
            new Patient(3, "Carol", 28, "Female"),
            new Patient(4, "Dave", 35, "Male")
        };

        var distinctGenders = patients.DistinctBy(p => p.Gender).ToList();

        assert(distinctGenders.Count == 2, "Should have 2 distinct genders");
    }

    public static void TestForEachInBatchesExtension()
    {
        var patients = new List<Patient>();
        for (int i = 1; i <= 5; i++)
        {
            patients.Add(new Patient(i, $"Patient {i}", 30, "Male"));
        }

        var batchSizes = new List<int>();
        patients.ForEachInBatches(2, batch => batchSizes.Add(batch.Count));

        assert(batchSizes.Count == 3, "Should have 3 batches");
        assert(batchSizes[0] == 2, "First batch should have 2");
        assert(batchSizes[2] == 1, "Last batch should have 1");
    }

    public static void TestSafeExecuteExtension()
    {
        var patient = new Patient(1, "Test", 30, "Male");
        var result = patient.SafeExecute(p => p.Age = 31);

        assert(result.IsSuccess, "Operation should succeed");
        assert(result.Entity.Age == 31, "Age should be updated");
    }

    public static void TestFindAndTransformExtension()
    {
        var patients = new List<Patient>
        {
            new Patient(1, "Alice", 30, "Female"),
            new Patient(2, "Bob", 45, "Male")
        };

        var result = patients.FindAndTransform(
            p => p.Id == 1,
            p => $"{p.Name} ({p.Age})"
        );

        assert(result == "Alice (30)", "Transform should produce correct string");
    }

    // Validator Tests

    public static void TestValidatorNotNull()
    {
        var patient = new Patient(1, "Test", 30, "Male");
        
        try
        {
            Validator<Patient>.ValidateNotNull(patient);
            assert(true, "Validation should pass for non-null patient");
        }
        catch
        {
            assert(false, "Should not throw for valid patient");
        }

        try
        {
            Patient? nullPatient = null;
            Validator<Patient>.ValidateNotNull(nullPatient!);
            assert(false, "Should throw for null patient");
        }
        catch (ArgumentNullException)
        {
            assert(true, "Correctly threw exception for null patient");
        }
    }

    public static void TestValidatorId()
    {
        var patient = new Patient(1, "Test", 30, "Male");
        
        try
        {
            Validator<Patient>.ValidateId(patient);
            assert(true, "Validation should pass for valid ID");
        }
        catch
        {
            assert(false, "Should not throw for valid ID");
        }
    }

    // Reporting Tests

    public static void TestMedicalRecordReporting()
    {
        var service = new HealthcareService();
        service.AddPatient(new Patient(1, "Patient", 30, "Male"));
        service.AddDoctor(new Doctor(101, "Dr. Smith", "Cardiology", "LIC-001"));
        
        service.IssuePrescription(new Prescription(201, 1, 101, "Aspirin", "100mg", 1, 30));
        service.ScheduleAppointment(new Appointment(301, 1, 101, DateTime.Now.AddDays(1), "Checkup"));

        var record = service.GetPatientMedicalRecord(1);

        assert(record.Patient.Id == 1, "Record should contain correct patient");
        assert(record.TotalVisits == 1, "Record should show 1 visit");
        assert(record.ActivePrescriptions == 1, "Record should show 1 active prescription");
    }

    public static void TestDoctorStatistics()
    {
        var service = new HealthcareService();
        service.AddPatient(new Patient(1, "Patient 1", 30, "Male"));
        service.AddPatient(new Patient(2, "Patient 2", 35, "Female"));
        service.AddDoctor(new Doctor(101, "Dr. Smith", "Cardiology", "LIC-001"));

        service.ScheduleAppointment(new Appointment(301, 1, 101, DateTime.Now.AddDays(1), "Checkup 1"));
        service.ScheduleAppointment(new Appointment(302, 2, 101, DateTime.Now.AddDays(2), "Checkup 2"));
        service.CompleteAppointment(301, "Healthy", "No issues");

        var stats = service.GetDoctorStatistics(101);

        assert(stats.TotalAppointments == 2, "Doctor should have 2 appointments");
        assert(stats.CompletedAppointments == 1, "Doctor should have 1 completed appointment");
        assert(stats.UpcomingAppointments == 1, "Doctor should have 1 upcoming appointment");
    }

    // Helper method
    private static void assert(bool condition, string message)
    {
        if (!condition)
            throw new AssertionException(message);
        Console.WriteLine($"✓ {message}");
    }
}

public class AssertionException : Exception
{
    public AssertionException(string message) : base(message) { }
}

/// <summary>
/// Test runner - uncomment to run all tests
/// </summary>
public class TestRunner
{
    public static void RunAllTests()
    {
        Console.WriteLine("========================================");
        Console.WriteLine("   Running Healthcare System Tests");
        Console.WriteLine("========================================\n");

        try
        {
            // Repository Tests
            Console.WriteLine("--- Repository Tests ---");
            HealthcareSystemTests.TestRepositoryAdd();
            HealthcareSystemTests.TestRepositoryDuplicateId();
            HealthcareSystemTests.TestRepositoryUpdate();
            HealthcareSystemTests.TestRepositoryGetWhere();
            HealthcareSystemTests.TestSpecializedRepositoryQueries();

            // Service Tests
            Console.WriteLine("\n--- Service Tests ---");
            HealthcareSystemTests.TestHealthcareServiceAddPatient();
            HealthcareSystemTests.TestHealthcareServicePrescriptionIssuing();
            HealthcareSystemTests.TestHealthcareServiceInvalidPrescription();
            HealthcareSystemTests.TestHealthcareServiceAppointmentScheduling();
            HealthcareSystemTests.TestHealthcareServiceCompleteAppointment();

            // Extension Tests
            Console.WriteLine("\n--- Generic Extensions Tests ---");
            HealthcareSystemTests.TestGroupByKeyExtension();
            HealthcareSystemTests.TestDistinctByExtension();
            HealthcareSystemTests.TestForEachInBatchesExtension();
            HealthcareSystemTests.TestSafeExecuteExtension();
            HealthcareSystemTests.TestFindAndTransformExtension();

            // Validator Tests
            Console.WriteLine("\n--- Validator Tests ---");
            HealthcareSystemTests.TestValidatorNotNull();
            HealthcareSystemTests.TestValidatorId();

            // Reporting Tests
            Console.WriteLine("\n--- Reporting Tests ---");
            HealthcareSystemTests.TestMedicalRecordReporting();
            HealthcareSystemTests.TestDoctorStatistics();

            Console.WriteLine("\n========================================");
            Console.WriteLine("   All Tests Passed! ✓");
            Console.WriteLine("========================================");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Test Failed: {ex.Message}");
            throw;
        }
    }
}
