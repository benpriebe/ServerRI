namespace Api.Common
{
    /// <summary>
    /// Resevered Message Codes that your implementation cannot you use.
    /// They are negative values to help aid with this.
    /// </summary>
    public enum MessageCodes
    {
        NotFound = -404,
        ValidationError = -1
    }
}