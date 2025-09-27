param location string
param openAiName string
param customSubDomainName string
resource openAi 'Microsoft.CognitiveServices/accounts@2024-10-01' = {
  name: openAiName
  location: location // Use a supported region
  kind: 'OpenAI'
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
