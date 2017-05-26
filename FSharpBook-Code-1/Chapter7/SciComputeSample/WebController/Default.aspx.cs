using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Text;

namespace WebController
{
    public partial class _Default : System.Web.UI.Page
    {

        protected static CloudQueue queue_in;
        protected static CloudQueue queue_out;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            SubmitTask(TextBox1.Text);
        }

        private void SubmitTask(string p)
        {
            InitStorage();
            queue_in.AddMessage(new CloudQueueMessage(p));
        }

        protected void InitStorage()
        {
            CloudStorageAccount acct = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            var clt = acct.CreateCloudQueueClient();
            queue_in = clt.GetQueueReference("sci-queue-in");
            queue_out = clt.GetQueueReference("sci-queue-out");
            queue_in.CreateIfNotExist();
            queue_out.CreateIfNotExist();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            InitStorage();
            StringBuilder b = new StringBuilder();
            CloudQueueMessage msg;
            double res = 0;
            while((msg = queue_out.GetMessage()) != null)
            {
                // b.AppendLine(msg.AsString);
                var r = msg.AsString.Split(':');
                res += double.Parse(r[1]);
            }
            b.AppendLine("The result is: " + res.ToString());
            Label1.Text = b.ToString();
        }

    }
}