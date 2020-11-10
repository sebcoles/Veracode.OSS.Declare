### Description
Scan will run through the same test verification, bit will then continue to run a full policy static scan if the schedule requires it.

The schedule is a CRON schedule, when this action is run it will check to see if the last succesfully completed scan occured within the shcedule. If so, the scan is skipped. This check can be skipped by using the `-i` or `--ignore_schedule` parameters.  

### Example
The below configuration has valid module configuration and will run a complete scan. 

```
{
  "application_profiles": [
    {
      "policy_schedule": "0 0 * * *",
      "application_name": "Test App",
      "criticality": "Very High",
      "business_owner": "seb",
      "business_owner_email": "scoles@veracode.com",
      "files": [
        {
          "location": "Assets/verademo.war"
        }
      ],
      "modules": [
        {
          "module_name": "verademo.war",
          "entry_point": "true"
        },
        {
          "module_name": "JS files within verademo.war",
          "entry_point": "true"
        }
      ],
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
      "mitigations": []
    }
  ]
}

```

### Usage
Add the tool to your path or run from the directory containing the binary.

`.\Veracode.OSS.Declare scan -f "_LOCATION_OF_CONFIG_FILE_"`


### Example Output

```

```