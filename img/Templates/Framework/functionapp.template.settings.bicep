param applicationInsightsInstrumentationKey string
param storageAccountName string
param functionAppName string
param functionAppStagingSlotName string

param appConfiguration_appConfigLabel_value_production string = 'production'
param appConfiguration_appConfigLabel_value_staging string = 'staging'

var BASE_SLOT_APPSETTINGS = {
  APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsightsInstrumentationKey
  APPLICATIONINSIGHTS_CONNECTION_STRING: 'Instrumentationkey=${applicationInsightsInstrumentationKey}'
  AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName}'
  FUNCTIONS_EXTENSION_VERSION: '~4'
  FUNCTIONS_WORKER_RUNTIME: 'node'
  WEBSITE_CONTENTSHARE: toLower(storageAccountName)
  WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=rgsandboxbramstevena87a;AccountKey=Wn1HQ+3gpYR5eyPtxQdVpEqVg2duQvDDLcVadTOLEkVLWUUoQy44iTbebq/qBGyenjfS81DMXx7P+AStO6petA==;EndpointSuffix=core.windows.net'
}
var PROD_SLOT_APPSETTINGS ={
  APP_CONFIGURATION_LABEL: appConfiguration_appConfigLabel_value_production
}

resource functionapp_web 'Microsoft.Web/sites/config@2022-03-01' = {
  name: '${functionAppName}/appsettings'
  properties: union(BASE_SLOT_APPSETTINGS, PROD_SLOT_APPSETTINGS)
}

var STAGING_SLOT_APPSETTINGS ={
  APP_CONFIGURATION_LABEL: appConfiguration_appConfigLabel_value_staging
}

resource functionapp_stagingslot_web 'Microsoft.Web/sites/slots/config@2022-03-01' = {
  name: '${functionAppStagingSlotName}/appsettings'
  properties: union(BASE_SLOT_APPSETTINGS, STAGING_SLOT_APPSETTINGS)
}

