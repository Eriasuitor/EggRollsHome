
using Newegg.MIS.API.Utilities.DataAccess;
using System;
using System.Threading;

namespace Newegg.MIS.API.EggRolls.DataAccess
{
    public interface IQuesDeleteDao
    {
        int DeleteQuestionnaire(int questionnaireID);
    }

    public class QuesItemDeleteByQidDao : IQuesDeleteDao
    {
        public static readonly Lazy<IQuesDeleteDao> LazyInstance =
            new Lazy<IQuesDeleteDao>(() => new QuesItemDeleteByQidDao(), LazyThreadSafetyMode.ExecutionAndPublication);

        private static IQuesDeleteDao _instance;

        public static IQuesDeleteDao Instance
        {
            get { return _instance ?? LazyInstance.Value; }
        }

        public static void SetInstance(IQuesDeleteDao instance)
        {
            _instance = instance;
        }

        public int DeleteQuestionnaire(int questionnaireID)
        {
            var commond = DataCommandFactory.Get("MIS_EggRolls_DeleteQuestionnaire")
                .SetParameterValue("@Questionnaireid", questionnaireID);
            commond.ExecuteNonQuery();


            //commond.ExecuteMultiple();
            ////对 .NET Framework 数据提供程序的 Connection 对象执行 SQL 语句，并返回受影响的行数。
            //commond.ExecuteNonQuery();

            //commond.ExecuteScalar();//执行查询，并返回由查询返回的结果集中第一行的第一列。 忽略其他列或行
            //commond.ExecuteScalar<>();
            //commond.ExecuteDataReader();
            //commond.ExecuteDataSet();//执行该命令并将结果返回到新的DataSet中。

            return 0;
        }
    }
}
