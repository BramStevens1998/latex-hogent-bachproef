//* --- PARAMETERS ---
param dIPEnvironment string
param dIPCompany string
param dIPLocation string
param dIPName string
param KeyVaultName string
param UserAssignedIdentityName string
param adminPW string
param adminName string
param usePE bool
param vNet string
param privateDnsZoneWeb string
param privateDnsZoneBlob string
param privateDnsZoneQueue string

//* --- EXISTING RESOURCES ---
resource keyvault 'Microsoft.KeyVault/vaults@2021-06-01-preview' existing = {
  name: KeyVaultName
}

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: UserAssignedIdentityName
}

//* --- MODULES ---
module dIP_Framework_StorageAccount 'storage.template.bicep' = {
  name: 'dIP-Framework-StorageAccount'
  params: {
    dIPEnvironment: dIPEnvironment
    dIPCompany: dIPCompany
    dIPLocation: dIPLocation
    dIPName: dIPName
    kvName: keyvault.name
    managedIdentityObjectId: managedIdentity.properties.principalId
    usePE: usePE
    vNet: vNet
    privateDnsZoneBlob: privateDnsZoneBlob
    privateDnsZoneQueue: privateDnsZoneQueue
  }
}

module dIP_Framework_Application_Insight 'application_insight.template.bicep' = {
  name: 'dIP-Framework-Application-Insight'
  params: {
    dIPEnvironment: dIPEnvironment
    dIPCompany: dIPCompany
    dIPLocation: dIPLocation
    dIPName: dIPName
    kvName: keyvault.name
  }
}

module dIP_Framework_Database 'database.template.bicep' = {
  name: 'dIP-Framework-Database'
  params: {
    dIPLocation: dIPLocation
    dIPCompany: dIPCompany
    dIPName: dIPName
    adminName: adminName
    adminPW: adminPW
    dIPEnvironment: dIPEnvironment
    usePE: usePE
    vNet: vNet
    privateDnsZone: privateDnsZoneWeb
  }
}

module dIP_Framework_Function_App 'functionapp.template.bicep' = {
  name: 'dIP-Framework-Function-App'
  params: {
    dIPLocation: dIPLocation
    dIPCompany: dIPCompany
    dIPName: dIPName
    dIPEnvironment: dIPEnvironment
    appInsightsKey: dIP_Framework_Application_Insight.outputs.appInsightsKey
    storageAccountName: dIP_Framework_StorageAccount.outputs.storageAccountName
    usePE: usePE
    vNet: vNet
    privateDnsZone: privateDnsZoneWeb
  }
}

