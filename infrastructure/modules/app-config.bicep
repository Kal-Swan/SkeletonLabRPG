param appConfigurationName string
param principalId string
param managedIdentities array
param databaseName string
param databaseEndpoint string
param databasePartitionKeyPath string
param environment string = 'uat'
param blobServiceEndpoint string
param queueServiceEndpoint string
param queueNames array
param blobContainerNames array

var queueConfigs = reduce(
  queueNames,
  {},
  (accum, queueName) =>
    union(accum, {
      'Storage:Queue:${queueName}:Name': {
        default: queueName
        uat: queueName
      }
    })
)

var blobConfigs = reduce(
  blobContainerNames,
  {},
  (accum, containerName) =>
    union(accum, {
      'Storage:Blob:${containerName}:Name': {
        default: containerName
        uat: containerName
      }
    })
)

var configs = union(queueConfigs, blobConfigs, {
  'Cosmosdb:DatabaseName': {
    default: databaseName
    uat: databaseName
    production: null
  }
  'Cosmosdb:Endpoint': {
    default: databaseEndpoint
    uat: databaseEndpoint
    production: null
  }
  'Cosmosdb:PartitionKeyPath': {
    default: databasePartitionKeyPath
    uat: databasePartitionKeyPath
    production: null
  }
  'Storage:Blob:Endpoint': {
    default: blobServiceEndpoint
    uat: blobServiceEndpoint
  }
  'Storage:Queue:Endpoint': {
    default: queueServiceEndpoint
    uat: queueServiceEndpoint
  }
  'AzureAuth:Instance': {
    default: 'https://skeletonlabrpg.ciamlogin.com'
    uat: 'https://skeletonlabrpg.ciamlogin.com'
  }
  'AzureAuth:ClientId': {
    default: 'de827b2c-7ddd-4903-8bd8-43d6315cdeab'
    uat: 'de827b2c-7ddd-4903-8bd8-43d6315cdeab'
  }
  'AzureAuth:Domain': {
    default: 'skeletonlabrpg.onmicrosoft.com'
    uat: 'skeletonlabrpg.onmicrosoft.com'
  }
  'AzureAuth:TenantId': {
    default: 'a293616a-8cfa-4f4e-9572-41d21ec06a05'
    uat: 'a293616a-8cfa-4f4e-9572-41d21ec06a05'
  }
  'AzureAuth:Audience': {
    default: 'de827b2c-7ddd-4903-8bd8-43d6315cdeab'
    uat: 'de827b2c-7ddd-4903-8bd8-43d6315cdeab'
  }
})

var environmentConfig = [
  for item in items(configs): {
    key: item.key
    value: string(item.value[environment] ?? item.value.default)
    label: environment
  }
]

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2024-05-01' = {
  name: appConfigurationName
  location: resourceGroup().location
  sku: {
    name: 'Free'
  }
}

resource environmentConfigKeyValue 'Microsoft.AppConfiguration/configurationStores/keyValues@2024-05-01' = [
  for item in environmentConfig: {
    parent: appConfig
    name: item.key
    properties: {
      value: item.value
      tags: {
        label: item.label
        environment: environment
      }
    }
  }
]

resource appConfigDataReaderRoleDefinition 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: subscription()
  name: '516239f1-63e1-4d78-a4de-a74fb236a071'
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: appConfig
  name: guid(appConfig.id, principalId, 'App Config Reader')
  properties: {
    principalId: principalId
    roleDefinitionId: appConfigDataReaderRoleDefinition.id
    principalType: 'Group'
  }
}

resource managedRoleAssignments 'Microsoft.Authorization/roleAssignments@2022-04-01' = [
  for managedIdentity in managedIdentities: {
    scope: appConfig
    name: guid(appConfig.id, managedIdentity, 'App Config Reader')
    properties: {
      roleDefinitionId: appConfigDataReaderRoleDefinition.id
      principalId: managedIdentity
      principalType: 'ServicePrincipal'
    }
  }
]

output appConfigurationEndpoint string = appConfig.properties.endpoint
