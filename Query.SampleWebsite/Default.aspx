﻿<%@ Page Title="Query" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Query.SampleWebsite._Default" %>
<%@ Register TagPrefix="query" Namespace="Query.Web" Assembly="Query.Web" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    
    <style>
        table { width:960px; }
        th input { width: 90%; }
    </style>

    <query:GridExtender ID="GridExtender" runat="server" GridViewId="GridView" AutoFilterDelay="2000"
                        OnFilter="GridExtender_Filter" OnSort="GridExtender_Sort" />
    <asp:GridView ID="GridView" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="True" AllowPaging="True" AllowSorting="False" PageSize="8">
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
        <PagerSettings Mode="NumericFirstLast" PageButtonCount="4"  FirstPageText="First" LastPageText="Last"/>
    </asp:GridView>
    
    <asp:ObjectDataSource ID="OdsEmpleado" runat="server" EnablePaging="True"
            SelectMethod="GetAll" SelectCountMethod="GetCount"
            MaximumRowsParameterName="maximumRows"
            StartRowIndexParameterName="startRowIndex"
            TypeName="Query.SampleWebSite.EmpleadoService"
            OnObjectCreating="OdsEmpleado_ObjectCreating"
            OnSelecting="OdsEmpleado_ObjectSelecting"
            OnSelected="OdsEmpleado_ObjectSelected"></asp:ObjectDataSource>
</asp:Content>
