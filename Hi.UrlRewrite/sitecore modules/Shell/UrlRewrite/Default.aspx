<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Hi.UrlRewrite.sitecore_modules.Shell.UrlRewrite.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="" />
    <meta name="author" content="" />
    <title></title>

    <!-- Bootstrap core CSS -->
    <link href="/sitecore modules/Shell/UrlRewrite/Content/bootstrap.min.css" rel="stylesheet" />

    <!-- Custom styles for this template -->

    <!-- HTML5 shim and Respond.js for IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/html5shiv/3.7.2/html5shiv.min.js"></script>
      <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
    <![endif]-->


</head>
<body>

    <div class="container">

        <div class="row">

            <form id="form1" runat="server" role="form" class="form-horizontal">

                <div runat="server" id="divFormGroup" class="form-group">
                    <label for="<%= txtUrl.ClientID %>" class="control-label">Url</label>
                    <asp:TextBox ID="txtUrl" runat="server" placeholder="Please enter a fully qualified URL to test..." CssClass="form-control" CausesValidation="True"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="vldTxtUrl" runat="server" ErrorMessage="URL is required." ControlToValidate="txtUrl" Display="Dynamic" Text="URL is required." ValidationGroup="AllValidators" CssClass="control-label"></asp:RequiredFieldValidator>
                </div>

                <div class="form-group">
                    <asp:Button ID="btnSubmit" runat="server" Text="Run" UseSubmitBehavior="True" CssClass="btn btn-primary" />
                </div>

                <div id="divTable" runat="server" visible="False">
                    <table id="tblResults" class="table table-striped">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Path</th>
                                <th>Original Url</th>
                                <th>Rewritten Url</th>
                                <th>Match?</th>
                                <th>Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater runat="server" ID="resultsRepeater" OnItemDataBound="resultsRepeater_OnItemDataBound">
                                <ItemTemplate>
                                    <tr id="tableRow" runat="server">
                                        <td id="cellName" runat="server"></td>
                                        <td id="cellPath" runat="server"></td>
                                        <td id="cellOriginalUrl" runat="server"></td>
                                        <td id="cellRewrittenUrl" runat="server"></td>
                                        <td id="cellMatch" runat="server"></td>
                                        <td id="cellAction" runat="server"></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>

                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title">Final Result</h3>
                        </div>
                        <div class="panel-body">
                            <div id="txtFinalUrl" runat="server"></div>
                        </div>
                    </div>
                </div>

            </form>

        </div>
    </div>

    <!-- Bootstrap core JavaScript
    ================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
    <script src="/sitecore modules/Shell/UrlRewrite/Scripts/bootstrap.min.js"></script>

</body>
</html>
