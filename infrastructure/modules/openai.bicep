param location string
param openAiName string
param customSubDomainName string
param managedIdentities array = []

resource openAi 'Microsoft.CognitiveServices/accounts@2024-10-01' = {
  name: openAiName
  location: location // Use a supported region
  kind: 'OpenAI'
  identity: {
    type: 'SystemAssigned'
  }
  sku: {
    name: 'S0' // The only supported SKU for Azure OpenAI
    tier: 'Standard'
  }
  properties: {
    apiProperties: {
      qnaRuntimeEndpoint: ''
    }
    customSubDomainName: customSubDomainName // Optional: changes endpoint URL to myopenaiendpoint.openai.azure.com
    publicNetworkAccess: 'Enabled' // or 'Disabled' for private endpoints
    networkAcls: {
      defaultAction: 'Allow' // or 'Deny'
      virtualNetworkRules: []
      ipRules: []
    }
  }
}

resource openAiRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = [
  for principalId in managedIdentities: {
    name: guid(openAi.id, principalId, 'CognitiveServicesOpenAIUser')
    scope: openAi
    properties: {
      roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd') // Cognitive Services OpenAI User
      principalId: principalId
      principalType: 'ServicePrincipal'
    }
  }
]
