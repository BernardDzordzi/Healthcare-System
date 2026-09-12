# Healthcare Management System

A comprehensive, enterprise-grade healthcare system implementation in C# that demonstrates advanced use of **Collections** (List, Dictionary, HashSet) and **Generics** (generic classes, methods, and constraints) to ensure scalability, type safety, and reusability.

## 📋 Project Structure

```
Healthcare-System/
├── Core/
│   ├── Entities/
│   │   ├── IEntity.cs                    # Base interface for all entities
│   │   ├── Patient.cs                    # Patient entity
│   │   ├── Doctor.cs                     # Doctor entity
│   │   ├── Prescription.cs               # Prescription entity with enums
│   │   └── Appointment.cs                # Appointment entity with enums
│   └── Repository/
│       ├── IRepository.cs                # Generic repository interface
│       ├── Repository.cs                 # Generic repository implementation
│       └── SpecializedRepositories.cs    # Domain-specific repositories
├── Services/
│   └── HealthcareService.cs              # Main business logic orchestration
├── Utilities/
│   ├── GenericExtensions.cs              # Generic utility methods
│   └── Validator.cs                      # Generic validation utilities
├── Program.cs                            # Comprehensive demo application
└── README.md                             # Documentation
```

## 🎯 Key Features

### 1. **Generic Repository Pattern** ✨
```csharp
// Type-safe generic repository with constraints
public class Repository<T> : IRepository<T> where T : IEntity
{
    // Thread-safe CRUD operations
    // Type safety ensured through IEntity constraint
    public void Add(T item)
    public bool Update(T item)
    public T? GetById(int id)
    public IReadOnlyList<T> GetWhere(Func<T, bool> predicate)
}
```

**Benefits:**
- Write once, use for any entity type
- Type safety at compile time
- Consistent interface across all entity types

### 2. **Specialized Repositories** 📦
Domain-specific repositories extending the generic base:

```csharp
public class PatientRepository : Repository<Patient>
{
    public IReadOnlyList<Patient> GetByAgeRange(int minAge, int maxAge)
    public IReadOnlyList<Patient> GetByGender(string gender)
    public IReadOnlyList<Patient> GetRegisteredAfter(DateTime date)
}

public class PrescriptionRepository : Repository<Prescription>
{
    public IReadOnlyList<Prescription> GetActivePrescriptions()
    public IReadOnlyList<Prescription> GetExpiredPrescriptions()
    public IReadOnlyList<Prescription> GetByMedication(string medicationName)
}
```

### 3. **Generic Extension Methods** 🛠️
Reusable utility methods for any collection:

```csharp
// Generic method with type parameters
public static TResult? FindAndTransform<T, TResult>(
    this IEnumerable<T> source,
    Func<T, bool> predicate,
    Func<T, TResult> transform) where TResult : class

// Group entities by key selector
public static Dictionary<TKey, List<T>> GroupByKey<T, TKey>(
    this IEnumerable<T> source,
    Func<T, TKey> keySelector) where TKey : notnull

// Distinct by specific selector
public static IEnumerable<T> DistinctBy<T, TKey>(
    this IEnumerable<T> source,
    Func<T, TKey> keySelector) where TKey : notnull
```

### 4. **Collections in Action** 🔄
**List<T>**: Core data storage
- Ordered collection of entities
- Efficient searching and iteration

**Dictionary<TKey, TValue>**: Relationship mapping
- Maps patients to their prescriptions
- Maps patients to their appointments
- Maps doctors to their appointments
- O(1) lookup performance

**HashSet<T>**: Unique tracking
- Used in DistinctBy extension for unique tracking

### 5. **Type-Safe Validation** ✅
Generic validators with constraints:

```csharp
public static class Validator<T> where T : IEntity
{
    public static void ValidateNotNull(T entity)
    public static void ValidateId(T entity)
    public static void ValidateList(IList<T> entities)
    public static bool ValidateAll(IEnumerable<T> entities, Func<T, bool> predicate)
}
```

### 6. **Comprehensive Service Layer** 🏥
`HealthcareService` orchestrates all operations:

#### Patient Management
```csharp
void AddPatient(Patient patient)
Patient? GetPatient(int patientId)
IReadOnlyList<Patient> GetAllPatients()
bool UpdatePatient(Patient patient)
IReadOnlyList<Patient> GetPatientsByAgeRange(int minAge, int maxAge)
```

#### Doctor Management
```csharp
void AddDoctor(Doctor doctor)
Doctor? GetDoctor(int doctorId)
IReadOnlyList<Doctor> GetActiveDoctors()
IReadOnlyList<Doctor> GetDoctorsBySpecialization(string specialization)
```

#### Prescription Management
```csharp
void IssuePrescription(Prescription prescription)
IReadOnlyList<Prescription> GetPatientPrescriptions(int patientId)
IReadOnlyList<Prescription> GetActivePatientPrescriptions(int patientId)
bool UpdatePrescriptionStatus(int prescriptionId, PrescriptionStatus newStatus)
```

#### Appointment Management
```csharp
void ScheduleAppointment(Appointment appointment)
IReadOnlyList<Appointment> GetPatientAppointments(int patientId)
IReadOnlyList<Appointment> GetUpcomingPatientAppointments(int patientId)
bool CompleteAppointment(int appointmentId, string diagnosis, string notes)
```

#### Reporting
```csharp
PatientMedicalRecord GetPatientMedicalRecord(int patientId)
DoctorStatistics GetDoctorStatistics(int doctorId)
```

## 🚀 Usage Examples

