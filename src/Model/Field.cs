using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace Model
{
    /// <summary>
    /// �ֶ�
    /// </summary>
    public class Field : IComparable
    {
        private string _descn;

        /// <summary>
        /// ���������ݿ��
        /// </summary>
        public Model.Table Table { get; set; }

        /// <summary>
        /// �ֶ�����
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// �ֶ���
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// �Ƿ��Ǳ�ʶ
        /// </summary>
        public bool IsIdentifier { get; set; }

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        public bool IsKeyField { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// ռ���ֽ���
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// �Ƿ������
        /// </summary>
        public bool AllowNull { get; set; }

        /// <summary>
        /// Ĭ��ֵ
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// �ֶ�˵��
        /// </summary>
        public string Descn
        {
            get
            {
                if (!string.IsNullOrEmpty(_descn))
                {
                    return _descn;
                }
                else
                {
                    return Name;
                }
            }
            set
            {
                _descn = Regex.Replace(value, @"\s*[\n]+\s*", ""); //���˵����У�������ǰ��Ŀո�
            }
        }

        /// <summary>
        /// �ֶ����������ݿ��е����͡��ڸ��ֿ��в���ͬ������ҪΪ������ֱ�Ӹ�ֵ��Ҫ����SetDbType��������
        /// </summary>
        public string FieldType { get; private set; }

        /// <summary>
        /// Ϊ�ֶθ�DbType����
        /// </summary>
        public void SetDbType(DatabaseTypes dbType, string fieldType)
        {
            this.FieldType = fieldType;
            string key;
            switch (dbType)
            {
                case DatabaseTypes.Access:
                    key = "OleDbType";
                    break;
                case DatabaseTypes.MySql:
                    key = "MySqlType";
                    break;
                case   DatabaseTypes.Oracle:
                    key = "OracleType";
                    break;
                case DatabaseTypes.SQLite:
                    key = "SQLiteType";
                    break;
                case DatabaseTypes.Sql2000:
                case DatabaseTypes.Sql2005:
                default:
                    key = "SqlType";
                    break;
            }

            FieldEx dataType = Model.FieldEx.All.Find(x =>
            {
                string[] sqlTypes = x.Properties[key].Split(',');
                foreach (string item in sqlTypes)
                {
                    if (item.Trim().Equals(fieldType.Trim(), StringComparison.CurrentCultureIgnoreCase))
                        return true;
                }
                return false;
            });

            if (dataType != null)
            {
                this.DbType = dataType.DbType;
            }
            else
            {
                this.DbType = Model.FieldEx.All.Find(x => x.Properties[key].Trim().Equals("*")).DbType;
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        public Dictionary<string, string> FieldEx
        {
            get
            {
                return Model.FieldEx.All.Find(x => x.DbType == this.DbType).Properties;
            }
        }

        #region ����2������Ϊ����V6.0��ģ����������粻��Ҫ���ݿ�ֱ��ɾ��
        /// <summary>
        /// SqlServer�е��������ͣ��ѹ�ʱ
        /// </summary>
        public string SqlTypeString
        {
            get { return FieldType; }
        }

        /// <summary>
        /// MySql�е��������ͣ��ѹ�ʱ
        /// </summary>
        public string MySqlTypeString
        {
            get { return FieldType; }
        }
        #endregion

        #region IComparable ��Ա

        public int CompareTo(object obj)
        {
            Field field = obj as Field;
            return this.Position.CompareTo(field.Position);
        }

        #endregion
    }
}
