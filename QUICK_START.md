# Healthcare System - Quick Start Guide

## Project Overview

A **production-ready healthcare management system** built with C# demonstrating:
- ✅ Generic Classes with Type Constraints
- ✅ Generic Methods with Multiple Type Parameters  
- ✅ Collections (List, Dictionary, HashSet, etc.)
- ✅ Repository Pattern with Specialization
- ✅ Service Layer Architecture
- ✅ Type-Safe Operations Throughout
- ✅ Thread-Safe Implementations
- ✅ SOLID Design Principles

---

## 🚀 Getting Started

### 1. Basic Setup (5 minutes)

```csharp
// Create the service
var service = new HealthcareService();

// Register a patient
var alice = new Patient(1, "Alice Smith", 30, "Female", "alice@email.com");
service.AddPatient(alice);

// Register a doctor
var doc = new Doctor(101, "Dr. Sarah Wilson", "Cardiology", "LIC-001");
service.AddDoctor(doc);

// Issue a prescription
var rx = new Prescription(201, 1, 101, "Lisinopril", "10mg", 2, 30);
rx.Notes = "Once daily, with meals";
service.IssuePrescription(rx);

// Schedule an appointment
var apt = new Appointment(301, 1, 101, DateTime.Now.AddDays(1), "Heart checkup");
service.ScheduleAppointment(apt);

Console.WriteLine("✓ Healthcare system initialized!");
```

---

## 📚 Common Tasks

### Patient Management

```csharp
// Add a patient
service.AddPatient(new Patient(2, "Bob Jones", 45, "Male"));

// Get a patient
var patient = service.GetPatient(1);
Console.WriteLine(patient?.Name);  // "Alice Smith"

// Get all patients
var allPatients = service.GetAllPatients();
foreach (var p in allPatients)
    Console.WriteLine(p.Name);

// Find patients by age range
var young = service.GetPatientsByAgeRange(25, 40);
Console.WriteLine($"Young patients: {young.Count}");

// Update patient
patient.Email = "newemail@example.com";
service.UpdatePatient(patient);
```

### Doctor Management

```csharp
// Add a doctor
service.AddDoctor(new Doctor(102, "Dr. Michael Brown", "Neurology", "LIC-002"));

// Get a doctor
var doctor = service.GetDoctor(101);
Console.WriteLine(doctor?.Specialization);  // "Cardiology"

// Find all active doctors
var activeDocs = service.GetActiveDoctors();
Console.WriteLine($"Active doctors: {activeDocs.Count}");

// Find doctors by specialization
var cardiologists = service.GetDoctorsBySpecialization("Cardiology");
foreach (var doc in cardiologists)
    Console.WriteLine($"  {doc.Name}");
```

### Prescription Management

```csharp
// Issue a prescription
var prescription = new Prescription(
    id: 202,
    patientId: 1,
    doctorId: 101,
    medicationName: "Aspirin",
    dosage: "100mg",
    frequency: 1,           // times per day
    durationDays: 30
);
prescription.Notes = "Daily for blood thinning";
service.IssuePrescription(prescription);

// Get patient's prescriptions
var rxList = service.GetPatientPrescriptions(1);
foreach (var rx in rxList)
    Console.WriteLine($"  {rx.MedicationName} - {rx.Status}");

// Get only active prescriptions
var activeRx = service.GetActivePatientPrescriptions(1);
Console.WriteLine($"Active: {activeRx.Count}");

// Update prescription status
service.UpdatePrescriptionStatus(201, PrescriptionStatus.Completed);
```

### Appointment Management

```csharp
// Schedule an appointment
var appointment = new Appointment(
    id: 302,
    patientId: 1,
    doctorId: 101,
    appointmentDate: DateTime.Now.AddDays(7),
    reason: "Follow-up checkup"
);
service.ScheduleAppointment(appointment);

// Get patient appointments
var aptList = service.GetPatientAppointments(1);
foreach (var apt in aptList)
    Console.WriteLine($"  {apt.AppointmentDate:g} - {apt.Status}");

// Get upcoming appointments
var upcoming = service.GetUpcomingPatientAppointments(1);
Console.WriteLine($"Upcoming: {upcoming.Count}");

// Get doctor appointments
var docApts = service.GetDoctorAppointments(101);
Console.WriteLine($"Dr's appointments: {docApts.Count}");

// Complete an appointment
service.CompleteAppointment(
    appointmentId: 301,
    diagnosis: "Patient shows normal cardiac function",
    notes: "No issues detected. Continue current medications."
);
```

---

## 🔧 Generic Extensions Usage

