using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using AutoFixture;

using LocalStack.Client.Extensions.Tests.Extensions;
using LocalStack.Client.Functional.Tests.Fixtures;
using LocalStack.Client.Functional.Tests.Scenarios.DynamoDb.Entities;

using Xunit;

namespace LocalStack.Client.Functional.Tests.Scenarios.DynamoDb
{
    [Collection(nameof(LocalStackCollection))]
    public class DynamoDbScenario : BaseDynamoDbScenario
    {
        public DynamoDbScenario(TestFixture testFixture, string configFile = TestConstants.LocalStackConfig) 
            : base(testFixture, configFile)
        {
        }

        [Fact]
        public async Task DynamoDbService_Should_Create_A_DynamoDb_Table()
        {
            var tableName = Guid.NewGuid().ToString();
            CreateTableResponse createTableResponse = await CreateTestTable(tableName);
            Assert.Equal(HttpStatusCode.OK, createTableResponse.HttpStatusCode);
        }

        [Fact]
        public async Task DynamoDbService_Should_Delete_A_DynamoDb_Table()
        {
            var tableName = Guid.NewGuid().ToString();
            await CreateTestTable(tableName);

            DeleteTableResponse deleteTableResponse = await DeleteTestTable(tableName);
            Assert.Equal(HttpStatusCode.OK, deleteTableResponse.HttpStatusCode);
        }

        [Fact]
        public async Task DynamoDbService_Should_Add_A_Record_To_A_DynamoDb_Table()
        {
            var tableName = Guid.NewGuid().ToString();
            var dynamoDbOperationConfig = new DynamoDBOperationConfig() {OverrideTableName = tableName};
            await CreateTestTable(tableName);

            Table targetTable = DynamoDbContext.GetTargetTable<MovieEntity>(dynamoDbOperationConfig);

            var movieEntity = new Fixture().Create<MovieEntity>();
            string modelJson = JsonSerializer.Serialize(movieEntity);
            Document item = Document.FromJson(modelJson);

            await targetTable.PutItemAsync(item);
            dynamoDbOperationConfig.IndexName = TestConstants.MoiveTableMovieIdGsi;
            List<MovieEntity> movieEntities = await DynamoDbContext.QueryAsync<MovieEntity>(movieEntity.MovieId, dynamoDbOperationConfig).GetRemainingAsync();

            Assert.True(movieEntity.DeepEquals(movieEntities.First()));
        }

        [Fact]
        public async Task DynamoDbService_Should_List_Records_In_A_DynamoDb_Table()
        {
            var tableName = Guid.NewGuid().ToString();
            const int recordCount = 5;

            var dynamoDbOperationConfig = new DynamoDBOperationConfig() {OverrideTableName = tableName};
            await CreateTestTable(tableName);

            Table targetTable = DynamoDbContext.GetTargetTable<MovieEntity>(dynamoDbOperationConfig);
            IList<MovieEntity> movieEntities = new Fixture().CreateMany<MovieEntity>(recordCount).ToList();
            List<Document> documents = movieEntities
                                  .Select(entity =>
                                  {
                                      string serialize = JsonSerializer.Serialize(entity);
                                      Document item = Document.FromJson(serialize);

                                      return item;
                                  })
                                  .ToList();

            foreach (Document document in documents)
            {
                await targetTable.PutItemAsync(document);
            }


            dynamoDbOperationConfig.IndexName = TestConstants.MoiveTableMovieIdGsi;
            List<MovieEntity> returnedMovieEntities = await DynamoDbContext.ScanAsync<MovieEntity>(new List<ScanCondition>(),dynamoDbOperationConfig).GetRemainingAsync();

            Assert.NotNull(movieEntities);
            Assert.NotEmpty(movieEntities);
            Assert.Equal(recordCount, movieEntities.Count);
            Assert.All(returnedMovieEntities, movieEntity =>
            {
                MovieEntity entity = movieEntities.First(e => e.MovieId == movieEntity.MovieId);

                Assert.True(movieEntity.DeepEquals(entity));
            });
        }
    }
}
