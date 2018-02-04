# DatabaseInvoke
A common C# class to manipulate ralational database system using ADO.NET. 

## Usage:
1. compile the class library and add reference of the dlls including the "DatabaseInvoke.dll" and other dependencies (ADO Drivers of the databases)
to your project.
2. add using statement to your namespaces.
3. you can call the functions by using the following code.
4. connection resources will be cleaned automatically after the *using* code block

```C#
using (SqlManipulation sm = new SqlManipulation(Properties.Settings.Default.ConnectionString, (SqlType)(Enum.Parse(typeof(SqlType), Properties.Settings.Default.SqlType, true))))
{
    sm.Init();
    //call functions inside
    sm.ExcuteNonQuery(sql);
    sm.OracleBlobNonQuery(sqlBlob, Path);
}
```

5. **Dapper** is very convenient ORM library. The *DatabaseInvoke* project works perfectly with Dapper. You **MUST** add reference to Dapper manually in your project to make use of it.

```C#
//POCO Class
public class Point3D
{
    public int X;
    public int Y;
    public int Z;
}

using Dapper;

//namespace and class declaration here.

public IEnumerable<Point3D> GetList()
{
    IEnumerable<Point3D> list = new List<Point3D>();
    string sql = "SELECT X = @X,Y = @Y, Z = @Z from table";
    using (SqlManipulation sm = new SqlManipulation(ConnectionString, SqlType.PostgresQL))
    {
        sm.Init();
        list = sm.Connection.Query<Point3D>(sql);
    }
    return list;
}
```

> alternatively, you can directly start a project from the solution. By adding a new project into the solution, you can easily make use of the class. 

## Notes

* Use the correct library (.NET4.0 or .NET Standard 2.0) 
* Oracle users **might** encounter denpendency errors when targeting .NET Framework if they are using .NET Standard version of the library. Replacing *Oracle.ManagedDataAccess.Core* with *Oracle.ManagedDataAccess* via nuget will solve the problem.
* If you can not compile the project when you include the project source to your existing project. Nuget commandline *update-package --reinstall* will help you. 

## Roadmap

* Port to .Net Standard
* Publish a nuget version

## Update notes

##### UPDATE 2018/11/10: Reorganize projects and add new usage for the high performance library **Dapper**.

##### UPDATE 2018/02/05: Add pre-release version for .net standard 2.0. And legacy codes are renamed for .NET Framework 4.0 users.

##### UPDATE 2017/07/06: Add DataReader way to read from database. DataReader is considered to be more efficient on memory usage compared to DataAdapter. It is useful when you do not need to keep a copy of data in the memory.

##### UPDATE 2016/12/12: Originally, there are a lot of *messagebox*s in the class. they indicate log or assert locations you can place. Since someone may be confused about *messagebox* statements in the class, i replace those statements with a function. Therefore you can implement your own logic. 