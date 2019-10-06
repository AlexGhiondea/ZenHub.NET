#!/bin/bash
if [ "$#" -eq 0 ]
then
    _config=Debug
else
    _config=$1
fi

echo --------------------------
echo !!! Restoring packages !!!
echo --------------------------
dotnet restore

echo -------------------------
echo !!! Cleaning solution !!!
echo -------------------------
dotnet clean -c $_config

echo -------------------------
echo !!! Building solution !!!
echo -------------------------
dotnet build -c $_config

echo ---------------------
echo !!! Running tests !!!
echo ---------------------
dotnet test --no-build -c $_config tests/ZenHub.Tests.csproj

echo ------------------------------
echo !!! Creating NuGet package !!!
echo ------------------------------
dotnet pack -c $_config

echo on