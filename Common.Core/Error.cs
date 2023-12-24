using Newtonsoft.Json;

namespace Common.Models
{
    /// <summary>
    /// </summary>
    public class Error : IException
    {
        public Error()
        {
        }

        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        [JsonProperty("code")] public string Code { get; set; }

        [JsonProperty("message")] public string Message { get; set; }

        public static Error Create(string code, string message)
        {
            return new Error(code, message);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            //types must be the exactly the same for non-sealed classes
            if (obj == null || obj.GetType() != GetType()) return false;

            return string.Equals(Code, ((Error)obj).Code);
        }

        public override string ToString()
        {
            return $"CODE: {Code}, MESSAGE: {Message}";
        }
    }
}