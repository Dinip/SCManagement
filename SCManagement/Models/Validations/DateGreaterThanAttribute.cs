using System.ComponentModel.DataAnnotations;
using SCManagement.Services;

namespace SCManagement.Models.Validations
{
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        public string Model { get; set; }
        private string GetErrorMessage(ValidationContext validationContext)
        {
            SharedResourceService errorTranslation = validationContext.GetService(typeof(SharedResourceService)) as SharedResourceService;
            return string.Format(errorTranslation.Get("Error_DateGreaterThan"));
        }

        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            DateTime? startDate;
            if (Model != null)
            {
                var obj = (Services.PlansService.Models.Goal)validationContext.ObjectInstance;
                startDate = obj.StartDate;
            }
            else
            {
                var model = (Services.PlansService.Models.Plan)validationContext.ObjectInstance;
                startDate = model.StartDate;
            }
            
            var val = (DateTime)value;

            if (val.Date <= startDate.Value.Date)
                return new ValidationResult(GetErrorMessage(validationContext));

            return ValidationResult.Success;
        }
    }
}
