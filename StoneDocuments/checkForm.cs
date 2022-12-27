using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Windows.Forms;


namespace StoneDocuments
{
    public partial class checkForm : System.Windows.Forms.Form
    {
        private RequestHandler m_Handler;
        private CancelHandler c_Handler;
        private ExternalEvent m_ExternalEvent;
        private ExternalEvent c_ExternalEvent;

        public checkForm(ExternalEvent exEvent, RequestHandler rHandler, CancelHandler cHandler, ExternalEvent cEvent, int count)
        {
            InitializeComponent();

            m_Handler = rHandler;
            c_Handler = cHandler;
            m_ExternalEvent = exEvent;
            c_ExternalEvent = cEvent;
            label2.Text = count.ToString() + " elements selected";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_ExternalEvent.Raise();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {           
            m_ExternalEvent.Raise();
            this.Close();
        }

    }
}
