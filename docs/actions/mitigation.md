Mitigations helps generate the configuration required to include mitigations in your veracode configuration. It will generate the template with the flaw details, you will need to include the TSRV and ideally a link to the code in your repo for review by your security team. The `policyonly` will only return open flaws that are breaking compliance.

`mitigation -f "_LOCATION_OF_CONFIG_FILE_"`

`mitigation --policyonly -f "_LOCATION_OF_CONFIG_FILE_"`

