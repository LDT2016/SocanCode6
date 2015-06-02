using System;
using System.Drawing;
using System.Windows.Forms;

namespace SocanCode
{
    public partial class DatabaseForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public event Action<Model.Database> OutputCode;
        public event Action<Model.Table> CreateCode;
        public event Action<string> ShowStatus;

        public DatabaseForm()
        {
            InitializeComponent();
        }

        private void btnAddDatabase_Click(object sender, EventArgs e)
        {
            ConnectionForm frm = new ConnectionForm();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                Model.Database database = frm.database;
                ShowDatabase(database);
            }
        }

        private void btnRemoveDatabase_Click(object sender, EventArgs e)
        {
            TreeNode node = tvDatabase.SelectedNode;
            if (node == null) { return; }
            while (node.Level > 0)
            {
                node = node.Parent;
            }
            tvDatabase.Nodes.Remove(node);
        }

        #region ---------------- ���ؽڵ㼰�Ҽ� -----------------
        /// <summary>
        /// ��ʾ��һ���ڵ㣺���ݿ�
        /// </summary>
        private void ShowDatabase(Model.Database database)
        {
            TreeNode nodeDB = new TreeNode(
                string.Format("{0}��{1}",
                    database.Name,
                    database.TypeDescription),
                0, 0);
            nodeDB.Tag = database;
            nodeDB.ContextMenuStrip = cmsDB;
            this.tvDatabase.Nodes.Add(nodeDB);

            ShowFolders(database, nodeDB);
            nodeDB.Expand();
        }

        /// <summary>
        /// ��ʾ�ڶ����ڵ㣺����ͼ���洢�����ļ���
        /// </summary>
        private void ShowFolders(Model.Database db, TreeNode nodeRoot)
        {
            //��ӡ����ļ���
            TreeNode nodeTableFolder = new TreeNode("��", 1, 1);
            nodeRoot.Nodes.Add(nodeTableFolder);

            //��ӱ�
            ShowTables(db, nodeTableFolder);
            nodeTableFolder.Expand();

            //��ӡ���ͼ���ļ���
            TreeNode nodeViewFolder = new TreeNode("��ͼ", 1, 1);
            nodeRoot.Nodes.Add(nodeViewFolder);

            //�����ͼ
            ShowViews(db, nodeViewFolder);

            //��ӡ��洢���̡��ļ���
            TreeNode nodeStoreProceduresFolder = new TreeNode("�洢����", 1, 1);
            nodeRoot.Nodes.Add(nodeStoreProceduresFolder);

            //��Ӵ洢����
            ShowStoreProcedures(db, nodeStoreProceduresFolder);
        }

        /// <summary>
        /// ��ʾ�������ڵ㣺��
        /// </summary>
        private void ShowTables(Model.Database database, TreeNode nodeRoot)
        {
            foreach (Model.Table table in database.Tables)
            {
                TreeNode nodeTable = new TreeNode(table.Name, 2, 2);
                nodeTable.Tag = table;
                nodeTable.ContextMenuStrip = cmsTable;
                nodeRoot.Nodes.Add(nodeTable);

                ShowFields(table, nodeTable);
            }
        }

        /// <summary>
        /// ��ʾ�������ڵ㣺��ͼ
        /// </summary>
        private void ShowViews(Model.Database database, TreeNode nodeRoot)
        {
            foreach (Model.Table table in database.Views)
            {
                TreeNode nodeTable = new TreeNode(table.Name, 2, 2);
                nodeTable.Tag = table;
                nodeTable.ContextMenuStrip = cmsView;
                nodeRoot.Nodes.Add(nodeTable);

                ShowFields(table, nodeTable);
            }
        }

        /// <summary>
        /// ��ʾ�������ڵ㣺�洢����
        /// </summary>
        private static void ShowStoreProcedures(Model.Database db, TreeNode nodeRoot)
        {
            foreach (string storeProcedure in db.StoreProcedures)
            {
                nodeRoot.Nodes.Add(new TreeNode(storeProcedure, 5, 5));
            }
        }

        /// <summary>
        /// ��ʾ���ļ��ڵ㣺�ֶ�
        /// </summary>
        private void ShowFields(Model.Table table, TreeNode nodeRoot)
        {
            foreach (Model.Field field in table.Fields)
            {
                string text = string.Format("{0}:{1}{2}{3}", field.Name, field.DbType.ToString(),
                    field.IsIdentifier ? "[id]" : "", field.IsKeyField ? "[key]" : "");
                TreeNode nodeField = new TreeNode(text, 3, 3);
                nodeRoot.Nodes.Add(nodeField);
            }
        }
        #endregion

        private void menuOutput_Click(object sender, EventArgs e)
        {
            OutputSelectedDatabaseCode();
        }

        private void menuCreateCode_Click(object sender, EventArgs e)
        {
            CreateCurrentTableCode();
        }

        /// <summary>
        /// ��ѡ�еĿ��������������
        /// </summary>
        public void OutputSelectedDatabaseCode()
        {
            if (OutputCode != null)
            {
                Model.Database database = tvDatabase.SelectedNode.Tag as Model.Database;
                if (database != null)
                {
                    OutputCode(database);
                }
                else
                {
                    ShowMessage.Alert("����ѡ��һ�����ݿ⡣");
                }
            }
        }

        /// <summary>
        /// ��ѡ�еı�������ɴ������
        /// </summary>
        public void CreateCurrentTableCode()
        {
            if (CreateCode != null)
            {
                Model.Table table = tvDatabase.SelectedNode.Tag as Model.Table;
                if (table != null)
                {
                    CreateCode(table);
                }
                else
                {
                    ShowMessage.Alert("����ѡ��һ�������ͼ��");
                }
            }
        }

        private void menuDeleteDatabase_Click(object sender, EventArgs e)
        {
            tvDatabase.Nodes.Remove(tvDatabase.SelectedNode);
        }

        private void tvDatabase_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode tn = GetMouseNode(tvDatabase, this);
            if (tn != null)
            {
                tvDatabase.SelectedNode = tn;
            }
        }

        /// <summary>
        /// �õ�TreeView�����ָ��Ľڵ�,ͬʱ�Ѹýڵ�����Ϊ��ǰѡ�еĽڵ�
        /// </summary>
        private TreeNode GetMouseNode(TreeView tv, Control currentForm)
        {
            Point pt = currentForm.PointToScreen(tv.Location);
            Point p = new Point(Control.MousePosition.X - pt.X, Control.MousePosition.Y - pt.Y);
            TreeNode tn = tv.GetNodeAt(p);
            return tn;
        }
    }
}