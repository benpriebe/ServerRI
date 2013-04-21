namespace Api.Common
{
    /// <summary>
    /// Resevered Message Codes that your implementation cannot you use.
    /// They are negative values to help aid with this.
    /// </summary>
    public enum MessageCodes
    {
        ValidationError = -1,
        Unauthorized = 401,
        NotFound = -404,
        UnexpectedResponseCode = -501,
        UnexpectedApiRequestError = -502,
    }
}