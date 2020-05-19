FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build
WORKDIR /app
COPY ./Yapi.Contracts/*.csproj ./Yapi.Contracts/
COPY ./YAPI.IntegrationTest/*.csproj ./YAPI.IntegrationTest/
COPY ./Yapi.Sdk/*.csproj ./Yapi.Sdk/
COPY ./Yapi.Sdk.Sample/*.csproj ./Yapi.Sdk.Sample/
COPY ./Yapi.Web/*.csproj ./Yapi.Web/
COPY ./YAPI/*.csproj ./YAPI/
COPY *.sln .
RUN dotnet restore
COPY . .
RUN dotnet publish ./YAPI/*.csproj -o /publish/
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /publish .
ENV ASPNETCORE_URLS="http://*:5000"
ENTRYPOINT [ "dotnet","YAPI.dll" ]

