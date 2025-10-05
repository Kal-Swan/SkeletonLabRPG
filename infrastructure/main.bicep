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

param keyValueKey object = {
  azureB2cWebClientId: 'azure-b2c-web-client-id'
  azureB2cAuthority: 'azure-b2c-authority'
  azureB2cRedirectUri: 'azure-b2c-redirect-uri'
  azureB2cTenant: 'azure-b2c-tenant'
  azureB2cApiAccessScope: 'azure-b2c-api-access-scope'
  azureB2cApiClientId: 'azure-b2c-api-client-id'
  azureB2cTenantId: 'azure-b2c-tenant-id'
  apiUrl: 'api-url'
}

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
      {
        name: 'PUBLIC_AZURE_B2C_WEB_CLIENT_ID'
        secretRef: keyValueKey.azureB2cWebClientId
      }
      {
        name: 'PUBLIC_AZURE_B2C_AUTHORITY'
        secretRef: keyValueKey.azureB2cAuthority
      }
      {
        name: 'PUBLIC_AZURE_B2C_REDIRECT_URI'
        secretRef: keyValueKey.azureB2cRedirectUri
      }
      {
        name: 'PUBLIC_AZURE_B2C_TENANT'
        secretRef: keyValueKey.azureB2cTenant
      }
      {
        name: 'PUBLIC_AZURE_B2C_API_ACCESS_SCOPE'
        secretRef: keyValueKey.azureB2cApiAccessScope
      }
      {
        name: 'PUBLIC_AZURE_B2C_API_CLIENT_ID'
        secretRef: keyValueKey.azureB2cApiClientId
      }
      {
        name: 'PUBLIC_AZURE_B2C_TENANT_ID'
        secretRef: keyValueKey.azureB2cTenantId
      }
      {
        name: 'PUBLIC_API_URL'
        secretRef: keyValueKey.apiUrl
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
  }
}
