using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AzureTest
{
    /// <summary>
    /// 数据库帮助类，默认SQLServer2005
    /// </summary>
    public class DbHelper
    {
        private SqlTransaction _transation;
        private string _connectionString;
        private DatabaseExeption _exception;
        private string _sql;
        private bool _debug;


        #region 构造函数
        private DbHelper() { }

        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
            if (string.IsNullOrEmpty(_connectionString))
            {
                _exception = new DatabaseExeption("Empty Connectionstring!");
                throw _exception;
            }
        }

        #endregion

        #region Private Method
        protected string ConnectionString()
        {
            return _connectionString;
        }

        protected SqlConnection CreateConnection()
        {
            SqlConnection conn;
            if (_transation != null && _transation.Connection != null)
            {
                conn = _transation.Connection;
            }
            else
            {
                conn = new SqlConnection();
                conn.ConnectionString = ConnectionString();
            }
            return conn;
        }

        protected SqlCommand CreateCommand(ref SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            if (_transation != null)
            {
                cmd.Transaction = _transation;
            }
            return cmd;
        }


        private int ExecuteNonQuery(ref SqlTransaction trans, CommandType type, string commandText, params SqlParameter[] parameters)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            try
            {
                if (trans == null || trans.Connection == null)
                {
                    con = CreateConnection();
                }
                else
                {
                    con = trans.Connection;
                    _transation = trans;
                }
                cmd = this.CreateCommand(ref con);
                cmd.CommandType = type;
                cmd.CommandText = commandText;

                if (parameters != null)
                {
                    foreach (SqlParameter p in parameters)
                    {
                        if (cmd.Parameters.Contains(p.ParameterName))
                        {
                            cmd.Parameters[p.ParameterName].Value = p.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add((SqlParameter)((ICloneable)p).Clone());
                        }
                    }
                }

                _sql = GetCommandSql(cmd);

                if (con.State != ConnectionState.Open)
                    con.Open();

                int objReturn = cmd.ExecuteNonQuery();

                return objReturn;
            }
            catch (Exception ex)
            {
                string sql = string.Empty;
                if (cmd != null)
                {
                    _sql = GetCommandSql(cmd);
                }
                _exception = new DatabaseExeption(ex.Message, ex, _sql);
                throw _exception;
            }
            finally
            {
                //如果存在事务，不释放连接
                if (_transation == null)
                {
                    if (con != null && con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }
                    con.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
        }

        private object ExecuteScalar(ref SqlTransaction trans, CommandType type, string commandText, params SqlParameter[] parameters)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            try
            {
                if (trans == null || trans.Connection == null)
                {
                    con = CreateConnection();
                }
                else
                {
                    con = trans.Connection;
                    _transation = trans;
                }
                cmd = this.CreateCommand(ref con);
                cmd.CommandType = type;
                cmd.CommandText = commandText;

                //添加参数
                if (parameters != null)
                {
                    foreach (SqlParameter p in parameters)
                    {
                        if (cmd.Parameters.Contains(p.ParameterName))
                        {
                            cmd.Parameters[p.ParameterName].Value = p.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add((SqlParameter)((ICloneable)p).Clone());
                        }
                    }
                }

                _sql = GetCommandSql(cmd);

                if (con.State != ConnectionState.Open)
                    con.Open();

                object objReturn = cmd.ExecuteScalar();

                return objReturn;
            }
            catch (Exception ex)
            {
                string sql = string.Empty;
                if (cmd != null)
                {
                    _sql = GetCommandSql(cmd);
                }
                _exception = new DatabaseExeption(ex.Message, ex, _sql);
                throw _exception;
            }
            finally
            {
                //如果存在事务，不释放连接
                if (_transation == null)
                {
                    if (con != null && con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }
                    con.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
        }

        private DataSet ExecuteDataSet(ref SqlTransaction trans, CommandType type, string commandText, params SqlParameter[] parameters)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            try
            {
                DateTime startTime = DateTime.Now;
                if (trans == null || trans.Connection == null)
                {
                    con = CreateConnection();
                }
                else
                {
                    con = trans.Connection;
                    _transation = trans;
                }
                cmd = this.CreateCommand(ref con);
                cmd.CommandType = type;
                cmd.CommandText = commandText;

                if (parameters != null)
                {
                    foreach (SqlParameter p in parameters)
                    {
                        if (cmd.Parameters.Contains(p.ParameterName))
                        {
                            cmd.Parameters[p.ParameterName].Value = p.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add((SqlParameter)((ICloneable)p).Clone());
                        }
                    }
                }

                _sql = GetCommandSql(cmd);
                if (con.State != ConnectionState.Open)
                    con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet("table");
                adapter.Fill(ds);

                return ds;
            }
            catch (Exception ex)
            {
                string sql = string.Empty;
                if (cmd != null)
                {
                    _sql = GetCommandSql(cmd);
                }
                _exception = new DatabaseExeption(ex.Message, ex, _sql);
                throw _exception;
            }
            finally
            {
                //如果存在事务，不释放连接
                if (_transation == null)
                {
                    if (con != null && con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }
                    con.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
        }


       private DataRow ExecuteDataRow(ref SqlTransaction trans, CommandType type, string commandText, params SqlParameter[] parameters)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            try
            {
                if (trans == null || trans.Connection == null)
                {
                    con = CreateConnection();
                }
                else
                {
                    con = trans.Connection;
                    _transation = trans;
                }
                cmd = this.CreateCommand(ref con);
                cmd.CommandType = type;
                cmd.CommandText = commandText;

                if (parameters != null)
                {
                    foreach (SqlParameter p in parameters)
                    {
                        if (cmd.Parameters.Contains(p.ParameterName))
                        {
                            cmd.Parameters[p.ParameterName].Value = p.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add((SqlParameter)((ICloneable)p).Clone());
                        }
                    }
                }

                _sql = GetCommandSql(cmd);

                if (con.State != ConnectionState.Open)
                    con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet("table");
                adapter.Fill(ds);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                string sql = string.Empty;
                if (cmd != null)
                {
                    _sql = GetCommandSql(cmd);
                }
                _exception = new DatabaseExeption(ex.Message, ex, _sql);
                throw _exception;
            }
            finally
            {
                if (_transation == null)
                {
                    if (con != null && con.State != ConnectionState.Closed)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
        }

        private string GetCommandSql(SqlCommand cmd)
        {
            string sql = string.Empty;
            if (cmd.CommandType == CommandType.Text)
            {
                sql = cmd.CommandText;
                foreach (SqlParameter p in cmd.Parameters)
                {
                    if (p.SqlDbType == SqlDbType.Char || p.SqlDbType == SqlDbType.Date ||
                       p.SqlDbType == SqlDbType.DateTime || p.SqlDbType == SqlDbType.DateTime2 ||
                       p.SqlDbType == SqlDbType.NChar || p.SqlDbType == SqlDbType.NText ||
                       p.SqlDbType == SqlDbType.NVarChar || p.SqlDbType == SqlDbType.Text ||
                       p.SqlDbType == SqlDbType.Time || p.SqlDbType == SqlDbType.UniqueIdentifier ||
                       p.SqlDbType == SqlDbType.VarChar || p.SqlDbType == SqlDbType.Xml)
                    {

                        sql = sql.Replace(p.ParameterName, "\'" + p.Value.ToString().Replace("\'", "\'\'") + "\'");
                    }
                    else
                    {
                        sql = sql.Replace(p.ParameterName, p.Value.ToString());
                    }
                }

            }
            return sql;
        }

        #endregion

        #region Public Method
        public string Sql
        {
            get
            {
                return _sql;
            }
        }
        public DatabaseExeption Exception
        {
            get
            {
                return _exception;
            }
        }

        public SqlParameter CreateParameter(string name, object value)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = name;
            parameter.Value = (value == null ? DBNull.Value : value);
            return parameter;
        }

        public SqlParameter CreateInParameter(string name, SqlDbType type, object value)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = name;
            parameter.SqlDbType = type;
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = (value == null ? DBNull.Value : value);
            return parameter;
        }


        public SqlParameter CreateOutParam(string name)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = name;
            parameter.Direction = ParameterDirection.Output;
            return parameter;
        }



        public SqlParameter CreateOutParam(string name, SqlDbType type)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = name;
            parameter.SqlDbType = type;
            parameter.Direction = ParameterDirection.Output;
            return parameter;
        }
        #endregion

        #region Trasation
        public SqlTransaction BeginTransation()
        {
            SqlConnection conn = CreateConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            _transation = conn.BeginTransaction();
            return _transation;
        }

        public void Rollback()
        {
            try
            {
                if (_transation != null)
                {
                    _transation.Rollback();
                }
                else
                {
                    _transation.Dispose();
                }
            }
            catch(Exception ex)
            {
                _exception = new DatabaseExeption(ex.Message,ex);
                throw _exception;
            }
            finally
            {
                if (_transation != null)
                {
                    if (_transation.Connection != null && _transation.Connection.State != ConnectionState.Closed)
                    {
                        _transation.Connection.Close();
                    }
                    _transation.Connection.Dispose();
                    _transation.Dispose();
                }
            }
        }

        public void Commit()
        {
            try
            {
                if (_transation != null)
                {
                    _transation.Commit();
                }
                else
                {
                    _transation.Dispose();
                }
            }
            catch (Exception ex)
            {
                _exception = new DatabaseExeption(ex.Message, ex);
                throw _exception;
            }
            finally
            {
                if (_transation != null)
                {
                    if (_transation.Connection != null && _transation.Connection.State != ConnectionState.Closed)
                    {
                        _transation.Connection.Close();
                    }
                    _transation.Connection.Dispose();
                    _transation.Dispose();
                }
            }
        }

        #endregion

        #region SQL Helper

        //NonQuery
        public int ExecuteNonQuery(string commandText, params SqlParameter[] parameters)
        {
            return ExecuteNonQuery(ref _transation, CommandType.Text, commandText, parameters);
        }

        public int ExecuteNonQuery(ref SqlTransaction trans, string commandText, params SqlParameter[] parameters)
        {
            return ExecuteNonQuery(ref trans, CommandType.Text, commandText, parameters);
        }

        public int RunProcNonQuery(string procName, params SqlParameter[] parameters)
        {
            return ExecuteNonQuery(ref _transation, CommandType.StoredProcedure, procName, parameters);
        }

        public int RunProcNonQuery(ref SqlTransaction trans, string procName, params SqlParameter[] parameters)
        {
            return ExecuteNonQuery(ref trans, CommandType.StoredProcedure, procName, parameters);
        }

        //Scalar
        public object ExecuteScalar(string commandText, params SqlParameter[] parameters)
        {
            return ExecuteScalar(ref _transation, CommandType.Text, commandText, parameters);
        }

        public object ExecuteScalar(ref SqlTransaction trans, string commandText, params SqlParameter[] parameters)
        {
            return ExecuteScalar(ref trans, CommandType.Text, commandText, parameters);
        }

        public object RunProcScalar(string procName, params SqlParameter[] parameters)
        {
            return ExecuteScalar(ref _transation, CommandType.StoredProcedure, procName, parameters);
        }

        public object RunProcScalar(ref SqlTransaction trans, string procName, params SqlParameter[] parameters)
        {
            return ExecuteScalar(ref trans, CommandType.StoredProcedure, procName, parameters);
        }

        //DataSet
        public DataSet ExecuteDataSet(string commandText, params SqlParameter[] parameters)
        {
            return ExecuteDataSet(ref _transation, CommandType.Text, commandText, parameters);
        }

        public DataSet ExecuteDataSet(ref SqlTransaction trans, string commandText, params SqlParameter[] parameters)
        {
            return ExecuteDataSet(ref trans, CommandType.Text, commandText, parameters);
        }

        public DataSet RunProcDataSet(string commandText, params SqlParameter[] parameters)
        {
            return ExecuteDataSet(ref _transation, CommandType.StoredProcedure, commandText, parameters);
        }

        public DataSet RunProcDataSet(ref SqlTransaction trans, string commandText, params SqlParameter[] parameters)
        {
            return ExecuteDataSet(ref trans, CommandType.StoredProcedure, commandText, parameters);
        }

        //DataRow
        public DataRow ExecuteDataRow(string commandText, params SqlParameter[] parameters)
        {
            return ExecuteDataRow(ref _transation, CommandType.Text, commandText, parameters);
        }

        public DataRow ExecuteDataRow(ref SqlTransaction trans, string commandText, params SqlParameter[] parameters)
        {
            return ExecuteDataRow(ref trans, CommandType.Text, commandText, parameters);
        }

        public DataRow RunProcDataRow(string commandText, params SqlParameter[] parameters)
        {
            return ExecuteDataRow(ref _transation, CommandType.StoredProcedure, commandText, parameters);
        }

        public DataRow RunProcDataRow(ref SqlTransaction trans, string commandText, params SqlParameter[] parameters)
        {
            return ExecuteDataRow(ref trans, CommandType.StoredProcedure, commandText, parameters);
        }

        #endregion
    }

    public class DatabaseExeption : Exception
    {
        private string _sql;

        public DatabaseExeption(string message)
            : base(message)
        {
        }

        public DatabaseExeption(string message, Exception e)
            : base(message, e)
        {
        }

        public DatabaseExeption(string message, Exception e, string sql)
            : base(message, e)
        {
            _sql = sql;
        }

        public string SQL
        {
            get
            {
                return _sql;
            }
        }
    }
}
