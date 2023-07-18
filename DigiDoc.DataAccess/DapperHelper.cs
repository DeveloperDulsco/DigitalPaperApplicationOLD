using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DigiDoc.DataAccess
{
    public class DapperHelper
    {
        
        public IEnumerable<T> ExecuteSP<T>(string sprocName, string connectionstring,object sprocParams = null)
        
        {
            IEnumerable<T> data = Activator.CreateInstance<List<T>>();
            
            using (var sc = new SqlConnection(connectionstring))
            {
                sc.Open();
                data = sc.Query<T>(sprocName, param: sprocParams, commandType: CommandType.StoredProcedure);
            }

            return data;
        }
        public IEnumerable<dynamic> ExecuteSP(string sprocName, string connectionstring, object sprocParams = null)
        {
            
            using (var sc = new SqlConnection(connectionstring))
            {
                sc.Open();
               
                var data = sc.Query(sprocName, param: sprocParams,commandTimeout:180,commandType: CommandType.StoredProcedure);
                return data;
            }

        }

        public DataTable ExecuteSPForDataSet(string sprocName, string connectionstring, object sprocParams = null)
        {
            using (var sc = new SqlConnection(connectionstring))
            {
                sc.Open();
                var list = SqlMapper.ExecuteReader(sc, sprocName, sprocParams, commandType: CommandType.StoredProcedure);
                var dataset = ConvertDataReaderToDataSet(list);
                return dataset;
            }
        }

        public DataTable ConvertDataReaderToDataSet(IDataReader data)
        {
           
            DataTable dt = new DataTable();
            while (!data.IsClosed)
            {
                dt.Load(data);
            }
            return dt;
        }

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}
