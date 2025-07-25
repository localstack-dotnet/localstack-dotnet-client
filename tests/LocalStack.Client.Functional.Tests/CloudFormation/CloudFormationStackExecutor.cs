﻿#pragma warning disable CA2254

using Tag = Amazon.CloudFormation.Model.Tag;

namespace LocalStack.Client.Functional.Tests.CloudFormation;

internal sealed class CloudFormationStackExecutor(IAmazonCloudFormation cloudFormationClient, CloudFormationResource cloudFormationResource, ILogger logger)
{
    // Name of the Tag for the stack to store the SHA256 of the CloudFormation template
    const string SHA256_TAG = "LocalStackAppHost_SHA256";

    // CloudFormation statuses for when the stack is in transition all end with IN_PROGRESS
    const string IN_PROGRESS_SUFFIX = "IN_PROGRESS";

    // Polling interval for checking status of CloudFormation stack when creating or updating the stack.
    TimeSpan StackPollingDelay { get; } = TimeSpan.FromSeconds(cloudFormationResource.StackPollingInterval);

    /// <summary>
    /// Using the template and configuration from the CloudFormationResource create or update
    /// the CloudFormation Stack.
    ///
    /// If a null is returned instead of the stack that implies the stack failed to be created or updated.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    internal async Task<Stack?> ExecuteTemplateAsync(CancellationToken cancellationToken = default)
    {
        Stack? existingStack = await FindStackAsync().ConfigureAwait(false);
        ChangeSetType changeSetType = await DetermineChangeSetTypeAsync(existingStack, cancellationToken).ConfigureAwait(false);

        string templateBody = await File.ReadAllTextAsync(cloudFormationResource.TemplatePath, cancellationToken);
        string computedSha256 = ComputeSha256(templateBody, cloudFormationResource.CloudFormationParameters);

        (List<Tag> tags, string? existingSha256) = SetupTags(existingStack, changeSetType, computedSha256);

        // Check to see if the template hasn't change. If it hasn't short circuit out.
        if (!cloudFormationResource.DisableDiffCheck && string.Equals(computedSha256, existingSha256, StringComparison.Ordinal))
        {
            logger.LogInformation("CloudFormation Template for CloudFormation stack {StackName} has not changed", cloudFormationResource.Name);

            return existingStack;
        }

        List<Parameter> templateParameters = SetupTemplateParameters();

        string? changeSetId = await CreateChangeSetAsync(changeSetType, templateParameters, templateBody, tags, cancellationToken).ConfigureAwait(false);

        if (changeSetId == null)
        {
            return existingStack;
        }

        Stack updatedStack = await ExecuteChangeSetAsync(changeSetId, changeSetType, cancellationToken).ConfigureAwait(false);

        return IsStackInErrorState(updatedStack) ? null : updatedStack;
    }

    /// <summary>
    /// Setup the tags collection by coping the any tags for existing stacks and then updating/adding the tag recording
    /// the SHA256 for template and parameters.
    /// </summary>
    /// <param name="existingStack"></param>
    /// <param name="changeSetType"></param>
    /// <param name="computedSha256"></param>
    /// <returns></returns>
    private static (List<Tag> tags, string? existingSha256) SetupTags(Stack? existingStack, ChangeSetType changeSetType, string computedSha256)
    {
        string? existingSha256 = null;
        var tags = new List<Tag>();

        if (changeSetType == ChangeSetType.UPDATE && existingStack != null)
        {
            tags = existingStack.Tags ?? new List<Tag>();
        }

        Tag? shaTag = tags.Find(x => string.Equals(x.Key, SHA256_TAG, StringComparison.Ordinal));

        if (shaTag != null)
        {
            existingSha256 = shaTag.Value;
            shaTag.Value = computedSha256;
        }
        else
        {
            tags.Add(new Tag { Key = SHA256_TAG, Value = computedSha256 });
        }

        return (tags, existingSha256);
    }

