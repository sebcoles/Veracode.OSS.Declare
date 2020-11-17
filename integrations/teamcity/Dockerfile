FROM jetbrains/teamcity-agent

# Install dotnet core SDK
RUN curl -LO https://dotnet.microsoft.com/download/dotnet-core/scripts/v1/dotnet-install.sh
RUN chmod +x ./dotnet-install.sh
RUN ./dotnet-install.sh
RUN rm dotnet-install.sh

# Install Declare
RUN curl -LO https://raw.githubusercontent.com/sebcoles/Veracode.OSS.Declare/master/scripts/get_latest.sh
RUN chmod +x ./get_latest.sh
RUN ./get_latest.sh
RUN rm get_latest.sh

# Install Declare Artifactory Provider
RUN curl -LO https://raw.githubusercontent.com/sebcoles/Veracode.OSS.Declare.Artifactory/main/scripts/get_latest.sh
RUN chmod +x ./get_latest.sh
RUN ./get_latest.sh
RUN rm get_latest.sh