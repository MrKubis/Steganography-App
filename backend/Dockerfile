FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app
COPY . .
RUN dotnet restore
CMD ["dotnet","run","--project","/app/Stenography.API/Stenography.API.csproj","--urls","http://0.0.0.0:5149"]
