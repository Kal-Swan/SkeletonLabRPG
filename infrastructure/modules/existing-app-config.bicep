param environment string = 'uat'
param appConfigurationName string
resource existingAppConfig 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: appConfigurationName
}

var defaultExistingConfigs = {
  'WorkerApi:Key': {
    default: ''
    uat: ''
  }
}

var defaultExistingEnvEntries = [
  for item in items(defaultExistingConfigs): {
    key: item.key
    value: string(item.value.default)
    label: environment
  }
]

resource defaultExistingEnvironmentConfigKeyValue 'Microsoft.AppConfiguration/configurationStores/keyValues@2024-05-01' = [
  for item in defaultExistingEnvEntries: {
    parent: existingAppConfig
    name: '${item.key}$${item.label}'
    properties: {
      value:  item.value
      tags: {
        environment: environment
      }
    }
  }
]
