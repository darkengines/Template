<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="TestClassTable.aspx.cs" Inherits="Template.TestClassTable" %>
<asp:Content ID="testClassTableHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="testClassTableContent" ContentPlaceHolderID="content" runat="server">
    <de:ClassTable runat="server" Id="classTable" />
    <de:ClassTableEntityEditor runat="server" Id="entityEditor" />
</asp:Content>
