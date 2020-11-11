### Description
Delete will delete all the Veracode components specified in the configuration file. This is useful for tearing down environments or offboarding applications.

### Usage
`delete -f "_LOCATION_OF_CONFIG_FILE_"`

### Example Output

```
| INFO|Entering Delete with scan options {"JsonFileLocation":".\\veracode.complete.json"}
| INFO|Tearing down Test App
| INFO|Checking if profile exists.
| INFO|Deleting profile.
| INFO|Profile deleted.
| INFO|Checking if policy exists.
| INFO|Deleting policy.
| INFO|Policy deleted.
| INFO|Checking if team exists.
| INFO|Deleting team.
| INFO|Team deleted.
| INFO|Checking if user exists.
| INFO|User deleted.
| INFO|Tear down complete for Test App
| INFO|Exiting Delete with value 1
```