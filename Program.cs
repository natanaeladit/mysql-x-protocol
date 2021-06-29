using System;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI.CRUD;

namespace mysql_x_protocol
{
    class Program
    {
        static void Main(string[] args)
        {
            string schemaName = "my_schema";
            string collectionName = "my_collection";

            // Define the connection URL.
            string connectionURL = Environment.GetEnvironmentVariable("MYSQLX");

            // Create the session.
            Session session = MySQLX.GetSession(connectionURL);

            // Create the schema object.
            Schema schema = session.GetSchema(schemaName);
            if (!schema.ExistsInDatabase())
                schema = session.CreateSchema(schemaName);

            // Create the collection.
            Collection myCollection = schema.GetCollection(collectionName);
            if (!myCollection.ExistsInDatabase())
                myCollection = schema.CreateCollection(collectionName);

            // Insert documents into the collection.
            var doc1 = new { _id = generateId(), Name = "Susan" };
            var doc2 = "{ \"_id\": \" " + generateId() + " \", \"Name\": \"Joey\" }";
            var doc3 = new DbDoc("{ \"_id\": \"" + generateId() + "\" }");

            doc3.SetValue("Name", "Mark");
            Result r = myCollection.Add(doc1).Execute();
            Console.WriteLine(r.AutoIncrementValue);
            r = myCollection.Add(doc2).Add(doc3).Execute();
            Console.WriteLine(r.AutoIncrementValue);

            var result = myCollection.Add(new { _id = generateId(), Name = "Budi" }).Execute();

            result = myCollection.Add(new { Name = "Santoso", _id = generateId() }).Execute();

            // Find documents within the collection.
            DbDoc docParams = new DbDoc(new { name1 = "Susan", _id1 = "3" });
            DocResult foundDocs = myCollection.Find("Name = :name1 || _id = :_id1").Bind(docParams).Execute();

            while (foundDocs.Next())
            {
                Console.WriteLine(foundDocs.Current["_id"] + " " + foundDocs.Current["Name"]);
            }

            session.Close();

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        static string generateId()
        {
            string base64Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            base64Guid = base64Guid.Replace("==", "");
            return base64Guid;
        }
    }
}
