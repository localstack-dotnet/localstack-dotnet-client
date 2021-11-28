namespace LocalStack.Client.Functional.Tests.Scenarios.DynamoDb;

public abstract class BaseDynamoDbScenario : BaseScenario
{
    protected const string TestTableName = "Movies";

    protected BaseDynamoDbScenario(TestFixture testFixture, string configFile, bool useServiceUrl = false)
        : base(testFixture, configFile, useServiceUrl)
    {
        DynamoDb = ServiceProvider.GetRequiredService<IAmazonDynamoDB>();
        DynamoDbContext = new DynamoDBContext(DynamoDb);
    }

    protected IAmazonDynamoDB DynamoDb { get; private set; }

    protected IDynamoDBContext DynamoDbContext { get; private set; }

    protected Task<CreateTableResponse> CreateTestTable(string tableName = null)
    {
        var postTableCreateRequest = new CreateTableRequest
        {
            AttributeDefinitions =
                new List<AttributeDefinition>
                {
                        new AttributeDefinition {AttributeName = nameof(MovieEntity.DirectorId), AttributeType = ScalarAttributeType.S},
                        new AttributeDefinition {AttributeName = nameof(MovieEntity.CreateDate), AttributeType = ScalarAttributeType.S},
                        new AttributeDefinition() {AttributeName = nameof(MovieEntity.MovieId), AttributeType = ScalarAttributeType.S}
                },
            TableName = tableName ?? TestTableName,
            KeySchema =
                new List<KeySchemaElement>()
                {
                        new KeySchemaElement() {AttributeName = nameof(MovieEntity.DirectorId), KeyType = KeyType.HASH},
                        new KeySchemaElement() {AttributeName = nameof(MovieEntity.CreateDate), KeyType = KeyType.RANGE}
                },
            GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
                {
                    new GlobalSecondaryIndex
                    {
                        Projection = new Projection {ProjectionType = ProjectionType.ALL},
                        IndexName = TestConstants.MoiveTableMovieIdGsi,
                        KeySchema = new List<KeySchemaElement> {new KeySchemaElement {AttributeName = nameof(MovieEntity.MovieId), KeyType = KeyType.HASH}},
                        ProvisionedThroughput = new ProvisionedThroughput {ReadCapacityUnits = 5, WriteCapacityUnits = 5}
                    }
                },
            ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 5, WriteCapacityUnits = 6 },
        };

        return DynamoDb.CreateTableAsync(postTableCreateRequest);
    }

    protected Task<DeleteTableResponse> DeleteTestTable(string tableName = null)
    {
        var deleteTableRequest = new DeleteTableRequest(tableName ?? TestTableName);

        return DynamoDb.DeleteTableAsync(deleteTableRequest);
    }
}
