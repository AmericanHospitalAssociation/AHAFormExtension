using Avectra.netForum.Common;
using Avectra.netForum.Data;
using System;
using System.Drawing;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using AHAeWebSSO;

namespace AHAFormExtension
{
    public class AHAForm
    {
        private PageClass page;
        AHAOpenAM sso = new AHAOpenAM();
        private string cst_oldid;
        private string add_user;
        private string cst_eml_address_dn;

        public void BuildSSOForm(PageClass oPage, Control oControl)
        {
            page = oPage;
            LiteralControl lineBreak = new LiteralControl("<br/><br/>");

            cst_oldid = ((object)oPage.oFacadeObject.GetValue("cst_oldid")).ToString();
            cst_eml_address_dn = ((object)oPage.oFacadeObject.GetValue("cst_eml_address_dn")).ToString();
            add_user = ((object)oPage.oFacadeObject.GetValue("CurrentUserName")).ToString();

            Label wiki = new Label();
            wiki.ID = "AHAWiki";
            wiki.Visible = true;
            wiki.CssClass = "tinyTXT";
            wiki.Text = "<a href=\"http://wiki.aha.org/ams/index.php/Reset_customer_SSO_password\" target=\"_blank\">Wiki FAQ</a><br/><br/>";
            oControl.Controls.Add((Control)wiki);


            Button button = new Button();
            button.ID = "AHAButton";
            button.Text = "Reset Password";
            button.Visible = true;
            button.Click += new EventHandler(this.btnReset_Click);
            oControl.Controls.Add((Control)button);

            oControl.Controls.Add(lineBreak);

            Label label = new Label();
            label.ID = "VinceLink";
            label.Visible = false;
            label.CssClass = "tinyTXT";
            label.Text = "";
            oControl.Controls.Add((Control)label);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //DataUtils.ExecuteSql("exec client_aha_delete_invoice '" + oPage.Request["inv_key"] + "'");
            PageClass pageClass = (PageClass)((Control)sender).Parent.Page;
            
            ErrorClass err = sso.Delete(cst_eml_address_dn);  //Delete account from OAM.

            Label label = (Label)pageClass.FindControl("VinceLink") as Label;
            string labelTxt;
            
            if (err.Number == 4) // If we wern't able to delete account
            {
                label.ForeColor = System.Drawing.Color.Red;
                label.Text = "Error with SSO. Contact AMS IT.";
            }
            else
            {
                DataSet dataSet = DataUtils.GetDataSet("exec dbo.client_aha_get_sso_url @cst_oldid = '" + cst_oldid + "', @add_user = '" + add_user + "'");
                if (dataSet == null || dataSet.Tables.Count <= 0 || dataSet.Tables[0].Rows.Count <= 0)
                    return;

                labelTxt = dataSet.Tables[0].Rows[0]["mailTo"].ToString();
                label.Text = labelTxt;
                //label.Text = "error " + err.Number + ".";
            }
 
            //oPage.oFacadeObject.SetValue("cst_cxa_key", dataSet.Tables[0].Rows[0]["cxa_key"].ToString());
            
            label.Visible = true;
        }
    }
}
