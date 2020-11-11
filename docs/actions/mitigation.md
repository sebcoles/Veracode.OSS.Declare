### Description
Mitigations helps generate the configuration required to include mitigations in your veracode configuration. It will generate the template with the flaw details, you will need to include the TSRV and ideally a link to the code in your repo for review by your security team. The `policyonly` will only return open flaws that are breaking compliance.

### Usage
Add the tool to your path or run from the directory containing the binary.

`.\Veracode.OSS.Declare mitigation -f "_LOCATION_OF_CONFIG_FILE_"`

If you only want templates to the flaws that break policy, use the `--policyonly` flag

`.\Veracode.OSS.Declare mitigation --policyonly -f "_LOCATION_OF_CONFIG_FILE_"`

You can then copy the output of this tool to your Declare configuraiton file. The mitigations will then be applied via the `configure` command.

### Example Output

```
| INFO|Scan options are {"PolicyOnly":true,"JsonFileLocation":".\\veracode.complete.json","Language":"en-GB"}
| INFO|Generating mitigations templates for Test App
| INFO|"mitigations":[
{
  "flaw_id": "18",
  "cwe_id": "78",
  "file_name": "ToolsController.java",
  "line_number": "56",
  "link": "__ADD_A_REPOSITORY_LINK__",
  "action": "fp || appdesign || osenv || netenv",
  "technique": "__ENTER_TECHNIQUES__",
  "specifics": "__ENTER_SPECIFICS__",
  "remaining_risk": "__ENTER_REMAINING_RISK__",
  "verification": "__ENTER_VERIFICATION__"
}
...omitted for brevity
{
  "flaw_id": "44",
  "cwe_id": "78",
  "file_name": "ToolsController.java",
  "line_number": "59",
  "link": "__ADD_A_REPOSITORY_LINK__",
  "action": "fp || appdesign || osenv || netenv",
  "technique": "__ENTER_TECHNIQUES__",
  "specifics": "__ENTER_SPECIFICS__",
  "remaining_risk": "__ENTER_REMAINING_RISK__",
  "verification": "__ENTER_VERIFICATION__"
}]
| INFO|Generated mitigations templates for Test App
```

### Troubleshooting
