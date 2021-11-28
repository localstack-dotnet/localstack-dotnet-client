namespace LocalStack.Client.Functional.Tests.Scenarios.SNS.Models;

internal record JobCreatedEvent(long JobId, int ServiceId, int UserId, string Description, string EventName = nameof(JobCreatedEvent));