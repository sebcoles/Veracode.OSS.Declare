# Description
Veracode Declare files can also describe the binaries that need to be download for the scan. This is an optional step and requires an auxillary binary to read the configuration and download the artifacts. 

The [Declare Artifactory Provider](https://github.com/sebcoles/Veracode.OSS.Declare.Artifactory) is available to support artifactory integrations. This release also include the JFrog CLI which pulls the binaries from artifactory as described in the configuration.


### Setup
To use the artifactory provider you must create an [Artifactory Access Token](https://www.jfrog.com/confluence/display/CLI/CLI+for+JFrog+Artifactory#CLIforJFrogArtifactory-AuthenticatingwithanAccessToken) that can be used with your instance of artifactory. It will need to have permissions to the artifactory repositories you describe in your configuration. This key needs to be set in the appsettings.json alongside the executable.

```
{
  "ArtifactoryUrl": "https://__YOUR_ARTIFACTORY_DOMAIN__/artifactory",
  "ArtifactoryApiKey": "__YOUR_ARTIFACTORY_ACCESS_TOKEN__"
}
```

note: You must include /artifactory on the end of your artifactory domain.

### Usage
You can then invoke the provider by calling the below command

```
Veracode.OSS.Declare.Artifactory -f __YOUR_DECLARE_CONFIG_FILE__ -t "Artifactory/"
```

`-t` is a target destination for the binary downloads, I would recommend placing this in a folder called Artifactory. Note whatever location you save the artifacts to will need to be reflected in the Declare `upload` section when running a scan.


### Example Configuration
To use the artifactory provider you must include the below snippet within your Declare configuration file.

```
{
  "application_profiles": [
    {
      "download": [
        {
          "name": "artifactory",
          "files": [
            { "location": "__ARTIFACTORY_URL_PATH__" },
            { "location": "mvn-public-local/org/owasp/encoder/encoder/1.2.3/encoder-1.2.3.jar" }
          ]
        }
      ],
    }
}
```

### Example Output for In-Complete Configuration

```
| INFO|Artifactory provider configuration found
| INFO|Starting download for mvn-public-local/org/owasp/encoder/encoder/1.2.3/encoder-1.2.3.jar
 Log path: %USERPROFILE%\.jfrog\logs\jfrog-cli.log
{
  "status": "success",
  "totals": {
    "success": 1,
    "failure": 0
  }
}
| INFO|Download complete for mvn-public-local/org/owasp/encoder/encoder/1.2.3/encoder-1.2.3.jar
```