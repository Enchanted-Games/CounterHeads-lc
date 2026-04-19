# Counter Heads

Adds some more counter play to coilheads by letting you kill them with kitchen knives!

## Features

- Kitchen knives can be used to kill coilheads after 3 hits
- Shotguns can also be used but take 4 shots to kill
- Almost every aspect of the mod is configurable, see the config section for more info
- Coilheads explode when killing them, inspired by the in-game bestiary log
   - *They have been known to combust into flames when being dissected*
- The config is automatically synced from the host to clients, so you don't need to fiddle around making sure everyone has the same config


## Who needs the mod installed?

**For the best experience, everyone should have the mod installed.**

Technically only the host can have it installed, however clients without the mod will not be able to see explosions, hear warning sounds, etc. 
If the host does not have the mod installed and a client does, the mod will automatically disable itself.

## Config

<details>
<summary><b>Behaviour</b></summary>
<ul>
<li>Coilheads explode (default <code>true</code>) - Whether or not coilheads explode on death. If enabled, coilheads will start playing a warning sound which will get higher pitch until eventually exploding. If disabled, coilheads will vanish on death and no warning sound will play</li>
<li>Stun coilheads on death (default <code>false</code>) - Whether or not coilheads should be stunned when dealing enough damage to them. If disabled, coilheads will still be able to chase you until they explode! Only takes effect when 'Coilheads explode' is enabled</li>
<li>Explosion damage (default <code>60</code>) - Maximum amount of damage coilhead explosions can inflict. Only takes effect when 'Coilheads explode' is enabled</li>
<li>Explosion range (default <code>5</code>) - Furthest distance coilhead explosions can do damage from. Only takes effect when 'Coilheads explode' is enabled</li>
<li>Min time until explosion (default <code>0.6</code>) - Minimum amount of time in seconds between dealing enough damage to a coilhead and it exploding Only takes effect when 'Coilheads explode' is enabled</li>
<li>Max time until explosion (default <code>1</code>) - Maximum amount of time in seconds between dealing enough damage to a coilhead and it exploding Only takes effect when 'Coilheads explode' is enabled</li>
</ul>
</details>

<details>
<summary><b>Damage</b></summary>
<ul>
<li>Coilhead health (default <code>8</code>) - How much health coilheads spawn with. 3 is the vanilla default, so setting this value to 3 will not modify coilhead health. This is useful if, for example, you want to use the coilhead health from another mod instead of overriding it</li>
<li>Coilhead weapons (default <code>{'kitchen knife':3},{'shotgun':2}</code>) - Which weapons can be used against coilheads and how much damage they should deal.
    <ul>
        <li>Entries are surrounded by curly brackets <code>{}</code> and seperated by a comma. Each entry can contain either a weapon name like so: <code>{'shotgun'}</code> (in this case the default damage amount for that item will be used), or a weapon name and damage amount seperated by a colon, like so: <code>{'shotgun':2}</code>. Weapon names should be as they appear in the top right of the screen while holding them, if the weapon name contains a <code>:</code> or <code>'</code> character, you can escape them like so: <code>\:</code> or <code>\'</code> An example config to make shovels deal 2 damage and knives deal 1 could look like this: <code>{'shovel':2},{'kitchen knife':1}</code></li>
    </ul>
</li>
</ul>
</details>
