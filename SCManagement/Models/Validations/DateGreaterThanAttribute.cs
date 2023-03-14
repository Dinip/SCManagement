using System.ComponentModel.DataAnnotations;
using SCManagement.Services;

namespace SCManagement.Models.Validations
{
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        public string StartDate { get; set; }

        private string GetErrorMessage(ValidationContext validationContext)
        {
            SharedResourceService errorTranslation = validationContext.GetService(typeof(SharedResourceService)) as SharedResourceService;
            return string.Format(errorTranslation.Get("Error_DateGreaterThan"));
        }

        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var model = (Services.PlansService.Models.Plan)validationContext.ObjectInstance;
            DateTime startDate = model.StartDate;
            var val = (DateTime)value;

            if (val < startDate)
                return new ValidationResult(GetErrorMessage(validationContext));

            return ValidationResult.Success;
        }
    }
}
