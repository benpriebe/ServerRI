namespace Api.Common
{
    public class Message
    {
        public Message()
        {
        }

        public Message(MessageLevel level, int code, string phrase, params string[] phraseTokens)
        {
            Level = level;
            Code = code;
            Phrase = phrase;
            Tokens = phraseTokens;
        }

        public MessageLevel Level { get; set; }
        public int Code { get; set; }
        public string Phrase { get; set; }
        public string[] Tokens { get; set; }
    }
}