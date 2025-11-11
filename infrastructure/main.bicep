param prefix string = 'skeletonlabrpg'
param environment string = 'uat'
param userGroupId string = 'eea6a68d-8077-49d1-823d-376f3de41030'
var appConfigEndpoint = 'https://${prefix}-appconfig-shared.azconfig.io'
var appConfigName = '${prefix}-shared-appconfig'
param queueNames array = [
  'characterattributequeue'
]
param blobContainerNames array = [
  'characterattributemodels'
  'characterattributemltraining'
]

module appConfig './modules/app-config.bicep' = {
  name: 'appConfigModule'
  params: {
    appConfigurationName: appConfigName
    principalId: userGroupId
    managedIdentities: [
      container.outputs.apiContainerPrincipleId
      container.outputs.llmContainerPrincipleId
      container.outputs.webContainerPrincipleId
    ]
    databaseName: cosmosdb.outputs.databaseName
    databaseEndpoint: cosmosdb.outputs.cosmosDbUri
    databasePartitionKeyPath: cosmosdb.outputs.partitionKeyPath
    blobServiceEndpoint: storage.outputs.blobServiceEndpoint
    queueServiceEndpoint: storage.outputs.queueServiceEndpoint
    queueNames: queueNames
    blobContainerNames: blobContainerNames
    webContainerUrl: container.outputs.webContainerUrl
  }
}

module webAppInsights './modules/app-insights.bicep' = {
  name: 'webAppInsightsModule'
  params: {
    appInsightsName: '${prefix}-web-${environment}-appinsights'
  }
}

module apiAppInsights './modules/app-insights.bicep' = {
  name: 'ApiAppInsightsModule'
  params: {
    appInsightsName: '${prefix}-api-${environment}-appinsights'
  }
}

// module api './modules/web.bicep' = {
//   name: 'apiModule'
//   params: {
//     isApp: false
//     fxVersion: 'dotnetcore|8.0'
//     appServicePlanName: '${prefix}-api-${environment}-appserviceplan'
//     webAppName: '${prefix}-api-${environment}-appservice'
//     appSettings: [
//       {
//         name: 'ASPNETCORE_ENVIRONMENT'
//         value: 'UAT'
//       }
//       {
//         name: 'AppConfiguration__Endpoint'
//         value: appConfigEndpoint
//       }
//       {
//         name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
//         value: apiAppInsights.outputs.connectionString
//       }
//       {
//         name: 'InstrumentationEngineExtensionVersion'
//         value: '~3'
//       }
//     ]
//   }
// }

// module web './modules/web.bicep' = {
//   name: 'webModule'
//   params: {
//     isApp: true
//     fxVersion: 'node|20-lts'
//     appServicePlanName: '${prefix}-web-${environment}-appserviceplan'
//     webAppName: '${prefix}-web-${environment}-appservice'
//     appSettings: [
//       {
//         name: 'PUBLIC_API_URL'
//         value: api.outputs.webAppUrl
//       }
//       {
//         name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
//         value: webAppInsights.outputs.connectionString
//       }
//       {
//         name: 'InstrumentationEngineExtensionVersion'
//         value: 'latest'
//       }
//       {
//         name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
//         value: '~3'
//       }
//       {
//         name: 'NODE_ENV'
//         value: 'UAT'
//       }
//     ]
//   }
// }

module cosmosdb './modules/cosmosdb.bicep' = {
  name: 'cosmosdbModule'
  params: {
    accountName: '${prefix}-${environment}-cosmosdb'
    location: resourceGroup().location
    databaseName: 'skeletonlabrpg'
    managedIdentities: [container.outputs.apiContainerPrincipleId]
  }
}

module storage './modules/storage.bicep' = {
  name: 'storageModule'
  params: {
    storageName: '${prefix}${environment}storage'
    location: resourceGroup().location
    blobContainerNames: blobContainerNames
    queueNames: queueNames
    managedIdentities: [container.outputs.llmContainerPrincipleId]
  }
}

module keyVault './modules/key-vault.bicep' = {
  name: 'keyvault'
  params: {
    name: '${prefix}-${environment}-kv'
  }
}

module container 'modules/container.bicep' = {
  name: 'containerModule'
  params: {
    containerEnvName: '${prefix}-${environment}-container-env'
    webAppName: '${prefix}-web-${environment}-container'
    apiAppName: '${prefix}-api-${environment}-container'
    llmAppName: '${prefix}-llm-${environment}-container'
    acrName: '${prefix}${environment}acr'
    llmApiAppSettings: [
      {
        name: 'AZURE_BLOB_STORAGE_URL'
        value: 'https://skeletonlabrpguatstorage.blob.core.windows.net/'
      }
      {
        name: 'Environment'
        value: 'UAT'
      }
      {
        name: 'AZURE_OPEN_AI_ENDPOINT'
        value: 'https://skeletonlabrpguatopenai.openai.azure.com'
      }
      {
        name: 'AZURE_AI_TEXT_EMBEDDING'
        value: 'skeletonlabrpg-text-embedding-3-small'
      }
    ]
    apiAppSettings: [
      {
        name: 'ASPNETCORE_ENVIRONMENT'
        value: 'UAT'
      }
      {
        name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
        value: apiAppInsights.outputs.connectionString
      }
      {
        name: 'Configuration__AzureAppConfigurationEndpoint'
        value: 'https://skeletonlabrpg-shared-appconfig.azconfig.io'
      }
    ]
    webAppSettings: [
      {
        name: 'PUBLIC_ENV'
        value: 'uat'
      }
      {
        name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
        value: webAppInsights.outputs.connectionString
      }
      {
        name: 'InstrumentationEngineExtensionVersion'
        value: 'latest'
      }
      {
        name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
        value: '~3'
      }
    ]
  }
}

module openAi './modules/openai.bicep' = {
  name: 'openAiModule'
  params: {
    openAiName: '${prefix}-${environment}-openai'
    customSubDomainName: '${prefix}${environment}openai'
    location: resourceGroup().location
    managedIdentities: [container.outputs.llmContainerPrincipleId]
  }
}

// module vnet './modules/vnet.bicep' = {
//   name: 'vnetModule'
//   params: {
//     vnetName: '${prefix}-${environment}-vnet'
//     prefix: '${prefix}-${environment}'
//     nsgId: nsg.outputs.nsgId
//   }
// }

// module nsg './modules/nsg.bicep' = {
//   name: 'nsgModule'
//   params: {
//     prefix: '${prefix}-${environment}'
//   }
// }

// module pe './modules/pe.bicep' = {
//   name: 'peModule'
//   params: {
//     prefix: '${prefix}-${environment}'
//     subnetId: vnet.outputs.subnetId
//     llmContainerId: container.outputs.llmContainerId
//     apiContainerId: container.outputs.apiContainerId
//   }
// }
