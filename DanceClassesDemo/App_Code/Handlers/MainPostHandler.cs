using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using WebMatrix.Data;

/// <summary>
/// Custom, general POST Handler
/// </summary>
public class MainPostHandler : IHttpHandler
{
    public MainPostHandler()
    {
        
    }

    public bool IsReusable
    {
        get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
        var email = context.Request.Form["entryEmail"];
        var title = context.Request.Form["entryTitle"];
        var description = context.Request.Form["entryDescription"];

        var connectionString = "DefaultConnection";
        using (var db = Database.Open(connectionString))
        {
            var sql = "INSERT INTO Entries (Email, Title, Description) " +
                "VALUES (@0, @1, @2)";
            db.Execute(sql, email, title, description);
        }

        WebMail.Send("wandoch.adam@gmail.com", "New entry in the DanceClassesDB", 
                     $"Title: {title} From: {email} " +
                     $"Wrote: {description} [---TimeStamp: {DateTime.Now}---]");

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