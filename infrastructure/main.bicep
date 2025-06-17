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
    managedIdentities: [api.outputs.webAppIdentityPrincipalId]
    databaseName: cosmosdb.outputs.databaseName
    databaseEndpoint: cosmosdb.outputs.cosmosDbUri
    databasePartitionKeyPath: cosmosdb.outputs.partitionKeyPath
    blobServiceEndpoint: storage.outputs.blobServiceEndpoint
    queueServiceEndpoint: storage.outputs.queueServiceEndpoint
    queueNames: queueNames
    blobContainerNames: blobContainerNames
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

module api './modules/web.bicep' = {
  name: 'apiModule'
  params: {
    isApp: false
    fxVersion: 'dotnetcore|8.0'
    appServicePlanName: '${prefix}-api-${environment}-appserviceplan'
    webAppName: '${prefix}-api-${environment}-appservice'
    appSettings: [
      {
        name: 'ASPNETCORE_ENVIRONMENT'
        value: 'UAT'
      }
      {
        name: 'AppConfiguration__Endpoint'
        value: appConfigEndpoint
      }
      {
        name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
        value: apiAppInsights.outputs.connectionString
      }
      {
        name: 'InstrumentationEngineExtensionVersion'
        value: '~3'
      }
    ]
  }
}

module web './modules/web.bicep' = {
  name: 'webModule'
  params: {
    isApp: true
    fxVersion: 'node|20-lts'
    appServicePlanName: '${prefix}-web-${environment}-appserviceplan'
    webAppName: '${prefix}-web-${environment}-appservice'
    appSettings: [
      {
        name: 'PUBLIC_API_URL'
        value: api.outputs.webAppUrl
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
      {
        name: 'NODE_ENV'
        value: 'UAT'
      }
    ]
  }
}

module cosmosdb './modules/cosmosdb.bicep' = {
  name: 'cosmosdbModule'
  params: {
    accountName: '${prefix}-${environment}-cosmosdb'
    location: resourceGroup().location
    databaseName: 'skeletonlabrpg'
    managedIdentities: [api.outputs.webAppIdentityPrincipalId]
  }
}

module storage './modules/storage.bicep' = {
  name: 'storageModule'
  params: {
    storageName: '${prefix}${environment}storage'
    location: resourceGroup().location
    blobContainerNames: blobContainerNames
    queueNames: queueNames
  }
}

module container 'modules/container.bicep' = {
  name: 'containerModule'
  params: {
    containerEnvName: '${prefix}-${environment}-container-env'
    apiAppName: '${prefix}-api-${environment}-container'
    webAppName: '${prefix}-web-${environment}-container'
    acrName: '${prefix}${environment}acr'
    apiAppSettings: [
      {
        name: 'ASPNETCORE_ENVIRONMENT'
        value: 'UAT'
      }
      {
        name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
        value: apiAppInsights.outputs.connectionString
      }
    ]
    webAppSettings: [
      {
        name: 'NODE_ENV'
        value: 'UAT'
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
