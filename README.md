# GofriBrowser
GofriBrowser is a cross-platform Linked Data browser based on .NET Core platform.

# How to compile

For compilation you'll need to have [.NET Core SDK, version 2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1) installed.

To compile the project run the following  command in the solution folder:
```
dotnet publish -c Release
```
On Windows with Visual Studio you can alternatively open `LinkedDataBrowser.sln` in Visual Studio 2017+ and go to menu Build -> Publish LinkedDataBrowser.

This will produce folder `LinkedDataBrowser\bin\Release\netcoreapp2.1\publish` with binaries and static files.

# How to run

Once you have compiled the project, you can start a web server with the following command in folder `publish`:
```
dotnet LinkedDataBrowser.dll
```

For running the compiled application you'll either need .NET Core SDK or [ASP.NET Core and .NET Core runtime](https://dotnet.microsoft.com/download/dotnet-core/2.1), version 2.1.
