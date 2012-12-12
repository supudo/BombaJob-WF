using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BombaJob.Utilities.Interfaces;

namespace BombaJob.Utilities.Misc
{
    public class DefaultError : IError
    {
        public object Instance { get; private set; }
        public string Key { get; private set; }
        public string Message { get; private set; }

        public DefaultError(object instance, string propertyName, string message)
        {
            Instance = instance;
            Key = propertyName;
            Message = message;
        }  
    }
}
