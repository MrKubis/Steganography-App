FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY *.sln .
COPY DCTClass/DCTClass.csproj ./DCTClass/
COPY LSBClass/LSBClass.csproj ./LSBClass/
COPY Stenography.API/Stenography.API.csproj ./Stenography.API/
RUN dotnet restore
COPY . .
WORKDIR /src/Stenography.API
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_HTTP_PORTS=5149
EXPOSE 5149
ENTRYPOINT ["dotnet", "Stenography.API.dll"]