### GroupByKey - Group entities by any property

```csharp
var patients = service.GetAllPatients();

// Group by gender
var byGender = patients.GroupByKey(p => p.Gender);
foreach (var group in byGender)
{
    Console.WriteLine($"{group.Key}:");
    foreach (var p in group.Value)
        Console.WriteLine($"  - {p.Name}");
}
```

### DistinctBy - Get unique values by property

```csharp
var doctors = service.GetActiveDoctors();

// Get distinct specializations
var specializations = doctors.DistinctBy(d => d.Specialization).ToList();
foreach (var doc in specializations)
    Console.WriteLine(doc.Specialization);
```

### ForEachInBatches - Process collections in chunks

```csharp
var patients = service.GetAllPatients();

// Process in batches of 10
patients.ForEachInBatches(10, batch =>
{
    Console.WriteLine($"Processing batch of {batch.Count} patients...");
    // Send to database, API, etc.
});
```

### SafeExecute - Error-safe operation execution

```csharp
var patient = service.GetPatient(1);
if (patient != null)
{
    var result = patient.SafeExecute(p =>
    {
        p.Age = 31;
        p.Email = "updated@email.com";
    });
    
    if (result.IsSuccess)
        Console.WriteLine($"✓ Updated: {result.Entity.Name}");
    else
        Console.WriteLine($"✗ Error: {result.Message}");
}
```

### FindAndTransform - Find and convert elements

```csharp
var patients = service.GetAllPatients();

// Find specific patient and convert to string
var result = patients.FindAndTransform(
    predicate: p => p.Id == 1,
    transform: p => $"{p.Name} (Age: {p.Age})"
);
Console.WriteLine(result);  // "Alice Smith (Age: 30)"
```

---

## 📊 Reporting & Analytics

### Patient Medical Record

```csharp
var record = service.GetPatientMedicalRecord(1);

Console.WriteLine($"Patient: {record.Patient.Name}");
Console.WriteLine($"Total Visits: {record.TotalVisits}");
Console.WriteLine($"Active Prescriptions: {record.ActivePrescriptions}");

Console.WriteLine("Appointments:");
foreach (var apt in record.Appointments)
    Console.WriteLine($"  - {apt.AppointmentDate:d}: {apt.Reason}");

Console.WriteLine("Prescriptions:");
foreach (var rx in record.Prescriptions)
    Console.WriteLine($"  - {rx.MedicationName} ({rx.Status})");
```

### Doctor Statistics

```csharp
var stats = service.GetDoctorStatistics(101);

Console.WriteLine($"Doctor: {stats.Doctor.Name}");
Console.WriteLine($"Total Appointments: {stats.TotalAppointments}");
Console.WriteLine($"Completed: {stats.CompletedAppointments}");
Console.WriteLine($"Upcoming: {stats.UpcomingAppointments}");
Console.WriteLine($"Cancelled: {stats.CancelledAppointments}");
```

---

## ✅ Validation Examples

### Validate entities

```csharp
var patient = new Patient(1, "Test", 30, "Male");

// Validate not null
try
{
    Validator<Patient>.ValidateNotNull(patient);
    Console.WriteLine("✓ Patient is valid");
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"✗ {ex.Message}");
}

// Validate ID
Validator<Patient>.ValidateId(patient);  // Won't throw

// Validate all entities
var patients = service.GetAllPatients();
bool allValid = Validator<Patient>.ValidateAll(
    patients,
    p => p.Id > 0 && !string.IsNullOrEmpty(p.Name)
);
```

### Validate strings

```csharp
try
{
    StringValidator.ValidateNotEmpty("test@email.com", "Email");
    StringValidator.ValidateEmail("test@email.com");
    StringValidator.ValidateLength("Alice", 2, 50, "Name");
    Console.WriteLine("✓ All validations passed");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"✗ Validation error: {ex.Message}");
}
```

---

## 📁 File Structure

```
Healthcare-System/
├── Core/
│   ├── Entities/
│   │   ├── IEntity.cs              - Base entity interface
│   │   ├── Patient.cs
│   │   ├── Doctor.cs
│   │   ├── Prescription.cs
│   │   └── Appointment.cs
│   └── Repository/
│       ├── IRepository.cs          - Generic repository interface
│       ├── Repository.cs           - Generic implementation
│       └── SpecializedRepositories.cs  - Domain-specific repos
├── Services/
│   └── HealthcareService.cs        - Main business logic
├── Utilities/
│   ├── GenericExtensions.cs        - Extension methods
│   └── Validator.cs                - Validation helpers
├── Program.cs                      - Demo application
├── HealthcareSystemTests.cs        - Unit tests
├── README.md                       - Full documentation
└── ARCHITECTURE.md                 - Design patterns
```

