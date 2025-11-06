
// param prefix string

// resource subnetNSG 'Microsoft.Network/networkSecurityGroups@2024-10-01' = {
//   name: '${prefix}-nsg'
//   location: resourceGroup().location
//   properties: {
//     securityRules: [
//       {
//         name: 'Allow-Internet-To-WebContainer'
//         properties: {
//           priority: 100
//           direction: 'Inbound'
//           access: 'Allow'
//           protocol: 'Tcp'
//           sourceAddressPrefix: 'Internet'           // allow from internet
//           sourcePortRange: '*'                      // clients use random ports
//           destinationAddressPrefix: '*'             // applies to any IP in subnet
//           destinationPortRange: '443'               // HTTPS traffic
//           description: 'Allow HTTPS traffic from Internet to web container'
//         }
//       }
//       // Allow outbound traffic from subnet → Internet (needed for image pull & platform)
//       {
//         name: 'Allow-Subnet-Outbound'
//         properties: {
//           priority: 100
//           direction: 'Outbound'
//           access: 'Allow'
//           protocol: 'Tcp'
//           sourceAddressPrefix: '*'
//           sourcePortRange: '*'
//           destinationAddressPrefix: '*'
//           destinationPortRange: '443'
//           description: 'Allow outbound HTTPS for container image pull and platform'
//         }
//       }
//     ]
//   }
// }

// output nsgId string = subnetNSG.id
