# DatabaseInvoke
A common C# class to manipulate ralational database system using ADO.NET

#usage:
1. compile the class library and add reference of the dlls including the "DatabaseInvoke.dll" and other dependencies (ADO Drivers of the databases)
to your project.
2. add using statement to your namespaces.
3. you can call the functions by using the following code.

<pre><code>
using (SqlManipulation sm = new SqlManipulation(Properties.Settings.Default.ConnectionString, (SqlType)(Enum.Parse(typeof(SqlType), Properties.Settings.Default.SqlType, true))))
            {
                sm.Init();
                //call functions inside
                sm.ExcuteNonQuery(sql);
                sm.OracleBlobNonQuery(sqlBlob, Path);
            }
</code></pre>

> alternatively, you can directly start a project from the solution. By adding a new project into the solution, you can easily make use of the class. 
