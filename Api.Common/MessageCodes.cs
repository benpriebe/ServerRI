namespace Api.Common
{
    /// <summary>
    /// Resevered Message Codes that your implementation cannot you use.
    /// They are negative values to help aid with this.
    /// </summary>
    public enum MessageCodes
    {
        ValidationError = -1,
        NotFound = -404,
        UnexpectedResponseCode = -401,
        UnexpectedApiRequestError = -402,
    }
}