FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app
COPY . ./
RUN dotnet restore
RUN dotnet publish /app/src/Presentation.API -o out

FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/src/Presentation.API/out .
ENTRYPOINT ["dotnet", "RU.Challenge.Presentation.API.dll"]