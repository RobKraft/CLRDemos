using System.Data;
using System.Data.SqlTypes;
using System.Net;
using System.IO;
using Microsoft.SqlServer.Server;
using System.Text;

//Documentation: https://www.sqlserversecurity.com/blog/clrapps/
public partial class StoredProcedures //To reference the code in SQL, will be DLL name + class name + method name
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void HelloWorld()
    {

        string strContent = "Hello World";

        SqlPipe pipe = SqlContext.Pipe; //We use a "pipe" to return our response to SQL Server
        SqlMetaData[] cols = new SqlMetaData[1]; //We use SqlMetaData to define columns of the results we return.
        cols[0] = new SqlMetaData("FullJson", SqlDbType.VarChar, 8000);

        SqlDataRecord record = new SqlDataRecord(cols); 
        record.SetSqlString(0, new SqlString(strContent));
        pipe.Send(record);
    }


    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void GetSQLServerVersions()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sqlserverinspector.com/api/currentsqlserverversions/");
        request.Method = "GET";
        request.ContentLength = 0;
        request.Credentials = CredentialCache.DefaultCredentials;
        request.ContentType = "application/json";
        request.Accept = "application/json";

        string strContent = "";
        //strContent += "1. App Ran" + Environment.NewLine;
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            //strContent += "2. Got response" + Environment.NewLine;
            using (Stream receiveStream = response.GetResponseStream())
            {
                //strContent += "3. Read stream" + Environment.NewLine;
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    //strContent += "4. read stream 2" + Environment.NewLine;
                    strContent += readStream.ReadToEnd();
                }
            }
        }
            
        SqlPipe pipe = SqlContext.Pipe;
        SqlMetaData[] cols = new SqlMetaData[1];
        cols[0] = new SqlMetaData("FullJson", SqlDbType.VarChar, 8000);

        SqlDataRecord record = new SqlDataRecord(cols);
        record.SetSqlString(0, new SqlString(strContent));
        pipe.Send(record);
    }
    //SQL for creating the assembly and proc in SQL Server
    /*
     *
DROP PROCEDURE spGetSQLVersions
DROP ASSEMBLY CLRDemos
go
CREATE ASSEMBLY CLRDemos     
        FROM 'C:\Demo\CLRDemos.dll'     
        WITH PERMISSION_SET = SAFE ; 
go
CREATE PROCEDURE spGetSQLVersions     
AS EXTERNAL NAME CLRDemos.StoredProcedures.GetSQLServerVersions;
go
CREATE PROCEDURE dbo.spJustReturnAString     
AS EXTERNAL NAME CLRDemos.StoredProcedures.JustReturnAString;
     */

    //SQL for running spGetSQLVersions with a formatted response:
    /*
     declare @tempJson table (jsonData varchar(max));
insert @tempJson
execute spGetSQLVersions
DECLARE @json VARCHAR(MAX);
select @json= jsonData from @tempJson
select * from OPENJSON(@json)
  WITH (
    commonName VARCHAR(50) 'strict $.commonName',
    numericName VARCHAR(50) '$.numericName',
    rtm VARCHAR(50) '$.rtm',
    currentName VARCHAR(50) '$.currentName',
    currentv VARCHAR(50) '$.current'
  ); 
     
     */

}