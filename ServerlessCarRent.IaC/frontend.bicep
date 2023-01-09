@description('The location wher you want to create the resources.')
param location string = resourceGroup().location

@description('The name of the environment. It will be used to create the name of the resources in the resource group.')
@maxLength(16)
@minLength(3)
param environmentName string = 'sfa${uniqueString(subscription().id, resourceGroup().name)}'

@description('The name of the KeyVault that contains secrets.')
param keyVaultName string 

@description('The name of the function app that implements backend APIs.')
param backEndFunctionName string

var appServiceName = toLower('${environmentName}-fe')
var appServicePlanName = toLower('${environmentName}-fe-plan')


resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource functionApp 'Microsoft.Web/sites@2021-03-01' existing = {
  name: backEndFunctionName
}

resource appServiceKeyVaultAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid('Key Vault Secret User', appServiceName, subscription().subscriptionId)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // this is the role "Key Vault Secrets User"
    principalId: frontEndAppService.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource backendUrlSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: 'BackEndUrl'
  parent: keyVault
  properties: {
    attributes: {
      enabled: true
    }
    value: 'https://${functionApp.properties.defaultHostName}'
  }
}

resource BackEndKeySecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: 'BackEndKey'
  parent: keyVault
  properties: {
    attributes: {
      enabled: true
    }
    value: listkeys('${functionApp.id}/host/default', '2016-08-01').functionKeys.default
  }
}

resource frontEndAppServicePlan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: appServicePlanName
  location: location
  properties: {
    reserved: false
  }
  sku: {
    name:'F1'
  }
  kind: 'windows'
}

resource frontEndAppService 'Microsoft.Web/sites@2021-02-01' = {
  name: appServiceName
  location: location
  kind: 'app'
  properties: {
    httpsOnly: true
    serverFarmId: frontEndAppServicePlan.id
    siteConfig: {
      netFrameworkVersion: 'v6.0'
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource appSettings 'Microsoft.Web/sites/config@2022-03-01' = {
  name: 'appsettings'
  parent: frontEndAppService
  properties: {
    'APISettings:BaseUrl': '@Microsoft.KeyVault(SecretUri=${backendUrlSecret.properties.secretUri})'
    'APISettings:ApiKey': '@Microsoft.KeyVault(SecretUri=${BackEndKeySecret.properties.secretUri})'
  }
  dependsOn:[
    keyVault
    appServiceKeyVaultAssignment
  ]
}


output frontEndHostName string = frontEndAppService.properties.defaultHostName
