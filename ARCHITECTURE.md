# Healthcare System - Architecture & Design Documentation

## System Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        User Interface Layer                      │
│                        (Program.cs / Main)                       │
└─────────────────────────────────────────────────────────────────┘
                                  ↓
┌─────────────────────────────────────────────────────────────────┐
│                     Business Logic Layer                         │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │          HealthcareService (Orchestrator)               │   │
│  │  - Patient Management                                   │   │
│  │  - Doctor Management                                    │   │
│  │  - Prescription Management                              │   │
│  │  - Appointment Management                               │   │
│  │  - Reporting & Analytics                                │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                                  ↓
┌─────────────────────────────────────────────────────────────────┐
│                      Data Access Layer                           │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ IRepository<T> Interface                                 │   │
│  │  - Generic contract for CRUD operations                  │   │
│  │  - Type-safe through constraints                         │   │
│  └──────────────────────────────────────────────────────────┘   │
│                                  ↓                               │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │   Repository<T> : IRepository<T>                         │   │
│  │  - Thread-safe implementation                            │   │
│  │  - Generic CRUD operations                               │   │
│  │  - Works with any IEntity                                │   │
│  └──────────────────────────────────────────────────────────┘   │
│           ↙              ↓              ↓              ↘         │
│  PatientRepository  DoctorRepository  PrescriptionRepository    │
│  - Age range queries   - Specialization  - Active prescriptions  │
│  - Gender filtering    - Active doctors  - Medication search     │
└─────────────────────────────────────────────────────────────────┘
                                  ↓
┌─────────────────────────────────────────────────────────────────┐
│                      Data Storage Layer                          │
│  ┌─────────────┐  ┌──────────────┐  ┌─────────────────────────┐ │
│  │List<Entity> │  │Dictionary    │  │   Specialized                 │
│  │ - Primary   │  │ Mappings     │  │   Collections          │
│  │   storage   │  │ - O(1) lookup│  │   - HashSet<T>         │
│  │ - Ordered   │  │              │  │   - Custom queries     │
│  └─────────────┘  └──────────────┘  └─────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                                  ↓
┌─────────────────────────────────────────────────────────────────┐
│                     Supporting Components                        │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │            Generic Extensions (Utilities)               │   │
│  │  - GroupByKey<T, TKey>()                                │   │
│  │  - DistinctBy<T, TKey>()                                │   │
│  │  - FindAndTransform<T, TResult>()                       │   │
│  │  - ForEachInBatches<T>()                                │   │
│  │  - SafeExecute<T>()                                     │   │
│  └──────────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │          Generic Validators<T> (Utilities)              │   │
│  │  - ValidateNotNull<T>()                                 │   │
│  │  - ValidateId<T>()                                      │   │
│  │  - ValidateList<T>()                                    │   │
│  │  - ValidateAll<T>()                                     │   │
│  └──────────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │              Entity Model (Domain Objects)              │   │
│  │  - Patient, Doctor, Prescription, Appointment           │   │
│  │  - All implement: IEntity                               │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

## Design Patterns Implemented

### 1. **Generic Repository Pattern** 🎯
**Purpose**: Abstract data access with type safety

```csharp
// ✓ Write once, use with any entity type
public class Repository<T> : IRepository<T> where T : IEntity
{
    // All CRUD operations are generic
}

// ✓ Automatically works with all entity types
var patientRepo = new Repository<Patient>();
var doctorRepo = new Repository<Doctor>();
var prescriptionRepo = new Repository<Prescription>();
```

