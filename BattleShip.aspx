<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BattleShip.aspx.cs" Inherits="WebApplication.BattleShip" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>BattleShip</title>
    
    <style type="text/css">
    .divOuter{
        display:inline;
        text-align:center;
    }

    .divInner1, .divInner2, .divInner3{
        border: 1px solid;
        float:left;
        width:550px;
        height:530px;
        margin-left:3px;
        margin-right:15px;
    }
    .cent{
        border: 1px solid;
        float:left;
        margin-left:15px;
        margin-top:5px;
    }
    .fullwidth{
        
        float:left;
        margin-left:15px;
        margin-top:25px;
        width:1500px;
    }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class='fullwidth'>
            
        </div>
    <div class='divInner1' runat="server" id="gv1Holder">
    </div>
    <div class='divInner2' runat="server" id="gv2Holder">
    </div>
    <div class='divInner3' runat="server" id="logsHolder"> . .  Logs >
    </div>
        <div>
            <input runat="server" type="hidden" id="inhAction" />
            <input runat="server" type="hidden"  id="inhRow" />
            <input runat="server" type="hidden"  id="inhColumn" />
        </div>
        <div class='fullwidth'>
            <asp:Label id="lblstatus" Text="Place Destroyer (2 cells)" runat="server" Font-Size="Large" />
        </div>
        <div class='fullwidth' runat="server" id="divButtonsHolder">
            
        </div>
    </form>
</body>
</html>
