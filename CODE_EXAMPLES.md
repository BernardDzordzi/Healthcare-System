# Healthcare System - Code Examples & Reference

## Quick Reference Guide

### 🎯 Core Concepts at a Glance

#### 1. Generic Repository with Type Constraint
```csharp
// Define generic class that works with ANY entity type
public class Repository<T> : IRepository<T> where T : IEntity
{
    private List<T> _items = new List<T>();

    // Add any entity
    public void Add(T item) => _items.Add(item);

    // Get entity by ID (works because T : IEntity)
    public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

    // Generic query
    public IReadOnlyList<T> GetWhere(Func<T, bool> predicate) 
        => _items.Where(predicate).ToList().AsReadOnly();
}

// Usage - same code, different entities!
var patientRepo = new Repository<Patient>();      // Works!
var doctorRepo = new Repository<Doctor>();        // Works!
var prescriptionRepo = new Repository<Prescription>();  // Works!
var stringRepo = new Repository<string>();        // ✗ Compile error!
```

---

#### 2. Generic Method with Multiple Type Parameters
```csharp
// Transform one type to another using generics
public static TResult? FindAndTransform<T, TResult>(
    this IEnumerable<T> source,          // Input collection type
    Func<T, bool> predicate,             // How to find
    Func<T, TResult> transform)          // How to transform
where TResult : class                    // Result must be reference type
{
    var item = source.FirstOrDefault(predicate);
    return item != null ? transform(item) : null;
}

// Usage examples
var patients = service.GetAllPatients();

// Find patient 1 and convert to string
string? patientSummary = patients.FindAndTransform(
    p => p.Id == 1,
    p => $"{p.Name}, Age {p.Age}"
);
// Result: "Alice Smith, Age 30"

// Find doctor by ID and get specialization
string? specialization = doctors.FindAndTransform(
    d => d.Id == 101,
    d => d.Specialization
);
// Result: "Cardiology"
```

---

#### 3. Generic Method with Dictionary Building
```csharp
// Group any collection by any property
public static Dictionary<TKey, List<T>> GroupByKey<T, TKey>(
    this IEnumerable<T> source,
    Func<T, TKey> keySelector)
where TKey : notnull  // Key can't be null
{
    var dictionary = new Dictionary<TKey, List<T>>();
    foreach (var item in source)
    {
        var key = keySelector(item);
        if (!dictionary.ContainsKey(key))
            dictionary[key] = new List<T>();
        dictionary[key].Add(item);
    }
    return dictionary;
}

// Usage with Patients
var patients = service.GetAllPatients();
var byGender = patients.GroupByKey(p => p.Gender);
// Result: {"Female": [Patient1, Patient3], "Male": [Patient2, Patient4]}
foreach (var kvp in byGender)
    Console.WriteLine($"{kvp.Key}: {kvp.Value.Count} patients");

// Usage with Doctors
var doctors = service.GetActiveDoctors();
var bySpec = doctors.GroupByKey(d => d.Specialization);
// Result: {"Cardiology": [Doc1, Doc3], "Neurology": [Doc2], ...}
foreach (var kvp in bySpec)
    Console.WriteLine($"{kvp.Key}: {kvp.Value.Count} doctors");

// Same method, different entities - that's the power of generics!
```

---

#### 4. Generic Class with Type Constraint
```csharp
// Generic validator that works with any entity
public static class Validator<T> where T : IEntity
{
    public static void ValidateNotNull(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
    }

    public static void ValidateId(T entity)
    {
        // Can safely access entity.Id because T : IEntity
        if (entity.Id <= 0)
            throw new ArgumentException($"Invalid ID: {entity.Id}");
    }
}

// Usage - same validation code for different entities!
var patient = new Patient(1, "Alice", 30, "Female");
var doctor = new Doctor(101, "Dr. Smith", "Cardiology", "LIC-001");

Validator<Patient>.ValidateNotNull(patient);  // Works!
Validator<Doctor>.ValidateNotNull(doctor);    // Works!
Validator<Patient>.ValidateId(patient);       // Works!
Validator<Doctor>.ValidateId(doctor);         // Works!
```

