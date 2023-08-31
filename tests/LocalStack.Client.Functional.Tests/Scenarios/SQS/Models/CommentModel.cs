#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace LocalStack.Client.Functional.Tests.Scenarios.SQS.Models;

public class CommentModel
{
    public Guid MovieId { get; set; }

    public string Comment { get; set; }

    public string CreateDate { get; set; }

    public Guid CommentId { get; set; }
}
