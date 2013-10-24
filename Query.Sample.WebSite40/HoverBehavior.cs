using System;
using System.Web.UI.WebControls;

namespace Bollore.DioutiLight.UI.WebSite.Common.WebControls
{
    public class HoverBehavior : WebControl
    {
        public string Target { get; set; }

        public string Container { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string targetId = this.NamingContainer.FindControl(this.Target).ClientID;

            string containerId = string.IsNullOrEmpty(this.Container)
                                     ? this.Parent.ClientID
                                     : this.NamingContainer.FindControl(this.Container).ClientID;

            var js = @"$('#BUTTON_ID').hide();
                       $('#CONTAINER_ID').hover(function() {
                           $('#BUTTON_ID').show();
                       }, function() {
                           $('#BUTTON_ID').hide();
                       });";

            JSUtil.AddLoad(this, this.ClientID, js.Replace("BUTTON_ID", targetId).Replace("CONTAINER_ID", containerId));
        }
    }
}