namespace Core
{
    public class OperationContext
    {
        public OperationContext(UserDetails userDetails)
        {
            UserDetails = userDetails;
        }

        public UserDetails UserDetails { get; private set; }
    }

    public class UserDetails
    {
        public UserDetails(string userId, string displayName, string logName)
        {
            UserId = userId;
            DisplayName = displayName;
            LogName = logName;
        }

        public string UserId { get; private set; }
        public string DisplayName { get; private set; }
        public string LogName { get; private set; }
    }
}
