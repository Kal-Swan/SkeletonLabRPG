// param vnetName string
// param prefix string
// param nsgId string

// // https://learn.microsoft.com/en-us/azure/container-apps/custom-virtual-networks?tabs=workload-profiles-env

// resource vnet 'Microsoft.Network/virtualNetworks@2024-10-01' = {
//   name: vnetName
//   location: resourceGroup().location
//   properties: {
//     addressSpace: {
//       addressPrefixes: [
//         '10.0.0.0/16'
//       ]
//     }
//     subnets: [
//       {
//         name: '${prefix}-subnet'
//         properties: {
//           addressPrefix: '10.0.0.0/23'
//           networkSecurityGroup: {
//             id: nsgId
//           }
//           delegations: [
//             {
//               name: 'delegation'
//               properties: {
//                 serviceName: 'Microsoft.App/environments'
//               }
//             }
//           ]
//         }
//       }
//     ]
//   }
// }

// output subnetId string = vnet.properties.subnets[0].id
