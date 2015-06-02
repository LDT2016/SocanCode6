using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using WeifenLuo.WinFormsUI.Docking;
using System.Diagnostics;

namespace SocanCode
{
    public partial class MainForm : Form
    {
        private const string HOME_URL = "http://www.Socansoft.com";
        private const string XML_URL = "http://www.socansoft.com/downloads/socancode/SocanCode.xml";
        private const string DOWNLOAD_URL = "http://www.socansoft.com/downloads/SocanCode/SocanCode.rar";

        public static WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private DatabaseForm frmDatabase;
        public TemplateReadmeForm frmTemplateReadme;

        public MainForm()
        {
            InitializeComponent();
            labNewVersion.Tag = DOWNLOAD_URL;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + " V" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            dockPanel = this.dockPanel1;

            frmDatabase = new DatabaseForm();
            frmDatabase.OutputCode += new Action<Model.Database>(frmDatabase_OutputCode);
            frmDatabase.CreateCode += new Action<Model.Table>(frmDatabase_CreateCode);
            frmDatabase.Show(dockPanel);

            frmTemplateReadme = new TemplateReadmeForm();
            frmTemplateReadme.Show(dockPanel);

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.RunWorkerAsync();

            OpenUrl(HOME_URL);
        }

        #region �汾���
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            int count = 0;
            bool hasGetVersion = false;
            while (!hasGetVersion && count < 3)
            {
                try
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(XML_URL);
                    e.Result = xml;
                    return;
                }
                catch
                {
                    count++;
                }
            }
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                labNewVersion.Text = "���ӷ�����ʧ��!";
                labNewVersion.LinkColor = Color.Red;
                return;
            }

            XmlDocument xml = e.Result as XmlDocument;
            XmlNode display = xml.SelectSingleNode("DOCUMENT").SelectSingleNode("item").SelectSingleNode("display");
            Version lastVersion = new Version(display.SelectSingleNode("content2").InnerText);
            Version currVersion = Assembly.GetExecutingAssembly().GetName().Version;
            labNewVersion.Tag = display.SelectSingleNode("button").Attributes["buttonlink"].Value;

            if (lastVersion > currVersion)
            {
                labNewVersion.Text = "�����°汾 V" + lastVersion + ", �����˴�����";
                labNewVersion.LinkColor = Color.Red;
            }
            else
            {
                labNewVersion.Text = "��ǰ�汾�Ѿ������°汾";
                labNewVersion.LinkColor = Color.Green;
            }
        }
        #endregion

        #region frmDatabase�¼�
        void frmDatabase_OutputCode(Model.Database db)
        {
            OutputCodeForm frm = new OutputCodeForm(db);
            frm.ShowReadmeOfTemplate += new Action<string>(ShowTemplateReadme);
            frm.Show(MainForm.dockPanel);
        }

        void frmDatabase_CreateCode(Model.Table table)
        {
            CreateCodeForm frm = new CreateCodeForm(table);
            frm.ShowReadmeOfTemplate += new Action<string>(ShowTemplateReadme);
            frm.Show(MainForm.dockPanel);
        }

        void ShowTemplateReadme(string obj)
        {
            if (string.IsNullOrEmpty(obj))
            {
                frmTemplateReadme.txtMsg.Text = string.Empty;
            }
            else
            {
                frmTemplateReadme.txtMsg.Text = obj;
                frmTemplateReadme.Show(dockPanel);
            }
        }
        #endregion

        #region ��һ�������ӵķ���
        private delegate void OpenUrlEventHandler(string url);

        public static void OpenUrl(string url)
        {
            if (dockPanel.InvokeRequired)
            {
                dockPanel.BeginInvoke(new OpenUrlEventHandler(OpenUrlEvent), new object[] { url });
            }
            else
            {
                OpenUrlEvent(url);
            }
        }

        private static void OpenUrlEvent(string url)
        {
            WebBrowser.BrowserForm frm = new WebBrowser.BrowserForm();
            frm.Show(dockPanel);
            frm.Go(url);
        }
        #endregion

        #region ��ǩҳ�ϵ��Ҽ��˵�
        private void mnuClose_Click(object sender, EventArgs e)
        {
            this.dockPanel1.ActiveContent.DockHandler.Close();
        }

        private void mnuCloseOther_Click(object sender, EventArgs e)
        {
            List<DockContent> contents = new List<DockContent>();
            foreach (DockContent content in dockPanel1.Documents)
            {
                contents.Add(content);
            }

            foreach (DockContent content in contents)
            {
                if (content != this.dockPanel1.ActiveContent)
                {
                    content.Close();
                }
            }
        }

        private void mnuCloseAll_Click(object sender, EventArgs e)
        {
            mnuCloseOther_Click(sender, e);
            mnuClose_Click(sender, e);
        }
        #endregion

        #region �˵�
        /// <summary>
        /// ���ɴ���
        /// </summary>
        private void menuCreateCode_Click(object sender, EventArgs e)
        {
            frmDatabase.CreateCurrentTableCode();
        }

        /// <summary>
        /// �������
        /// </summary>
        private void menuOutputCode_Click(object sender, EventArgs e)
        {
            frmDatabase.OutputSelectedDatabaseCode();
        }

        /// <summary>
        /// ���ݿ���Դ������
        /// </summary>
        private void menufrmDatabase_Click(object sender, EventArgs e)
        {
            frmDatabase.Show(dockPanel);
        }

        /// <summary>
        /// ģ��˵��
        /// </summary>
        private void menuReadmeOfTemplate_Click(object sender, EventArgs e)
        {
            frmTemplateReadme.Show(dockPanel);
        }

        /// <summary>
        /// HtmlתJs
        /// </summary>
        private void menuHtmlToJs_Click(object sender, EventArgs e)
        {
            HtmlToJsForm frm = new HtmlToJsForm();
            frm.Show(dockPanel);
        }

        /// <summary>
        /// ����С����
        /// </summary>
        private void menuCodeTools_Click(object sender, EventArgs e)
        {
            CodeToolsForm frm = new CodeToolsForm();
            frm.Show(dockPanel);
        }

        /// <summary>
        /// ������������
        /// </summary>
        private void menuRegexGen_Click(object sender, EventArgs e)
        {
            RegexForm frm = new RegexForm();
            frm.Show(dockPanel1);
        }

        /// <summary>
        /// �ٷ���վ
        /// </summary>
        private void menuWebsite_Click(object sender, EventArgs e)
        {
            OpenUrl(HOME_URL);
        }

        /// <summary>
        /// ����
        /// </summary>
        private void menuAbout_Click(object sender, EventArgs e)
        {
            AboutForm frm = new AboutForm();
            frm.ShowDialog();
        }
        #endregion

        private void labNewVersion_Click(object sender, EventArgs e)
        {
            string url = (sender as ToolStripStatusLabel).Tag.ToString();
            Process.Start("IEXPLORE.exe", url);
        }

        private void dockPanel1_ContentAdded(object sender, DockContentEventArgs e)
        {
            if (e.Content.DockHandler.ShowHint == DockState.Document
                   || e.Content.DockHandler.ShowHint == DockState.Unknown)
            {
                e.Content.DockHandler.TabPageContextMenuStrip = cmsDockPanel;
            }
        }
    }
}