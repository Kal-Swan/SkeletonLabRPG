param storageName string
param location string = resourceGroup().location
param blobContainerNames array
param queueNames array

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

output blobServiceEndpoint string = storage.properties.primaryEndpoints.blob
output queueServiceEndpoint string = storage.properties.primaryEndpoints.queue
