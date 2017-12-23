using System;
using System.Collections.Generic;
using Newegg.Oversea.DataAccess;

namespace Newegg.MIS.API.Utilities.DataAccess
{
    public class GridReaderWrapper : IGridReader
    {
        private GridReader _internalGridReader;

        public GridReaderWrapper(GridReader reader)
        {
            _internalGridReader = reader;
        }

        public IEnumerable<T> Read<T>()
        {
            if (null == _internalGridReader)
            {
                throw new InvalidOperationException(
                    "The internal grid reader not initialized.");
            }
            return _internalGridReader.Read<T>();
        }

        public void Dispose()
        {
            if (_internalGridReader == null)
            {
                return;
            }

            _internalGridReader.Dispose();
            _internalGridReader = null;
        }
    }
}
