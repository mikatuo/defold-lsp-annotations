[EmmyLua](https://github.com/EmmyLua) annotations for [Defold](https://defold.com)

# Use in IntelliJ IDEA (recommended)

1. Click `Ctrl+Alt+S` to open settings
2. Go to `Plugins` and install the [Luanalysis](https://github.com/Benjamin-Dobell/IntelliJ-Luanalysis) plugin
2. Unzip the archive with annotations into `.defold` folder
3. Enjoy!

Examples:
[player.script](https://user-images.githubusercontent.com/7230306/211431570-f5a05423-2693-450b-8406-8c1cf99d9157.png), 
[main.script](https://user-images.githubusercontent.com/7230306/211431591-7bc300dd-57ba-422d-b8a5-c5582f685707.png)

# Use in VS Code

1. Install the EmmyLua plugin
2. Unzip the archive with annotations into `.defold` folder
3. Create `.vscode/settings.json` if it does not exist already and add the following lines:
```json
{
    "Lua.diagnostics.globals": [
        "init",
        "final",
        "update",
        "fixed_update",
        "on_message",
        "on_input",
        "on_reload"
    ],
    "search.exclude": {
        "**/.defold": true
    }
}
```
4. Enjoy!

Examples:
[player.script](https://user-images.githubusercontent.com/7230306/211433659-11c93653-1b59-402b-8d6a-c28d7059fdb3.png),
[main.script](https://user-images.githubusercontent.com/7230306/211433750-b9fd32b9-3862-4dbf-b940-8c9b44bf0a12.png)

Note: You might notice that [Luanalysis](https://github.com/Benjamin-Dobell/IntelliJ-Luanalysis) actually does much a better job at type checking.
