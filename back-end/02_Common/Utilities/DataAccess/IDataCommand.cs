using System.Collections.Generic;
using System.Data;
using Newegg.Oversea.DataAccess;

namespace Newegg.MIS.API.Utilities.DataAccess
{
    public interface IDataCommand
    {
        IDataCommand SetParameterValue(string parameterName, object value);

        object GetParameterValue(string parameterName);

        object ExecuteScalar();

        T ExecuteScalar<T>();

        List<TEntity> ExecuteEntityList<TEntity>() where TEntity : class , new();

        TEntity ExecuteEntity<TEntity>() where TEntity : class, new();

        IDataCommand ReplaceParameterValue(string sqlPlaceHoder, string valueString);

        int ExecuteNonQuery();

        DataSet ExecuteDataSet();

        IDataReader ExecuteDataReader();

        void CloseConnection();

        GridReader ExecuteMultiple(dynamic parameter);

        IGridReader ExecuteMultiple();
    }
}
