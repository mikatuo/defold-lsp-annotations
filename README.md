![example workflow](https://github.com/mikatuo/defold-lua-annotations/actions/workflows/dotnet.yml/badge.svg?event=push)

## Lua annotations for [Defold](https://defold.com)

# Use in VS Code

1. Install `Defold Buddy` extension or `Defold Extension Pack`
2. Open a Defold project
3. You will be prompted to add the annotations into your project

[Read more...](https://forum.defold.com/t/vscode-extensions-for-defold-aka-defold-extension-pack/72508?u=mikatuo)

<details><summary>Manual installation</summary><p>

1. Install [Lua](https://marketplace.visualstudio.com/items?itemName=sumneko.lua)
2. Grab annotations from the [Releases](https://github.com/mikatuo/Defold-Emmylua-Annotations/releases)
3. Unzip the archive with annotations inside your Defold project into the `.defold` folder
4. Modify your settings as needed. Have a look at [settings.json](https://github.com/mikatuo/Defold-Lua-Annotations/blob/main/Examples/settings.json) that worked best for me
5. Enjoy!
</p></details>

## Demo

![Code_eFgeiVhGoN](https://user-images.githubusercontent.com/7230306/213931566-78acccca-6335-4407-8e1a-3ab000899525.gif)

![Code_QTdS86DU9h](https://user-images.githubusercontent.com/7230306/213931602-ad59d8f3-e409-4691-b070-5a4f8e751566.png)

![Code_8mIXL8p95G](https://user-images.githubusercontent.com/7230306/213931616-2262a7f5-00bd-4757-b2dd-b1458b29322b.png)

# Use in IntelliJ IDEA

1. In the IDEA click `Ctrl+Alt+S` to open settings
2. Go to `Plugins` and install the [Luanalysis](https://github.com/Benjamin-Dobell/IntelliJ-Luanalysis) plugin
3. Grab annotations from the [Releases](https://github.com/mikatuo/Defold-Emmylua-Annotations/releases)
4. Unzip the archive with annotations inside your Defold project into the `.defold` folder
5. In the `/.defold/*.lua` files replace all `@class` keywords with `@shape` keywords.
6. Enjoy!
