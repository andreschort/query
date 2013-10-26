<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Query.Sample.WebSite40._Default" %>

<%@ Register TagPrefix="query" Namespace="Query.Web" Assembly="Query.Web" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <query:GridExtender ID="GridExtender" runat="server" GridViewId="GridView" AutoFilterDelay="2000" Placeholder="Filter..."
                        OnFilter="GridExtender_Filter" OnSort="GridExtender_Sort" />
    <asp:GridView ID="GridView" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="True" AllowPaging="True" AllowSorting="False" PageSize="8">
        <Columns>
            <query:TextField Name="FullName" HeaderText="Full name" OnClick="FullName_Click" />
            
            <query:TextField Name="Nombre" HeaderText="Nombre"
                             UrlFormat="https://www.google.com.ar/search?q={0} {1}" UrlFields="Nombre, Apellido" />
            <query:TextField Name="Apellido" HeaderText="Apellido" />
            <query:TextField Name="Dni" HeaderText="Dni" />
            <query:DropDownField Name="EstadoCivil" HeaderText="Estado civil" />
            <query:TextField Name="Edad" HeaderText="Edad" />
            <query:TextField Name="Salario" HeaderText="Salario" />
            <query:DateField Name="FechaNacimiento" HeaderText="Fecha Nacimiento" Format="d" />
            <query:TextField Name="AttachmentCount" HeaderText="Number of attachments" />
            <query:TextField Name="Cuit" HeaderText="CUIT" />
            <query:TextField Name="AverageHourlyWage" HeaderText="Wage" />
            <query:DynamicField Name="Dynamic" HeaderText="Dynamic" FieldType="Text" />
        </Columns>
        <PagerSettings Mode="NumericFirstLast" PageButtonCount="4"  FirstPageText="First" LastPageText="Last"/>
    </asp:GridView>
    <asp:LinkButton runat="server" OnClick="Link_Click" ID="Link" Text="Link"></asp:LinkButton>
    <asp:ObjectDataSource ID="OdsEmpleado" runat="server" EnablePaging="True"
                          SelectMethod="GetAll" SelectCountMethod="GetCount"
                          MaximumRowsParameterName="maximumRows"
                          StartRowIndexParameterName="startRowIndex"
                          TypeName="Query.Sample.WebSite40.EmpleadoService"
                          OnObjectCreating="OdsEmpleado_ObjectCreating"
                          OnSelecting="OdsEmpleado_ObjectSelecting"
                          OnSelected="OdsEmpleado_ObjectSelected" />
</asp:Content>
