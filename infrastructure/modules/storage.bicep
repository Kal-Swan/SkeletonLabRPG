param storageName string
param location string = resourceGroup().location
param blobContainerNames array
param queueNames array
param managedIdentities array = []

resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
  parent: storage
  name: 'default'
}

resource queueService 'Microsoft.Storage/storageAccounts/queueServices@2024-01-01' = {
  parent: storage
  name: 'default'
}

resource blobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = [
  for containerName in blobContainerNames: {
    parent: blobService
    name: containerName
    properties: {
      publicAccess: 'None'
    }
  }
]

resource queueContainer 'Microsoft.Storage/storageAccounts/queueServices/queues@2024-01-01' = [
  for queueName in queueNames: {
    parent: queueService
    name: queueName
  }
]

var storageBlobDataContributor = subscriptionResourceId(
  'Microsoft.Authorization/roleDefinitions',
  'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
)
// var storageQueueDataContributor = subscriptionResourceId(
//   'Microsoft.Authorization/roleDefinitions',
//   '974c5e8b-45b9-4653-ba55-5f855dd0fb88'
// )

resource blobRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = [
  for principalId in managedIdentities: {
    name: guid(storage.id, principalId, 'blob-contributor')
    scope: storage
    properties: {
      principalId: principalId
      roleDefinitionId: storageBlobDataContributor
      principalType: 'ServicePrincipal'
    }
  }
]

// resource queueRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = [
//   for principalId in managedIdentities: {
//     name: guid(storage.id, principalId, 'queue-contributor')
//     scope: storage
//     properties: {
//       principalId: principalId
//       roleDefinitionId: storageQueueDataContributor
//       principalType: 'ServicePrincipal'
//     }
//   }
// ]

output blobServiceEndpoint string = storage.properties.primaryEndpoints.blob
output queueServiceEndpoint string = storage.properties.primaryEndpoints.queue
