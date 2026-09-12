using System;

/// <summary>
/// Represents a patient in the healthcare system
/// </summary>
public class Patient : IEntity
{
    public int Id { get; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; }
    public DateTime RegistrationDate { get; set; }

    public Patient(int id, string name, int age, string gender, string email = "", string phoneNumber = "")
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
        Email = email;
        PhoneNumber = phoneNumber;
        RegistrationDate = DateTime.Now;
    }

    public override string ToString()
    {
        return $"Patient[ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}, Email: {Email}]";
    }

    public override bool Equals(object? obj)
    {
        if (obj is Patient other)
            return Id == other.Id;
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
