param dIPEnvironment string
param dIPCompany string
param dIPLocation string
param dIPName string
@secure()
param odsConnectionString string
param serverFarmId string
@secure()
param storageAccountConnectionString string
@secure()
param appInsightsInstrumentationKey string
param vNet string
param faSubnet string
param managedIdentityId string
param privateDnsZone string

resource dataAccelerator 'Microsoft.Web/sites@2021-02-01' = {
  name: '${dIPCompany}-${dIPName}-dataaccelerator-${dIPEnvironment}-func'
  location: dIPLocation
  kind: 'functionapp'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityId}': {}
    }
  }
  properties: {
    serverFarmId: serverFarmId
    hostNameSslStates: [
      {
        name: '${dIPCompany}-${dIPName}-dataaccelerator-${dIPEnvironment}-func.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Standard'
      }
      {
        name: '${dIPCompany}-${dIPName}-dataaccelerator-${dIPEnvironment}-func.scm.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Repository'
      }
    ]
    reserved: false
    isXenon: false
    hyperV: false
    siteConfig: {
      numberOfWorkers: 1
      acrUseManagedIdentityCreds: false
      alwaysOn: false
      http20Enabled: true
      minTlsVersion: '1.2'
      functionAppScaleLimit: 200
      minimumElasticInstanceCount: 1
    }
    containerSize: 1536
    dailyMemoryTimeQuota: 0
    scmSiteAlsoStopped: false
    clientAffinityEnabled: false
    clientCertEnabled: false
    clientCertMode: 'Required'
    hostNamesDisabled: false
    httpsOnly: true
    redundancyMode: 'None'
    storageAccountRequired: false
    virtualNetworkSubnetId: '${vNet}/subnets/${faSubnet}'
  }
  resource config 'config' = {
    name: 'web'
    properties: {
      numberOfWorkers: 1
      netFrameworkVersion: 'v4.0'
      use32BitWorkerProcess: false
      scmType: 'None'
      http20Enabled: true
      minTlsVersion: '1.2'
      preWarmedInstanceCount: 1
      minimumElasticInstanceCount: 1
      appSettings: [
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'TaskHubName'
          value: 'dataacceleratortaskhub'
        }
        {
          name: 'AzureWebJobsStorage'
          value: storageAccountConnectionString
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: storageAccountConnectionString
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: '${dIPCompany}-${dIPName}-dataaccelerator-${dIPEnvironment}-func'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsightsInstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: 'InstrumentationKey=${appInsightsInstrumentationKey}'
        }
        {
          name: 'IOCContainer:Type'
          value: 'Blob'
        }
        {
          name: 'IOCContainer:SQLEnvironments:Default:Name'
          value: 'Default'
        }
        {
          name: 'IOCContainer:SQLEnvironments:Default:ConnectionString'
          value: odsConnectionString
        }
        {
          name: 'IOCContainer:StorageEnvironments:Default:Name'
          value: 'Default'
        }
        {
          name: 'IOCContainer:StorageEnvironments:Default:ConnectionString'
          value: storageAccountConnectionString
        }
        {
          name: 'IOCContainer:StorageEnvironments:Default:QueryContainer'
          value: 'data-accelerator-queries'
        }
        {
          name: 'IOCContainer:StorageEnvironments:Default:StoredProcedureContainer'
          value: 'data-accelerator-stored-procedures'
        }
        {
          name: 'IOCContainer:StorageEnvironments:Default:MappingContainer'
          value: 'data-accelerator-mappings'
        }
      ]
    }
  }
  resource slotConfig 'config' = {
    name: 'slotConfigNames'
    properties: {
      appSettingNames: [
        'APPINSIGHTS_INSTRUMENTATIONKEY'
        'TaskHubName'
      ]
    }
  }
  resource staging 'slots' = {
    name: 'stg'
    location: dIPLocation
    identity: {
      type: 'SystemAssigned'
    }
    properties: {
      serverFarmId: serverFarmId
      reserved: false
      clientAffinityEnabled: false
      clientCertEnabled: false
      hostNamesDisabled: false
      httpsOnly: true
    }
    resource config 'config' = {
      name: 'web'
      properties: {
        appSettings: [
          {
            name: 'FUNCTIONS_EXTENSION_VERSION'
            value: '~3'
          }
          {
            name: 'FUNCTIONS_WORKER_RUNTIME'
            value: 'dotnet'
          }
          {
            name: 'TaskHubName'
            value: 'dataacceleratorstgtaskhub'
          }
          {
            name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
            value: storageAccountConnectionString
          }
          {
            name: 'AzureWebJobsStorage'
            value: storageAccountConnectionString
          }
          {
            name: 'WEBSITE_CONTENTSHARE'
            value: '${dIPCompany}-${dIPName}-dataaccelerator-${dIPEnvironment}-func'
          }
          {
            name: 'IOCContainer:Type'
            value: 'Blob'
          }
          {
            name: 'IOCContainer:SQLEnvironments:Default:Name'
            value: 'Default'
          }
          {
            name: 'IOCContainer:SQLEnvironments:Default:ConnectionString'
            value: odsConnectionString
          }
          {
            name: 'IOCContainer:StorageEnvironments:Default:Name'
            value: 'Default'
          }
          {
            name: 'IOCContainer:StorageEnvironments:Default:ConnectionString'
            value: storageAccountConnectionString
          }
          {
            name: 'IOCContainer:StorageEnvironments:Default:QueryContainer'
            value: 'data-accelerator-queries'
          }
          {
            name: 'IOCContainer:StorageEnvironments:Default:StoredProcedureContainer'
            value: 'data-accelerator-stored-procedures'
          }
          {
            name: 'IOCContainer:StorageEnvironments:Default:MappingContainer'
            value: 'data-accelerator-mappings'
          }
        ]
      }
    }
  }
  resource vNetConnection 'virtualNetworkConnections@2021-03-01' = {
    name: 'vnetconnection'
    properties: {
      vnetResourceId: '${vNet}/subnets/${faSubnet}'
      isSwift: true
    }
  }
}
resource dataAcceleratorPrivateEndpoint 'Microsoft.Network/privateEndpoints@2021-05-01' = {
  name: '${dIPCompany}-${dIPName}-dataaccelerator-${dIPEnvironment}-func-pe'
  location: dIPLocation
  properties: {
    privateLinkServiceConnections: [
      {
        name: '${dIPCompany}-${dIPName}-dataaccelerator-${dIPEnvironment}-func-pe-sc'
        properties: {
          privateLinkServiceId: dataAccelerator.id
          privateLinkServiceConnectionState: {
            status: 'Approved'
            actionsRequired: 'None'
          }
          groupIds: [
            'sites'
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
            privateDnsZoneId: privateDnsZone
          }
        }
      ]
    }
  }
}
