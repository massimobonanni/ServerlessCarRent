# Serverless Car Rent - IaC

This project contains the bicep templates you can use to create the environment to host the Serverless car Rent project.

To create the resource group and to deploy the resources you need for the project, simply run the following command:

```
az deployment sub create --location <your region> --template-file main.bicep
```

where 
- `<your region>` is the location where you want to create the deployment


You can also set these parameters:

- `location` : the location you want to deploy (by default the location is the same of your deployment)
- `resourceGroupName` : the name of the resource group (by default its value is `ServerlessCarRent-rg`)

```
az deployment sub create --location <your region> --template-file main.bicep --parameters location=<location to deploy> resourceGroupName=<rg name>
```