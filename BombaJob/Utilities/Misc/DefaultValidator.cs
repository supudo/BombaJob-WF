using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

using BombaJob.Utilities.Interfaces;

namespace BombaJob.Utilities.Misc
{
    public class DefaultValidator : IValidator
    {
        public bool ShouldValidate(PropertyInfo property)
        {
            return property.GetCustomAttributes(true).OfType<ValidationAttribute>().Any();
        }

        public IEnumerable<IError> Validate(object instance)
        {
            return from property in instance.GetType().GetProperties()
                   from error in GetValidationErrors(instance, property)
                   select error;
        }

        public IEnumerable<IError> Validate(object instance, string propertyName)
        {
            PropertyInfo property = instance.GetType().GetProperty(propertyName);
            return GetValidationErrors(instance, property);
        }

        private IEnumerable<IError> GetValidationErrors(object instance, PropertyInfo property)
        {
            var context = new ValidationContext(instance, null, null);
            IEnumerable<DefaultError> validators =
                from attribute in property.GetCustomAttributes(true).OfType<ValidationAttribute>()
                where
                    attribute.GetValidationResult(property.GetValue(instance, null), context) !=
                    ValidationResult.Success
                select new DefaultError(
                    instance,
                    property.Name,
                    attribute.FormatErrorMessage(property.Name)
                    );

            return validators.OfType<IError>();
        }
    }
}
