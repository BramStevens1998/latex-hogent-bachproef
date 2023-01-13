
//* --- PARAMETERS ---
param dIPLocation string
param dIPCompany string
param dIPName string
param dIPEnvironment string
param adminName string
param adminPW string
param usePE bool
param vNet string
param privateDnsZone string

resource sql_datahub 'Microsoft.Sql/servers@2022-02-01-preview' = {
  name: '${dIPCompany}-${dIPName}-${dIPEnvironment}-SQLServer'
  location: dIPLocation
  kind: 'v12.0'
  properties: {
    administratorLogin: adminName
    administratorLoginPassword: adminPW 
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    restrictOutboundNetworkAccess: 'Disabled'
  }
}

resource sqldb_datahub 'Microsoft.Sql/servers/databases@2022-02-01-preview' = {
  parent: sql_datahub
  name: '${dIPCompany}-${dIPName}-${dIPEnvironment}-database'
  location: dIPLocation
  sku: {
    name: 'GP_S_Gen5'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: 1
  }
  kind: 'v12.0,user,vcore,serverless'
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 34359738368
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: false
    readScale: 'Disabled'
    autoPauseDelay: 60
    requestedBackupStorageRedundancy: 'Local'
    minCapacity: '0.5'
    maintenanceConfigurationId: '/subscriptions/71e33943-63fc-4631-85c7-ed164c40c1cd/providers/Microsoft.Maintenance/publicMaintenanceConfigurations/SQL_Default'
    isLedgerOn: false
  }
}

resource sql_datahub_firewall_azure 'Microsoft.Sql/servers/firewallRules@2022-02-01-preview' = {
  parent: sql_datahub
  name: 'AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource dataAcceleratorPrivateEndpoint 'Microsoft.Network/privateEndpoints@2021-05-01' = if(usePE) {
  name: '${dIPCompany}-${dIPName}-da-${dIPEnvironment}-func-pe'
  location: dIPLocation
  properties: {
    privateLinkServiceConnections: [
      {
        name: '${dIPCompany}-${dIPName}-da-${dIPEnvironment}-func-pe-sc'
        properties: {
          privateLinkServiceId: sql_datahub.id
          privateLinkServiceConnectionState: {
            status: 'Approved'
            actionsRequired: 'None'
          }
          groupIds: [
            'sqlServer'
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

