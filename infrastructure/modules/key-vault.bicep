param name string

resource keyVault 'Microsoft.KeyVault/vaults@2024-11-01' = {
  name: name
  location: resourceGroup().location
  properties: {
    sku: {
      name: 'standard'
      family: 'A'
    }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true
    enabledForTemplateDeployment: true
  }
}

output vaultUri string = keyVault.properties.vaultUri
