using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CleanOrderAPI.Services
{
    public class EmailValidationService : IEmailValidationService
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Validates if a single email address has a valid format
        /// </summary>
        /// <param name="email">Email address to validate</param>
        /// <returns>True if email format is valid, false otherwise</returns>
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Use both regex and EmailAddressAttribute for comprehensive validation
                var emailAttribute = new EmailAddressAttribute();
                return emailAttribute.IsValid(email) && EmailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates all email properties in a payload object
        /// </summary>
        /// <typeparam name="T">Type of the payload object</typeparam>
        /// <param name="payload">Payload object containing email properties</param>
        /// <param name="emailPropertyNames">Names of properties that contain email addresses</param>
        /// <returns>True if all emails are valid, false if any email is invalid</returns>
        public bool ValidateEmailsInPayload<T>(T payload, params string[] emailPropertyNames)
        {
            if (payload == null)
                return false;

            var invalidEmails = GetInvalidEmails(payload, emailPropertyNames);
            return !invalidEmails.Any();
        }

        /// <summary>
        /// Gets a list of invalid email addresses from a payload object
        /// </summary>
        /// <typeparam name="T">Type of the payload object</typeparam>
        /// <param name="payload">Payload object containing email properties</param>
        /// <param name="emailPropertyNames">Names of properties that contain email addresses</param>
        /// <returns>List of invalid email addresses found in the payload</returns>
        public List<string> GetInvalidEmails<T>(T payload, params string[] emailPropertyNames)
        {
            var invalidEmails = new List<string>();

            if (payload == null)
                return invalidEmails;

            var type = typeof(T);

            foreach (var propertyName in emailPropertyNames)
            {
                var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                
                if (property == null)
                    continue;

                var value = property.GetValue(payload);
                
                if (value is string email)
                {
                    if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
                    {
                        invalidEmails.Add(email);
                    }
                }
                else if (value is IEnumerable<string> emails)
                {
                    foreach (var emailItem in emails)
                    {
                        if (!string.IsNullOrWhiteSpace(emailItem) && !IsValidEmail(emailItem))
                        {
                            invalidEmails.Add(emailItem);
                        }
                    }
                }
            }

            return invalidEmails;
        }
    }
}