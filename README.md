# Tymer

[![NuGet](https://img.shields.io/nuget/v/m2dev.Tymer.svg?label=NuGet)](https://www.nuget.org/packages/m2dev.Tymer/)

Tymer is a CLI-based time logger to track time entries for your work.

## Installation

Tymer is built as a .NET Core Global Tool. You can install with the following
command:

```bash
dotnet tool install -g m2dev.tymer
```

Or temporarily from source by:

```bash
dotnet tool install --global --add-source ./nupkg tymer
```

## Guide

**Log time entry**

```bash
tymer log <start_time<end_time-C "<your_comments>"
```

**List time entries**

```bash
tymer list [--help]
```

**Migrate data from JSON file to SQLite DB**

```bash
tymer migrate-to-db
```

**Help**

`tymer --help` will show a list of available commands.

## TODO
- List entries per day (given date/range or week)
- Summary per day (given date/range or week)
- Support for "projects" / groups
- Ability to edit/remove entries
- `appsettings.json` support for settings
