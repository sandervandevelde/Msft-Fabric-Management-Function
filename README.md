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
  "application client Id": "92ac349c-3cb8-42a7-af0e-4c3292e52d15",
  "displayName": "msftfabriccontributor",
  "password": "aRp8Q~eY6Gkk17WnC-14U1nw4eY2i9wLOi7QycSC",
  "directory tenant Id": "e8bf76a5-4456-4e61-8fdd-a7fa11b9efbf"
}
```

I have taken the liberty to make the client id and tenant id more descriptive.

## Application settings

The Azure Function expects a number of Azure function application settings:

- apiVersion
- appId
- dedicatedCapacityName
- password
- resourceGroupName
- subsciptionId
- tennant

