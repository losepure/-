using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 系统维护
{
    public class DbManager
    {
        private string path;
        private string connectiongString;
        private OleDbConnection odcConnection;
        private DataRow sysConfigTable;
        private DataRow pcInfoTable;
        /// <summary>
        /// 几个常用的kv表的字典
        /// </summary>
        private Dictionary<string, DataRow> dataRowDic = new Dictionary<string, DataRow>();
        private OleDbDataAdapter adapter;
        private DataSet ds;
        private OleDbCommandBuilder odcCommandBuilder;
        public DbManager()
        {
            this.path = Application.StartupPath + "\\datebase.mdb";
            this.connectiongString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";";

        }
        /// <summary>
        /// 连接数据库
        /// </summary>
        public void open()
        {
            try
            {
                this.odcConnection = new OleDbConnection(this.connectiongString);
                odcConnection.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("打开数据库失败！具体原因为：" + e.Message);
            }

        }

        /// <summary>
        /// 根据sql语句获得数据集
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public DataSet getDataSet(string sql)
        {
            ds = new DataSet();
            try
            {
                adapter = new OleDbDataAdapter(sql, this.odcConnection);
                //这句话很重要
                odcCommandBuilder = new OleDbCommandBuilder(adapter);
                adapter.Fill(ds);
            }
            catch (Exception e)
            {
                MessageBox.Show("执行Sql语句失败！具体原因是：" + e.Message);
            }
            return ds;
        }

        public void init()
        {
            open();

        }


        //private void addOrReplaceKv(Dictionary<string, DataRow> dic, string key, DataRow value)
        //{
        //    if (dataRowDic.ContainsKey(key))
        //        dataRowDic[key] = value;
        //    else
        //        dataRowDic.Add(key, value);
        //}

        public DataRow getRow(string table)
        {
            DataRowCollection rows = getDataSet("select * from " + table).Tables[0].Rows;
            int n = rows.Count;
            if (n > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                return row;
            }
            return null;

        }
        /// <summary>
        /// 根据列名和关键字找到行
        /// </summary>
        /// <param name="table">表名称</param>
        /// <param name="keyColName">要找的列名</param>
        /// <param name="keyWords">列的关键字</param>
        /// <returns></returns>
        public DataRow getRow(string table, string keyColName, string keyWords)
        {
            DataRowCollection rows = getDataSet("select * from " + table).Tables[0].Rows;
            foreach (DataRow row in rows)
            {

                if (row[keyColName].ToString() == keyWords)
                {
                    return row;
                }
            }
            return null;
        }
        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="table">表名称</param>
        /// <param name="keyColName">要找的列名</param>
        /// <param name="keyWords">列的关键字</param>
        /// <returns></returns>
        public bool deleteRow(string table, string keyColName, string keyWords)
        {
            DataRow row = getRow(table, keyColName, keyWords);
            row.Delete();
            updateDateSet();
            return true;
        }
        /// <summary>
        /// 添加行
        /// </summary>
        /// <param name="table"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool addRow(string table,object[] values)
        {
            DataRowCollection rows = getRows(table);
            rows.Add(values);
            updateDateSet();
            return true;
        }
        /// <summary>
        /// 获得一列数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="ColName"></param>
        /// <returns></returns>
        public T[] getCol<T>(string table, string ColName)
        {
            DataRowCollection rows = getDataSet("select * from " + table).Tables[0].Rows;
            T[] array = new T[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                array[i] = (T)rows[i][ColName];
            }
            return array;
        }




        /// <summary>
        /// 将kv表转化为键值对
        /// </summary>
        /// <param name="table">表名称</param>
        /// <returns></returns>
        public Dictionary<string, string> getRow2Dic(string table)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            getDataSet("select * from " + table);
            foreach (DataColumn col in ds.Tables[0].Columns)
            {
                string key = col.Caption;
                string value = ds.Tables[0].Rows[0][key].ToString();
                dic.Add(key, value);
            }
            return dic;
        }

        /// <summary>
        /// 获得表中所有行
        /// </summary>
        /// <param name="table">表名称</param>
        /// <returns></returns>
        public DataRowCollection getRows(string table)
        {
            getDataSet("select * from " + table);
            DataRowCollection rows = ds.Tables[0].Rows;
            return rows;
        }

        /// <summary>
        /// 组合表中任意2个字段为键值对
        /// </summary>
        /// <param name="table">表名称</param>
        /// <param name="keyField">键字段</param>
        /// <param name="valField">值字段</param>
        /// <returns></returns>
        public Dictionary<TKey, TValue> getDic<TKey, TValue>(string table, string keyField, string valField)
        {
            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
            foreach (DataRow row in getRows(table))
            {
                TKey key = row.Field<TKey>(keyField);
                TValue value = row.Field<TValue>(valField);
                dic.Add(key, value);
            }
            return dic;
        }

        /// <summary>
        /// 组合表中任意2个字段为键值对
        /// </summary>
        /// <typeparam name="TKey">键字段类型</typeparam>
        /// <typeparam name="TValue">值字段类型</typeparam>
        /// <param name="row">行数据</param>
        /// <param name="keyField">键字段</param>
        /// <param name="valField">值字段</param>
        /// <returns></returns>
        public Dictionary<TKey, TValue> getDic<TKey, TValue>(DataRow row, string keyField, string valField)
        {
            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
            TKey key = row.Field<TKey>(keyField);
            TValue value = row.Field<TValue>(valField);
            return dic;
        }

        /// <summary>
        /// kv表中判断是否存在key
        /// </summary>
        /// <param name="table"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public bool isContainColName(string table, string colName)
        {
            return getDataSet("select * from " + table).Tables[0].Columns.Contains(colName);
        }



        /// <summary>
        /// kv表中根据列名(键)获得值
        /// </summary>
        /// <param name="table">kv表</param>
        /// <param name="colName">列名称</param>
        /// <returns></returns>
        public string getField(string table, string colName)
        {
            if (isContainColName(table, colName))
            {
                return getRow(table)[colName].ToString();

            }
            return null;
        }



        /// <summary>
        /// 更新数据库中数据
        /// </summary>
        public bool updateDateSet()
        {
            this.adapter.Update(ds);
            ds.Clear();
            adapter.Fill(ds);
            return true;
        }


        /// <summary>
        /// 更新数据库中数据(仅限配置表)
        /// </summary>
        /// <param name="table">要更新的表</param>
        /// <param name="field">要更新的字段</param>
        /// <param name="value">更新的新值</param>
        public bool updateConfigField<T>(string table, string field, T value)
        {
            getRow(table)[field] = value;
            this.adapter.Update(ds);
            ds.Clear();
            adapter.Fill(ds);
            return true;
        }

        /// <summary>
        /// 给表中增加一个字段并赋值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="table">表</param>
        /// <param name="colName">列</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool addColumn<T>(string table, string colName, T value)
        {
            string type = "";
            if (value is bool)
            {
                type = " Bit";
            }
            else if (value is string)
            {
                type = " Char";
            }
            else if (value is int)
            {
                type = " Short";
            }
            string sql = "alter table " + table + " add COLUMN " + colName + type;
            bool result = exeSql(sql);
            if (result)
            {
                updateConfigField<T>(table, colName, value);
            }
            return result;

        }

        /// <summary>
        /// 删除表中某列
        /// </summary>
        /// <param name="table">表名称</param>
        /// <param name="colName">要删除的列名</param>
        /// <returns></returns>
        public bool deleteColumn(string table, string colName)
        {
            bool result = false;
            if (isContainColName(table, colName))
            {
                string sql = "alter table " + table + " drop COLUMN " + colName;
                result = exeSql(sql);
            }
            return result;
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句字符串</param>
        /// <returns></returns>
        public bool exeSql(string sql)
        {
            OleDbCommand cmd;
            OleDbTransaction tran = odcConnection.BeginTransaction();
            try
            {
                cmd = new OleDbCommand();
                cmd.Transaction = tran;
                cmd.Connection = odcConnection;
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                MessageBox.Show("sql执行错误：" + ex.Message);
                return false;
            }

        }


        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void close()
        {
            this.odcConnection.Close();
        }


    }
}
