dotnet build ./build/LocalStack.Build/LocalStack.Build.csproj >$null 2>&1
dotnet run --project ./build/LocalStack.Build/LocalStack.Build.csproj --no-launch-profile --no-build -- $args
exit $LASTEXITCODE;