**Benefits:**
- DRY (Don't Repeat Yourself)
- Type-safe at compile time
- Easy to maintain and test

---

### 2. **Service Layer Pattern** 🏛️
**Purpose**: Orchestrate business logic and coordinate repositories

```csharp
public class HealthcareService
{
    // Coordinates all repositories
    private readonly PatientRepository _patientRepo;
    private readonly DoctorRepository _doctorRepo;
    private readonly PrescriptionRepository _prescriptionRepo;
    
    // Maintains relationship mappings
    private readonly Dictionary<int, List<Prescription>> _patientPrescriptionsMap;
    
    // Provides high-level business operations
    public void IssuePrescription(Prescription prescription)
    {
        // Validates relationships
        // Updates mappings
        // Maintains data consistency
    }
}
```

**Benefits:**
- Centralizes business logic
- Encapsulates complex operations
- Easier to test

---

### 3. **Specialized Repository Pattern** 📦
**Purpose**: Add domain-specific queries to generic base

```csharp
// Base: Generic, works for any entity
public class Repository<T> : IRepository<T> where T : IEntity { }

// Specialized: Adds domain-specific methods
public class PatientRepository : Repository<Patient>
{
    public IReadOnlyList<Patient> GetByAgeRange(int minAge, int maxAge)
    public IReadOnlyList<Patient> GetByGender(string gender)
}

public class PrescriptionRepository : Repository<Prescription>
{
    public IReadOnlyList<Prescription> GetActivePrescriptions()
    public IReadOnlyList<Prescription> GetExpiredPrescriptions()
}
```

**Benefits:**
- Combines generic reusability with domain specificity
- Open/Closed principle (open for extension)
- Clean separation of concerns

---

### 4. **Generic Extensions Pattern** 🛠️
**Purpose**: Provide reusable utility methods for any type

```csharp
// Single implementation works with any collection
public static Dictionary<TKey, List<T>> GroupByKey<T, TKey>(
    this IEnumerable<T> source,
    Func<T, TKey> keySelector) where TKey : notnull
{
    // Single implementation serves all types
}

// Usage
var patientsByGender = patients.GroupByKey(p => p.Gender);
var doctorsBySpecialization = doctors.GroupByKey(d => d.Specialization);
var prescriptionsByMedication = prescriptions.GroupByKey(p => p.MedicationName);
```

**Benefits:**
- Code reuse across different types
- Consistent API
- Easy to discover and use

---

### 5. **Entity Model Pattern** 🏥
**Purpose**: Define domain objects with consistent structure

```csharp
// Common interface for all entities
public interface IEntity
{
    int Id { get; }
}

// Entities implement interface
public class Patient : IEntity
public class Doctor : IEntity
public class Prescription : IEntity
public class Appointment : IEntity

// Enables generic operations that require identity
public class Repository<T> where T : IEntity
{
    public T? GetById(int id)  // Works for any entity type
}
```

**Benefits:**
- Ensures all entities have consistent structure
- Enables generic ID-based operations
- Type safety through constraints

---

### 6. **Immutable Views Pattern** 🔒
**Purpose**: Prevent accidental modification of collections

```csharp
// Repository returns read-only views
public IReadOnlyList<T> GetAll()
{
    lock (_lockObject)
    {
        return _items.AsReadOnly();  // Immutable view
    }
}

// Caller cannot modify the collection
var patients = service.GetAllPatients();
// patients.Add(newPatient);  // ✗ Compile error: no Add method
```

**Benefits:**
- Enforces encapsulation
- Prevents accidental data corruption
- Better thread safety guarantees

---

## Generic Constraints and Their Purpose

### `where T : IEntity` 📋
Ensures type has an ID (necessary for repository operations)

```csharp
public class Repository<T> where T : IEntity
{
    public T? GetById(int id)  // Can rely on T having Id property
}
```

### `where TKey : notnull` 🔑
Ensures key type is valid for dictionary (can't be null)

```csharp
public static Dictionary<TKey, List<T>> GroupByKey<T, TKey>(
    this IEnumerable<T> source,
    Func<T, TKey> keySelector) where TKey : notnull
{
    var dictionary = new Dictionary<TKey, List<T>>();  // Valid key type
}
```

### `where TResult : class` 📦
Ensures result type is a reference type (can be null)

```csharp
public static TResult? FindAndTransform<T, TResult>(
    this IEnumerable<T> source,
    Func<T, bool> predicate,
    Func<T, TResult> transform) where TResult : class
{
    return item != null ? transform(item) : null;  // Can return null
}
```

---

## Collections Usage Summary

| Collection | Purpose | Time Complexity | Use Case |
|-----------|---------|-----------------|----------|
| `List<T>` | Ordered storage | O(n) search | Primary entity storage |
| `Dictionary<K, V>` | Key-value mapping | O(1) lookup | Patient→Prescriptions, Doctor→Appointments |
| `HashSet<T>` | Unique tracking | O(1) add/check | Distinct filtering |
| `IReadOnlyList<T>` | Immutable view | O(1) read | Public API returns |

---

## Thread Safety Features

### Lock-Based Synchronization
```csharp
private readonly object _lockObject = new object();

public void Add(T item)
{
    lock (_lockObject)  // Thread-safe access
    {
        if (_items.Any(x => x.Id == item.Id))
            throw new InvalidOperationException("Duplicate ID");
        _items.Add(item);
    }
}
```

**Benefits:**
- Prevents race conditions
- Ensures data consistency
- Safe for multi-threaded scenarios

---

## Scalability Considerations

### 1. **Dictionary Mappings for O(1) Lookup** ⚡
```csharp
// Instead of searching List<Prescription> for each patient query
var prescriptions = _prescriptionRepo.GetAll()
    .Where(p => p.PatientId == patientId)  // O(n)
    .ToList();

// Use pre-built Dictionary for O(1) lookup
_patientPrescriptionsMap[patientId]  // O(1)
```

### 2. **Lazy Loading / On-Demand Building**
```csharp
// Option: Build mapping on first use
private Dictionary<int, List<Prescription>>? _patientPrescriptionsMap;

private void EnsureMappingBuilt()
{
    if (_patientPrescriptionsMap == null)
    {
        _patientPrescriptionsMap = BuildMapping();
    }
}
```

### 3. **Batch Processing Support**
```csharp
// Process large datasets efficiently
patientList.ForEachInBatches(1000, batch =>
{
    ProcessBatch(batch);  // Handle 1000 at a time
    GarbageCollect();
});
```

### 4. **Extension Points for Caching**
```csharp
// Easy to add caching layer without changing interface
public class CachedRepository<T> : Repository<T> where T : IEntity
{
    private Dictionary<int, T> _cache = new();
    
    public override T? GetById(int id)
    {
        if (_cache.TryGetValue(id, out var cached))
            return cached;
        
        var result = base.GetById(id);
        if (result != null)
            _cache[id] = result;
        return result;
    }
}
```

---

## Type Safety Features

### Compile-Time Checks
```csharp
var patientRepo = new Repository<Patient>();      // ✓ Compiles
var doctorRepo = new Repository<Doctor>();        // ✓ Compiles
var stringRepo = new Repository<string>();        // ✗ Error: doesn't implement IEntity
```

### Constraint Validation
```csharp
public bool ValidateAll(IEnumerable<T> entities, Func<T, bool> predicate)
where T : IEntity
{
    // Can safely assume T has Id property
    foreach (var entity in entities)
    {
        if (entity.Id <= 0)  // Valid access
            return false;
    }
    return true;
}
```

---

## SOLID Principles Alignment

| Principle | Implementation |
|-----------|----------------|
| **S**ingle Responsibility | Each class has one purpose (Repository: CRUD, Service: Orchestration) |
| **O**pen/Closed | Open for extension (specialized repositories), closed for modification |
| **L**iskov Substitution | All entities implement IEntity interface consistently |
| **I**nterface Segregation | Small, focused interfaces (IEntity, IRepository) |
| **D**ependency Inversion | Service depends on IRepository abstraction, not concrete class |

---

## Extension Opportunities

### Add Caching Layer
```csharp
public class CachedHealthcareService : HealthcareService
{
    private Dictionary<int, Patient> _patientCache = new();
    
    public override Patient? GetPatient(int id)
    {
        if (!_patientCache.TryGetValue(id, out var patient))
        {
            patient = base.GetPatient(id);
            _patientCache[id] = patient;
        }
        return patient;
    }
}
```

### Add Event Notifications
```csharp
public class EventfulHealthcareService : HealthcareService
{
    public event Action<Patient>? PatientAdded;
    public event Action<Prescription>? PrescriptionIssued;
    
    public override void AddPatient(Patient patient)
    {
        base.AddPatient(patient);
        PatientAdded?.Invoke(patient);
    }
}
```

### Add Audit Logging
```csharp
public class AuditedHealthcareService : HealthcareService
{
    private List<AuditLog> _auditLogs = new();
    
    public override void IssuePrescription(Prescription prescription)
    {
        base.IssuePrescription(prescription);
        LogAudit($"Prescription {prescription.Id} issued");
    }
}
```

---

## Performance Characteristics

| Operation | Time | Space | Notes |
|-----------|------|-------|-------|
| Add Patient | O(1) | O(1) | Append to list |
| Get Patient by ID | O(n) | O(1) | Linear search in list |
| Get Prescriptions | O(1) | O(1) | Dictionary lookup |
| Filter by Age Range | O(n) | O(k) | Linear scan, k = results |
| Group by Gender | O(n) | O(n) | Building dictionary |
| Complete Appointment | O(n) | O(1) | Find then update |

**Optimization Idea**: Add ID-based dictionary to Repository for O(1) GetById

---

## Key Takeaways

1. **Generics minimize code duplication** - Repository<T> works for all entities
2. **Constraints ensure type safety** - where T : IEntity prevents runtime errors
3. **Collections offer flexibility** - Choose based on access patterns (List, Dict, Set)
4. **Service layer orchestrates complexity** - Single point for business logic
5. **Specialized repos add domain knowledge** - Inherit from generic base
6. **Immutable views protect data** - IReadOnlyList prevents accidental changes
7. **Patterns enable testing** - Mock repositories easily with interfaces
8. **Extensions promote reuse** - Generic methods work with any type

This architecture is **scalable**, **maintainable**, and **extensible** for enterprise healthcare applications!
