@description('The location wher you want to create the resources.')
param location string = resourceGroup().location

@description('The name of the environment. It will be used to create the name of the resources in the resource group.')
@maxLength(16)
@minLength(3)
param environmentName string = 'sfa${uniqueString(subscription().id, resourceGroup().name)}'

var eventGridTopicName = toLower('${environmentName}-topic')

resource eventGridTopic 'Microsoft.EventGrid/topics@2022-06-15' = {
  name: eventGridTopicName
  location: location
  properties: {
    inputSchema: 'EventGridSchema'
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: false
    dataResidencyBoundary: 'WithinGeopair'
  }
}

//-------------------------------------------------------------
// EventGrid Event Viewer
//-------------------------------------------------------------
var eventViewerAppName = toLower('${environmentName}-eventviewer')
var eventViewerAppPlanName = toLower('${environmentName}-eventviewerplan')
var viewerRepoUrl = 'https://github.com/azure-samples/azure-event-grid-viewer.git'

resource eventViewerAppServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: eventViewerAppPlanName
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
    size: 'F1'
    family: 'F'
    capacity: 0
  }
  properties: {}
  kind: 'app'
}

resource eventViewerAppService 'Microsoft.Web/sites@2022-03-01' = {
  name: eventViewerAppName
  location: location
  kind: 'app'
  properties: {
    serverFarmId: eventViewerAppServicePlan.id
    hostNameSslStates: [
      {
        hostType: 'Standard'
        sslState: 'Disabled'
        name: '${eventViewerAppName}.azurewebsites.net'
      }
      {
        hostType: 'Standard'
        sslState: 'Disabled'
        name: '${eventViewerAppName}.scm.azurewebsites.net'
      }
    ]
    siteConfig: {
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
      netFrameworkVersion:'v6.0'
    }
    httpsOnly: true
  }
}

resource eventViewerAppServiceDeploy 'Microsoft.Web/sites/sourcecontrols@2022-03-01' = {
  parent: eventViewerAppService
  name: 'web'
  properties: {
    repoUrl: viewerRepoUrl
    branch: 'main'
    isManualIntegration: true
  }
}

//-------------------------------------------------------------
// EventGrid Event Viewer Subscription
//-------------------------------------------------------------
var eventViewerSubName = toLower('${environmentName}-eventviewersub')

resource eventViewerSubscription 'Microsoft.EventGrid/eventSubscriptions@2022-06-15' = {
  name: eventViewerSubName
  scope: eventGridTopic
  properties: {
    destination: {
      endpointType: 'WebHook'
      properties: {
        endpointUrl: 'https://${eventViewerAppService.properties.defaultHostName}/api/updates'
      }
    }
  }
  dependsOn:[
    eventViewerAppServiceDeploy
  ]
}

output eventGridTopicName string = eventGridTopic.name
