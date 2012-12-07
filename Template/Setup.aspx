<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Setup.aspx.cs" Inherits="Template.Setup" %>
<asp:Content ID="setupHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="setupContent" ContentPlaceHolderID="content" runat="server">
    <asp:Button runat="server" ID="btnResetSchema" OnClick="btnResetSchema_Click" />
    <asp:Button runat="server" Text="Generate Test DATA" ID="btnGenerateUsers" OnClick="btnGenerateUsers_Click" />
</asp:Content>
