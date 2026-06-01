# IndY Lib

This is a C# library for the IndY-API.

## How to use

```csharp
// create host
using IHost host = Host.CreateApplicationBuilder(args).Build();

// add library
var services = new ServiceCollection();
services.AddIndyAuth();
var serviceProvider = services.BuildServiceProvider();

// get auth class
var indyAuth = serviceProvider.GetRequiredService<IIndyAuth>();

// get token
var client = await indyAuth.CreateClientAsync(username, password);
```

## What is IndY?

IndY is a project in my school, where students can freely decide what, where and with who they work on. There are 6 IndY hours all in all (2x Monday, 2x Wendsday, 2x Friday)

## Why?

The IndY-team currently has a website and an android app, technically enough **but** the more the better right? My intention was to create a Discord bot using this library.
