// param prefix string
// param vnetId string

// resource privateDnsZone 'Microsoft.Network/privateDnsZones@2024-06-01' = {
//   name: '${prefix}-pdnszone'
//   location: 'global'
// }

// resource privateDnsVnetLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2024-06-01' = {
//   name: '${privateDnsZone.name}-vnet-link'
//   location: 'global'
//   properties: {
//     virtualNetwork: {
//       id: vnetId
//     }
//     registrationEnabled: true
//   }
// }
