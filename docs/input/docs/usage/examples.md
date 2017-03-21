# Build Script

You can reference Cake.Grunt in your build script as a cake addin:

```cake
#addin "Cake.Grunt"
```

or nuget reference:

```cake
#addin "nuget:https://www.nuget.org/api/v2?package=Cake.Grunt"
```

Then some examples:

```cake
Task("Default")
    .Does(() => 
    {
        // Executes gulp from a global installation (npm install -g gulp)
        Grunt.Global.Execute();

        // Executes gulp from a local installation (npm install gulp)
        Grunt.Local.Execute();
    });
```