---

### 📊 Collections Comparison

```csharp
// List<T> - Ordered, index-based access
var patients = service.GetAllPatients();
patients[0];              // Access by index O(1)
patients.Add(newPatient); // Add O(1)
patients.FirstOrDefault(p => p.Id == 1);  // Search O(n)

// Dictionary<K, V> - Key-value pairs, fast lookup
var prescriptionMap = new Dictionary<int, List<Prescription>>();
prescriptionMap[1];  // Get O(1) - FAST!
prescriptionMap[1].Add(rx);  // Add O(1)

// HashSet<T> - Unique values only
var uniqueGenders = new HashSet<string>();
uniqueGenders.Add("Male");
uniqueGenders.Add("Female");
uniqueGenders.Add("Male");  // Won't add duplicate
uniqueGenders.Count;  // Result: 2

// IReadOnlyList<T> - Read-only view (prevents modification)
public IReadOnlyList<Patient> GetAllPatients()
{
    return _patients.AsReadOnly();  // Return immutable view
}

var allPatients = service.GetAllPatients();
// allPatients.Add(newPatient);  // ✗ Compile error!
```

---

### 🏗️ Architecture Patterns

#### Repository Pattern
```csharp
// Generic base repository
public class Repository<T> : IRepository<T> where T : IEntity
{
    public void Add(T item) { }
    public T? GetById(int id) { }
    public IReadOnlyList<T> GetWhere(Func<T, bool> predicate) { }
}

// Specialized repository for specific entity
public class PatientRepository : Repository<Patient>
{
    // Add patient-specific queries
    public IReadOnlyList<Patient> GetByAgeRange(int min, int max)
        => GetWhere(p => p.Age >= min && p.Age <= max);
    
    public IReadOnlyList<Patient> GetByGender(string gender)
        => GetWhere(p => p.Gender == gender);
}

// Usage
var patientRepo = new PatientRepository();
patientRepo.Add(new Patient(1, "Alice", 30, "Female"));
patientRepo.GetByGender("Female");  // Domain-specific query
```

---

#### Service Layer Pattern
```csharp
// Service orchestrates repositories and business logic
public class HealthcareService
{
    private readonly PatientRepository _patientRepo;
    private readonly DoctorRepository _doctorRepo;
    private readonly PrescriptionRepository _prescriptionRepo;
    
    // Service manages relationships
    private readonly Dictionary<int, List<Prescription>> 
        _patientPrescriptionsMap;

    public void IssuePrescription(Prescription prescription)
    {
        // Validate relationships exist
        if (_patientRepo.GetById(prescription.PatientId) == null)
            throw new InvalidOperationException("Patient not found");
        
        if (_doctorRepo.GetById(prescription.DoctorId) == null)
            throw new InvalidOperationException("Doctor not found");

        // Add to repository
        _prescriptionRepo.Add(prescription);

        // Update mapping
        if (!_patientPrescriptionsMap.ContainsKey(prescription.PatientId))
            _patientPrescriptionsMap[prescription.PatientId] = new();
        
        _patientPrescriptionsMap[prescription.PatientId].Add(prescription);
    }

    // Service provides high-level operations
    public IReadOnlyList<Prescription> GetPatientPrescriptions(int patientId)
    {
        if (_patientPrescriptionsMap.ContainsKey(patientId))
            return _patientPrescriptionsMap[patientId].AsReadOnly();
        return new List<Prescription>().AsReadOnly();
    }
}

// Usage - Simple, clean API
var service = new HealthcareService();
service.IssuePrescription(newPrescription);  // Service handles complexity
```

---

### 💻 Real-World Examples

#### Example 1: Register Patient and Issue Prescription

