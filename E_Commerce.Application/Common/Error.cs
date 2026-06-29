using System.Text.Json.Serialization;

namespace E_Commerce.Application.Common
{
    public sealed record Error(string Code , string Description , ErrorType ErrorType = ErrorType.Failure)
    {
        public static Error Failure(string code = "General.Failure", string desctiption = "General Failure Has Occured")
            => new(code, desctiption, ErrorType.Failure);
        public static Error Validation(string code = "General.Validation", string desctiption = "General Validation Has Occured")
            => new(code, desctiption, ErrorType.Validation);

        public static Error NotFound(string code = "General.NotFound", string desctiption = "Resource Not Found")
            => new(code, desctiption, ErrorType.NotFound);
        public static Error Conflict(string code = "General.Conflict", string desctiption = "General Conflict Has Occured")
            => new(code, desctiption, ErrorType.Conflict);
        public static Error Unauthorized(string code = "General.Unauthorized", string desctiption = "Access Is Denied Due To Bad Authorization")
            => new(code, desctiption, ErrorType.Unauthorized);
        public static Error Forbidden(string code = "General.Forbidden", string desctiption = "This Operation Is Forbidden")
            => new(code, desctiption, ErrorType.Forbidden);
        public static Error InvalidCredentials(string code = "General.InvalidCredentials", string desctiption = "Provided Credentials Are Invalid")
            => new(code, desctiption, ErrorType.InvalidCredentials);
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ErrorType
    {
        Failure = 0 ,
        Validation = 1, 
        NotFound = 2 ,
        Conflict = 3 ,
        Unauthorized = 4 ,
        Forbidden = 5 ,
        InvalidCredentials = 6 
    }
}