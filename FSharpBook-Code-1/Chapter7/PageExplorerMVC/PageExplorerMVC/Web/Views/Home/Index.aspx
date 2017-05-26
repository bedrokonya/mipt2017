<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FSharpMVC2.Web.Models.InputModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: ViewData["Message"] %></h2>
    <% using (Html.BeginForm())
       { %>
    <%: Html.TextBoxFor(model => model.Url)%>
    <% } %>
    <ul>
        <% if (ViewData["Urls"] != null)
           {
               foreach (var x in (string[])ViewData["Urls"])
               { %>
           <li><%=x%></li>
        <% }} %>
    </ul>
</asp:Content>