```csharp
// Register patient
var patient = new Patient(
    id: 1,
    name: "Alice Smith",
    age: 30,
    gender: "Female",
    email: "alice@email.com"
);
service.AddPatient(patient);

// Add doctor
var doctor = new Doctor(
    id: 101,
    name: "Dr. Sarah Wilson",
    specialization: "Cardiology",
    licenseNumber: "LIC-001"
);
service.AddDoctor(doctor);

// Issue prescription
var prescription = new Prescription(
    id: 201,
    patientId: 1,
    doctorId: 101,
    medicationName: "Lisinopril",
    dosage: "10mg",
    frequency: 2,  // Twice daily
    durationDays: 30
);
prescription.Notes = "Once daily, with meals";
service.IssuePrescription(prescription);

// Verify
var patientRx = service.GetPatientPrescriptions(1);
Console.WriteLine($"Patient {patient.Name} has {patientRx.Count} prescriptions");
```

---

#### Example 2: Schedule and Complete Appointment

```csharp
// Schedule appointment
var appointment = new Appointment(
    id: 301,
    patientId: 1,
    doctorId: 101,
    appointmentDate: DateTime.Now.AddDays(7),
    reason: "Follow-up checkup"
);
service.ScheduleAppointment(appointment);

// Later: Complete appointment
service.CompleteAppointment(
    appointmentId: 301,
    diagnosis: "Patient shows normal cardiac function",
    notes: "No issues detected. Continue current medications."
);

// Verify
var apt = service.GetPatientAppointments(1)[0];
Console.WriteLine($"Status: {apt.Status}");       // Completed
Console.WriteLine($"Diagnosis: {apt.Diagnosis}");  // Normal function
```

---

#### Example 3: Query with Generic Extensions

```csharp
// Get all patients
var patients = service.GetAllPatients();

// Group by gender using generic extension
var byGender = patients.GroupByKey(p => p.Gender);
foreach (var group in byGender)
{
    Console.WriteLine($"{group.Key}:");
    foreach (var p in group.Value)
        Console.WriteLine($"  - {p.Name}");
}

// Get distinct genders
var genders = patients.DistinctBy(p => p.Gender).ToList();

// Process in batches
patients.ForEachInBatches(10, batch =>
{
    Console.WriteLine($"Processing batch of {batch.Count}");
    // Send to database, export, etc.
});

// Safe operation with error handling
var result = patient.SafeExecute(p => 
{
    p.Age = 31;
    p.Email = "newemail@example.com";
});

if (result.IsSuccess)
    Console.WriteLine("✓ Patient updated successfully");
```

---

#### Example 4: Reporting

```csharp
// Get patient medical record
var record = service.GetPatientMedicalRecord(1);

Console.WriteLine($"Patient: {record.Patient.Name}");
Console.WriteLine($"Total Visits: {record.TotalVisits}");
Console.WriteLine($"Active Prescriptions: {record.ActivePrescriptions}");

Console.WriteLine("\nAppointment History:");
foreach (var apt in record.Appointments)
{
    Console.WriteLine($"  {apt.AppointmentDate:d} - {apt.Reason} ({apt.Status})");
}

Console.WriteLine("\nCurrent Medications:");
foreach (var rx in record.Prescriptions.Where(p => p.Status == PrescriptionStatus.Active))
{
    Console.WriteLine($"  - {rx.MedicationName} ({rx.Dosage})");
}

// Get doctor statistics
var stats = service.GetDoctorStatistics(101);
Console.WriteLine($"\nDoctor: {stats.Doctor.Name}");
Console.WriteLine($"Total Appointments: {stats.TotalAppointments}");
Console.WriteLine($"Completed: {stats.CompletedAppointments}");
Console.WriteLine($"Upcoming: {stats.UpcomingAppointments}");
```

---

### 🧠 Type Constraints Explained

