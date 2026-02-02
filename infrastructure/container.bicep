param prefix string
param environment string
param apiInsightsConnectionString string
param webInsightsConnectionString string
param useExistingContainerApp bool
param imageTag string

module existingcontainer './modules/container-existing.bicep' = {
  params: {
    webAppName: '${prefix}-web-${environment}-container'
    apiAppName: '${prefix}-api-${environment}-container'
    llmAppName: '${prefix}-llm-${environment}-container'
  }
}

module newcontainer './modules/container.bicep' = {
  name: 'containerModule'
  params: {
    imageTag: imageTag
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
        value: apiInsightsConnectionString
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
        value: webInsightsConnectionString
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
        name: 'BODY_SIZE_LIMIT'
        value: 'Infinity'
      }
    ]
  }
}

output type object = useExistingContainerApp ? existingcontainer.outputs : newcontainer.outputs
