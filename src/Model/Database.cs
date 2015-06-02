using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace Model
{
    /// <summary>
    /// ���ݿ�����
    /// </summary>
    public enum DatabaseTypes
    {
        Access,
        Sql2000,
        Sql2005,
        MySql,
        Oracle,
        SQLite
    }

    /// <summary>
    /// ���ݿ�
    /// </summary>
    public class Database
    {
        public string TypeDescription
        {
            get
            {
                switch (Type)
                {
                    case DatabaseTypes.Sql2005:
                        return "Sql2005����߰汾";
                    default:
                        return Type.ToString();
                }
            }
        }

        /// <summary>
        /// ����һ�������ݿ�
        /// </summary>
        public Database()
        {
            Tables = new List<Table>();
            Views = new List<Table>();
            StoreProcedures = new List<string>();
            SelectedTables = new List<Table>();
        }

        #region ����
        /// <summary>
        /// ���ݿ����Ӳ���
        /// </summary>
        public string ConnectionString { get; set; }

        private string name = null;

        /// <summary>
        /// ���ݿ���
        /// </summary>
        public string Name
        {
            get
            {
                if (name != null)
                {
                    return name;
                }
                else
                {
                    switch (Type)
                    {
                        case DatabaseTypes.Access:
                            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
                            {
                                try
                                {
                                    FileInfo file = new FileInfo(conn.DataSource);
                                    return file.Name.Remove(file.Name.LastIndexOf("."));
                                }
                                catch (Exception)
                                {
                                    return conn.DataSource;
                                }
                            }
                        case DatabaseTypes.Sql2000:
                        case DatabaseTypes.Sql2005:
                            using (SqlConnection conn = new SqlConnection(ConnectionString))
                            {
                                try
                                {
                                    FileInfo file = new FileInfo(conn.Database);
                                    int start = file.Name.LastIndexOf(".");
                                    if (start > 0)
                                        return file.Name.Remove(start);

                                    return conn.Database;
                                }
                                catch (Exception)
                                {
                                    return conn.Database;
                                }
                            }
                        case DatabaseTypes.MySql:
                            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
                            {
                                try
                                {
                                    return conn.Database;
                                }
                                catch (Exception)
                                {
                                    return "MySqlDB";
                                }
                            }
                        case DatabaseTypes.Oracle:
                            using (OracleConnection conn = new OracleConnection(ConnectionString))
                            {
                                try
                                {
                                    return conn.DataSource;
                                }
                                catch (Exception)
                                {
                                    return "OracleDB";
                                }
                            }
                        case DatabaseTypes.SQLite:
                            using (System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(ConnectionString))
                            {
                                try
                                {
                                    Regex r = new Regex("Data Source=(?<source>[^;]+);");
                                    Match m = r.Match(conn.ConnectionString);
                                    if (m.Success)
                                    {
                                        string path = m.Groups["source"].Value;
                                        FileInfo fi = new FileInfo(path);
                                        return fi.Name.Substring(0, fi.Name.LastIndexOf('.'));
                                    }
                                    else
                                    {
                                        return "SQLiteDB";
                                    }
                                }
                                catch (Exception)
                                {
                                    return "SQLiteDB";
                                }
                            }
                        default:
                            return "UnKnownDB";
                    }
                }
            }
            set { name = value; }
        }

        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public DatabaseTypes Type { get; set; }

        /// <summary>
        /// ���б�
        /// </summary>
        public List<Model.Table> Tables { get; set; }

        /// <summary>
        /// ������ͼ��
        /// </summary>
        public List<Model.Table> Views { get; set; }

        /// <summary>
        /// ���д洢������
        /// </summary>
        public List<string> StoreProcedures { get; set; }

        /// <summary>
        /// ѡ�еı����ͼ
        /// </summary>
        public List<Model.Table> SelectedTables { get; set; }
        #endregion

        #region ����
        /// <summary>
        /// ���һ����
        /// </summary>
        public void AddTable(Model.Table table)
        {
            table.Database = this;
            Tables.Add(table);
        }

        /// <summary>
        /// ���һ����ͼ
        /// </summary>
        public void AddView(Model.Table view)
        {
            view.Database = this;
            Views.Add(view);
        }

        /// <summary>
        /// ת��Ϊ�ַ���
        /// </summary>
        /// <returns>���ر���</returns>
        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}
