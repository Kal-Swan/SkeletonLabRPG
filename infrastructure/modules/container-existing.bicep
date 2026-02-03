param apiAppName string
param webAppName string
param llmAppName string

resource llmApp 'Microsoft.App/containerApps@2025-01-01' existing = {
  name: llmAppName
}

resource apiApp 'Microsoft.App/containerApps@2025-01-01' existing = {
  name: apiAppName
}

resource webApp 'Microsoft.App/containerApps@2025-01-01' existing = {
  name: webAppName
}

// resource llmAcrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
//   name: guid(acr.id, llmApp.id, 'AcrPull')
//   scope: acr
//   properties: {
//     roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d') // AcrPull
//     principalId: llmApp.identity.principalId
//     principalType: 'ServicePrincipal'
//   }
// }

// resource apiAcrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
//   name: guid(acr.id, apiApp.id, 'AcrPull')
//   scope: acr
//   properties: {
//     roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d') // AcrPull
//     principalId: apiApp.identity.principalId
//     principalType: 'ServicePrincipal'
//   }
// }

// resource webAcrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
//   name: guid(acr.id, webApp.id, 'AcrPull')
//   scope: acr
//   properties: {
//     roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d') // AcrPull
//     principalId: webApp.identity.principalId
//     principalType: 'ServicePrincipal'
//   }
// }

output apiContainerPrincipleId string = apiApp.identity.principalId
output llmContainerPrincipleId string = llmApp.identity.principalId
output webContainerPrincipleId string = webApp.identity.principalId
output webContainerUrl string = 'https://${webApp.properties.configuration.ingress.fqdn}'
output apiContainerUrl string = 'https://${apiApp.properties.configuration.ingress.fqdn}'
