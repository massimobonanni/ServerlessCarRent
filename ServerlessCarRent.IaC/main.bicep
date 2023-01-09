targetScope = 'subscription'

@description('The name of the resource group that contains all the resources')
param resourceGroupName string = 'ServerlessCarRent-rg'

@description('The name of the environment. It will be used to create the name of the resources in the resource group.')
@maxLength(16)
@minLength(3)
param environmentName string = 'scr${uniqueString(subscription().id,resourceGroupName)}'

@description('The location of the resource group and resources')
param location string = deployment().location

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: resourceGroupName
  location: location
}

module infraModule 'infrastructure.bicep' = {
  scope: resourceGroup
  name: 'infrastructure'
  params: {
    location: location
    environmentName: environmentName
  }
}

module integrationModule 'integration.bicep' = {
  scope: resourceGroup
  name: 'integration'
  params: {
    location: location
    environmentName: environmentName
  }
}

module frontendModule 'frontend.bicep' = {
  scope: resourceGroup
  name: 'frontend'
  params: {
    location: location
    environmentName: environmentName
    keyVaultName: infraModule.outputs.keyVaultName
    backEndFunctionName:backendModule.outputs.backEndFunctionname
  }
}

module backendModule 'backend.bicep' = {
  scope: resourceGroup
  name: 'backend'
  params: {
    location: location
    environmentName: environmentName
    keyVaultName:infraModule.outputs.keyVaultName
    eventGridTopicName:integrationModule.outputs.eventGridTopicName
  }
}
