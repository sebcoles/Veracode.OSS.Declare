### Description
Evaluate will check the scan run status and compliance status of your policy and sandboxes.

### Usage
`evaluate -f "_LOCATION_OF_CONFIG_FILE_"`

Evaluate only uses the application profile name in your configuration file. Using a basic configuration such as

```
{
  "application_profiles": [
    {
      "application_name": "Test App"
    }
  ]
}
```

### Example Output

```
[Test App][Policy][Scan Status] Results Ready
[Test App][Policy][Compliance Status] Did Not Pass
[Test App][Sandbox Seb Coles's Sandbox][Scan Status] There are no scans!
```