using System;

/// <summary>
/// Represents a doctor in the healthcare system
/// </summary>
public class Doctor : IEntity
{
    public int Id { get; }
    public string Name { get; set; }
    public string Specialization { get; set; }
    public string LicenseNumber { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }

    public Doctor(int id, string name, string specialization, string licenseNumber, 
                  string email = "", string phoneNumber = "")
    {
        Id = id;
        Name = name;
        Specialization = specialization;
        LicenseNumber = licenseNumber;
        Email = email;
        PhoneNumber = phoneNumber;
        HireDate = DateTime.Now;
        IsActive = true;
    }

    public override string ToString()
    {
        return $"Doctor[ID: {Id}, Name: {Name}, Specialization: {Specialization}, License: {LicenseNumber}]";
    }

    public override bool Equals(object? obj)
    {
        if (obj is Doctor other)
            return Id == other.Id;
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
