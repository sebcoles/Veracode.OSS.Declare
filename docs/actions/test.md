### Description
Test will initiate a Veracode prescan and check to see if your scan configuration is valid. The crux to this is making sure module selection is part of the pull request process. Module selection is the biggest challenge with Veracode, it is easy to get wrong. I hope this way of working helps people make a correct module selection. If your scan configuration fails the test, the tool will print out what module selection is missing from your configuration.

### Example In-Complete Configuration
The below configuration does not have a completed module selection. The test will fail and provide the expected modules configuration.
```
{
  "application_profiles": [
    {
      "policy_schedule": "0 0 * * *",
      "application_name": "Test App",
      "criticality": "Very High",
      "business_owner": "seb",
      "business_owner_email": "scoles@veracode.com",
      "upload": [
        {
          "location": "Assets/verademo.war"
        }
      ],
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
      "mitigations": []
    }
  ]
}

```

### Usage
Add the tool to your path or run from the directory containing the binary.

`.\Veracode.OSS.Declare test -f "_LOCATION_OF_CONFIG_FILE_"`


### Example Output for In-Complete Configuration

```
| INFO|Entering Test with scan options {"LastScan":false,"JsonFileLocation":".\\veracode.incomplete.json"}
| INFO|Testing configuration for Test App
| INFO|Testing against a new scan
| INFO|New scan created with Build Id ********. Uploading files
| INFO|Uploading Assets/verademo.war to scan ********.
| INFO|Upload of Assets/verademo.war complete.
| INFO|Scan ******** is still running and has been running for 00:00:01.79.
| INFO|Scan ******** is still running and has been running for 00:00:23.89.
| INFO|Scan ******** is still running and has been running for 00:00:45.99.
| INFO|Scan ******** is still running and has been running for 00:01:08.29.
| INFO|Scan complete for ******** and took 00:01:30.28.
| WARN|There are 2 modules from prescan that do not match the config.
| WARN|Please include and complete the below configuration and add to your .json file
| INFO|"modules":[
{ "module_name":"verademo.war" ,"entry_point":"true"},
{ "module_name":"JS files within verademo.war" ,"entry_point":"true"}
]
| WARN|PRE SCAN ERRORS: verademo.war:No supporting files or PDB files
| WARN|PRE SCAN ERRORS: JS files within verademo.war:No supporting files or PDB files
| WARN|Module selection configuration was incorrect for ********.
| INFO|Test Finished. Deleting Build Id ********.
| INFO|Scan does not conform. Deleting Build Id ********.
| WARN|Application Test App scan config DOES NOT conforms.
| INFO|Scan Configuration testing complete.
| INFO|Exiting Test with value 1
```

### Example Complete Configuration
The below configuration now has a valid module configuration so the test will pass. 

```
{
  "application_profiles": [
    {
      "policy_schedule": "0 0 * * *",
      "application_name": "Test App",
      "criticality": "Very High",
      "business_owner": "seb",
      "business_owner_email": "scoles@veracode.com",
      "upload": [
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

### Example Output for In-Complete Configuration

```
| INFO|Entering Test with scan options {"LastScan":false,"JsonFileLocation":".\\veracode.complete.json"}
| INFO|Testing configuration for Test App
| INFO|Testing against a new scan
| INFO|New scan created with Build Id ********. Uploading files
| INFO|Uploading Assets/verademo.war to scan ********.
| INFO|Upload of Assets/verademo.war complete.
| INFO|Scan ******** is still running and has been running for 00:00:02.34.
| INFO|Scan ******** is still running and has been running for 00:00:23.83.
| INFO|Scan ******** is still running and has been running for 00:00:45.69.
| INFO|Scan ******** is still running and has been running for 00:01:07.37.
| INFO|Scan ******** is still running and has been running for 00:01:29.26.
| INFO|Scan ******** is still running and has been running for 00:01:50.99.
| INFO|Scan complete for ******** and took 00:02:12.72.
| WARN|PRE SCAN ERRORS: verademo.war:No supporting files or PDB files
| WARN|PRE SCAN ERRORS: JS files within verademo.war:No supporting files or PDB files
| INFO|Module selection conforms for ******** and the scan can commence.
| INFO|Test Finished. Deleting Build Id ********.
| INFO|Configuration conforms.
| INFO|Application Test App scan config DOES conforms.
| INFO|Scan Configuration testing complete.
| INFO|Exiting Test with value 1
```