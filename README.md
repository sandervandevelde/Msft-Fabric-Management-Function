# Microsoft Fabric Management Function App

This Azure Function paused your Microsoft Fabric capacity on a daily bases (at 18:01 UTC).

It first checks the current state. If the capacity is running, the capacity is paused.

## Credits

This Azure functions uses the REST API as found [here](https://github.com/nocsi-zz/fabric-capacity-management/tree/main/postman).

Check out that repository for more details.

## Entra ID application 

The Entra ID application, needed to get access to the service, can be aquired via commandline:

```
az ad sp create-for-rbac -n msftfabriccontributor
```

This will result a result that looks like this:

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

