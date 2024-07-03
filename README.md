## Overview

This plugin provides a login interface that sits atop the GodotBlockchain Plugin as well
as implements social login for a game. The UI is totally customizable and arbitrary. 
Just mind the signals and you can change whatever you want.

## Features


## Installation

Ensure that you have the .NET SDK installed. Version 7.0 or later is recommended for this project.

Ensure you have the godotblockchain addon (requires .NET for the plugin, but you do not need to write scripts in C#).

Add the following to the PackageReference inside the ItemGroup in your csproj file. If there is no ItemGroup, drop this block as a child of the Project tag:

```
<ItemGroup>
  <!-- Other NuGet package references -->
  <PackageReference Include="Thirdweb" Version="1.0.0" />
  <!-- Update with the latest version when necessary -->
</ItemGroup>
```

An complete csproj file might look like
```
<Project Sdk="Godot.NET.Sdk/4.3.0">
  <PropertyGroup>
	<TargetFramework>net6.0</TargetFramework>
	<TargetFramework Condition=" '$(GodotTargetPlatform)' == 'android' ">net7.0</TargetFramework>
	<TargetFramework Condition=" '$(GodotTargetPlatform)' == 'ios' ">net8.0</TargetFramework>
	<EnableDynamicLoading>true</EnableDynamicLoading>
  </PropertyGroup>

  <ItemGroup>
	<!-- Other NuGet package references -->
	<PackageReference Include="Thirdweb" Version="1.0.0" />
	<!-- Update with the latest version when necessary -->
  </ItemGroup>
</Project>
```

If this is a new project you will also need to create a C# file so that the Godot Engine (as of Godot 4.3) will identify that you have a C# project. There is also an assembly which 
needs to be added to pull in the Thirdweb .NET SDK.

## Usage

1/ Enable the Plugin under Project Settings->Plugins called GodotBlockchain

To ensure the login functionality is properly configured, add a Blockchain

Modify the scripts in the UI folder to handle your post login functionality. This project simply takes the next scene and loads that.

## Getting Started
