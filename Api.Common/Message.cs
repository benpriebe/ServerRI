namespace Api.Common
{
    public class Message
    {
        public Message()
        {
        }

        public Message(MessageLevel level, int code, string phrase, string detail)
        {
            Level = level;
            Code = code;
            Phrase = phrase;
            Detail = detail;
        }

        public MessageLevel Level { get; set; }
        public int Code { get; set; }
        public string Phrase { get; set; }
        public string Detail { get; set; }
    }
}