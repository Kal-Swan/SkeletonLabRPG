param accountName string
param location string = resourceGroup().location
param databaseName string
param managedIdentities array
param partitionKeyPath string = '/partitionKey'

resource account 'Microsoft.DocumentDB/databaseAccounts@2024-11-15' = {
  name: accountName
  location: location
  properties: {
    enableFreeTier: true
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
      }
    ]
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-11-15' = {
  parent: account
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
    options: {
      throughput: 400
    }
  }
}

resource container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-11-15' = [
  for containerName in ['default']: {
    parent: database
    name: containerName
    properties: {
      resource: {
        id: containerName
        partitionKey: {
          paths: [
            partitionKeyPath
          ]
        }
      }
    }
  }
]

var roleDefinitionId = guid('sql-role-definition-', account.id)
resource roleDefinition 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2024-11-15' = {
  parent: account
  name: roleDefinitionId
  properties: {
    roleName: 'Cosmos DB Write Role'
    type: 'CustomRole'
    assignableScopes: [
      account.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/*'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/*'
        ]
      }
    ]
  }
}

var developerGroupId = '3c143b8c-b01a-49b5-95d4-af80e5d9033c'

resource roleAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2024-11-15' = {
  parent: account
  name: guid(roleDefinitionId, developerGroupId, account.id)
  properties: {
    roleDefinitionId: roleDefinition.id
    principalId: developerGroupId
    scope: account.id
  }
}

resource managedRoleAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2024-11-15' = [
  for managedIdentityId in managedIdentities: {
    parent: account
    name: guid(roleDefinitionId, managedIdentityId, account.id)
    properties: {
      roleDefinitionId: roleDefinition.id
      principalId: managedIdentityId
      scope: account.id
    }
  }
]

output databaseName string = databaseName
output cosmosDbUri string = account.properties.documentEndpoint
output partitionKeyPath string = partitionKeyPath
