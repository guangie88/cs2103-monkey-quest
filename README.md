# CS2103 Monkey Quest

My project team game project for NUS CS2103 for reminiscence!

## How to Build

The original project requires .NET 6.0 target framework, and was tested to build
with VS2022. For reference, the original solution was VS2010 using .NET 4.0
target framework, and was upgrade via Assistant Visual Studio extension in
<https://devblogs.microsoft.com/dotnet/upgrade-assistant-now-in-visual-studio/>.

You will need the .NET 6.0 Developer Pack installed. For distribution, the
binary will require the .NET 6.0 Runtime installed, though it is also possible
to distribute with the runtime included.

Both the aforementioned Developer Pack and Runtime can be downloaded here:
<https://dotnet.microsoft.com/en-us/download/visual-studio-sdks>

### Via VS Studio

Change to `Release` mode for build to get a more optimized binary!

Change the startup project to `MonkeyQuestMain` and run the project to run the
game! Or you could just run the executable generated after the build.

### Via Developer Command Prompt MSBuild

Go the the repository root and run:

```bash
MSBuild MonkeyQuest.sln /t:Restore
MSBuild MonkeyQuest.sln /property:Configuration=Release
```

This will generate binaries in `MonkeyQuestMain\bin\x64\Release\net6.0-windows\`.

Simply run `MonkeyQuestMain.exe` to start playing.

## Game Objective

As the game monkey `Money`, simply navigate the puzzle stages and collect all
the bananas and proceed to the next level by entering the exit door once all the
bananas in the stage have been collected.

You can push crates to the left or right, but only crates not adjacent to walls
or other crates can be pushed (i.e. two crates when put together cannot be
pushed any more).

There are monsters in many of the stages, you can get rid of them by stepping
on their heads, just like that one popular game in the world.

In the current version, there are a total of 10 stages, and as you go along each
stage, the stage increases in difficulty.

## Game Movement Keys

- `[LEFT]` and `[RIGHT]` buttons to move `Money`
- `[SPACE]` to jump (alternatively `[LEFT SHIFT]` works too)
- To jump wall, you need to cling onto a wall by holding `[LEFT]` on a left wall
  (`[RIGHT]` on a right wall), then jump `[SPACE]` to wall-jump. There are two
  kinds of wall jump
  1. Without holding `[UP]` together with `[LEFT]` / `[RIGHT]`, you get the
     horizontal jump. This allows `Money` to jump wider at about 2 blocks away
     from the wall, but at a shorter height at about slightly less than 1 block
     up.
  2. While holding `[UP]` together with `[LEFT]` / `[RIGHT]`, you get the
     vertical jump. This allows `Money` to jump higher at about 1.5 blocks up,
     but only up to about 1 block away horizontally.
- `[R]` to redo map if you are stuck (or about to get eaten by the monsters?!)

Also, there are some CHEAT CODE buttons to press that can be useful in your
banana collecting adventure!

### CHEAT CODE buttons

- `[V]`/`[B]` to shoot rocket to the left/right (max rockets per restart: 3)
- `[=]` (same button as `[+]`) to increase lives (max lives: 10)