---

## 🎯 Key Generic Concepts

### Generic Class with Constraint
```csharp
// Only works with entities that have ID
public class Repository<T> where T : IEntity
{
    public T? GetById(int id) { ... }  // Can use T.Id safely
}
```

### Generic Method with Multiple Types
```csharp
// Transform one type to another
public static TResult? FindAndTransform<T, TResult>(
    this IEnumerable<T> source,
    Func<T, bool> predicate,
    Func<T, TResult> transform)
where TResult : class
{
    ...
}
```

### Generic Dictionary Usage
```csharp
// Maps one type to list of another
private readonly Dictionary<int, List<Prescription>> 
    _patientPrescriptionsMap;

// O(1) lookup of all prescriptions for a patient
var prescriptions = _patientPrescriptionsMap[1];
```

---

## 🧪 Running Tests

```csharp
// Uncomment in Program.cs to run tests
TestRunner.RunAllTests();

// Or run individual tests
HealthcareSystemTests.TestRepositoryAdd();
HealthcareSystemTests.TestGroupByKeyExtension();
HealthcareSystemTests.TestMedicalRecordReporting();
```

---

## 💡 Advanced Usage

### Extend with custom repositories

```csharp
public class MedicalHistoryRepository : Repository<MedicalRecord>
{
    public IReadOnlyList<MedicalRecord> GetByDateRange(DateTime start, DateTime end)
    {
        return GetWhere(m => m.Date >= start && m.Date <= end);
    }
}
```

### Add new generic utilities

```csharp
public static IEnumerable<T> OrderByProperty<T, TProperty>(
    this IEnumerable<T> source,
    Func<T, TProperty> selector) where TProperty : IComparable
{
    return source.OrderBy(selector);
}
```

### Custom generic validators

```csharp
public static class DateValidator
{
    public static void ValidateDateRange(DateTime? start, DateTime? end)
    {
        if (start.HasValue && end.HasValue && start > end)
            throw new ArgumentException("Invalid date range");
    }
}
```

---

## 🔗 Relationships Between Entities

```
Patient (1)---∞---(Prescription)---(Doctor)
  |
  +--∞--> Appointment <---(Doctor)

Examples:
- Patient 1 can have multiple prescriptions
- Each prescription is issued by exactly one doctor
- Patient 1 can have multiple appointments
- Each appointment is with exactly one doctor
```

---

## 📋 Type Safety Benefits

✅ **Compile-Time Checking**
- Errors caught before runtime
- IDE autocomplete support
- Refactoring confidence

✅ **Self-Documenting Code**
```csharp
// Clear what types are involved
var result = patients.GroupByKey(p => p.Gender);
// Dictionary<string, List<Patient>> type is obvious
```

✅ **Runtime Performance**
- No boxing/unboxing
- No runtime type checks
- Optimal IL generation

---

## 🚨 Common Mistakes to Avoid

❌ **Violating Generic Constraints**
```csharp
var stringRepo = new Repository<string>();  // ✗ string doesn't implement IEntity
```

✅ **Correct Usage**
```csharp
var patientRepo = new Repository<Patient>();  // ✓ Patient implements IEntity
```

---

❌ **Assuming Lazy Evaluation**
```csharp
var result = patients.GroupByKey(p => p.Gender);  
// ✗ Result is computed immediately, not lazy
```

✅ **Use IEnumerable for lazy evaluation**
```csharp
var result = patients.Where(p => p.Age > 30);  // ✓ Lazy evaluation
```

---

## 📖 Further Learning

1. **Generic Constraints** - See Repository<T> implementation
2. **Extension Methods** - Check GenericExtensions.cs
3. **Service Pattern** - Study HealthcareService.cs
4. **SOLID Principles** - Review architecture in ARCHITECTURE.md
5. **Collections** - See dictionary usage in HealthcareService.cs

---

## ✨ Summary

This healthcare system demonstrates:

| Concept | Implementation |
|---------|----------------|
| Generics | Repository<T>, Validator<T> classes |
| Generic Methods | Extension methods with 2+ type parameters |
| Collections | List, Dictionary, HashSet usage |
| Constraints | where T : IEntity, where TKey : notnull |
| Patterns | Repository, Service Layer, Specialized Repos |
| Safety | Thread-safe locking, immutable views |
| Validation | Type-safe validators with constraints |

**Result**: A scalable, type-safe, maintainable healthcare system! 🏥
