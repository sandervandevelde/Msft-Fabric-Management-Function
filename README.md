# Microsoft Fabric Management Function App

This Azure Function paused your Microsoft Fabric capacity on a daily basis (at 18:01 UTC).

It first checks the current state. If the capacity is running, the capacity is paused.

## Credits

This Azure function uses the REST API as found [here](https://github.com/nocsi-zz/fabric-capacity-management/tree/main/postman).

Check out that repository for more details.

The [repository](https://github.com/nocsi-zz/fabric-capacity-management/), maintained by [Kasper Kirkegaard](https://github.com/nocsi-zz), offers automated solutions using Azure Data Factory and Fabric data pipelines.

## Entra ID application 

The Entra ID application, needed to get access to the service, can be acquired via the command line:

```
az ad sp create-for-rbac -n msftfabriccontributor
```

Just run it on the command line in the Azure portal.

This will result in a JSON message that looks like this:

```
{
  "application client Id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "displayName": "msftfabriccontributor",
  "password": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "directory tenant Id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
}
```

I have taken the liberty to make the clientId and tenantId more descriptive.

## Application settings

The Azure Function expects a number of Azure function application settings:

- apiVersion
- appId
- dedicatedCapacityName
- password
- resourceGroupName
- subsciptionId
- tennant