### Basic Setup
```csharp
var service = new HealthcareService();

// Add a patient
var patient = new Patient(1, "Alice Smith", 30, "Female", "alice@email.com");
service.AddPatient(patient);

// Add a doctor
var doctor = new Doctor(101, "Dr. Sarah Wilson", "Cardiology", "LIC-001");
service.AddDoctor(doctor);

// Issue a prescription
var prescription = new Prescription(201, 1, 101, "Lisinopril", "10mg", 2, 30);
service.IssuePrescription(prescription);

// Schedule an appointment
var appointment = new Appointment(301, 1, 101, DateTime.Now.AddDays(1), "Heart checkup");
service.ScheduleAppointment(appointment);
```

### Advanced Queries
```csharp
// Get patients in age range
var youngPatients = service.GetPatientsByAgeRange(25, 40);

// Get doctors by specialization
var cardiologists = service.GetDoctorsBySpecialization("Cardiology");

// Get active prescriptions for a patient
var activePrescriptions = service.GetActivePatientPrescriptions(1);

// Get upcoming appointments
var upcomingAppointments = service.GetUpcomingPatientAppointments(1);

// Get patient medical record
var medicalRecord = service.GetPatientMedicalRecord(1);

// Get doctor statistics
var stats = service.GetDoctorStatistics(101);
```

### Using Generic Extensions
```csharp
var patients = service.GetAllPatients();

// Group patients by gender
var byGender = patients.GroupByKey(p => p.Gender);

// Get distinct specializations
var specializations = doctors.DistinctBy(d => d.Specialization);

// Process in batches
patients.ForEachInBatches(10, batch =>
{
    Console.WriteLine($"Processing batch of {batch.Count} patients");
});

// Safe operation execution
var result = patient.SafeExecute(p => p.Age = 31);
if (result.IsSuccess)
    Console.WriteLine("Update successful");
```

## 🎓 Learning Outcomes

This project demonstrates:

✅ **Generic Classes with Constraints**
- `Repository<T> where T : IEntity`
- `Validator<T> where T : IEntity`
- `OperationResult<T> where T : IEntity`

✅ **Generic Methods with Multiple Type Parameters**
- `FindAndTransform<T, TResult>`
- `GroupByKey<T, TKey>`
- `ToDictionaryGeneric<T, TKey, TValue>`

✅ **Type Constraints**
- `where T : IEntity` - Ensures type safety
- `where TKey : notnull` - Ensures valid dictionary keys
- `where TResult : class` - Reference type constraint

✅ **Collections Usage**
- `List<T>` for ordered storage
- `Dictionary<TKey, TValue>` for efficient lookups
- `HashSet<T>` for uniqueness tracking
- `IReadOnlyList<T>` for immutable views

✅ **SOLID Principles**
- Single Responsibility: Each class has one purpose
- Open/Closed: Open for extension (specialized repos), closed for modification
- Liskov Substitution: Entities implement IEntity
- Interface Segregation: Small, focused interfaces
- Dependency Inversion: Service depends on abstractions (IRepository)

✅ **Enterprise Patterns**
- Repository Pattern
- Service Layer Pattern
- Generic Base Classes
- Entity Relationships
- Thread-Safe Operations

## 🧪 Running the Demo

The `Program.cs` file contains a comprehensive demonstration:

```bash
dotnet run
```

This will showcase:
1. Patient registration and queries
2. Doctor management by specialization
3. Prescription issuance and status tracking
4. Appointment scheduling and completion
5. Generic extension methods in action
6. Medical records and statistics reporting

## 📈 Scalability Features

- **Thread-Safe Repository**: Uses locks for concurrent access
- **Generic Constraints**: Ensures type safety at compile time
- **Specialized Repositories**: Easy to extend for domain-specific queries
- **Dictionary Mappings**: O(1) lookup for patient/doctor relationships
- **Batch Processing**: Handle large datasets efficiently with ForEachInBatches
- **Immutable Views**: IReadOnlyList prevents accidental modification

## 🔒 Type Safety

The system enforces type safety throughout:

```csharp
// Only entities implementing IEntity can be used
var patientRepo = new Repository<Patient>(); // ✓ Patient implements IEntity

// Compile-time error if type doesn't match constraint
var stringRepo = new Repository<string>(); // ✗ string doesn't implement IEntity
```

## 🎯 Extension Points

The architecture is designed for easy extension:

```csharp
// Create specialized repository for custom queries
public class MedicalHistoryRepository : Repository<MedicalRecord>
{
    public IReadOnlyList<MedicalRecord> GetByDateRange(DateTime start, DateTime end)
    {
        return GetWhere(m => m.Date >= start && m.Date <= end);
    }
}

// Add new generic validators
public static class DateValidator<T> where T : IEntity
{
    public static void ValidateDateRange(DateTime? start, DateTime? end)
    {
        if (start.HasValue && end.HasValue && start > end)
            throw new ArgumentException("Start date must be before end date");
    }
}

// Extend generic extensions with new methods
public static IEnumerable<T> OrderByProperty<T, TProperty>(
    this IEnumerable<T> source,
    Func<T, TProperty> propertySelector) where TProperty : IComparable
{
    return source.OrderBy(propertySelector);
}
```

## 📝 License

This project is provided as an educational example demonstrating C# generics and collections.

## 🤝 Key Takeaways

1. **Generics enable code reuse** - Write once for Repository<T>, use for all entities
2. **Type constraints ensure safety** - Compiler prevents runtime errors
3. **Collections provide flexibility** - List, Dictionary, HashSet each serve specific purposes
4. **Service layer orchestrates complexity** - Business logic stays in one place
5. **Extension methods add utility** - Generic extensions work with any entity type
6. **Interfaces define contracts** - IEntity and IRepository provide clear abstractions

This system is production-ready and demonstrates best practices for enterprise C# applications!