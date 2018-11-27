# SevenMod

SevenMod is a general server management mod for 7 Days to Die servers intended to mimic the basic functionality of SourceMod.

This mod is in early development and is not ready for use in a live environment. **Use at your own risk.**

## Features

* Flag-based admin permission system
* Admin groups
* Admin immunity levels
* Chat triggers for admin console commands
* Modular design with plugins that can be enabled or disabled

## Plugins

SevenMod comes with several plugins that provide basic functionality.

### AdminFlatFile

Loads admin users from a local configuration file.

### AdminSQL

Loads admin users from a SQL database.

### BaseBans

Adds the ban, addban, and unban admin commands.

### BaseChat

Adds basic chat commands for admins.

### BaseCommands

Adds the kick and who admin commands.

### BaseVotes

Adds the vote, votekick, and voteban admin commands.

### PlayerCommands

Adds player targeting commands like slay.

### ReservedSlots

Adds reserved slots based on admin flags.

## License

The source code for SevenMod is available under the terms of the [MIT License](https://github.com/SevenMod/SevenMod/blob/master/LICENSE.txt).
See the LICENSE.txt in the project root for details.