#!/bin/bash

dotnet restore

dotnet build

dotnet test --no-build tests/ZenHub.Tests.csproj