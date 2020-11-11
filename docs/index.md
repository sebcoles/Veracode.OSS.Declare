This is a GitOps/Declarative way of working with Veracode through configuration.

The advantages of working in a GitOps style
- Allows a development team to onboard themselves into Veracode in a change controlled manner.
- All of your configuration is in source control, including module selection.
- Modifying your Veracode configuration can occur just within the config file, no need to login to tht Veracode platform.
- Either work from a single centralized configuration, or a distributed configuration accross many repositories. 

## Usage
All actions **require** the below parameters

`-f` of `--jsonfile __LOCATION_CONFIG_FILE__` with the location of the Declare configuration file.

All actions have an optional language flag for limited Localization support.

`-l` of `--language en-GB` with the location of the Declare configuration file.

Supported Localizations
- en-GB (Full)
- de-DE (Partial)
