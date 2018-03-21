FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

COPY src/Domain/ src/Domain/
COPY src/Infrastructure/ src/Infrastructure/
COPY src/Infrastructure.Akka/ src/Infrastructure.Akka/
COPY src/Infrastructure.Dapper/ src/Infrastructure.Dapper/
COPY src/Infrastructure.Identity/ src/Infrastructure.Identity/
COPY src/Presentation.API/ src/Presentation.API/

RUN dotnet publish /app/src/Presentation.API -o out

FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/src/Presentation.API/out .
ENTRYPOINT ["dotnet", "RU.Challenge.Presentation.API.dll"]