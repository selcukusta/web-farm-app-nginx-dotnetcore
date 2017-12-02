FROM microsoft/dotnet:2.0-sdk AS build-env
COPY ./app /farm-example-app
WORKDIR /farm-example-app
RUN dotnet restore
RUN dotnet publish -c Release -o publish-folder -r linux-x64
# build runtime image
FROM microsoft/dotnet:2.0-runtime 
COPY --from=build-env /farm-example-app/publish-folder ./
ENTRYPOINT ["dotnet", "app.dll"]