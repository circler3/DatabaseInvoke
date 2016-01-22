using System;
using System.Collections.Generic;
using System.Data.Odbc;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Data.SQLite;
using Npgsql;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Windows.Forms;
using System.Data;
using System.IO;

namespace DatabaseInvoke
{
    /// <summary>
    /// 可以根据支持的Sql类型增加或删除类型，需要增加或删除对应的GetConnection和GetDbDataAdapter方法。
    /// </summary>
    public enum SqlType
    {
        SqlServer,
        MySql,
        PostgresQL,
        Oracle,
        SQLite,
        //对ODBC方式需要格外注意，目标系统必须预先安装有对应的数据驱动，如果使用DSN，那么还需要使用配置ODBC数据源
        Odbc
    }
    /// <summary>
    /// 使用ADO.NET控制对数据库的基本访问方法，对同一个活动对象（不关闭）线程安全。
    /// </summary>
    public class SqlManipulation : IDisposable
    {

        public SqlManipulation(string strDSN, SqlType sqlType)
        {
            _sqlType = sqlType;
            _strDSN = strDSN;
        }

        #region private variables
        private SqlType _sqlType;
        private string _strDSN;
        private DbConnection _conn;
        private bool _disposed;
        #endregion

        private DbConnection GetConnection()
        {
            DbConnection conn;
            switch (_sqlType)
            {
                case SqlType.SqlServer:
                    conn = new SqlConnection(_strDSN);
                    return conn;
                case SqlType.MySql:
                    conn = new MySqlConnection(_strDSN);
                    return conn;
                case SqlType.PostgresQL:
                    conn = new NpgsqlConnection(_strDSN);
                    return conn;
                case SqlType.Oracle:
                    conn = new OracleConnection(_strDSN);
                    return conn;
                case SqlType.SQLite:
                    conn = new SQLiteConnection(_strDSN);
                    return conn;
                case SqlType.Odbc:
                    conn = new OdbcConnection(_strDSN);
                    return conn;
                default:
                    return null;
            }
        }

        private DbDataAdapter GetDbDataAdapter(string sql)
        {
            DbDataAdapter adp;
            switch (_sqlType)
            {
                case SqlType.SqlServer:
                    adp = new SqlDataAdapter(sql, _conn as SqlConnection);
                    return adp;
                case SqlType.MySql:
                    adp = new MySqlDataAdapter(sql, _conn as MySqlConnection);
                    return adp;
                case SqlType.PostgresQL:
                    adp = new NpgsqlDataAdapter(sql, _conn as NpgsqlConnection);
                    return adp;
                case SqlType.Oracle:
                    adp = new OracleDataAdapter(sql, _conn as OracleConnection);
                    return adp;
                case SqlType.SQLite:
                    adp = new SQLiteDataAdapter(sql, _conn as SQLiteConnection);
                    return adp;
                case SqlType.Odbc:
                    adp = new OdbcDataAdapter(sql, _conn as OdbcConnection);
                    return adp;
                default:
                    return null;
            }
        }

        private DbCommand GetCommand(DbConnection conn, string strSQL)
        {
            DbCommand command = conn.CreateCommand();
            command.CommandText = strSQL;
            return command;
        }

        /// <summary>
        /// 初始化连接并打开
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            try
            {
                _conn = GetConnection();
                _conn.Open();
                return true;
            }
            catch (Exception e)
            {
                //记录日志，退出
                MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 执行SELECT查询语句，并返回DataTable对象。
        /// </summary>
        /// <param name="strSQL">需要执行的sql语句</param>
        /// <returns>DataTable对象</returns>
        public DataTable ExcuteQuery(string strSQL)
        {
            DbDataAdapter adp = GetDbDataAdapter(strSQL);
            DataTable dt = new DataTable();
            try
            {
                adp.Fill(dt);
            }
            catch (Exception e)
            {
                //记录日志，并返回空
                MessageBox.Show(strSQL + "\n" + e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return dt;
        }

        /// <summary>
        /// 执行非Select语句，包括UPDATE DELETE INSERT
        /// </summary>
        /// <param name="strSQL">需要执行的sql语句</param>
        /// <returns>受影响的行数</returns>
        public int ExcuteNonQuery(string strSQL)
        {
            //实例化OdbcCommand对象
            DbCommand myCmd = GetCommand(_conn, strSQL);

            try
            {
                //执行方法
                return myCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //记录日志，并返回0
                MessageBox.Show(strSQL + "\n" + e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        /// <summary>
        /// 通过事务批量执行非查询SQL语句
        /// </summary>
        /// <param name="strSQLs">需要批量执行的SQL</param>
        /// <returns>受影响的行数，发生回滚则返回-1</returns>
        public int ExecuteNonQueryTransaction(List<string> strSQLs)
        {
            DbCommand myCmd = GetCommand(_conn, "");
            int sumAffected = 0;

            DbTransaction transaction = _conn.BeginTransaction();
            myCmd.Transaction = transaction;

            try
            {
                foreach (var n in strSQLs)
                {
                    myCmd.CommandText = n;
                    sumAffected += myCmd.ExecuteNonQuery();
                }
                transaction.Commit();
                return sumAffected;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                transaction.Rollback();
                return -1;
            }
        }

        /// <summary>
        /// 执行非查询SQL，使用Oracle的Blob对象
        /// </summary>
        /// <param name="sql">需要执行的sql语句，语句中有且仅有一个‘:blob’参数</param>
        /// <param name="path">需要插入的blob文件路径</param>
        /// <returns>受影响的行数，执行不成功则返回-1</returns>
        public int OracleBlobNonQuery(string sql, string path)
        {
            if (!(_conn is OracleConnection))
            {
                return -1;
            }

            //OracleCommand cmd = new OracleCommand("INSERT INTO TEST SET F2 =:blob", _conn as OracleConnection);
            OracleCommand cmd = new OracleCommand(sql, _conn as OracleConnection);

            cmd.Parameters.Add(new OracleParameter("blob", OracleDbType.Blob));
            FileInfo fi = new FileInfo(path);
            var mydata = File.ReadAllBytes(path);
            cmd.Parameters["blob"].Value = mydata;
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception e1)
            {
                MessageBox.Show("插入数据错误！" + e1.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        #region 资源清理
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //托管对象释放
                }
                //非托管对象释放
                if (_conn != null)
                {
                    if (_conn.State != ConnectionState.Closed)
                    {
                        _conn.Close();
                    }
                    else
                    {
                        _conn = null;
                    }
                }
                _disposed = true;
            }
        }

        ~SqlManipulation()
        {
            Dispose(false);
        }
        #endregion
    }
}
