using System;
using System.Collections.Generic;
using System.Reflection;

namespace BombaJob.Utilities.Interfaces
{
    public interface IValidator
    {
        bool ShouldValidate(PropertyInfo property);
        IEnumerable<IError> Validate(object instance);
        IEnumerable<IError> Validate(object instance, string propertyName);
    }
}
