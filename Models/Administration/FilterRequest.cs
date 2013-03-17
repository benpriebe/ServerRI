namespace Models.Administration
{
    public class FilterRequest
    {
        /// <summary>
        /// Get/Set the number of results to return.
        /// </summary>
        public int? Top { get; set; }

        /// <summary>
        /// Get/Set the number of results to skip before returning the Top value.
        /// </summary>
        public int? Skip { get; set; }
    }
}