using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using WebMatrix.Data;

/// <summary>
/// Summary description for PostHandler
/// </summary>
public class MainPostHandler : IHttpHandler
{
    public MainPostHandler()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public bool IsReusable
    {
        get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
        var email = context.Request.Form["entryEmail"];
        var entryTitle = context.Request.Form["entryTitle"];
        var entryDescription = context.Request.Form["entryDescription"];

        var connectionString = "DefaultConnection";
        using (var db = Database.Open(connectionString))
        {
            var sql = "INSERT INTO Entries (Email, Title, Description) " +
                "VALUES (@0, @1, @2)";
            db.Execute(sql, email, entryTitle, entryDescription);
        }

        WebMail.Send("wandoch.adam@gmail.com", "New entry in the DanceClassesDB", 
                     $"Title: {entryTitle} From: {email} " +
                     $"Wrote: {entryDescription} [---TimeStamp: {DateTime.Now}---]");

        context.Response.Redirect("~/Default");
    }

    public static IEnumerable<dynamic> GetEntries()
    {
        var connectionString = "DefaultConnection";

        using (var db = Database.Open(connectionString))
        {
            var sql = "SELECT * FROM Entries";
            return db.Query(sql);
        }
    }
}