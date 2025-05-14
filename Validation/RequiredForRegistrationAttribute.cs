using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AgriEnergyConnect.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredForRegistrationAttribute : ValidationAttribute
    {
        public string ContextKey { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Try to get ViewData from the validation context's service provider
            var viewData = validationContext.GetService(typeof(ViewDataDictionary)) as ViewDataDictionary;

            // Check the context - only require password for "Register" context, not "Edit"
            if (viewData != null && (viewData[ContextKey]?.ToString() == "Register"))
            {
                if (string.IsNullOrWhiteSpace(value as string))
                {
                    return new ValidationResult("Password is required for registration.");
                }
            }

            return ValidationResult.Success;
        }
    }
}