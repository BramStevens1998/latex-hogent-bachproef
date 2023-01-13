//* --- PARAMETERS ---
@description('Name of the environment to deploy the resources to')
param dIPEnvironment string
param dIPCompany string
param dIPLocation string = resourceGroup().location
param dIPName string
param KeyvaultName string
param UserAssignedIdentityName string
param adminName string
param adminPW string
param usePE bool
param vNet string
param privateDnsZoneBlob string
param privateDnsZoneQueue string
param privateDnsZoneWeb string

//* --- MODULES ---
module dIP_Framework 'Framework/_framework.template.bicep'  = {
  name: 'dip-framework'
  params: {
    dIPEnvironment: dIPEnvironment
    dIPCompany: dIPCompany
    dIPLocation: dIPLocation
    dIPName: dIPName
    KeyVaultName: KeyvaultName
    UserAssignedIdentityName: UserAssignedIdentityName
    adminName: adminName
    adminPW: adminPW
    usePE: usePE
    vNet: vNet
    privateDnsZoneBlob: privateDnsZoneBlob
    privateDnsZoneQueue: privateDnsZoneQueue
    privateDnsZoneWeb: privateDnsZoneWeb
  }
}
