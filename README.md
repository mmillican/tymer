# Tymer

Tymer is a CLI-based time logger to track time entries for your work.

## Installation

Tymer is built as a .NET Core Global Tool. You can install with the following
command:

```bash
> dotnet tool install -g dotnetsay
```

Or temporarily from source by:

```bash
> dotnet tool install --global --add-source ./nupkg tymer
```

## Guide

**Log time enry**

```bash
> tymer log <start_time> <end_time> -C "<your_comments>"
```

**Help**

`> tymer --help` will show a list of available commands.


## TODO
- List entries per day (given date/range or week)
- Summary per day (given date/range or week)
- Support for "projects" / groups
- Ability to edit/remove entries
- `appsettings.json` support for settings
- Split log files between periods (day, week, month, etc)
