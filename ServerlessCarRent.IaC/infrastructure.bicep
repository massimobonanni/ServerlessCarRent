@description('The location wher you want to create the resources.')
param location string = resourceGroup().location

@description('The name of the environment. It will be used to create the name of the resources in the resource group.')
@maxLength(16)
@minLength(3)
param environmentName string = 'sfa${uniqueString(subscription().id, resourceGroup().name)}'

var keyVaultName = toLower('${environmentName}-kv')

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01'={
  name: keyVaultName
  location: location
  properties: {
    accessPolicies: []
    enableRbacAuthorization: true
    enableSoftDelete: false
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    tenantId: subscription().tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
}

output keyVaultName string = keyVault.name
