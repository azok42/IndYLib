<img align="left" width="130" height="130" src="./assets/MindyLogo.png">

```
 ___           ___   __  _     _ _     
|_ _|_ __   __| \ \ / / | |   (_) |__  
 | || '_ \ / _` |\ V /  | |   | | '_ \ 
 | || | | | (_| | | |   | |___| | |_) |
|___|_| |_|\__,_| |_|   |_____|_|_.__/ 
```

---

This is a C# library for the IndY-API.

## How to use

```csharp
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
