// param prefix string
// param subnetId string
// param llmContainerId string
// param apiContainerId string

// resource llmPrivateEndpoint 'Microsoft.Network/privateEndpoints@2024-10-01' = {
//   name: '${prefix}-llm-pe'
//   location: resourceGroup().location
//   properties: {
//     subnet: {
//       id: subnetId
//     }
//     privateLinkServiceConnections: [
//       {
//         name: '${prefix}-llm-pe-connection'
//         properties: {
//           privateLinkServiceId: llmContainerId
//           groupIds: [
//             'containerApps'
//           ]
//         }
//       }
//     ]
//   }
// }

// resource apiPrivateEndpoint 'Microsoft.Network/privateEndpoints@2024-10-01' = {
//   name: '${prefix}-api-pe'
//   location: resourceGroup().location
//   properties: {
//     subnet: {
//       id: subnetId
//     }
//     privateLinkServiceConnections: [
//       {
//         name: '${prefix}-api-pe-connection'
//         properties: {
//           privateLinkServiceId: apiContainerId
//           groupIds: [
//             'containerApps'
//           ]
//         }
//       }
//     ]
//   }
// }
