@description('The location wher you want to create the resources.')
param location string = resourceGroup().location

@description('The name of the environment. It will be used to create the name of the resources in the resource group.')
@maxLength(16)
@minLength(3)
param environmentName string = 'sfa${uniqueString(subscription().id, resourceGroup().name)}'

@description('The name of the KeyVault that contains secrets.')
param keyVaultName string 

@description('The name of the custom event grid topic.')
param eventGridTopicName string


var functionAppStorageAccountName = toLower('${environmentName}appstore')
var funcHostingPlanName = toLower('${environmentName}-plan')
var functionAppName = toLower('${environmentName}-func')

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource eventGridTopic 'Microsoft.EventGrid/topics@2022-06-15' existing = {
  name: eventGridTopicName
}

resource appServiceKeyVaultAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid('Key Vault Secret User', functionAppName, subscription().subscriptionId)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // this is the role "Key Vault Secrets User"
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource azureWebJobsStorageSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: 'AzureWebJobsStorage'
  parent: keyVault
  properties: {
    attributes: {
      enabled: true
    }
    value: 'DefaultEndpointsProtocol=https;AccountName=${functionAppStorageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${functionAppStorageAccount.listKeys().keys[0].value}'
  }
}

resource AdminEmailSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: 'AdminEmail'
  parent: keyVault
  properties: {
    attributes: {
      enabled: true
    }
    value: ''
  }
}

resource SendGridApiKeySecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: 'SendGridApiKey'
  parent: keyVault
  properties: {
    attributes: {
      enabled: true
    }
    value: ''
  }
}

resource eventGridTopicEndpointSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: 'EventGridTopicServiceEndpoint'
  parent: keyVault
  properties: {
    attributes: {
      enabled: true
    }
    value: eventGridTopic.properties.endpoint
  }
}

resource eventGridTopicKeySecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: 'EventGridTopicKey'
  parent: keyVault
  properties: {
    attributes: {
      enabled: true
    }
    value: eventGridTopic.listKeys().key1
  }
}

resource functionAppStorageAccount 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: functionAppStorageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource funcHostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: funcHostingPlanName
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  properties: {}
}

resource functionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: funcHostingPlan.id
    siteConfig: {
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
}

resource appSettings 'Microsoft.Web/sites/config@2022-03-01' = {
  name: 'appsettings'
  parent: functionApp
  properties: {
    AzureWebJobsStorage: '@Microsoft.KeyVault(SecretUri=${azureWebJobsStorageSecret.properties.secretUri})'
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: '@Microsoft.KeyVault(SecretUri=${azureWebJobsStorageSecret.properties.secretUri})'
    WEBSITE_CONTENTSHARE: toLower(functionAppName)
    FUNCTIONS_EXTENSION_VERSION: '~4'
    WEBSITE_NODE_DEFAULT_VERSION: '~10'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    AdminEmail:'@Microsoft.KeyVault(SecretUri=${AdminEmailSecret.properties.secretUri})'
    SendGridApiKey:'@Microsoft.KeyVault(SecretUri=${SendGridApiKeySecret.properties.secretUri})'
    TopicEndpoint: '@Microsoft.KeyVault(SecretUri=${eventGridTopicEndpointSecret.properties.secretUri})'
    TopicKey: '@Microsoft.KeyVault(SecretUri=${eventGridTopicKeySecret.properties.secretUri})'
  }
  dependsOn:[
    keyVault
    appServiceKeyVaultAssignment
  ]
}

output backEndFunctionname string = functionApp.name
