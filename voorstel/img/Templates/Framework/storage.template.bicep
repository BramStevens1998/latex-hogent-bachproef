//* --- PARAMETERS ---
param dIPEnvironment string
param dIPCompany string
param dIPLocation string
param dIPName string
param kvName string
param managedIdentityObjectId string
param usePE bool
param vNet string
param privateDnsZoneBlob string
param privateDnsZoneQueue string

//* --- EXISTING RESOURCES ---
resource KeyVault 'Microsoft.KeyVault/vaults@2021-06-01-preview' existing = {
  name: kvName
}

//* --- RESOURCES ---
resource storageaccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: '${dIPCompany}${dIPName}01${dIPEnvironment}store'
  location: dIPLocation
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
    tier: 'Standard'
  }
  properties: {
    networkAcls: {
      bypass: 'AzureServices'
      virtualNetworkRules: []
      ipRules: []
      defaultAction: 'Allow'
    }
    supportsHttpsTrafficOnly: true
    encryption: {
      services: {
        file: {
          enabled: true
        }
        blob: {
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
    accessTier: 'Hot'
  }
}
resource storageaccountPrivateEndpointBlob 'Microsoft.Network/privateEndpoints@2021-05-01' = if(usePE) {
  name: '${dIPCompany}${dIPName}01${dIPEnvironment}store-blob-pe'
  location: dIPLocation
  properties: {
    privateLinkServiceConnections: [
      {
        name: '${dIPCompany}${dIPName}01${dIPEnvironment}store-blob-pe-sc'
        properties: {
          privateLinkServiceId: storageaccount.id
          privateLinkServiceConnectionState: {
            status: 'Approved'
            actionsRequired: 'None'
          }
          groupIds: [
            'blob'
          ]
        }
      }
    ]
    manualPrivateLinkServiceConnections: []
    subnet: {
      id: '${vNet}/subnets/${dIPCompany}-${dIPName}-${dIPEnvironment}-subnet'
    }
    customDnsConfigs: []
  }
  resource privateDnsZoneGroups 'privateDnsZoneGroups@2021-05-01' = {
    name: 'default'
    properties: {
      privateDnsZoneConfigs: [
        {
          name: 'privatelink_azurewebsites_net'
          properties: {
            privateDnsZoneId: privateDnsZoneBlob
          }
        }
      ]
    }
  }
}

resource storageaccountPrivateEndpointTable 'Microsoft.Network/privateEndpoints@2021-05-01' = if(usePE) {
  name: '${dIPCompany}${dIPName}01${dIPEnvironment}store-table-pe'
  location: dIPLocation
  properties: {
    privateLinkServiceConnections: [
      {
        name: '${dIPCompany}${dIPName}01${dIPEnvironment}store-table-pe-sc'
        properties: {
          privateLinkServiceId: storageaccount.id
          privateLinkServiceConnectionState: {
            status: 'Approved'
            actionsRequired: 'None'
          }
          groupIds: [
            'table'
          ]
        }
      }
    ]
    manualPrivateLinkServiceConnections: []
    subnet: {
      id: '${vNet}/subnets/${dIPCompany}-${dIPName}-${dIPEnvironment}-subnet'
    }
    customDnsConfigs: []
  }
  resource privateDnsZoneGroups 'privateDnsZoneGroups@2021-05-01' = {
    name: 'default'
    properties: {
      privateDnsZoneConfigs: [
        {
          name: 'privatelink_azurewebsites_net'
          properties: {
            privateDnsZoneId: privateDnsZoneQueue
          }
        }
      ]
    }
  }
}

resource storageaccountPrivateEndpointQueue 'Microsoft.Network/privateEndpoints@2021-05-01' = if(usePE) {
  name: '${dIPCompany}${dIPName}01${dIPEnvironment}store-queue-pe'
  location: dIPLocation
  properties: {
    privateLinkServiceConnections: [
      {
        name: '${dIPCompany}${dIPName}01${dIPEnvironment}store-queue-pe-sc'
        properties: {
          privateLinkServiceId: storageaccount.id
          privateLinkServiceConnectionState: {
            status: 'Approved'
            actionsRequired: 'None'
          }
          groupIds: [
            'queue'
          ]
        }
      }
    ]
    manualPrivateLinkServiceConnections: []
    subnet: {
      id: '${vNet}/subnets/${dIPCompany}-${dIPName}-${dIPEnvironment}-subnet'
    }
    customDnsConfigs: []
  }
  resource privateDnsZoneGroups 'privateDnsZoneGroups@2021-05-01' = {
    name: 'default'
    properties: {
      privateDnsZoneConfigs: [
        {
          name: 'privatelink_azurewebsites_net'
          properties: {
            privateDnsZoneId: privateDnsZoneQueue
          }
        }
      ]
    }
  }
}

resource roleAuthorization 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: guid(storageaccount.id, resourceGroup().id, managedIdentityObjectId)
  scope: storageaccount
  properties: {
      principalId: managedIdentityObjectId
      roleDefinitionId: '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c' // Contributor
  }
}

resource blobRoleAuthorization 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: guid(storageaccount.id, resourceGroup().id, managedIdentityObjectId, 'blob')
  scope: storageaccount
  properties: {
      principalId: managedIdentityObjectId
      roleDefinitionId: '/providers/Microsoft.Authorization/roleDefinitions/ba92f5b4-2d11-453d-a403-e96b0029c9fe' // Blob Contributor
  }
}

resource tableRoleAuthorization 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: guid(storageaccount.id, resourceGroup().id, managedIdentityObjectId, 'table')
  scope: storageaccount
  properties: {
      principalId: managedIdentityObjectId
      roleDefinitionId: '/providers/Microsoft.Authorization/roleDefinitions/0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3' // Table Contributor
  }
}

resource blobservice 'Microsoft.Storage/storageAccounts/blobServices@2021-04-01' = {
  name: '${storageaccount.name}/default'
  properties: {
    cors: {
      corsRules: []
    }
    deleteRetentionPolicy: {
      enabled: false
    }
  }
}

resource querycontainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  name: '${storageaccount.name}/default/data-accelerator-queries'
  properties: {
    publicAccess: 'None'
  }
}

resource storedprocedurecontainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  name: '${storageaccount.name}/default/data-accelerator-stored-procedures'
  properties: {
    publicAccess: 'None'
  }
}

resource mappingcontainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  name: '${storageaccount.name}/default/data-accelerator-mappings'
  properties: {
    publicAccess: 'None'
  }
}

resource scriptcontainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' ={
  name: '${storageaccount.name}/default/data-accelerator-scripts'
  properties: {
    publicAccess: 'None'
  }
}

//* --- SECRETS ---
resource storageAccountConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: 'storageAccountConnectionString-${dIPEnvironment}'
  parent: KeyVault
  properties: {
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageaccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageaccount.id, storageaccount.apiVersion).keys[0].value}'
  }
}

resource storageAccountKeySecret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: 'storageAccountKey-${dIPEnvironment}'
  parent: KeyVault
  properties: {
    value: '${listKeys(storageaccount.id, storageaccount.apiVersion).keys[0].value}'
  }
}

//* --- OUTPUTS ---
output storageAccountName string = storageaccount.name
output storageAccountConnectionStringSecretName string = storageAccountConnectionStringSecret.name
output storageAccountKeySecretName string = storageAccountKeySecret.name
