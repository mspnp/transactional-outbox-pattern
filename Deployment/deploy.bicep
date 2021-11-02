var location = resourceGroup().location

// Cosmos DB Account
resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: 'cosmos-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: true
    publicNetworkAccess: 'Enabled'
  }
}

// Cosmos DB
resource cosmosDbDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-03-15' = {
  name: '${cosmosDbAccount.name}/tobp'
  location: location
  properties: {
    resource: {
      id: 'tobp'
    }
    options: {
      autoscaleSettings: {
        maxThroughput: 4000
      }
    }
  }
}

// Data Container
resource containerData 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-03-15' = {
  name: '${cosmosDbDatabase.name}/data'
  location: location
  properties: {
    resource: {
      id: 'data'
      partitionKey: {
        paths: [
          '/partitionKey'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        automatic: true
        indexingMode: 'consistent'
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
      defaultTtl: -1
    }
  }
}

// Leases Container
resource containerLeases 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-03-15' = {
  name: '${cosmosDbDatabase.name}/leases'
  location: location
  properties: {
    resource: {
      id: 'leases'
      partitionKey: {
        paths: [
          '/id'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        automatic: true
        indexingMode: 'consistent'
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
    }
  }
}

// ServiceBus
resource sb 'Microsoft.ServiceBus/namespaces@2017-04-01' = {
  name: 'sb-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  // Contacts Topic
  resource sbtContacts 'topics@2017-04-01' = {
    name: 'sbt-contacts'
    properties: {
      defaultMessageTimeToLive: 'P2D'
      maxSizeInMegabytes: 1024
      requiresDuplicateDetection: false
      duplicateDetectionHistoryTimeWindow: 'PT10M'
      enableBatchedOperations: false
      supportOrdering: false
      autoDeleteOnIdle: 'P2D'
      enablePartitioning: true
      enableExpress: false
    }
    
    resource sbtTestSubscription 'subscriptions@2017-04-01' = {
      name: 'testSubscription'
      properties: {
        lockDuration: 'PT5M'
        requiresSession: true
        defaultMessageTimeToLive: 'P2D'
        deadLetteringOnMessageExpiration: true
        maxDeliveryCount: 10
        enableBatchedOperations: false
        autoDeleteOnIdle: 'P2D'
      }
    }
  }
}

// Storage Account for Function
resource stgForFunctions 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'stfn${take(uniqueString(resourceGroup().id), 11)}'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

// ApplicationInsights
resource appi 'Microsoft.Insights/components@2015-05-01' = {
  name: 'appi-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

// Dynamic Hostingplan
resource hostingPlan 'Microsoft.Web/serverfarms@2020-10-01' = {
  name: 'plan-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  sku: {
    name: 'Y1' 
    tier: 'Dynamic'
  }
}

// Function App
resource functionApp 'Microsoft.Web/sites@2020-06-01' = {
  name: 'funcapp-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  kind: 'functionapp'
  properties: {
    httpsOnly: true
    serverFarmId: hostingPlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appi.properties.InstrumentationKey
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${stgForFunctions.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(stgForFunctions.id, stgForFunctions.apiVersion).keys[0].value}'
        }
        {
          'name': 'FUNCTIONS_EXTENSION_VERSION'
          'value': '~3'
        }
        {
          'name': 'FUNCTIONS_WORKER_RUNTIME'
          'value': 'dotnet'
        }
        {
          'name': 'SERVICEBUS_CONNECTION'
          'value': '${listKeys('${sb.id}/AuthorizationRules/RootManageSharedAccessKey', sb.apiVersion).primaryConnectionString}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${stgForFunctions.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(stgForFunctions.id, stgForFunctions.apiVersion).keys[0].value}'
        }
      ]
    }
  }

  dependsOn: [
    appi
    hostingPlan
    stgForFunctions
    sb
  ]
}

// Function
resource function 'Microsoft.Web/sites/functions@2020-12-01' = {
  name: '${functionApp.name}/funcapp-tobp-${uniqueString(resourceGroup().id)}'
  properties: {
    function_app_id: functionApp.id
    config: {
      bindings: [
        {
          name: 'mySbMsg'
          type: 'serviceBusTrigger'
          direction: 'in'
          topicName: 'sbt-contacts'
          subscriptionName: 'testSubscription'
          connection: 'SERVICEBUS_CONNECTION'
          isSessionsEnabled: true
        }
      ]
    }
    files: {
      'run.csx': '''using System;
using System.Threading.Tasks;

public static void Run(string mySbMsg, ILogger log)
{
    log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
}'''
    }
    
  }
  dependsOn: [
    functionApp
  ]
}

resource appServicePlan 'Microsoft.Web/serverfarms@2021-01-01' = {
  name: 'appservice-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  sku: {
    name: 'S1'
    capacity: 1
  }
}

resource appService 'Microsoft.Web/sites@2021-01-01' = {
  name: 'webapp-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: false
    siteConfig: {
      alwaysOn: true
      use32BitWorkerProcess: false
      cors: {
        allowedOrigins: [
          '*'
        ]
      }
      appSettings: [
        {
          name: 'Cosmos:Url'
          value: 'https://${cosmosDbAccount.name}.documents.azure.com:443/'
        }
        {
          name: 'Cosmos:Key'
          value: listKeys('${cosmosDbAccount.id}', cosmosDbAccount.apiVersion).primaryMasterKey
        }
        {
          name: 'Cosmos:Db'
          value: 'tobp'
        }
        {
          name: 'Cosmos:Container'
          value: 'data'
        }
        {
          name: 'Events:Ttl'
          value: '864000'
        }
      ]
    }
  }

  dependsOn: [
    appServicePlan
    cosmosDbAccount
  ]
}

output sbConnectionString string = listKeys('${sb.id}/AuthorizationRules/RootManageSharedAccessKey', sb.apiVersion).primaryConnectionString
output sbTopicName string = 'sbt-contacts'
output cosmosUri string = 'https://${cosmosDbAccount.name}.documents.azure.com:443/'
output cosmosKey string = listKeys('${cosmosDbAccount.id}', cosmosDbAccount.apiVersion).primaryMasterKey
output cosmosDbName string = 'tobp'
output cosmosDbDataContainerName string = 'data'
output cosmosDbLeasesContainerName string = 'leases'
