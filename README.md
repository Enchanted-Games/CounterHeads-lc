# Counter Heads

Lets you kill coil heads with knives

## Downloads

Releases of this mod are made on [thunderstore](https://thunderstore.io/c/lethal-company/p/Enchanted_Games/CounterHeads/)

## Dev Setup

- Rename `CounterHeads.template.csproj.user` to `CounterHeads.csproj.user`, then update `PluginDirectory` to your bepinex plugins folder 
- By default game libs are stored in a directory relative to the git repository root `./_lcdata/Managed`. I recommend making this a symlink to your `Lethal Company_Data` folder
    - For example if this repo is in `D:\SomeFolder\CounterHeads`, make the symlink at `D:SomeFolder\_lcdata\`
    - Alternatively update the links manually in `CounterHeads.csproj`
- Check `include/readme.txt` and download any extra dlls required