param appServicePlanName string
param webAppName string
param location string = resourceGroup().location
param pricingPlan string = 'F1'
param pricingTier string = 'Free'
param appSettings array = []
param fxVersion string
param isApp bool

resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: pricingPlan
    tier: pricingTier
    size: 'F1'
    family: 'F'
    capacity: 1
  }
  properties: {
    reserved: isApp
  }
  kind: isApp ? 'linux' : 'app'
}

resource webApp 'Microsoft.Web/sites@2024-04-01' = {
  name: webAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      windowsFxVersion: !isApp ? fxVersion : ''
      linuxFxVersion: isApp ? fxVersion : ''
      appSettings: appSettings
    }
  }
}

output webAppUrl string = 'https://${webAppName}.azurewebsites.net'
output webAppIdentityPrincipalId string = webApp.identity.principalId
