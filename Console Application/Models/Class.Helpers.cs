namespace ConsoleApplication.Models;

public partial class Class
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(ClassName)}: {ClassName}";
    }
}