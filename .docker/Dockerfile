FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim

RUN useradd -s /bin/bash -m vscode
RUN groupadd docker && usermod -aG docker vscode

USER vscode

ENTRYPOINT ["sleep", "infinity"]