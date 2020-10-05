#!/bin/bash

echo "Restoring .NET Core tools"
dotnet tool restore

if [ -f "./includes.cake" ]; then
    rm -f "./includes.cake"
fi

echo "Updating 'includes.cake' with Cake.Recipe content files"
find ./Cake.Recipe/Content -type f \( -iname "*.cake" -and -not -iname "version.cake" \) -exec echo "#load \"local:?path={}\"" >> includes.cake \;

echo "Bootstrapping Cake"
dotnet cake recipe.cake --bootstrap

echo "Running Build"
dotnet cake recipe.cake "$@"
