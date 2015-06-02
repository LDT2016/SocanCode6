using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// ���ݿ��
    /// </summary>
    public class Table
    {
        /// <summary>
        /// ����һ�������ݿ��
        /// </summary>
        public Table()
        {
            this.Fields = new List<Field>();
        }

        #region ����
        /// <summary>
        /// �������Ŀ�
        /// </summary>
        public Database Database { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ���е�������
        /// </summary>
        public List<Model.Field> Fields { get; set; }
        #endregion

        /// <summary>
        /// ��������
        /// </summary>
        public List<Model.Field> KeyFields
        {
            get
            {
                List<Model.Field> conditionRows = new List<Field>();
                foreach (Model.Field model in Fields)
                {
                    if (model.IsKeyField)
                    {
                        conditionRows.Add(model);
                    }
                }
                return conditionRows;
            }
        }

        /// <summary>
        /// �����ĸ���
        /// </summary>
        public int KeyFieldsCount
        {
            get { return this.KeyFields.Count; }
        }

        /// <summary>
        /// ���з�����
        /// </summary>
        public List<Model.Field> UnKeyFields
        {
            get
            {
                List<Model.Field> unkeyFields = new List<Field>();
                List<Model.Field> keyFields = this.KeyFields;
                foreach (Model.Field item in Fields)
                {
                    if (false == keyFields.Contains(item))
                    {
                        unkeyFields.Add(item);
                    }
                }
                return unkeyFields;
            }
        }

        /// <summary>
        /// �������ĸ���
        /// </summary>
        public int UnKeyFieldsCount
        {
            get { return this.UnKeyFields.Count; }
        }

        /// <summary>
        /// ȡ�������ֶΣ���ѡ��ʶ�У�Ȼ����������
        /// </summary>
        public List<Model.Field> CondFields
        {
            get
            {
                List<Model.Field> conditionRows = new List<Field>();

                //�鿴�Ƿ��б�ʶ���б�ʶ�򷵻����б�ʶ
                foreach (Model.Field model in Fields)
                {
                    if (model.IsIdentifier)
                    {
                        conditionRows.Add(model);
                    }
                }
                if (conditionRows.Count > 0) { return conditionRows; }

                //�鿴�Ƿ����������������򷵻�����
                if (KeyFields.Count > 0) { return KeyFields; }

                //���ޱ�ʶҲ���������򷵻ص�һ����
                if (Fields.Count > 0)
                {
                    Fields[0].IsKeyField = true;
                    conditionRows.Add(Fields[0]);
                }

                return conditionRows;
            }
        }

        /// <summary>
        /// ȡ�������ֶεĸ���
        /// </summary>
        public int CondFieldsCount
        {
            get { return this.CondFields.Count; }
        }

        /// <summary>
        /// ȡ�÷������ֶ�
        /// </summary>
        public List<Model.Field> UnCondFields
        {
            get
            {
                List<Model.Field> updateRows = new List<Field>();
                List<Model.Field> conditionRows = this.CondFields;
                foreach (Model.Field item in Fields)
                {
                    if (false == conditionRows.Contains(item))
                    {
                        updateRows.Add(item);
                    }
                }
                return updateRows;
            }
        }

        /// <summary>
        /// �������ֶεĸ���
        /// </summary>
        public int UnCondFieldsCount
        {
            get { return this.UnCondFields.Count; }
        }

        #region ����
        /// <summary>
        /// ���һ���ֶ�
        /// </summary>
        public void AddField(Model.Field field)
        {
            field.Table = this;
            Fields.Add(field);
        }

        /// <summary>
        /// ת��Ϊ�ַ���
        /// </summary>
        /// <returns>���ر���</returns>
        public override string ToString()
        {
            return this.Name;
        }
        #endregion
    }
}
