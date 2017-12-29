using ConferenceModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class hd_DefaultHD : System.Web.UI.Page
{
    #region 字段

    /// <summary>
    /// 用户名称
    /// </summary>
    string UserName = string.Empty;

    /// <summary>
    /// 会议名称
    /// </summary>
    string ConferenceName = string.Empty;

    /// <summary>
    /// 用户名称关键字
    /// </summary>
    string UserNameKey = "Parameters";

    /// <summary>
    /// 会议名称关键字
    /// </summary>
    string ConferenceNameKey = "ConferenceName";

    #endregion

    #region 持久层资源

    /// <summary>
    /// 信息交流服务地址
    /// </summary>
    static string ConferenceAudioWebServiceAddress = System.Configuration.ConfigurationManager.AppSettings["ConferenceAudioWebServiceAddress"];

    /// <summary>
    /// 参会人头像存储地址
    /// </summary>
    static string ConferencePersongImgAddress = System.Configuration.ConfigurationManager.AppSettings["ConferencePersongImgAddress"];

    /// <summary>
    /// 参会人头像文件类型
    /// </summary>
    static string ConferencePersongImgExetion = System.Configuration.ConfigurationManager.AppSettings["ConferencePersongImgExetion"];

    /// <summary>
    /// queryString参数分割符
    /// </summary>
    static string QueryStringParamSplit = System.Configuration.ConfigurationManager.AppSettings["QueryStringParamSplit"];


    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString.Keys.Count >= 2)
        {
           
        }

        string CMD = Request.Form["CMD"];
        if (CMD == "submit1")
        {
            string TextBox1value = Request.Form["TextBox1value"];
            string username = Request.Form["username"];
            string ConferenceName = Request.Form["ConferenceName"];


            //ConferenceName = URLDecoder.decode(ConferenceName, "utf-8");

            //byte[] buffer = Encoding.ASCII.GetBytes(ConferenceName);
            //string strDest = Encoding.GetEncoding("UTF-8").GetString(buffer);

            //NameValueCollection nc = HttpUtility.ParseQueryString(Request.Url.AbsoluteUri, Encoding.GetEncoding("utf-8")); 
            ////string sort = nc["Sort"]; 
            ////string zgdw = nc["Zgdw"];

            //string TextBox1value = nc["TextBox1value"];
            //string username = nc["username"];
            //string ConferenceName = nc["ConferenceName"];

            //获取用户参数
            this.UserName = username;

            //获取会议信息
            this.ConferenceName = ConferenceName;
            //服务初始化
            ModelManage.ClientInit(ConferenceAudioWebServiceAddress, ConferenceModel.Enum.ClientModelType.ConferenceAudio, null, null, null);

            SubmitHelper(TextBox1value);

        }


    }

    #region 信息提交

    /// <summary>
    /// 信息提交
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SubmitHelper(string Message)
    {
        if (!string.IsNullOrEmpty(this.UserName) && !string.IsNullOrEmpty(this.ConferenceName))
        {
            //当前时间
            var dateNow = DateTime.Now.ToLongTimeString();
            //生成的消息格式
            var header = dateNow + "  " + this.UserName;

            ConferenceModel.ConferenceAudioWebservice.ConferenceAudioItemTransferEntity conferenceAudioItemTransferEntity = new ConferenceModel.ConferenceAudioWebservice.ConferenceAudioItemTransferEntity()
            {
                AddAuthor = this.UserName,
                MessageHeader = header,
                AudioMessage = Message,
                AudioUrl = string.Empty,
                PersonalImg = ConferencePersongImgAddress + this.UserName + ConferencePersongImgExetion,

            };

           

            ModelManage.ConferenceAudio.Add(this.ConferenceName, new Action<bool>((successed) =>
            {
                Response.Write(Serialize(new { Data = "true" }));

            }), conferenceAudioItemTransferEntity);
        }

    }

    #endregion

    public  string Serialize(object obj)
    {
        JavaScriptSerializer js = new JavaScriptSerializer();
        return js.Serialize(obj);
    }
}