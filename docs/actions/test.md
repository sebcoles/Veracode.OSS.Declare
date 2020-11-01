Test will initiate a Veracode prescan and check to see if your scan configuration is valid. The crux to this is making sure module selection is part of the pull request process. Module selection is the biggest challenge with Veracode, it is easy to get wrong. I hope this way of working helps people make a correct module selection. If your scan configuration fails the test, the tool will print out what module selection is missing from your configuration.

`test -f "_LOCATION_OF_CONFIG_FILE_"`
