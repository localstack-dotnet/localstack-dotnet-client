using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LocalStack.Client.Functional.Tests.Scenarios.DynamoDb;

public abstract class BaseDynamoDbScenario : BaseScenario
{
    protected const string TestTableName = "Movies";

    protected BaseDynamoDbScenario(TestFixture testFixture, ILocalStackFixture localStackFixture, string configFile = TestConstants.LocalStackConfig,
                                   bool useServiceUrl = false) : base(testFixture, localStackFixture, configFile, useServiceUrl)
    {
        DynamoDb = ServiceProvider.GetRequiredService<IAmazonDynamoDB>();
        DynamoDbContext = new DynamoDBContext(DynamoDb);
    }

    protected IAmazonDynamoDB DynamoDb { get; private set; }

    protected IDynamoDBContext DynamoDbContext { get; private set; }

    [Fact]
    public virtual async Task DynamoDbService_Should_Create_A_DynamoDb_Table_Async()
    {
        var tableName = Guid.NewGuid().ToString();
        CreateTableResponse createTableResponse = await CreateTestTableAsync(tableName);
        Assert.Equal(HttpStatusCode.OK, createTableResponse.HttpStatusCode);
    }

    [Fact]
    public virtual async Task DynamoDbService_Should_Delete_A_DynamoDb_Table_Async()
    {
        var tableName = Guid.NewGuid().ToString();
        await CreateTestTableAsync(tableName);

        DeleteTableResponse deleteTableResponse = await DeleteTestTableAsync(tableName);
        Assert.Equal(HttpStatusCode.OK, deleteTableResponse.HttpStatusCode);
    }

    [Fact]
    public virtual async Task DynamoDbService_Should_Add_A_Record_To_A_DynamoDb_Table_Async()
    {
        var tableName = Guid.NewGuid().ToString();
        var dynamoDbOperationConfig = new DynamoDBOperationConfig() { OverrideTableName = tableName };
        await CreateTestTableAsync(tableName);

        Table targetTable = DynamoDbContext.GetTargetTable<MovieEntity>(dynamoDbOperationConfig);

        var movieEntity = new Fixture().Create<MovieEntity>();
        string modelJson = JsonSerializer.Serialize(movieEntity);
        Document item = Document.FromJson(modelJson);

        await targetTable.PutItemAsync(item);
        dynamoDbOperationConfig.IndexName = TestConstants.MovieTableMovieIdGsi;
        List<MovieEntity> movieEntities =
            await DynamoDbContext.QueryAsync<MovieEntity>(movieEntity.MovieId, dynamoDbOperationConfig).GetRemainingAsync();

        Assert.True(movieEntity.DeepEquals(movieEntities[0]));
    }

    [Fact]
    public virtual async Task DynamoDbService_Should_List_Records_In_A_DynamoDb_Table_Async()
    {
        var tableName = Guid.NewGuid().ToString();
        const int recordCount = 5;

        var dynamoDbOperationConfig = new DynamoDBOperationConfig() { OverrideTableName = tableName };
        await CreateTestTableAsync(tableName);

        Table targetTable = DynamoDbContext.GetTargetTable<MovieEntity>(dynamoDbOperationConfig);
        IList<MovieEntity> movieEntities = new Fixture().CreateMany<MovieEntity>(recordCount).ToList();
        List<Document> documents = movieEntities.Select(entity =>
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

        dynamoDbOperationConfig.IndexName = TestConstants.MovieTableMovieIdGsi;
        List<MovieEntity> returnedMovieEntities =
            await DynamoDbContext.ScanAsync<MovieEntity>(new List<ScanCondition>(), dynamoDbOperationConfig).GetRemainingAsync();

        Assert.NotNull(movieEntities);
        Assert.NotEmpty(movieEntities);
        Assert.Equal(recordCount, movieEntities.Count);
        Assert.All(returnedMovieEntities, movieEntity =>
        {
            MovieEntity entity = movieEntities.First(e => e.MovieId == movieEntity.MovieId);

            Assert.True(movieEntity.DeepEquals(entity));
        });
    }

    protected Task<CreateTableResponse> CreateTestTableAsync(string? tableName = null)
    {
        var postTableCreateRequest = new CreateTableRequest
        {
            AttributeDefinitions =
                new List<AttributeDefinition>
                {
                    new() { AttributeName = nameof(MovieEntity.DirectorId), AttributeType = ScalarAttributeType.S },
                    new() { AttributeName = nameof(MovieEntity.CreateDate), AttributeType = ScalarAttributeType.S },
                    new() { AttributeName = nameof(MovieEntity.MovieId), AttributeType = ScalarAttributeType.S },
                },
            TableName = tableName ?? TestTableName,
            KeySchema =
                new List<KeySchemaElement>()
                {
                    new() { AttributeName = nameof(MovieEntity.DirectorId), KeyType = KeyType.HASH },
                    new() { AttributeName = nameof(MovieEntity.CreateDate), KeyType = KeyType.RANGE },
                },
            GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
            {
                new()
                {
                    Projection = new Projection { ProjectionType = ProjectionType.ALL },
                    IndexName = TestConstants.MovieTableMovieIdGsi,
                    KeySchema = new List<KeySchemaElement> { new() { AttributeName = nameof(MovieEntity.MovieId), KeyType = KeyType.HASH } },
                    ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 5, WriteCapacityUnits = 5 }
                }
            },
            ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 5, WriteCapacityUnits = 6 },
        };

        return DynamoDb.CreateTableAsync(postTableCreateRequest);
    }

    protected Task<DeleteTableResponse> DeleteTestTableAsync(string? tableName = null)
    {
        var deleteTableRequest = new DeleteTableRequest(tableName ?? TestTableName);

        return DynamoDb.DeleteTableAsync(deleteTableRequest);
    }
}