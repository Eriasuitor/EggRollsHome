using System.Collections.Generic;
using System.Data;
using Newegg.Oversea.DataAccess;

namespace Newegg.MIS.API.Utilities.DataAccess
{
    public class DataCommandWrapper : IDataCommand
    {
        public DataCommand Command { get; private set; }

        public DataCommandWrapper()
        {
        }

        public DataCommandWrapper(string name)
        {
            Command = DataCommandManager.GetDataCommand(name);
        }

        public IDataCommand SetParameterValue(string parameterName, object value)
        {
            Command.SetParameterValue(parameterName, value);
            return this;
        }

        public object GetParameterValue(string parameterName)
        {
            return Command.GetParameterValue(parameterName);
        }

        public object ExecuteScalar()
        {
            return Command.ExecuteScalar();
        }

        public T ExecuteScalar<T>()
        {
            return Command.ExecuteScalar<T>();
        }

        public List<TEntity> ExecuteEntityList<TEntity>() where TEntity : class, new()
        {
            return Command.ExecuteEntityList<TEntity>();
        }

        public TEntity ExecuteEntity<TEntity>() where TEntity : class, new()
        {
            return Command.ExecuteEntity<TEntity>();
        }

        public IDataCommand ReplaceParameterValue(string sqlPlaceHoder, string valueString)
        {
            Command.ReplaceParameterValue(sqlPlaceHoder, valueString);
            return this;
        }

        public int ExecuteNonQuery()
        {
            return Command.ExecuteNonQuery();
        }

        public DataSet ExecuteDataSet()
        {
            return Command.ExecuteDataSet();
        }

        public IDataReader ExecuteDataReader()
        {
            return Command.ExecuteDataReader();
        }

        public void CloseConnection()
        {
        }

        public GridReader ExecuteMultiple(dynamic parameter)
        {
            return Command.ExecuteMultiple(parameter);
        }

        public IGridReader ExecuteMultiple()
        {
            var reader = Command.ExecuteMultiple(null);

            return new GridReaderWrapper(reader);
        }
    }
}