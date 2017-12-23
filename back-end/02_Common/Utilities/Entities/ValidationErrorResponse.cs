namespace Newegg.MIS.API.Utilities.Entities
{
    public class ValidationErrorResponse
    {
        public object AttemptedValue { get; set; }
        public object CustomState { get; set; }
        public string ErrorMessage { get; set; }
        public string PropertyName { get; set; }
    }
}
