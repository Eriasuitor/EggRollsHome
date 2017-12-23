using System;
using System.Collections.Generic;

namespace Newegg.MIS.API.Utilities.DataAccess
{
    public interface IGridReader : IDisposable
    {
        IEnumerable<T> Read<T>();
    }
}
