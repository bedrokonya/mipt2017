using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PageExplorerWF
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var links = WebExplore.explore(TextBox1.Text);
            ListBox1.Items.Clear();
            foreach (var s in links)
            {
                ListBox1.Items.Add(s);
            }
        }
    }
}
