using System;

namespace BombaJob.Utilities.Interfaces
{
    public interface IError
    {
        string Key { get; }
        string Message { get; }
    }
}
