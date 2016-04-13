using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Tortuga.Anchor.Modeling;
using Tortuga.Chain;

namespace Tests
{
    [TestClass]
    public class DapperReadmeVsChain

    {
        private static readonly SqlServerDataSource s_DataSource;
        private static readonly string s_ConnectionString;

        static DapperReadmeVsChain()
        {
            //This would be handled by your Dependency Injection framework
            s_ConnectionString = ConfigurationManager.ConnectionStrings["CodeFirstModels"].ConnectionString;
            s_DataSource = SqlServerDataSource.CreateFromConfig("CodeFirstModels");
        }

        public class Dog
        {
            public int? Age { get; set; }
            [IgnoreOnInsert, IgnoreOnUpdate]
            public Guid Id { get; set; }
            public string Name { get; set; }
            public float? Weight { get; set; }

            public int IgnoredProperty { get { return 1; } }
        }

        /// <summary>
        /// Execute a query and map the results to a strongly typed List
        /// </summary>
        [TestMethod]
        public void Example1_Dapper()
        {
            IEnumerable<Dog> dog;
            var guid = Guid.NewGuid();
            using (var connection = new SqlConnection(s_ConnectionString))
            {
                connection.Open();
                dog = connection.Query<Dog>("select Age = @Age, Id = @Id", new { Age = (int?)null, Id = guid });
            }

            Assert.AreEqual(1, dog.Count());
            Assert.IsNull(dog.First().Age);
            Assert.AreEqual(guid, dog.First().Id);


            //Make it more realistic by actually inserting a record
            var originalDog = new Dog() { Age = 2, Name = "Fido", Weight = 2.5f };

            Guid key;
            using (var connection = new SqlConnection(s_ConnectionString))
            {
                const string insertSql = "INSERT INTO dbo.Dog (Age, Name, Weight) OUTPUT Inserted.Id VALUES (@Age, @Name, @Weight);";
                key = connection.ExecuteScalar<Guid>(insertSql, originalDog);
            }

            //And then re-read it back
            Dog fetchedDog;
            using (var connection = new SqlConnection(s_ConnectionString))
            {
                const string selectSql = "SELECT Age, Name, Weight FROM dbo.Dog WHERE Id = @Id;";
                fetchedDog = connection.Query<Dog>(selectSql, new { Id = key }).Single();
            }

            Assert.AreEqual(originalDog.Age, fetchedDog.Age);
            Assert.AreEqual(originalDog.Name, fetchedDog.Name);
            Assert.AreEqual(originalDog.Weight, fetchedDog.Weight);
        }

        /// <summary>
        /// Execute a query and map the results to a strongly typed List
        /// </summary>
        [TestMethod]
        public void Example1_Chain()
        {
            var guid = Guid.NewGuid();
            var dog = s_DataSource.Sql("select Age = @Age, Id = @Id", new { Age = (int?)null, Id = guid }).ToCollection<Dog>().Execute(); ;

            Assert.AreEqual(1, dog.Count());
            Assert.IsNull(dog.First().Age);
            Assert.AreEqual(guid, dog.First().Id);


            //Or if you just want one:
            var aDog = s_DataSource.Sql("select Age = @Age, Id = @Id", new { Age = (int?)null, Id = guid }).ToObject<Dog>().Execute(); ;
            Assert.IsNull(aDog.Age);
            Assert.AreEqual(guid, aDog.Id);

            //Make it more realistic by actually inserting a record
            var originalDog = new Dog() { Age = 2, Name = "Fido", Weight = 2.5f };

            var key = s_DataSource.Insert("dbo.Dog", originalDog).ToGuid().Execute();

            //And then re-read it back
            var fetchedDog = s_DataSource.From("dbo.Dog", new { Id = key }).ToObject<Dog>().Execute();

            Assert.AreEqual(originalDog.Age, fetchedDog.Age);
            Assert.AreEqual(originalDog.Name, fetchedDog.Name);
            Assert.AreEqual(originalDog.Weight, fetchedDog.Weight);
        }


        /// <summary>
        /// Execute a query and map it to a list of dynamic objects
        /// </summary>
        [TestMethod]
        public void Example2_Dapper()
        {
            List<dynamic> rows;

            using (var connection = new SqlConnection(s_ConnectionString))
            {
                rows = connection.Query("select 1 A, 2 B union all select 3, 4").ToList();
            }

            Assert.AreEqual(1, ((int)rows[0].A));
            Assert.AreEqual(2, ((int)rows[0].B));
            Assert.AreEqual(3, ((int)rows[1].A));
            Assert.AreEqual(4, ((int)rows[1].B));
        }

        /// <summary>
        /// Execute a query and map it to a list of dynamic objects
        /// </summary>
        [TestMethod]
        public void Example2_Chain()
        {
            //Dictionary based approach
            var rows = s_DataSource.Sql("select 1 A, 2 B union all select 3, 4").ToDynamicCollection().Execute();

            Assert.AreEqual(1, ((int)rows[0].A));
            Assert.AreEqual(2, ((int)rows[0].B));
            Assert.AreEqual(3, ((int)rows[1].A));
            Assert.AreEqual(4, ((int)rows[1].B));
        }

        /// <summary>
        /// Execute a Command that returns no results
        /// </summary>
        [TestMethod]
        public void Example3_Dapper()
        {
            using (var connection = new SqlConnection(s_ConnectionString))
            {
                Assert.AreEqual(2, connection.Execute(@"
                      set nocount on 
                      create table #t(i int) 
                      set nocount off 
                      insert #t 
                      select @a a union all select @b 
                      set nocount on 
                      drop table #t", new { a = 1, b = 2 }));
            }
        }

        /// <summary>
        /// Execute a Command that returns no results
        /// </summary>
        [TestMethod]
        public void Example3_Chain()
        {
            //Truly returning no results
            s_DataSource.Sql(@"
                      set nocount on 
                      create table #t(i int) 
                      set nocount off 
                      insert #t 
                      select @a a union all select @b 
                      set nocount on 
                      drop table #t", new { a = 1, b = 2 }).Execute();


            //Or if you need the rows affected count
            Assert.AreEqual(2,
                       s_DataSource.Sql(@"
                      set nocount on 
                      create table #t(i int) 
                      set nocount off 
                      insert #t 
                      select @a a union all select @b 
                      set nocount on 
                      drop table #t", new { a = 1, b = 2 }).AsRowsAffected().Execute()
                );
        }

        /// <summary>
        /// Execute a Command multiple times
        /// </summary>
        [TestMethod]
        public void Example4_Dapper()
        {
            using (var connection = new SqlConnection(s_ConnectionString))
            {
                Assert.AreEqual(3, connection.Execute(@"insert MyTable(colA, colB) values (@a, @b)", new[] { new { a = 1, b = 1 }, new { a = 2, b = 2 }, new { a = 3, b = 3 } }));
            }
        }


        /// <summary>
        /// Execute a Command multiple times
        /// </summary>
        [TestMethod]
        public void Example4_Chain()
        {
            Assert.Inconclusive("Bulk inserts are not yet supported. See issue #48");
        }


    }
}



