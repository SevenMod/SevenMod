# Changelog

## v0.2.0 (4/1/19):

* Merged Advertisements, BloodMoon, and ServerShutdown plugins into the main package
* Added PlayerLog plugin for logging chat and kills
* BloodMoon Plugin
   * Now uses the BloodMoonFrequency server preference instead of the BloodMoonInterval ConVar
   * Requires server preference BloodMoonRange to be set to 0
* ServerShutdown Plugin
   * Blocks entrance to the server in the final minute of the countdown to clients without access to the cancel command
   * Kicks all clients before shutting down the server

## v0.1.0 (2/9/19):

* Initial release
