using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tests.Models;
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
                const string insertSql = "INSERT INTO Dog (Age, Name, Weight) OUTPUT Inserted.Id VALUES (@Age, @Name, @Weight);";
                key = connection.ExecuteScalar<Guid>(insertSql, originalDog);
            }

            //And then re-read it back
            Dog fetchedDog;
            using (var connection = new SqlConnection(s_ConnectionString))
            {
                const string selectSql = "SELECT Age, Name, Weight FROM Dog WHERE Id = @Id;";
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

            var key = s_DataSource.Insert("Dog", originalDog).ToGuid().Execute();

            //And then re-read it back
            var fetchedDog = s_DataSource.GetByKey("Dog", key).ToObject<Dog>().Execute();

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
            //DataTable - When you want to bind to WinForms or WPF data grids.
            DataTable dt = s_DataSource.Sql("select 1 A, 2 B union all select 3, 4").ToDataTable().Execute();

            //Table - When you want a lightweight alternative to DataTable
            Table table = s_DataSource.Sql("select 1 A, 2 B union all select 3, 4").ToTable().Execute();

            //Dynamic Objects - When you want the convience of dynamic typing
            List<dynamic> rows = s_DataSource.Sql("select 1 A, 2 B union all select 3, 4").ToDynamicCollection().Execute();

            Assert.AreEqual(1, rows[0].A);
            Assert.AreEqual(2, rows[0].B);
            Assert.AreEqual(3, rows[1].A);
            Assert.AreEqual(4, rows[1].B);
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
            //Bulk inserts are not yet supported. See issue #48

            s_DataSource.Insert("MyTable", new { colA = 1, colB = 1 }).Execute();
            s_DataSource.Insert("MyTable", new { colA = 2, colB = 2 }).Execute();
            s_DataSource.Insert("MyTable", new { colA = 3, colB = 3 }).Execute();
        }

        /// <summary>
        /// List Support
        /// </summary>
        [TestMethod]
        public void Example5_Dapper()
        {
            using (var connection = new SqlConnection(s_ConnectionString))
            {
                connection.Query<int>("select * from (select 1 as Id union all select 2 union all select 3) as X where Id in @Ids", new { Ids = new int[] { 1, 2, 3 } });
            }
        }

        /// <summary>
        /// List Support
        /// </summary>
        [TestMethod]
        public void Example5_Chain()
        {
            //List support is only available for primary keys.
            var posts = s_DataSource.GetByKey("Posts", 1, 2, 3).ToCollection<Post>().Execute();
        }

        /// <summary>
        /// Multi Mapping
        /// </summary>
        [TestMethod]
        public void Example6_Dapper()
        {

            SetupExample6();


            using (var connection = new SqlConnection(s_ConnectionString))
            {
                var sql =
  @"select * from Posts p 
            left join Users u on u.Id = p.OwnerId 
            Order by p.Id";

                var data = connection.Query<Post, User, Post>(sql, (p, user) => { p.Owner = user; return p; });
                var post = data.First();

                Assert.AreEqual("Sams Post1", post.Content);
                Assert.AreEqual(1, post.Id);
                Assert.AreEqual("Sam", post.Owner.Name);
                Assert.AreEqual(99, post.Owner.Id);
            }
        }

        /// <summary>
        /// Multi Mapping
        /// </summary>
        [TestMethod]
        public void Example6_Chain()
        {
            SetupExample6();


            var data = s_DataSource.From("PostsWithOwnersView").ToCollection<Post>().Execute();
            var post = data.First();

            Assert.AreEqual("Sams Post1", post.Content);
            Assert.AreEqual(1, post.Id);
            Assert.AreEqual("Sam", post.Owner.Name);
            Assert.AreEqual(99, post.Owner.Id);

        }

        void SetupExample6()
        {
            s_DataSource.Sql("DELETE FROM Posts").Execute();
            s_DataSource.Sql("DELETE FROM Users").Execute();

            s_DataSource.Insert("Users", new User() { Id = 99, Name = "Sam" }).Execute();
            s_DataSource.Insert("Posts", new Post() { Id = 1, Content = "Sams Post1", OwnerId = 99, Title = "About me" }).Execute();
        }

        /// <summary>
        /// Multiple Results
        /// </summary>
        [TestMethod]
        public void Example7_Dapper()
        {
            var sql =
@"
select * from Sales.Customer where CustomerKey = @id
select * from Sales.[Order] where CustomerKey = @id
select * from Sales.[Return] where CustomerKey = @id";

            var selectedId = 1;

            using (var connection = new SqlConnection(s_ConnectionString))
            {
                using (var multi = connection.QueryMultiple(sql, new { id = selectedId }))
                {
                    var customer = multi.Read<Customer>().SingleOrDefault();
                    var orders = multi.Read<Order>().ToList();
                    var returns = multi.Read<Return>().ToList();

                }
            }
        }

        /// <summary>
        /// Multiple Results
        /// </summary>
        [TestMethod]
        public void Example7_Chain()
        {
            var sql =
@"
select * from Sales.Customer where CustomerKey = @id
select * from Sales.[Order] where CustomerKey = @id
select * from Sales.[Return] where CustomerKey = @id"; ;

            var selectedId = 1;

            //Note: This can be reduced to one line if C# 7 supports multiple returns
            var tableSet = s_DataSource.Sql(sql, new { id = selectedId }).ToTableSet("Customer", "Order", "Return").Execute();
            var customer = tableSet["Customer"].ToObjects<Customer>().SingleOrDefault();
            var orders = tableSet["Order"].ToObjects<Order>();
            var returns = tableSet["Return"].ToObjects<Return>();
        }

        /// <summary>
        /// Stored Procedures
        /// </summary>
        [TestMethod]
        public void Example8_Dapper()
        {
            using (var cnn = new SqlConnection(s_ConnectionString))
            {
                var user = cnn.Query<User>("spGetUser", new { Id = 1 }, commandType: CommandType.StoredProcedure).SingleOrDefault();
            }

            //With OUT parameters
            using (var cnn = new SqlConnection(s_ConnectionString))
            {
                var p = new DynamicParameters();
                p.Add("@a", 11);
                p.Add("@b", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@c", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                cnn.Execute("spMagicProc", p, commandType: CommandType.StoredProcedure);

                int b = p.Get<int>("@b");
                int c = p.Get<int>("@c");

                Assert.AreEqual(10, b);
                Assert.AreEqual(5, c);
            }

        }

        /// <summary>
        /// Stored Procedures
        /// </summary>
        [TestMethod]
        public void Example8_Chain()
        {
            var user = s_DataSource.Procedure("spGetUser", new { Id = 1 }).ToObject<User>(RowOptions.AllowEmptyResults).Execute();

            //With OUT parameters
            var p = new List<SqlParameter>();
            p.Add(new SqlParameter("@a", 11));
            p.Add(new SqlParameter("@b", SqlDbType.Int) { Direction = ParameterDirection.Output });
            p.Add(new SqlParameter("@c", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue });

            s_DataSource.Procedure("spMagicProc", p).Execute();

            int b = (int)p[1].Value;
            int c = (int)p[2].Value;

            Assert.AreEqual(10, b);
            Assert.AreEqual(5, c);
        }


        /// <summary>
        /// Ansi Strings and varchar
        /// </summary>
        [TestMethod]
        public void Example9_Dapper()
        {
            using (var cnn = new SqlConnection(s_ConnectionString))
            {
                var users = cnn.Query<User>("select * from Users where Name = @Name", new { Name = new DbString { Value = "abcde", IsFixedLength = true, Length = 50, IsAnsi = true } });
            }



        }

        /// <summary>
        /// Ansi Strings and varchar
        /// </summary>
        [TestMethod]
        public void Example9_Chain()
        {
            //We automatically use ANSI strings if the column is a char or varChar.
            var users = s_DataSource.From("Users", new { Name = "abcde" }).ToCollection<User>().Execute();
        }
    }
}



