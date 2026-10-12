using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Results
{
    /// <summary>
    /// Expected failures — a duplicate project number, a missing id — are
    /// return values, not exceptions. Keeps the controller mapping simple and
    /// avoids try/catch as flow control.
    /// </summary>
    public class Result<T>
    {
        public bool Succeeded { get; private set; }
        public T? Data { get; private set; }
        public string? Error { get; private set; }

        public static Result<T> Success(T data)
            => new() { Succeeded = true, Data = data };

        public static Result<T> Failure(string error)
            => new() { Succeeded = false, Error = error };
    }
}
