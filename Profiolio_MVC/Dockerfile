FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out
#COPY Portfolio.db /out
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
#ENV DOTNET_URLS=http://+:7021
#ENV ASPNETCORE_URLS=http://+:7021
WORKDIR /App
EXPOSE 7021
COPY --from=build-env /App/out .
COPY --from=build-env /App/Portfolio.db .
ENTRYPOINT ["dotnet", "Profiolio_MVC.dll"]