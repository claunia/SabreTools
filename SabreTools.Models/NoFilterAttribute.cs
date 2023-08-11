namespace SabreTools.Models
{
    /// <summary>
    /// Marks a property as required on write
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class NoFilterAttribute : System.Attribute { }
}