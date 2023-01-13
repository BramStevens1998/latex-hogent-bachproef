//* --- PARAMETERS ---
param dIPLocation string
param dIPCompany string
param dIPName string
param dIPEnvironment string
@secure()
param appInsightsKey string
param storageAccountName string
param usePE bool
param vNet string
param privateDnsZone string

resource hostingPlan 'Microsoft.Web/serverfarms@2020-10-01' = {
  name: '${dIPCompany}-${dIPName}-${dIPEnvironment}-app'
  location: dIPLocation
  sku: {
    name: 'Y1' 
    tier: 'Dynamic'
  }
  properties: {}
}


resource functionapp_resource 'Microsoft.Web/sites@2022-03-01' = {
  name: '${dIPCompany}-${dIPName}-${dIPEnvironment}-functionapp'
  location: dIPLocation
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: hostingPlan.id
    clientAffinityEnabled: true
    reserved: true
    siteConfig: {
    }
  }
}

resource functionSlotStaging 'Microsoft.Web/sites/slots@2022-03-01' = {
  name: '${functionapp_resource.name}/staging'
  location: dIPLocation
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    enabled: true
    httpsOnly: true
  }
}

resource functionapp_web 'Microsoft.Web/sites/config@2022-03-01' = {
  parent: functionapp_resource
  name: 'slotConfigNames'
  properties: {
    appSettingNames: [
      'APP_CONFIGURATION_LABEL'
    ]
  }
}

module appService_appSettings 'functionapp.template.settings.bicep' = {
  name: '${dIPCompany}-${dIPName}-${dIPEnvironment}-appConfig'
  params: {
    applicationInsightsInstrumentationKey: appInsightsKey
    storageAccountName: storageAccountName
    functionAppName: functionapp_resource.name
    functionAppStagingSlotName: functionSlotStaging.name
  }
}

resource functionAppPrivateEndpoint 'Microsoft.Network/privateEndpoints@2021-05-01' = if(usePE) {
  name: '${dIPCompany}-${dIPName}-da-${dIPEnvironment}-func-pe'
  location: dIPLocation
  properties: {
    privateLinkServiceConnections: [
      {
        name: '${dIPCompany}-${dIPName}-da-${dIPEnvironment}-func-pe-sc'
        properties: {
          privateLinkServiceId: functionapp_resource.id
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

output functionAppName string = functionapp_resource.name
output functionAppSlotName string = functionSlotStaging.name


