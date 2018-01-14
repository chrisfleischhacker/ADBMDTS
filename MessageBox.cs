using System;
using System.Reflection;
using System.Windows.Forms;

namespace ADBMDTS
{
	partial class MessageBox : Form
	{
		public MessageBox()
		{
			InitializeComponent();
			this.Text = String.Format("{0} Message", AssemblyTitle);
            this.txtMessage.Text = "Unknown Error";
		}

        public MessageBox(string texttodisplay)
        {
            InitializeComponent();
            this.Text = String.Format("{0} Message", AssemblyTitle);
            this.txtMessage.Text = texttodisplay;
        }

		#region Assembly Attribute Accessors

		public string AssemblyTitle
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0)
				{
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title != "")
					{
						return titleAttribute.Title;
					}
				}
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

        #endregion

    }
}