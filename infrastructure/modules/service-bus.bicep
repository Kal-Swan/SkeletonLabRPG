param prefix string
param queueName string = 'build-requests'
param busDataReceiverRoleManagedIdentities array
param busDataSenderRoleManagedIdentities array
param userObjectId string

resource sbNamespace 'Microsoft.ServiceBus/namespaces@2024-01-01' = {
  name: '${prefix}-llm-worker-bus'
  location: resourceGroup().location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
}

resource sbQueue 'Microsoft.ServiceBus/namespaces/queues@2024-01-01' = {
  parent: sbNamespace
  name: queueName
  properties: {
    enablePartitioning: false // Basic tier doesn't support partitioning
    maxDeliveryCount: 10      // Messages moved to DLQ after N failed deliveries
  }
}

resource azureServiceBusDataReceiverRoleDefinition 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: subscription()
  name: '4f6d3b9b-027b-4f4c-9142-0e5a2a2247e0'
}

resource azureServiceBusDataSenderRoleDefinition 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: subscription()
  name: '69a216fc-b8fb-44d8-bc22-1f3c2cd27a39'
}

var roles = [{
      name: 'Azure Service Bus Data Receiver'
      roleDefinitionId: azureServiceBusDataReceiverRoleDefinition.id
    }
    {
      name: 'Azure Service Bus Data Sender'
      roleDefinitionId: azureServiceBusDataSenderRoleDefinition.id
    }]

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = [
  for role in roles: {
    scope: sbNamespace
    name: guid(sbNamespace.id, userObjectId, role.name)
    properties: {
      principalId: userObjectId
      roleDefinitionId: role.roleDefinitionId
      principalType: 'User'
    }
  }
]

resource busDataReceiverRoleManagedAssignments 'Microsoft.Authorization/roleAssignments@2022-04-01' = [
  for managedIdentity in busDataReceiverRoleManagedIdentities: {
    scope: sbNamespace
    name: guid(sbNamespace.id, managedIdentity, 'Azure Service Bus Data Receiver')
    properties: {
      roleDefinitionId: azureServiceBusDataReceiverRoleDefinition.id
      principalId: managedIdentity
      principalType: 'ServicePrincipal'
    }
  }
]

resource busDataSenderRoleManagedAssignments 'Microsoft.Authorization/roleAssignments@2022-04-01' = [
  for managedIdentity in busDataSenderRoleManagedIdentities: {
    scope: sbNamespace
    name: guid(sbNamespace.id, managedIdentity, 'Azure Service Bus Data Sender')
    properties: {
      roleDefinitionId: azureServiceBusDataSenderRoleDefinition.id
      principalId: managedIdentity
      principalType: 'ServicePrincipal'
    }
  }
]
var hostName = replace(sbNamespace.properties.serviceBusEndpoint, ':443/', '')
output serviceBusEndpoint string = hostName
output queueName string = sbQueue.name
