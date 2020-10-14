# VeracodeDSC

This is a GitOps approach to using Veracode, it appears to have aquired a few names such as VeraForm or Veracode-as-code. It is a way of working with Veracode through a configuration files included in the source code of the application.

The advantages of working in a GitOps style
- Allows a development team to onboard themselves into Veracode in a change controlled manner.
- All of your configuration is in source control, including module selection.
- Modifying your Veracode configuration can occur just within the config file, no need to login to th Veracode platform.

Simple, it is decentralised Veracode configuration in a safe way that works with developer practises.


## Usage
As it's a .NET Core application you will need the .NET Core modules to run on a non-windows agent. The tool also relies on the Veracode .credentials file, the location of which can be configured in the appsetings.json.

The executable has 3 options:

### Configure
Configure will create the Veracode components specified in the configuration such as the applicaiton profile, the users, the team and a specific policy for the application.

`configure -f "_LOCATION_OF_CONFIG_FILE_"`

### Test
Test will initiate a Veracode prescan and check to see if your scan configuration is valid. The crux to this is making sure module selection is part of the pull request process. Module selection is the biggest challenge with Veracode, it is easy to get wrong. I hope this way of working helps people make a correct module selection. If your scan configuration fails the test, the tool will print out what module selection is missing from your configuration.

`test -f "_LOCATION_OF_CONFIG_FILE_"`

### Scan
Scan will run through the smae test verification, bit will then continue to run a full policy static scan.

`scan -f "_LOCATION_OF_CONFIG_FILE_"`

