﻿global using Amazon;
global using Amazon.Amplify;
global using Amazon.APIGateway;
global using Amazon.ApiGatewayManagementApi;
global using Amazon.ApiGatewayV2;
global using Amazon.AppConfig;
global using Amazon.AppSync;
global using Amazon.Athena;
global using Amazon.AutoScaling;
global using Amazon.AWSSupport;
global using Amazon.AWSMarketplaceMetering;
global using Amazon.Backup;
global using Amazon.Batch;
global using Amazon.CertificateManager;
global using Amazon.CloudFormation;
global using Amazon.CloudFront;
global using Amazon.CloudSearch;
global using Amazon.CloudTrail;
global using Amazon.CloudWatch;
global using Amazon.CloudWatchEvents;
global using Amazon.CloudWatchLogs;
global using Amazon.CodeCommit;
global using Amazon.CognitoIdentity;
global using Amazon.CognitoIdentityProvider;
global using Amazon.ConfigService;
global using Amazon.CostExplorer;
global using Amazon.DocDB;
global using Amazon.DynamoDBv2;
global using Amazon.EC2;
global using Amazon.ECR;
global using Amazon.ECS;
global using Amazon.EKS;
global using Amazon.ElastiCache;
global using Amazon.ElasticBeanstalk;
global using Amazon.ElasticFileSystem;
global using Amazon.ElasticLoadBalancing;
global using Amazon.ElasticLoadBalancingV2;
global using Amazon.ElasticMapReduce;
global using Amazon.Elasticsearch;
global using Amazon.EventBridge;
global using Amazon.FIS;
global using Amazon.Glue;
global using Amazon.IdentityManagement;
global using Amazon.IoT;
global using Amazon.IoTAnalytics;
global using Amazon.IotData;
global using Amazon.IoTEvents;
global using Amazon.IoTEventsData;
global using Amazon.IoTJobsDataPlane;
global using Amazon.IoTWireless;
global using Amazon.Kafka;
global using Amazon.KeyManagementService;
global using Amazon.KinesisAnalytics;
global using Amazon.KinesisAnalyticsV2;
global using Amazon.KinesisFirehose;
global using Amazon.LakeFormation;
global using Amazon.Lambda;
global using Amazon.MediaConvert;
global using Amazon.MediaStore;
global using Amazon.MediaStoreData;
global using Amazon.MWAA;
global using Amazon.Neptune;
global using Amazon.OpenSearchService;
global using Amazon.Organizations;
global using Amazon.QLDB;
global using Amazon.RDS;
global using Amazon.RDSDataService;
global using Amazon.Redshift;
global using Amazon.RedshiftDataAPIService;
global using Amazon.ResourceGroups;
global using Amazon.ResourceGroupsTaggingAPI;
global using Amazon.Route53;
global using Amazon.Route53Resolver;
global using Amazon.Runtime;
global using Amazon.S3;
global using Amazon.S3Control;
global using Amazon.SageMaker;
global using Amazon.SageMakerRuntime;
global using Amazon.SecretsManager;
global using Amazon.SecurityToken;
global using Amazon.ServerlessApplicationRepository;
global using Amazon.ServiceDiscovery;
global using Amazon.SimpleEmail;
global using Amazon.SimpleEmailV2;
global using Amazon.SimpleNotificationService;
global using Amazon.SimpleSystemsManagement;
global using Amazon.SimpleWorkflow;
global using Amazon.SQS;
global using Amazon.StepFunctions;
global using Amazon.TimestreamQuery;
global using Amazon.TimestreamWrite;
global using Amazon.Transfer;
global using Amazon.WAF;
global using Amazon.WAFV2;
global using Amazon.XRay;
global using Amazon.MQ;
global using Amazon.TranscribeService;

global using LocalStack.Client.Contracts;
global using LocalStack.Client.Exceptions;
global using LocalStack.Client.Models;
global using LocalStack.Client.Options;

global using System;
global using System.Reflection;

global using Amazon.QLDBSession;

global using Xunit;