# Godot POC Project

## Purpose

The purpose of this project is to allow me to trial and prototype new ideas. This is not a full game, but a basic implementation of a 
number of concepts and ideas.

## C# Implementation

The first purpose of this project was to test using the GODOT engine with C#. When using Unity I have much preferred the workflow provided 
by working in Visual Studio and C# for code changes, but using the Unity editor for actually building out my game.

However - I have never been completely happy with Unity as a full solution, primarily because of performance concerns (building the domain
each time you want to test a change can be exhausting).

Initially aiming to build a small game where a character can walk around an island and attack enemies (which attack in return) while keeping track 
of health will allow me to test out the main features of the engine.

Graphics are provided from a few asset set available here: https://kenmi-art.itch.io/cute-fantasy-rpg

## Automatic Builds from GitHub

Another aim for me was to look at how I release builds of my games, and how I do that automatically. Ideally I would want to 
use GitHub actions to build my game on each commit, and for each "release" publish that to GitHub releases, or to stores
such as Itch.io in the future.

This was actually a little bit fiddly to figure out - particularly as in order to test each change to my GitHub actions
script it seems I need to check it in and run it, leaving behind a trail of failed runs.

In the end I managed to get there by creating an Ubuntu environment and then setting up my script to download
and extract both Godot and the Godot Export templates and then using Godot in headless mode to complete the build process.

I have also set the actions to only apply when tagging a particular checkin with v* (such as v0.1) so that I do not release
every single check in that I push. This gives me much better control over my release pipeline.
