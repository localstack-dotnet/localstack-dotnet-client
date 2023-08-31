namespace LocalStack.Client.Functional.Tests.Scenarios.SNS.Models;

internal sealed record JobCreatedEvent(long JobId, int ServiceId, int UserId, string Description, string EventName = nameof(JobCreatedEvent));