param acrName string
param containerEnvName string
param apiAppName string
param webAppName string
param llmAppName string
param apiAppSettings array
param webAppSettings array
param llmApiAppSettings array
// param subnetId string
param imageTag string = 'latest'

resource acr 'Microsoft.ContainerRegistry/registries@2025-04-01' = {
  name: acrName
  location: resourceGroup().location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
  }
}

resource containerEnv 'Microsoft.App/managedEnvironments@2025-01-01' = {
  name: containerEnvName
  location: resourceGroup().location
  properties: {
  }
}

resource llmApp 'Microsoft.App/containerApps@2025-01-01' = {
  name: llmAppName
  location: resourceGroup().location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerEnv.id
    configuration: {
      registries: [
        {
          server: acr.properties.loginServer
          identity: 'system'
        }
      ]
      ingress: {
        external: true
        targetPort: 8000
      }
    }
    template: {
      containers: [
        {
          name: 'llm'
          image: '${acr.properties.loginServer}/${llmAppName}:${imageTag}'
          resources: {
            cpu: any('1.0')
            memory: '2.0Gi'
          }
          env: llmApiAppSettings
        }
      ]
      scale: {
        minReplicas: 1
      }
    }
  }
}

resource apiApp 'Microsoft.App/containerApps@2025-01-01' = {
  name: apiAppName
  location: resourceGroup().location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerEnv.id
    configuration: {
      registries: [
        {
          server: acr.properties.loginServer
          identity: 'system'
        }
      ]
      ingress: {
        external: true
        targetPort: 8080
      }
    }
    template: {
      containers: [
        {
          name: 'api'
          image: '${acr.properties.loginServer}/${apiAppName}:${imageTag}'
          resources: {
            cpu: any('0.5')
            memory: '1Gi'
          }
          env: [
            ...apiAppSettings
            {
              name: 'Configuration__LlmEndpoint'
              value: 'https://${llmApp.properties.configuration.ingress.fqdn}'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
      }
    }
  }
}

resource webApp 'Microsoft.App/containerApps@2025-01-01' = {
  name: webAppName
  location: resourceGroup().location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerEnv.id
    configuration: {
      registries: [
        {
          server: acr.properties.loginServer
          identity: 'system'
        }
      ]
      ingress: {
        external: true
        targetPort: 80
      }
    }
    template: {
      containers: [
        {
          name: 'frontend'
          image: '${acr.properties.loginServer}/${webAppName}:${imageTag}'
          resources: {
            cpu: any('0.25')
            memory: '0.5Gi'
          }
          env: [...webAppSettings, {
            name: 'PUBLIC_API_URL'
            value: 'https://${apiApp.properties.configuration.ingress.fqdn}'
          }]
        }
      ]
      scale: {
        minReplicas: 1
      }
    }
  }
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
output llmContainerId string = llmApp.id
output apiContainerId string = apiApp.id
