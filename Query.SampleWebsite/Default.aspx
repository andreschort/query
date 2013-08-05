<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Query.SampleWebsite._Default" %>
<%@ Register TagPrefix="query" Namespace="Query.Web" Assembly="Query.Web" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>Modify this template to jump-start your ASP.NET application.</h2>
            </hgroup>
            <p>
                To learn more about ASP.NET, visit <a href="http://asp.net" title="ASP.NET Website">http://asp.net</a>.
                The page features <mark>videos, tutorials, and samples</mark> to help you get the most from ASP.NET.
                If you have any questions about ASP.NET visit
                <a href="http://forums.asp.net/18.aspx" title="ASP.NET Forum">our forums</a>.
            </p>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    
    <style>
        table { width:960px; }
        th input { width: 90%; }
    </style>

    <h3>We suggest the following:</h3>
        <query:GridExtender ID="GridExtender" runat="server" GridViewId="GridView" AutoFilterDelay="2000"
                            OnFilter="GridExtender_Filter" OnSort="GridExtender_Sort" />
    <asp:GridView ID="GridView" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="True">
        <Columns>
            <query:TextField Name="Nombre" HeaderText="Nombre" DataField="Nombre"
                             UrlFormat="https://www.google.com.ar/search?q={0} {1}" UrlFields="Nombre, Apellido" />
            <query:TextField Name="Apellido" HeaderText="Apellido" DataField="Apellido" />
            <query:TextField Name="Dni" HeaderText="Dni" DataField="Dni" />
            <query:DropDownField Name="EstadoCivil" HeaderText="Estado civil" DataField="EstadoCivil" DefaultValue="-1" />
            <query:TextField Name="Edad" HeaderText="Edad" DataField="Edad" />
            <query:TextField Name="Salario" HeaderText="Salario" DataField="Salario" />
            <query:DateField Name="FechaNacimiento" HeaderText="Fecha Nacimiento" DataField="FechaNacimiento" />
        </Columns>
    </asp:GridView>
    
    <asp:ObjectDataSource ID="OdsEmpleado" runat="server" EnablePaging="True"
            SelectMethod="GetAll" SelectCountMethod="GetAllCount"
            TypeName="Query.SampleWebSite.EmpleadoService"
            OnObjectCreating="OdsEmpleado_ObjectCreating"
            OnSelecting="OdsEmpleado_ObjectSelecting"
            OnSelected="OdsEmpleado_ObjectSelected"></asp:ObjectDataSource>
</asp:Content>
