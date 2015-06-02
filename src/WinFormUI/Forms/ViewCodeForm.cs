using System.Windows.Forms;

namespace SocanCode
{
    public partial class ViewCodeForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="caption">����</param>
        /// <param name="text">����</param>
        /// <param name="lang">����:"ASP3/XHTML","BAT","Boo","Coco","C++.NET","C#","HTML",
        /// "Java","JavaScript","PHP","TeX","VBNET","XML","TSQL"</param>
        public ViewCodeForm(string caption, string text, string language)
        {
            InitializeComponent();
            this.TabText = caption;
            TextEditor.SetStyle(txtCode, language);
            txtCode.Text = text;
        }
    }
}