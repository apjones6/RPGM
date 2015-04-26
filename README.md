# Roleplay Game Manager #

This is the beginnings of a combined Windows Phone, and potentially Windows Desktop application for managing various aspects of roleplaying games, from both the DM and player perspectives. The utilities are intended to be game agnostic, so shouldn't be specific to one or few games, but features may reach a point where templates could be created for games.

Intended initial features:

 - Note pages
 - Automatic links between notes
 - Customization of note link aliases
 - Note synchronization across devices (likely through OneDrive)
 - Dice roller
 - Automatic clickable dice expressions
 - Tabletop maps (draw tools)
 - Actor tracking on maps (position, properties, initiative)
 - Link actors with notes (simple character sheets)
 - Note/actor properties

## NuGet Packages ##

Currently this project uses NuGet packages which are not included in the GIT repository. Building the solution in Visual Studio should automatically install any missing packages, using the package restore feature.

## SQLite ##

This project now uses SQLite to store created data. To do this, you must install the appropriate Visual Studio extension SDK for their development environment. Please follow the [guidance information](https://sqlitepcl.codeplex.com/documentation) for windows phone 8.1 to do this (and don't forget to set your solution to build in x86).

 - SQLite for Windows Phone 8.1: 3.8.9

## Git Flow ##

This project uses the development model set out in http://nvie.com/posts/a-successful-git-branching-model. The conventions set out in this article, such as develop and master branches, and naming release-*, and so on are followed as written.

## Version History ##

There haven't been any releases yet!