using Microsoft.Data.SqlClient;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RE360.API.Common
{
    public class CommonDBHelper
    {

        static IConfiguration conf = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
        public static string dbConnection = conf["ConnectionStrings:ConnStr"].ToString();

        public static void ErrorLog(string ControllerName, string Error, string StackTrace)
        {
            try
            {
                SqlConnection con = new SqlConnection(dbConnection);
                var stList = StackTrace.ToString().Split('\\');
                var sterror = "";
                //for (int i = 0; i < stList.Length; i++)
                //{
                //    sterror += stList[i];
                //}
                string query = "insert into [ErrorLogs] (ControllerName,Error,StackTrace,Timest) values('" + ControllerName + "','" + Error.Replace("'", "''") + "','" + stList[0] + "',getdate())";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataSet GetDataSet(string query, Dictionary<string, object> Params)
        {
            try
            {
                DataSet dt = new DataSet();
                if (string.IsNullOrEmpty(query))
                    return dt;

                SqlConnection con = new SqlConnection(dbConnection);
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (Params != null)
                {
                    foreach (KeyValuePair<string, object> kvp in Params)
                        cmd.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value));
                }
                SqlDataReader reader = cmd.ExecuteReader();
                dt = ConvertDataReaderToDataSet(reader);
                con.Close();
                return dt;
            }
            catch (Exception ex)
            {
                CommonDBHelper.ErrorLog("CommonDBHelper - GetDataSet", ex.Message, ex.StackTrace);
                throw;
            }
        }
        public static DataSet ConvertDataReaderToDataSet(IDataReader data)
        {
            DataSet ds = new DataSet();
            int i = 0;
            while (!data.IsClosed)
            {
                ds.Tables.Add("Table" + (i + 1));
                ds.EnforceConstraints = false;
                ds.Tables[i].Load(data);
                i++;
            }
            return ds;
        }
    }
}