    /// <summary>
    /// Setup the template parameters from the CloudFormationResource to how the SDK expects parameters.
    /// </summary>
    /// <returns></returns>
    private List<Parameter> SetupTemplateParameters()
    {
        var templateParameters = new List<Parameter>();

        foreach (KeyValuePair<string, string> kvp in cloudFormationResource.CloudFormationParameters)
        {
            templateParameters.Add(new Parameter { ParameterKey = kvp.Key, ParameterValue = kvp.Value });
        }

        return templateParameters;
    }

    /// <summary>
    /// Create the CloudFormation change set.
    /// </summary>
    /// <param name="changeSetType"></param>
    /// <param name="templateParameters"></param>
    /// <param name="templateBody"></param>
    /// <param name="tags"></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    /// <exception cref="AwsProvisioningException"></exception>
    private async Task<string?> CreateChangeSetAsync(ChangeSetType changeSetType, List<Parameter> templateParameters, string templateBody, List<Tag> tags,
                                                     CancellationToken cancellationToken)
    {
        CreateChangeSetResponse changeSetResponse;

        try
        {
            logger.LogInformation("Creating CloudFormation change set.");
            var capabilities = new List<string>();

            if (cloudFormationResource.DisabledCapabilities.FirstOrDefault(x => string.Equals(x, "CAPABILITY_IAM", StringComparison.OrdinalIgnoreCase)) == null)
            {
                capabilities.Add("CAPABILITY_IAM");
            }

            if (cloudFormationResource.DisabledCapabilities.FirstOrDefault(x => string.Equals(x, "CAPABILITY_NAMED_IAM", StringComparison.OrdinalIgnoreCase)) == null)
            {
                capabilities.Add("CAPABILITY_NAMED_IAM");
            }

            if (cloudFormationResource.DisabledCapabilities.FirstOrDefault(x => string.Equals(x, "CAPABILITY_AUTO_EXPAND", StringComparison.OrdinalIgnoreCase)) == null)
            {
                capabilities.Add("CAPABILITY_AUTO_EXPAND");
            }

            var changeSetRequest = new CreateChangeSetRequest
            {
                StackName = cloudFormationResource.Name,
                Parameters = templateParameters,

                // Change set name needs to be unique. Since the changeset isn't be created directly by the user the name isn't really important.
                ChangeSetName = "LocalStack-AppHost-" + DateTime.UtcNow.Ticks,
                ChangeSetType = changeSetType,
                Capabilities = capabilities,
                RoleARN = cloudFormationResource.RoleArn,
                TemplateBody = templateBody,
                Tags = tags
            };

            changeSetResponse = await cloudFormationClient.CreateChangeSetAsync(changeSetRequest, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            throw new AwsProvisioningException($"Error creating change set: {e.Message}", e);
        }

        // The change set can take a few seconds to be reviewed and be ready to be executed.
        if (await WaitForChangeSetBeingAvailableAsync(changeSetResponse.Id, cancellationToken).ConfigureAwait(false))
        {
            return changeSetResponse.Id;
        }

        return null;
    }

    /// <summary>
    /// Once the change set is created execute the change set to apply the changes to the stack.
    /// </summary>
    /// <param name="changeSetId"></param>
    /// <param name="changeSetType"></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    /// <exception cref="AwsProvisioningException"></exception>
    private async Task<Stack> ExecuteChangeSetAsync(string changeSetId, ChangeSetType changeSetType, CancellationToken cancellationToken)
    {
        var executeChangeSetRequest = new ExecuteChangeSetRequest { StackName = cloudFormationResource.Name, ChangeSetName = changeSetId };

        DateTimeOffset timeChangeSetExecuted = DateTimeOffset.UtcNow;

        try
        {
            logger.LogInformation("Executing CloudFormation change set");

            // Execute the change set.
            await cloudFormationClient.ExecuteChangeSetAsync(executeChangeSetRequest, cancellationToken).ConfigureAwait(false);

            if (changeSetType == ChangeSetType.CREATE)
            {
                logger.LogInformation("Initiated CloudFormation stack creation for {Name}", cloudFormationResource.Name);
            }
            else
            {
                logger.LogInformation("Initiated CloudFormation stack update on {Name}", cloudFormationResource.Name);
            }
        }
        catch (Exception e)
        {
            throw new AwsProvisioningException($"Error executing CloudFormation change set: {e.Message}", e);
        }

        return await WaitStackToCompleteAsync(timeChangeSetExecuted, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///  Determine the type of change set to create (CREATE or UPDATE). If the stack is in an incomplete state
    ///  wait or delete the stack till it is in a ready state.
    /// </summary>
    /// <param name="stack"></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    /// <exception cref="AwsProvisioningException"></exception>
    private async Task<ChangeSetType> DetermineChangeSetTypeAsync(Stack? stack, CancellationToken cancellationToken)
    {
        ChangeSetType changeSetType;

        if (stack == null || stack.StackStatus == StackStatus.REVIEW_IN_PROGRESS || stack.StackStatus == StackStatus.DELETE_COMPLETE)
        {
            changeSetType = ChangeSetType.CREATE;
        }

        // If the status was ROLLBACK_COMPLETE that means the stack failed on initial creation
        // and the resources were cleaned up. It is safe to delete the stack so we can recreate it.
        else if (stack.StackStatus == StackStatus.ROLLBACK_COMPLETE)
        {
            await DeleteRollbackCompleteStackAsync(stack, cancellationToken).ConfigureAwait(false);
            changeSetType = ChangeSetType.CREATE;
        }

        // If the status was ROLLBACK_IN_PROGRESS that means the initial creation is failing.
        // Wait to see if it goes into ROLLBACK_COMPLETE status meaning everything got cleaned up and then delete it.
        else if (stack.StackStatus == StackStatus.ROLLBACK_IN_PROGRESS)
        {
            stack = await WaitForNoLongerInProgressAsync(cancellationToken).ConfigureAwait(false);

            if (stack != null && stack.StackStatus == StackStatus.ROLLBACK_COMPLETE)
            {
                await DeleteRollbackCompleteStackAsync(stack, cancellationToken).ConfigureAwait(false);
            }

            changeSetType = ChangeSetType.CREATE;
        }

        // If the status was DELETE_IN_PROGRESS then just wait for delete to complete
        else if (stack.StackStatus == StackStatus.DELETE_IN_PROGRESS)
        {
            await WaitForNoLongerInProgressAsync(cancellationToken).ConfigureAwait(false);
            changeSetType = ChangeSetType.CREATE;
        }

        // The Stack state is in a normal state and ready to be updated.
        else if (stack.StackStatus == StackStatus.CREATE_COMPLETE ||
                 stack.StackStatus == StackStatus.UPDATE_COMPLETE ||
                 stack.StackStatus == StackStatus.UPDATE_ROLLBACK_COMPLETE)
        {
            changeSetType = ChangeSetType.UPDATE;
        }

        // All other states means the Stack is in an inconsistent state.
        else
        {
            throw new AwsProvisioningException($"The stack's current state of {stack.StackStatus} is invalid for updating");
        }

        return changeSetType;
    }

    /// <summary>
    /// Check to see if the state of the stack indicates the executed change set failed.
    /// </summary>
    /// <param name="stack"></param>
    /// <returns></returns>
    private static bool IsStackInErrorState(Stack stack)
    {
        if (stack.StackStatus.ToString(CultureInfo.InvariantCulture).EndsWith("FAILED", StringComparison.OrdinalIgnoreCase) ||
            stack.StackStatus.ToString(CultureInfo.InvariantCulture).EndsWith("ROLLBACK_COMPLETE", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// If a stack is in the ROLLBACK_COMPLETE if failed during initial creation. There are no resources
    /// left in the stack and it is safe to delete. If the stack is not deleted the recreation of the stack will fail.
    /// </summary>
    /// <param name="stack"></param>
    /// <param name="cancellation">A <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    /// <exception cref="AwsProvisioningException"></exception>
    private async Task DeleteRollbackCompleteStackAsync(Stack stack, CancellationToken cancellation)
    {
        try
        {
            if (stack.StackStatus == StackStatus.ROLLBACK_COMPLETE)
            {
                await cloudFormationClient.DeleteStackAsync(new DeleteStackRequest { StackName = stack.StackName }, cancellation).ConfigureAwait(false);
            }

            await WaitForNoLongerInProgressAsync(cancellation).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            throw new AwsProvisioningException($"Error removing previous failed stack creation {stack.StackName}: {e.Message}", e);
        }
    }

    /// <summary>
    /// Wait till the stack transitions from an in progress state to a stable state.
    /// </summary>
    /// <param name="cancellation">A <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    /// <exception cref="AwsProvisioningException"></exception>
    private async Task<Stack?> WaitForNoLongerInProgressAsync(CancellationToken cancellation)
    {
        try
        {
            long start = DateTime.UtcNow.Ticks;
            Stack? currentStack = null;

            do
            {
                if (currentStack != null)
                {
                    logger.LogInformation(
                        "... Waiting for stack's state to change from {CurrentStackStackStatus}: {PadLeft} secs", currentStack.StackStatus, TimeSpan.FromTicks(DateTime.UtcNow.Ticks - start).TotalSeconds.ToString("0", CultureInfo.InvariantCulture).PadLeft(3));
                }

                await Task.Delay(StackPollingDelay, cancellation).ConfigureAwait(false);
                currentStack = await FindStackAsync().ConfigureAwait(false);
            } while (currentStack != null && currentStack.StackStatus.ToString(CultureInfo.InvariantCulture).EndsWith(IN_PROGRESS_SUFFIX, StringComparison.Ordinal));

            return currentStack;
        }
        catch (Exception e)
        {
            throw new AwsProvisioningException($"Error waiting for stack state change: {e.Message}", e);
        }
    }

    /// <summary>
    /// Wait for the change set to be created and in success state to begin executing.
    /// </summary>
    /// <param name="changeSetId"></param>
    /// <param name="cancellation">A <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    /// <exception cref="AwsProvisioningException"></exception>
    private async Task<bool> WaitForChangeSetBeingAvailableAsync(string changeSetId, CancellationToken cancellation)
    {
        try
        {
            var request = new DescribeChangeSetRequest { ChangeSetName = changeSetId };

            logger.LogInformation("... Waiting for change set to be reviewed");
            DescribeChangeSetResponse response;

            do
            {
                await Task.Delay(this.StackPollingDelay, cancellation).ConfigureAwait(false);
                response = await cloudFormationClient.DescribeChangeSetAsync(request, cancellation).ConfigureAwait(false);
            } while (response.Status == ChangeSetStatus.CREATE_IN_PROGRESS || response.Status == ChangeSetStatus.CREATE_PENDING);

            if (response.Status != ChangeSetStatus.FAILED)
            {
                return true;
            }

            // There is no code returned from CloudFormation to tell if failed because there is no changes so
            // the status reason has to be check for the string.
            if (response.StatusReason?.Contains("The submitted information didn't contain changes", StringComparison.InvariantCultureIgnoreCase) != true)
            {
                throw new AwsProvisioningException($"Failed to create CloudFormation change set: {response.StatusReason}");
            }

            logger.LogInformation("No changes detected for change set");

            return false;
        }
        catch (Exception e)
        {
            throw new AwsProvisioningException($"Error getting status of change set: {e.Message}", e);
        }
    }

    /// <summary>
    /// Wait for the CloudFormation stack to get to a stable state after creating or updating the stack.
    /// </summary>
    /// <param name="minTimeStampForEvents">Minimum timestamp for events.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    private async Task<Stack> WaitStackToCompleteAsync(DateTimeOffset minTimeStampForEvents, CancellationToken cancellationToken)
    {
        const int TIMESTAMP_WIDTH = 20;
        const int LOGICAL_RESOURCE_WIDTH = 40;
        const int RESOURCE_STATUS = 40;
        var mostRecentEventId = string.Empty;

        var waitingMessage = $"... Waiting for CloudFormation stack {cloudFormationResource.Name} to be ready";
        logger.LogInformation(waitingMessage);
        logger.LogInformation(new string('-', waitingMessage.Length));

        Stack stack;

        do
        {
            await Task.Delay(StackPollingDelay, cancellationToken).ConfigureAwait(false);

            // If we are in the WaitStackToCompleteAsync then we already know the stack exists.
            stack = (await FindStackAsync().ConfigureAwait(false))!;

            List<StackEvent> events = await GetLatestEventsAsync(minTimeStampForEvents, mostRecentEventId, cancellationToken).ConfigureAwait(false);

            if (events.Count > 0)
            {
                mostRecentEventId = events[0].EventId;
            }

            for (int i = events.Count - 1; i >= 0; i--)
            {
                var line = new StringBuilder();
                line.Append(events[i].Timestamp?.ToString("g", CultureInfo.InvariantCulture).PadRight(TIMESTAMP_WIDTH));
                line.Append(' ');
                line.Append(events[i].LogicalResourceId.PadRight(LOGICAL_RESOURCE_WIDTH));
                line.Append(' ');
                line.Append(events[i].ResourceStatus.ToString(CultureInfo.InvariantCulture).PadRight(RESOURCE_STATUS));

                if (!events[i].ResourceStatus.ToString(CultureInfo.InvariantCulture).EndsWith(IN_PROGRESS_SUFFIX, StringComparison.Ordinal) &&
                    !string.IsNullOrEmpty(events[i].ResourceStatusReason))
                {
                    line.Append(' ');
                    line.Append(events[i].ResourceStatusReason);
                }

                if (minTimeStampForEvents < events[i].Timestamp && events[i].Timestamp != null)
                {
                    minTimeStampForEvents = (DateTimeOffset)events[i].Timestamp!;
                }

                logger.LogInformation(line.ToString());
            }
        } while (!cancellationToken.IsCancellationRequested &&
                 stack.StackStatus.ToString(CultureInfo.InvariantCulture).EndsWith(IN_PROGRESS_SUFFIX, StringComparison.Ordinal));

        return stack;
    }

    private async Task<List<StackEvent>> GetLatestEventsAsync(DateTimeOffset minTimeStampForEvents, string mostRecentEventId, CancellationToken cancellationToken)
    {
        var noNewEvents = false;
        var events = new List<StackEvent>();
        DescribeStackEventsResponse? response = null;

        do
        {
            var request = new DescribeStackEventsRequest() { StackName = cloudFormationResource.Name };

            if (response != null)
            {
                request.NextToken = response.NextToken;
            }

            try
            {
                response = await cloudFormationClient.DescribeStackEventsAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new AwsProvisioningException($"Error getting events for CloudFormation stack: {e.Message}", e);
            }

            foreach (StackEvent? evnt in response.StackEvents)
            {
                if (string.Equals(evnt.EventId, mostRecentEventId, StringComparison.Ordinal) || evnt.Timestamp < minTimeStampForEvents)
                {
                    noNewEvents = true;

                    break;
                }

                events.Add(evnt);
            }
        } while (!noNewEvents && !string.IsNullOrEmpty(response.NextToken));

        return events;
    }

    private static string ComputeSha256(string templateBody, IDictionary<string, string> parameters)
    {
        string content = templateBody;

        if (parameters != null)
        {
            content += string.Join(';', parameters.Select(x => x.Key + "=" + x.Value).ToArray());
        }

        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(content));

        return Convert.ToHexString(bytes).ToUpperInvariant();
    }

    private async Task<Stack?> FindStackAsync()
    {
        await foreach (Stack? stack in cloudFormationClient.Paginators.DescribeStacks(new DescribeStacksRequest()).Stacks.ConfigureAwait(false))
        {
            if (string.Equals(cloudFormationResource.Name, stack.StackName, StringComparison.Ordinal))
            {
                return stack;
            }
        }

        return null;
    }
}