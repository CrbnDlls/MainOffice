using MainOffice.Models;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MainOffice.Functions
{
    public class OraDb
    {
        public static bool OpenConnection(OracleConnection DBConn, bool Silent)
        {

            try
            {
                if (DBConn.State == ConnectionState.Closed | DBConn.State == ConnectionState.Broken)
                {
                    DBConn.Open();
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static OracleConnection SetConnection(string UserName, string OraBase, string PassWord)
        {
            OracleConnection connection = new OracleConnection();
            OracleConnectionStringBuilder connString = new OracleConnectionStringBuilder();
            connString.DataSource = OraBase;
            connString.UserID = UserName;
            connString.Password = PassWord;
            connection.ConnectionString = connString.ConnectionString;
            return connection;
        }

        /// <summary>
        /// Возвращает OracleDataReader, при вводе OracleCommand; выводит окно с возможностью повторить попытку или отменить, при отмене возвращает null
        /// </summary>
        /// <param name="inputCmd">Команда Оракле для которой создать ридер</param>
        /// <returns>OracleDataReader</returns>
        public static OracleDataReader ExecuteReader(OracleCommand inputCmd, bool Silent)
        {
            OracleDataReader reader = null;

            try
            {
                if (OpenConnection(inputCmd.Connection, Silent))
                {
                    reader = inputCmd.ExecuteReader();
                }
            }
            catch (OracleException oe)
            {
                string test = oe.Message;
            }
            return reader;
        }



        /// <summary>
        /// Возвращает DataTable, с данными выборки косанды Oracle;
        /// </summary>
        /// <param name="cmd">OracleDataReader которому перевести строку</param>
        /// <returns>DataTable</returns>
        public static DataTable GetData(OracleCommand cmd, bool Silent)
        {

            OracleDataReader reader = ExecuteReader(cmd, Silent);
            if (reader != null)
            {
                DataTable result = new DataTable();
                try
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        result.Columns.Add();
                    }
                }
                catch
                {
                    return null;
                }

                try
                {
                    if (OpenConnection(cmd.Connection, Silent))
                    {

                        while (reader.Read())
                        {
                            DataRow row = result.NewRow();

                            for (int x = 0; x < reader.FieldCount; x++)
                            {
                                switch (reader.GetDataTypeName(x))
                                {
                                    case "VARCHAR2":
                                        try
                                        {
                                            row[x] = reader.GetString(x);
                                        }
                                        catch
                                        {

                                        }
                                        break;
                                    case "NUMBER":
                                        try
                                        {
                                            decimal? num = reader.GetDecimal(x);

                                            if (num.HasValue)
                                            {
                                                row[x] = num.Value;
                                            }
                                        }
                                        catch
                                        {

                                        }
                                        break;
                                    case "DATE":
                                        try
                                        {

                                            row[x] = reader.GetString(x);
                                        }
                                        catch
                                        {

                                        }
                                        break;
                                }
                            }
                            result.Rows.Add(row);
                        }
                        reader.Close();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Возвращает DataTable, с данными выборки косанды Oracle;
        /// </summary>
        /// <param name="cmd">OracleDataReader которому перевести строку</param>
        /// <returns>DataTable</returns>
        public static List<TemporaryBillModel> GetBills(OracleCommand cmd, bool Silent)
        {

            OracleDataReader reader = ExecuteReader(cmd, Silent);
            if (reader != null)
            {
                List<TemporaryBillModel> result = new List<TemporaryBillModel>();

                try
                {
                    if (OpenConnection(cmd.Connection, Silent))
                    {

                        while (reader.Read())
                        {
                            TemporaryBillModel bill = new TemporaryBillModel();
                            bill.id = Convert.ToInt32(reader["IDN"]);
                            bill.Salon = (string)reader["SNAME"];
                            string date = reader["DAT"].ToString();
                            date = date.Substring(0, date.IndexOf(" "));
                            bill.Date = date;
                            bill.BillNumber = Convert.ToInt32(reader["NUM"]);
                            bill.Worker = (string)reader["WORKER"];
                            date = reader["STIME"].ToString();
                            if (date.IndexOf(":") == 12)
                            {
                                date = date.Substring(date.IndexOf(" ") + 1, 4);
                            }
                            else
                            {
                                date = date.Substring(date.IndexOf(" ") + 1, 5);
                            }

                            bill.StartTime = date;
                            bill.SellPrice = Convert.ToDecimal(reader["SUM(D.SELLPRICE*D.QUANTITY)"]);
                            bill.Actual = Convert.ToInt32(reader["SUM(D.ACTUAL)"]);
                            int x = Convert.ToInt32(reader["SUM(D.BO)"]);
                            if (x > 0)
                            {
                                bill.BO = "Да";
                                bill.SellPrice = Convert.ToDecimal(reader["SUM(D.PRICE*D.QUANTITY)"]);
                            }
                            else
                            { bill.BO = "Нет"; }
                            result.Add(bill);

                        }
                        reader.Close();
                        reader.Dispose();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Возвращает DataTable, с данными выборки косанды Oracle;
        /// </summary>
        /// <param name="cmd">OracleDataReader которому перевести строку</param>
        /// <returns>DataTable</returns>
        public static List<TemporaryBillLine> GetBillLines(OracleCommand cmd, bool Silent, int idn)
        {


            OracleDataReader reader = ExecuteReader(cmd, Silent);
            if (reader != null)
            {
                List<TemporaryBillLine> result = new List<TemporaryBillLine>();

                try
                {
                    if (OpenConnection(cmd.Connection, Silent))
                    {

                        while (reader.Read())
                        {
                            TemporaryBillLine line = new TemporaryBillLine();
                            line.Bill = idn;
                            line.CashRegCode = Convert.ToInt32(reader["SCODE"]);
                            line.Discount = reader["DISC"].ToString();
                            line.Quantity = Convert.ToInt32(reader["QUANTITY"]);

                            line.ServiceProductName = (string)reader["SNAME"];

                            int x = Convert.ToInt32(reader["BO"]);
                            if (x > 0)
                            {
                                line.SellPrice = Convert.ToDecimal(reader["PRICE"]);
                            }
                            else
                            {
                                line.SellPrice = Convert.ToDecimal(reader["SELLPRICE"]);
                            }
                            line.Summ = line.Quantity * line.SellPrice;
                            result.Add(line);

                        }
                        reader.Close();
                        reader.Dispose();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        public static TemporaryBillModel GetBill(OracleCommand cmd, bool Silent)
        {

            OracleDataReader reader = ExecuteReader(cmd, Silent);
            if (reader != null)
            {
                TemporaryBillModel bill = new TemporaryBillModel();

                try
                {
                    if (OpenConnection(cmd.Connection, Silent))
                    {

                        while (reader.Read())
                        {

                            bill.id = Convert.ToInt32(reader["IDN"]);
                            bill.Salon = (string)reader["SNAME"];
                            string date = reader["DAT"].ToString();
                            date = date.Substring(0, date.IndexOf(" "));
                            bill.Date = date;
                            bill.BillNumber = Convert.ToInt32(reader["NUM"]);
                            bill.Worker = (string)reader["WORKER"];
                            date = reader["STIME"].ToString();
                            if (date.IndexOf(":") == 12)
                            {
                                date = date.Substring(date.IndexOf(" ") + 1, 4);
                            }
                            else
                            {
                                date = date.Substring(date.IndexOf(" ") + 1, 5);
                            }

                            bill.StartTime = date;
                            bill.SellPrice = Convert.ToDecimal(reader["SUM(D.SELLPRICE*D.QUANTITY)"]);
                            bill.Actual = Convert.ToInt32(reader["SUM(D.ACTUAL)"]);
                            int x = Convert.ToInt32(reader["SUM(D.BO)"]);
                            if (x > 0)
                            {
                                bill.BO = "Да";
                                bill.SellPrice = Convert.ToDecimal(reader["SUM(D.PRICE*D.QUANTITY)"]);
                            }
                            else
                            { bill.BO = "Нет"; }


                        }
                        reader.Close();
                        reader.Dispose();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
                return bill;
            }
            else
            {
                return null;
            }
        }
        public static List<AnySelectList> GetSalons(OracleCommand cmd, bool Silent)
        {
            OracleDataReader reader = ExecuteReader(cmd, Silent);
            if (reader != null)
            {
                List<AnySelectList> result = new List<AnySelectList>();

                try
                {
                    if (OpenConnection(cmd.Connection, Silent))
                    {

                        while (reader.Read())
                        {
                            AnySelectList list = new AnySelectList();
                            list.Id = reader["IDN"].ToString();
                            list.Name = (string)reader["SNAME"];

                            result.Add(list);

                        }
                        reader.Close();
                        reader.Dispose();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        public static List<AnySelectList> GetWorker(OracleCommand cmd, bool Silent)
        {
            OracleDataReader reader = ExecuteReader(cmd, Silent);
            if (reader != null)
            {
                List<AnySelectList> result = new List<AnySelectList>();

                try
                {
                    if (OpenConnection(cmd.Connection, Silent))
                    {

                        while (reader.Read())
                        {
                            AnySelectList list = new AnySelectList();
                            list.Id = reader[0].ToString();
                            list.Name = reader[0].ToString();

                            result.Add(list);

                        }
                        reader.Close();
                        reader.Dispose();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        public static List<string> GetServiceNames(OracleCommand cmd, bool Silent)
        {
            OracleDataReader reader = ExecuteReader(cmd, Silent);
            if (reader != null)
            {
                List<string> result = new List<string>();

                try
                {
                    if (OpenConnection(cmd.Connection, Silent))
                    {

                        while (reader.Read())
                        {
                            string list = reader[0].ToString();
                            result.Add(list);

                        }
                        reader.Close();
                        reader.Dispose();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        #region Report
        public static List<TemporaryBillReport> GetReport(OracleCommand cmd, bool Silent)
        {

            OracleDataReader reader = ExecuteReader(cmd, Silent);
            if (reader != null)
            {
                List<TemporaryBillReport> result = new List<TemporaryBillReport>();

                try
                {
                    if (OpenConnection(cmd.Connection, Silent))
                    {

                        while (reader.Read())
                        {
                            TemporaryBillReport line = new TemporaryBillReport();
                            line.id = Convert.ToInt32(reader["WORKERNUM"]);
                            line.Salon = (string)reader["SNAME"];
                            line.Worker = (string)reader["WORKER"]; ;
                            result.Add(line);

                        }
                        reader.Close();
                        reader.Dispose();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        public static List<TemporaryBillReport> GetReport1(OracleCommand cmd, bool Silent, List<TemporaryBillReport> result)
        {

            OracleDataReader reader = ExecuteReader(cmd, Silent);
            if (reader != null)
            {
                try
                {
                    if (OpenConnection(cmd.Connection, Silent))
                    {

                        while (reader.Read())
                        {
                            result.Single(x => x.id == Convert.ToInt32(reader["WORKERNUM"]) & x.Salon == reader["SNAME"].ToString() & x.Worker == reader["WORKER"].ToString()).Summ = Convert.ToDecimal(reader["SUM(c.SELLPRICE*c.QUANTITY)"]);
                        }
                        reader.Close();
                        reader.Dispose();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        public static List<TemporaryBillReport> GetReport2(OracleCommand cmd, bool Silent, List<TemporaryBillReport> result)
        {

            OracleDataReader reader = ExecuteReader(cmd, Silent);
            if (reader != null)
            {
                try
                {
                    if (OpenConnection(cmd.Connection, Silent))
                    {

                        while (reader.Read())
                        {
                            result.Single(x => x.id == Convert.ToInt32(reader["WORKERNUM"]) & x.Salon == reader["SNAME"].ToString() & x.Worker == reader["WORKER"].ToString()).SummDisc10 = Convert.ToDecimal(reader["SUM(c.SELLPRICE*c.QUANTITY)"]);
                        }
                        reader.Close();
                        reader.Dispose();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        public static List<TemporaryBillReport> GetReport3(OracleCommand cmd, bool Silent, List<TemporaryBillReport> result)
        {

            OracleDataReader reader = ExecuteReader(cmd, Silent);
            if (reader != null)
            {
                try
                {
                    if (OpenConnection(cmd.Connection, Silent))
                    {

                        while (reader.Read())
                        {
                            result.Single(x => x.id == Convert.ToInt32(reader["WORKERNUM"]) & x.Salon == reader["SNAME"].ToString() & x.Worker == reader["WORKER"].ToString()).SummDiscStaff = Convert.ToDecimal(reader["SUM(c.SELLPRICE*c.QUANTITY)"]);
                        }
                        reader.Close();
                        reader.Dispose();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        public static List<TemporaryBillReport> GetReport4(OracleCommand cmd, bool Silent, List<TemporaryBillReport> result)
        {

            OracleDataReader reader = ExecuteReader(cmd, Silent);
            if (reader != null)
            {
                try
                {
                    if (OpenConnection(cmd.Connection, Silent))
                    {

                        while (reader.Read())
                        {
                            result.Single(x => x.id == Convert.ToInt32(reader["WORKERNUM"]) & x.Salon == reader["SNAME"].ToString() & x.Worker == reader["WORKER"].ToString()).SummBO = Convert.ToDecimal(reader["SUM(c.PRICE*c.QUANTITY)"]);
                        }
                        reader.Close();
                        reader.Dispose();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        public static List<TemporaryBillReport> GetReport5(OracleCommand cmd, bool Silent, List<TemporaryBillReport> result)
        {

            OracleDataReader reader = ExecuteReader(cmd, Silent);
            if (reader != null)
            {
                try
                {
                    if (OpenConnection(cmd.Connection, Silent))
                    {

                        while (reader.Read())
                        {
                            result.Single(x => x.id == Convert.ToInt32(reader["WORKERNUM"]) & x.Salon == reader["SNAME"].ToString() & x.Worker == reader["WORKER"].ToString()).BillQuantity = Convert.ToInt32(reader[0]);
                        }
                        reader.Close();
                        reader.Dispose();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        public static List<TemporaryBillReport> GetReport6(OracleCommand cmd, bool Silent, List<TemporaryBillReport> result)
        {

            OracleDataReader reader = ExecuteReader(cmd, Silent);
            if (reader != null)
            {
                try
                {
                    if (OpenConnection(cmd.Connection, Silent))
                    {

                        while (reader.Read())
                        {
                            result.Single(x => x.id == Convert.ToInt32(reader["WORKERNUM"]) & x.Salon == reader["SNAME"].ToString() & x.Worker == reader["WORKER"].ToString()).BillErrorQuantity = Convert.ToInt32(reader[0]);
                        }
                        reader.Close();
                        reader.Dispose();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        #endregion
        #region SaveBill
        public static int SaveBill(TemporaryBillModel bill, OracleConnection conn)
        {
            #region cmdBill LONDA.SAVEBILLTOCHECK Комманда добавления заглавия счета
            OracleCommand cmdBill = new OracleCommand();
            cmdBill.CommandType = CommandType.StoredProcedure;
            cmdBill.CommandText = "LONDA.SAVEBILLTOCHECK";
            cmdBill.Connection = conn;
            OracleParameter par = cmdBill.CreateParameter();
            par.ParameterName = "IN_NUM";
            par.OracleDbType = OracleDbType.Int32;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBill.Parameters.Add(par);
            par = cmdBill.CreateParameter();
            par.ParameterName = "IN_DAT";
            par.OracleDbType = OracleDbType.Varchar2;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBill.Parameters.Add(par);
            par = cmdBill.CreateParameter();
            par.ParameterName = "IN_SALON";
            par.OracleDbType = OracleDbType.Int32;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBill.Parameters.Add(par);
            par = cmdBill.CreateParameter();
            par.ParameterName = "IN_WORKERNUM";
            par.OracleDbType = OracleDbType.Int32;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBill.Parameters.Add(par);
            par = cmdBill.CreateParameter();
            par.ParameterName = "IN_WORKER";
            par.OracleDbType = OracleDbType.Varchar2;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBill.Parameters.Add(par);
            par = cmdBill.CreateParameter();
            par.ParameterName = "IN_STIME";
            par.OracleDbType = OracleDbType.Varchar2;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBill.Parameters.Add(par);
            par = cmdBill.CreateParameter();
            par.ParameterName = "IN_FTIME";
            par.OracleDbType = OracleDbType.Varchar2;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBill.Parameters.Add(par);
            par = cmdBill.CreateParameter();

            par.ParameterName = "RESULT";
            par.OracleDbType = OracleDbType.NVarchar2;
            par.Size = 4000;
            par.Direction = ParameterDirection.Output;
            cmdBill.Parameters.Add(par);
            #endregion

            #region cmdBillData LONDA.SAVEBILLDATATOCHECK Комманда добавления строк счета
            OracleCommand cmdBillData = new OracleCommand();
            cmdBillData.CommandType = CommandType.StoredProcedure;
            cmdBillData.CommandText = "LONDA.SAVEBILLDATATOCHECK";
            cmdBillData.Connection = conn;
            par = cmdBillData.CreateParameter();
            par.ParameterName = "IN_BILL";
            par.OracleDbType = OracleDbType.Int32;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBillData.Parameters.Add(par);
            par = cmdBillData.CreateParameter();
            par.ParameterName = "IN_SCODE";
            par.OracleDbType = OracleDbType.Int32;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBillData.Parameters.Add(par);
            par = cmdBillData.CreateParameter();
            par.ParameterName = "IN_SNAME";
            par.OracleDbType = OracleDbType.Varchar2;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBillData.Parameters.Add(par);
            par = cmdBillData.CreateParameter();
            par.ParameterName = "IN_PRICE";
            par.OracleDbType = OracleDbType.Varchar2;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBillData.Parameters.Add(par);
            par = cmdBillData.CreateParameter();
            par.ParameterName = "IN_QUANTITY";
            par.OracleDbType = OracleDbType.Int32;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBillData.Parameters.Add(par);
            par = cmdBillData.CreateParameter();
            par.ParameterName = "IN_DISC";
            par.OracleDbType = OracleDbType.Varchar2;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBillData.Parameters.Add(par);
            par = cmdBillData.CreateParameter();
            par.ParameterName = "IN_SELLPRICE";
            par.OracleDbType = OracleDbType.Varchar2;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBillData.Parameters.Add(par);
            par = cmdBillData.CreateParameter();
            par.ParameterName = "IN_BO";
            par.OracleDbType = OracleDbType.Int32;
            par.Size = 0;
            par.Direction = ParameterDirection.Input;
            cmdBillData.Parameters.Add(par);

            par = cmdBillData.CreateParameter();
            par.ParameterName = "RESULT";
            par.OracleDbType = OracleDbType.NVarchar2;
            par.Size = 4000;
            par.Direction = ParameterDirection.Output;
            cmdBillData.Parameters.Add(par);
            #endregion

            int idn = -1;
            cmdBill.Parameters["IN_NUM"].Value = bill.BillNumber;
            cmdBill.Parameters["IN_DAT"].Value = bill.Date;
            cmdBill.Parameters["IN_SALON"].Value = int.Parse(bill.Salon);
            cmdBill.Parameters["IN_WORKERNUM"].Value = int.Parse(bill.Worker.Substring(bill.Worker.LastIndexOf(" ") + 1));
            cmdBill.Parameters["IN_WORKER"].Value = bill.Worker.Substring(0, bill.Worker.LastIndexOf(" "));
            cmdBill.Parameters["IN_STIME"].Value = bill.StartTime;
            cmdBill.Parameters["IN_FTIME"].Value = "xxx";
            if (ExecuteNonQuery(cmdBill, true))
            {
                idn = int.Parse(cmdBill.Parameters["RESULT"].Value.ToString());
            }


            if (idn != -1)
            {
                foreach (TemporaryBillLine line in bill.BillLines)
                {
                    cmdBillData.Parameters["IN_BILL"].Value = idn;
                    cmdBillData.Parameters["IN_SCODE"].Value = line.CashRegCode;
                    cmdBillData.Parameters["IN_SNAME"].Value = line.ServiceProductName;
                    cmdBillData.Parameters["IN_PRICE"].Value = line.SellPrice.ToString();
                    cmdBillData.Parameters["IN_QUANTITY"].Value = line.Quantity;
                    cmdBillData.Parameters["IN_DISC"].Value = line.Discount;
                    cmdBillData.Parameters["IN_SELLPRICE"].Value = line.SellPrice.ToString();
                    cmdBillData.Parameters["IN_BO"].Value = int.Parse(bill.BO);
                    ExecuteNonQuery(cmdBillData, true);
                }

            }
            return idn;
        }
        #endregion

        /// <summary>
        /// Возвращает true если команда исполнена; выводит окно с возможностью повторить попытку или отменить, при отмене возвращает false
        /// </summary>
        /// <param name="cmd">Команда Оракле которую необходимо выполнить</param>
        /// <returns>bool</returns>
        public static bool ExecuteNonQuery(OracleCommand cmd, bool Silent)
        {

            try
            {
                if (OpenConnection(cmd.Connection, Silent))
                {
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (OracleException ex)
            {
                string test = ex.Message;
                return false;
            }
        }
    }
}