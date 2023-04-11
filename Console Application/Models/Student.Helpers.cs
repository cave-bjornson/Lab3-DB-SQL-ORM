namespace ConsoleApplication.Models;

public partial class Student
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(PersonalIdentityNumber)}: {PersonalIdentityNumber}, {nameof(FirstName)}: {FirstName}, {nameof(LastName)}: {LastName}, {nameof(Class)}: {Class}";
    }
}
