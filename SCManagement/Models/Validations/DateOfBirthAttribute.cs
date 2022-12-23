using System.ComponentModel.DataAnnotations;
using SCManagement.Services;

namespace SCManagement.Models.Validations
{
    public class DateOfBirthAttribute : ValidationAttribute
    {
        public int MinAge { get; set; }
        public int MaxAge { get; set; }

        private string GetErrorMessage(ValidationContext validationContext)
        {
            SharedResourceService errorTranslation = validationContext.GetService(typeof(SharedResourceService)) as SharedResourceService;
            return string.Format(errorTranslation.Get("Error_DateOfBirth"), MinAge, MaxAge);
        }

        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var val = (DateTime)value;

            if (val.AddYears(MinAge) > DateTime.Now)
                return new ValidationResult(GetErrorMessage(validationContext));

            if (val.AddYears(MaxAge) < DateTime.Now)
                return new ValidationResult(GetErrorMessage(validationContext));

            return ValidationResult.Success;

        }
    }
}
