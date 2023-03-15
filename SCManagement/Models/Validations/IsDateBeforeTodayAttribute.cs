using System.ComponentModel.DataAnnotations;
using SCManagement.Services;

namespace SCManagement.Models.Validations
{
    public class IsDateBeforeTodayAttribute : ValidationAttribute
    {
        private string GetErrorMessage(ValidationContext validationContext)
        {
            SharedResourceService errorTranslation = validationContext.GetService(typeof(SharedResourceService)) as SharedResourceService;
            return string.Format(errorTranslation.Get("Error_IsDateBeforeToday"), validationContext.DisplayName);
        }

        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var val = (DateTime)value;

            if (val < DateTime.Now.Date)
                return new ValidationResult(GetErrorMessage(validationContext));

            return ValidationResult.Success;

        }
    }
}
