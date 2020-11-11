### Description
Configure will create the Veracode components specified in the configuration.

This currently supports creating:
- Application Profile
- Policy
- Team
- Users
- Sandboxes
- Mitigations

### Example Configuration
The below configuration can be used to create a profile for the application "Test App".

```
{
  "application_profiles": [
    {
      "policy_schedule": "0 0 * * *",
      "application_name": "Test App",
      "criticality": "Very High",
      "business_owner": "seb",
      "business_owner_email": "scoles@veracode.com",
      "upload": [],
      "modules": [],
      "policy": {
        "custom_severities": [
          {
            "cwe": 0,
            "severity": 0
          }
        ],
        "finding_rules": [
          {
            "scan_type": [
              "STATIC"
            ],
            "type": "FAIL_ALL",
            "value": "string"
          }
        ],
        "scan_frequency_rules": [
          {
            "frequency": "NOT_REQUIRED",
            "scan_type": "STATIC"
          }
        ],
        "sev0_grace_period": 0,
        "sev1_grace_period": 0,
        "sev2_grace_period": 0,
        "sev3_grace_period": 0,
        "sev4_grace_period": 0,
        "sev5_grace_period": 0
      },
      "users": [
        {
          "first_name": "Seb",
          "last_name": "Coles",
          "email_address": "scoles@veracode.com",
          "roles": "Creator, Any Scan"
        }
      ],
      "sandboxes": [
        {
          "sandbox_name": "Release Candidate"
        }
      ],
      "mitigations": [
        {
          "flaw_id": "",
          "cve_id": "",
          "file_name": "",
          "line_number": "",
          "link": "",
          "action": "",
          "technique": "",
          "specifics": "",
          "residual_risk": "",
          "verification": ""
        }
      ]
    }
  ]
}

```

### Usage
Add the tool to your path or run from the directory containing the binary.

`.\Veracode.OSS.Declare configure -f "_LOCATION_OF_CONFIG_FILE_"`


### Example Output

```
| INFO|Entering Configure with scan options {"JsonFileLocation":".\\veracode.complete.json"}
| INFO|Starting build for Test App
| INFO|Checking to see if Application Profile Test App already exists.
| INFO|Application Profile Test App does not exist, adding configuration.
| INFO|Application Profile Test App created succesfully.
| INFO|Checking to see if policy for Test App already exists.
| INFO|Policy for Test App does not exist, adding configuration.
| INFO|Policy for Test App created succesfully.
| INFO|Checking to see if team Test App already exists.
| INFO|Team Test App does not exist, adding configuration.
| INFO|Team Test App created succesfully.
| INFO|Checking to see if user scoles@veracode.com already exists.
| INFO|User scoles@veracode.com does not exist, adding configuration.
| INFO|User scoles@veracode.com created succesfully.
| INFO|Test App has no completed scans, cannot apply mitigations.
| INFO|[Test App] Checking Sandboxes...
| INFO|[Test App] Does not have sandbox with name Release Candidate. Creating...
| INFO|[Test App] Release Candidate creation complete!
| INFO|[Test App] Finished Sandboxes!
| INFO|build complete for Test App
| INFO|Exiting Configure with value 1
```

### Troubleshooting
