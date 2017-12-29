using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : Page
{

    #region 字段

    /// <summary>
    /// 用户名称
    /// </summary>
   public string UserName = string.Empty;

    /// <summary>
    /// 会议名称
    /// </summary>
   public string ConferenceName = string.Empty;

    

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {

        if (Request.QueryString.Keys.Count >= 2)
        {
            //获取用户参数
            this.UserName = Request.QueryString[0];

            //获取会议信息
            this.ConferenceName = Request.QueryString[1];
            //服务初始化
            
        }
    }
}