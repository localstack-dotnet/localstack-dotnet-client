namespace LocalStack.Client.Functional.Tests.Scenarios.DynamoDb.Entities;

public class MovieEntity
{
    [DynamoDBHashKey]
    public Guid DirectorId { get; set; }

    [DynamoDBRangeKey]
    public string CreateDate { get; set; }

    public Guid MovieId { get; set; }

    public string MovieName { get; set; }
}
