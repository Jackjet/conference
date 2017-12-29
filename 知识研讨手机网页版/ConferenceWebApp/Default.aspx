<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true" Async="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="Scripts/jquery-1.8.2.min.js"></script>
    <script src="hd/DefaultHD.aspx"></script>
    <title></title>
    <style type="text/css">
        #TextBox1 {
            height: 201px;
            width: 687px;
        }
        #Button1 {
            height: 33px;
            width: 99px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <p>
        </p>
        <p>
            <textarea id="TextBox1" cols="20" rows="2"></textarea>
            
            <%-- <asp:TextBox ID="TextBox1" TextMode="MultiLine"  Width="800" Font-Size="25px"  Height="100" runat="server"></asp:TextBox>  --%>
        </p>

        <p>
            <input type="button" value="提交信息" id="Button1" onclick="submit1()" />
            <%--<asp:Button OnClick="提交信息_Click" ID="Button1" runat="server" Font-Size="25px" Text="提交信息" Height="61px" Width="222px" /--%>>   
                                                              
        </p>
    </form>
</body>
</html>
<script type="text/javascript">

    function submit1() {
      
        var username = "<%=UserName%>";
        var ConferenceName = "<%=ConferenceName%>";
                  
            var TextBox1value = $("#TextBox1").val();
            var postData = { CMD: "submit1", TextBox1value: TextBox1value, username: username, ConferenceName: ConferenceName };
            $.ajax({
                type: "Post",
                url: "hd/DefaultHD.aspx",
                data: postData,
                dataType: "text",
                success: function (returnVal) {
                    returnVal = $.parseJSON(returnVal);
                    var flg = returnVal.Data;
                    if (flg != "true") {
                        alert("发送信息发生异常");
                    }

                },
                error: function (errMsg) {
                    alert('数据加载失败！');
                }
            });
        }
            
    //}
</script>