#### Constraint: `where T : IEntity`
```csharp
// ✓ Requires T to implement IEntity (have Id property)
public class Repository<T> where T : IEntity
{
    public T? GetById(int id)  // Can use id safely
    {
        return _items.FirstOrDefault(x => x.Id == id);
    }
}

// Valid uses:
var repo1 = new Repository<Patient>();       // ✓ Patient : IEntity
var repo2 = new Repository<Doctor>();        // ✓ Doctor : IEntity
var repo3 = new Repository<Prescription>();  // ✓ Prescription : IEntity

// Invalid use:
var repo4 = new Repository<string>();  // ✗ string doesn't implement IEntity
```

---

#### Constraint: `where TKey : notnull`
```csharp
// ✓ Requires key type to be non-nullable
public static Dictionary<TKey, List<T>> GroupByKey<T, TKey>(
    this IEnumerable<T> source,
    Func<T, TKey> keySelector)
where TKey : notnull  // Can't be null!
{
    var dict = new Dictionary<TKey, List<T>>();
    // TKey can't be null, so safe for dictionary keys
}

// Valid uses:
patients.GroupByKey(p => p.Gender);      // ✓ string is notnull
patients.GroupByKey(p => p.Age);         // ✓ int is notnull

// Invalid use (would compile error):
// patients.GroupByKey(p => p.OptionalField);  // ✗ if OptionalField is nullable
```

---

#### Constraint: `where TResult : class`
```csharp
// ✓ Requires result type to be a reference type
public static TResult? FindAndTransform<T, TResult>(
    this IEnumerable<T> source,
    Func<T, bool> predicate,
    Func<T, TResult> transform)
where TResult : class  // Must be reference type
{
    var item = source.FirstOrDefault(predicate);
    return item != null ? transform(item) : null;  // Can return null
}

// Valid uses:
patients.FindAndTransform(p => p.Id == 1, p => p.Name);    // ✓ string is class
patients.FindAndTransform(p => p.Id == 1, p => p);         // ✓ Patient is class

// Invalid use would need handling for value types:
// patients.FindAndTransform(p => p.Id == 1, p => p.Age);  // ✗ int is struct, not class
```

---

### 🔒 Thread Safety Example

```csharp
// Thread-safe repository using locks
public class Repository<T> : IRepository<T> where T : IEntity
{
    private readonly List<T> _items = new List<T>();
    private readonly object _lockObject = new object();

    // Lock protects concurrent access
    public void Add(T item)
    {
        lock (_lockObject)  // Only one thread at a time
        {
            if (_items.Any(x => x.Id == item.Id))
                throw new InvalidOperationException("Duplicate ID");
            _items.Add(item);
        }
    }

    public T? GetById(int id)
    {
        lock (_lockObject)
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }
    }

    // Multiple threads can read simultaneously
    public IReadOnlyList<T> GetAll()
    {
        lock (_lockObject)
        {
            return _items.AsReadOnly();
        }
    }
}

// Safe to use from multiple threads
var service = new HealthcareService();
Parallel.For(0, 100, i =>
{
    service.AddPatient(new Patient(i, $"Patient {i}", 30, "Male"));
});
// No race conditions!
```

---

## 🎓 Learning Path

1. **Start**: Understand generic classes (`Repository<T>`)
2. **Next**: Learn generic methods with single type parameter
3. **Then**: Graduate to multiple type parameters (`FindAndTransform<T, TResult>`)
4. **Finally**: Master type constraints (`where T : IEntity`)

---

## ✨ Key Takeaways

| Concept | Benefit |
|---------|---------|
| **Generic Classes** | Write once, use with any entity type |
| **Generic Methods** | Transform and query any collection |
| **Type Constraints** | Compile-time safety, no runtime type checking |
| **Collections** | Choose based on access patterns (List vs Dict) |
| **Service Layer** | Centralize business logic |
| **Repository Pattern** | Abstract data access, easy to test |

---

This healthcare system showcases professional-grade C# patterns you'll see in enterprise applications! 🏥
