# DatabaseInvoke
A common C# class to manipulate ralational database system using ADO.NET. 

## usage:
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

> alternatively, you can directly start a project from the solution. By adding a new project into the solution, you can easily make use of the class. 

## Roadmap

^_^ I have no idea. I am trying to keep the project simple to use. Issues and PRs are welcomed.

## Update notes

##### UPDATE 2017/07/06: Add DataReader way to read from database. DataReader is considered to be more efficient on memory usage compared to DataAdapter. It is useful when you do not need to keep a copy of data in the memory.

##### UPDATE 2016/12/12: Originally, there are a lot of *messagebox*s in the class. they indicate log or assert locations you can place. Since someone may be confused about *messagebox* statements in the class, i replace those statements with a function. Therefore you can implement your own logic. 