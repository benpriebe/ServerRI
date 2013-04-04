namespace Core
{
    public interface IPropertyChangeTracker
    {
        bool IsModified(string propertyName);
    }
}