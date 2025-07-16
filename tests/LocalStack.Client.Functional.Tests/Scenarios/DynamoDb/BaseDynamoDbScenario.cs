using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LocalStack.Client.Functional.Tests.Scenarios.DynamoDb;

public abstract class BaseDynamoDbScenario : BaseScenario
{
    protected const string TestTableName = "Movies";

    protected BaseDynamoDbScenario(TestFixture testFixture, ILocalStackFixture localStackFixture, string configFile = TestConstants.LocalStackConfig,
                                   bool useServiceUrl = false) : base(testFixture, localStackFixture, configFile, useServiceUrl)
    {
        DynamoDb = ServiceProvider.GetRequiredService<IAmazonDynamoDB>();
        DynamoDbContext = new DynamoDBContextBuilder().WithDynamoDBClient(() => DynamoDb).Build();
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

        // Fix: Use GetTargetTableConfig instead of DynamoDBOperationConfig
        var getTargetTableConfig = new GetTargetTableConfig() { OverrideTableName = tableName };

        await CreateTestTableAsync(tableName);

        var describeResponse = await DynamoDb.DescribeTableAsync(new DescribeTableRequest(tableName));
        var gsiExists = describeResponse.Table.GlobalSecondaryIndexes?.Exists(gsi => gsi.IndexName == TestConstants.MovieTableMovieIdGsi) == true;

        if (!gsiExists)
        {
            var availableGsis = describeResponse.Table.GlobalSecondaryIndexes?.Select(g => g.IndexName).ToArray() ?? ["none"];

            throw new System.InvalidOperationException($"GSI '{TestConstants.MovieTableMovieIdGsi}' was not found on table '{tableName}'. " +
                                                       $"Available GSIs: {string.Join(", ", availableGsis)}");
        }

        // Fix: Cast to Table and use GetTargetTableConfig
        var targetTable = (Table)DynamoDbContext.GetTargetTable<MovieEntity>(getTargetTableConfig);

        var movieEntity = new Fixture().Create<MovieEntity>();
        string modelJson = JsonSerializer.Serialize(movieEntity);
        Document item = Document.FromJson(modelJson);

        await targetTable.PutItemAsync(item);

        // Fix: Use QueryConfig instead of DynamoDBOperationConfig
        var queryConfig = new QueryConfig() { OverrideTableName = tableName, IndexName = TestConstants.MovieTableMovieIdGsi };

        List<MovieEntity> movieEntities = await DynamoDbContext.QueryAsync<MovieEntity>(movieEntity.MovieId, queryConfig).GetRemainingAsync();

        Assert.True(movieEntity.DeepEquals(movieEntities[0]));
    }

    [Fact]
    public virtual async Task DynamoDbService_Should_List_Records_In_A_DynamoDb_Table_Async()
    {
        var tableName = Guid.NewGuid().ToString();
        const int recordCount = 5;

        // Fix: Use GetTargetTableConfig instead of DynamoDBOperationConfig
        var getTargetTableConfig = new GetTargetTableConfig() { OverrideTableName = tableName };
        await CreateTestTableAsync(tableName);

        // Fix: Cast to Table and use GetTargetTableConfig
        var targetTable = (Table)DynamoDbContext.GetTargetTable<MovieEntity>(getTargetTableConfig);
        List<MovieEntity> movieEntities = [.. new Fixture().CreateMany<MovieEntity>(recordCount)];
        List<Document> documents = [.. movieEntities.Select(entity =>
                                                {
                                                    string serialize = JsonSerializer.Serialize(entity);
                                                    Document item = Document.FromJson(serialize);

                                                    return item;
                                                }),];

        foreach (Document document in documents)
        {
            await targetTable.PutItemAsync(document);
        }

        // Fix: Use ScanConfig instead of DynamoDBOperationConfig
        var scanConfig = new ScanConfig() { OverrideTableName = tableName, IndexName = TestConstants.MovieTableMovieIdGsi };

        List<MovieEntity> returnedMovieEntities = await DynamoDbContext.ScanAsync<MovieEntity>(new List<ScanCondition>(), scanConfig).GetRemainingAsync();

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
            [
                new AttributeDefinition { AttributeName = nameof(MovieEntity.DirectorId), AttributeType = ScalarAttributeType.S },
                new AttributeDefinition { AttributeName = nameof(MovieEntity.CreateDate), AttributeType = ScalarAttributeType.S },
                new AttributeDefinition { AttributeName = nameof(MovieEntity.MovieId), AttributeType = ScalarAttributeType.S },
            ],
            TableName = tableName ?? TestTableName,
            KeySchema =
            [
                new KeySchemaElement { AttributeName = nameof(MovieEntity.DirectorId), KeyType = KeyType.HASH },
                new KeySchemaElement { AttributeName = nameof(MovieEntity.CreateDate), KeyType = KeyType.RANGE },
            ],
            GlobalSecondaryIndexes =
            [
                new GlobalSecondaryIndex
                {
                    Projection = new Projection { ProjectionType = ProjectionType.ALL },
                    IndexName = TestConstants.MovieTableMovieIdGsi,
                    KeySchema = [new KeySchemaElement { AttributeName = nameof(MovieEntity.MovieId), KeyType = KeyType.HASH }],
                    ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 5, WriteCapacityUnits = 5 },
                },

            ],
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