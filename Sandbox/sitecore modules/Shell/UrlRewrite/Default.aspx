<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AndysPlayground.sitecore_modules.Shell.UrlRewrite.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="background-color: white;">
    <style>
        .wrapperForm div {
            padding: 20px;
        }

        .wrapperForm table {
            width: 100%;
        }

        .wrapperForm table tr.matched {
            color: green;
            font-weight: bolder;
        }
    </style>

    <form id="form1" runat="server" class="wrapperForm">
        <div>
            <asp:TextBox ID="txtUrl" runat="server"></asp:TextBox>
            <asp:Button ID="btnSubmit" runat="server" Text="Run" UseSubmitBehavior="True" />
        </div>

        <div id="divResults" runat="server">

            <table>
                <thead>
                    <tr>
                        <th>Name</th><th>Path</th><th>Original Url</th><th>Rewritten Url</th><th>Match?</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater runat="server" ID="resultsRepeater" OnItemDataBound="resultsRepeater_OnItemDataBound">
                        <ItemTemplate>
                            <tr id="tableRow" runat="server"><td id="cellName" runat="server"></td><td id="cellPath" runat="server"></td><td id="cellOriginalUrl" runat="server"></td><td id="cellRewrittenUrl" runat="server"></td><td id="cellMatch" runat="server"></td></tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>

            <div id="txtFinalUrl" runat="server"></div>

        </div>

    </form>
</body>
</html>
