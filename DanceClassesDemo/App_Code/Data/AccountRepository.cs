using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using WebMatrix.Data;

/// <summary>
/// Summary description for UserRepository
/// </summary>
public class AccountRepository
{
    private static readonly string _connectionString = "DefaultConnection";

    public static dynamic Get(int id)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Accounts WHERE AccountID = @0";
            return db.QuerySingle(sql, id);
        }
    }

    public static dynamic Get(string username)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Accounts WHERE Username = @0";
            return db.QuerySingle(sql, username);
        }
    }

    public static IEnumerable<dynamic> GetAll(string orderBy = null, string where = null)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Accounts";

            if (!string.IsNullOrEmpty(where))
            {
                sql += " WHERE " + where;
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                sql += " ORDER BY " + orderBy;
            }

            return db.Query(sql);
        }
    }

    public static void Add(string username, string password, string email, IEnumerable<int> roles)
    {
        using (var db = Database.Open(_connectionString))
        {
            var selectSql = "SELECT * FROM Accounts WHERE Username = @0";
            var user = db.QuerySingle(selectSql, username);
            if (user != null)
            {
                throw new Exception("User already exist!");
            }
            var sql = "INSERT INTO Accounts (Username, Password, Email) " +
                "VALUES (@0, @1, @2)";
            db.Execute(sql, username, password, email);

            user = db.QuerySingle(selectSql, username);

            AddRoles(user.AccountID, roles, db);
        }
    }

    public static void Edit(int id, string username, string password, string email, IEnumerable<int> roles)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "UPDATE Accounts SET Username = @0, Password = @1, " +
                "Email = @2 WHERE AccountID = @3";

            db.Execute(sql, username, password, email, id);

            //DeleteRoles(id, db);

            //AddRoles(id, roles, db);
        }
    }

    public static void Remove(string username)
    {
        var user = Get(username);
        if (user == null) { return; }
        using (var db = Database.Open(_connectionString))
        {
            var sql = "DELETE FROM Accounts WHERE Username = @0";
            db.Execute(sql, username);
            sql = "DELETE FROM AccountsRolesMap WHERE UserId = @0";
            db.Execute(sql, user.AccountID);
        }
    }

    private static void AddRoles(int userId, IEnumerable<int> roles, Database db)
    {
        if (!roles.Any())
        {
            return;
        }
        var sql = "INSERT INTO AccountsRolesMap (AccountID, RoleId) VALUES (@0, @1)";

        foreach (var role in roles)
        {
            db.Execute(sql, userId, role);
        }
    }

    private static void DeleteRoles(int id, Database db)
    {
        var sql = "DELETE FROM AccountsRolesMap WHERE AccountID = @0";
        db.Execute(sql, id);
    }
}