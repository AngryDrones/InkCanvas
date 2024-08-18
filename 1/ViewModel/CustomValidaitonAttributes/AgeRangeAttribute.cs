using System;
using System.ComponentModel.DataAnnotations;

namespace InkCanvas.ViewModel.CustomValidaitonAttributes
{
    public class AgeRangeAttribute : ValidationAttribute
    {
        private readonly int _minAge;
        private readonly int _maxAge;

        public AgeRangeAttribute(int minAge, int maxAge)
        {
            _minAge = minAge;
            _maxAge = maxAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && int.TryParse(value.ToString(), out int age))
            {
                if (age < _minAge)
                {
                    return new ValidationResult("You must be older than 16 to register.");
                }
                else if (age > _maxAge)
                {
                    return new ValidationResult("Yeah, I don't think so.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
