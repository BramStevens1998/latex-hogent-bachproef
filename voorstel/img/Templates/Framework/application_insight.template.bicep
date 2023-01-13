//* --- PARAMETERS ---
param dIPEnvironment string
param dIPCompany string
param dIPLocation string
param dIPName string
param kvName string

//* --- EXISTING RESOURCES ---
resource AIKeyVault 'Microsoft.KeyVault/vaults@2021-06-01-preview' existing = {
  name: kvName
}

//* --- RESOURCES ---
resource applicationinsight 'Microsoft.Insights/components@2020-02-02' = {
  name: '${dIPCompany}-${dIPName}-${dIPEnvironment}-ia'
  location: dIPLocation
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Bluefield'
    IngestionMode: 'ApplicationInsights'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    Request_Source: 'AppServiceEnablementCreate'
    RetentionInDays: 30
  }
}

//* --- SECRETS ---
resource AppInsightsKeySecret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: 'ai-key-${dIPEnvironment}'
  parent: AIKeyVault 
  properties: {
      value: reference(applicationinsight.id, '2014-04-01').InstrumentationKey
  }
}

//* --- OUTPUTS ---
output appInsightsKeySecret string = AppInsightsKeySecret.name
output appInsightsKey string = reference(applicationinsight.id, '2014-04-01').InstrumentationKey
