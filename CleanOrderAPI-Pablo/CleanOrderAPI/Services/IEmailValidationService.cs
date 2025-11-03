namespace CleanOrderAPI.Services
{
    public interface IEmailValidationService
    {
        bool IsValidEmail(string email);
        bool ValidateEmailsInPayload<T>(T payload, params string[] emailPropertyNames);
        List<string> GetInvalidEmails<T>(T payload, params string[] emailPropertyNames);
    }
}