param acrName string
param containerEnvName string
param apiAppName string
param webAppName string
param llmAppName string
param apiAppSettings array
param webAppSettings array
param llmApiAppSettings array

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
    daprAIInstrumentationKey: ''
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
        targetPort: 80
      }
    }
    template: {
      containers: [
        {
          name: 'llm'
          image: '${acr.properties.loginServer}/${llmAppName}:latest'
          resources: {
            cpu: any('0.5')
            memory: '1Gi'
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
        targetPort: 80
      }
    }
    template: {
      containers: [
        {
          name: 'api'
          image: '${acr.properties.loginServer}/${apiAppName}:latest'
          resources: {
            cpu: any('0.5')
            memory: '1Gi'
          }
          env: apiAppSettings
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
          image: '${acr.properties.loginServer}/${webAppName}:latest'
          resources: {
            cpu: any('0.25')
            memory: '0.5Gi'
          }
          env: webAppSettings
        }
      ]
      scale: {
        minReplicas: 1
      }
    }
  }
}
