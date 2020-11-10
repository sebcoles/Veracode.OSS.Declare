
`mitigation -f "_LOCATION_OF_CONFIG_FILE_"`

`mitigation --policyonly -f "_LOCATION_OF_CONFIG_FILE_"`



### Description
Mitigations helps generate the configuration required to include mitigations in your veracode configuration. It will generate the template with the flaw details, you will need to include the TSRV and ideally a link to the code in your repo for review by your security team. The `policyonly` will only return open flaws that are breaking compliance.

### Usage
Add the tool to your path or run from the directory containing the binary.

`.\Veracode.OSS.Declare mitigation -f "_LOCATION_OF_CONFIG_FILE_"`

If you only want templates to the flaws that break policy, use the `--policyonly` flag

`.\Veracode.OSS.Declare mitigation --policyonly -f "_LOCATION_OF_CONFIG_FILE_"`

### Example Output

```
| INFO|Entering Scan with scan options {"IgnoreSchedule":false,"JsonFileLocation":".\\veracode.complete.json"}
| WARN|[Test App] Has not had a scan yet, the first scan is due.
| INFO|Starting scan for Test App
| INFO|Files being scanned are:
| INFO|Assets/verademo.war
| INFO|Modules being scanned are:
| INFO|module_name=verademo.war,entry_point=True
| INFO|module_name=JS files within verademo.war,entry_point=True
| INFO|New scan created with Build Id ********. Uploading files
| INFO|Uploading Assets/verademo.war to scan ********.
| INFO|Upload of Assets/verademo.war complete.
| INFO|Scan ******** is still running and has been running for 00:00:01.02.
| INFO|Scan ******** is still running and has been running for 00:00:22.16.
| INFO|Scan ******** is still running and has been running for 00:00:43.63.
| INFO|Scan ******** is still running and has been running for 00:01:05.73.
| INFO|Scan ******** is still running and has been running for 00:01:27.54.
| INFO|Scan ******** is still running and has been running for 00:01:50.26.
| INFO|Scan complete for ******** and took 00:05:13.89.
| WARN|PRE SCAN ERRORS: verademo.war:No supporting files or PDB files
| WARN|PRE SCAN ERRORS: JS files within verademo.war:No supporting files or PDB files
| INFO|Module selection conforms for ******** and the scan can commence.
| INFO|Configuration conforms.
| INFO|Starting scan.
| INFO|Scan ******** is still running and has been running for 00:00:01.43.
| INFO|Scan ******** is still running and has been running for 00:00:23.20.
| INFO|Scan ******** is still running and has been running for 00:00:44.86.
| INFO|Scan ******** is still running and has been running for 00:01:54.72.
| INFO|Scan complete for ******** and took 00:02:16.25.
| INFO|Deployment complete.
| INFO|Exiting Scan with value 1

```

### Troubleshooting
