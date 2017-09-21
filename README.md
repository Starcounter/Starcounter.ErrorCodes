# Starcounter.ErrorCodes

Shared project for all Starcounter versions (including BlueStar and unmanaged projects). Contains a set of errorcodes and utilites for creating adorned exceptions, as well as some other helpers around errorcodes.

Includes assemblies for C# and header and source files for c/c++.

Starcounter.ErrorCodes contains of 3 parts:
- The source, `errorcodes.xml`. All codes are defined in this file.
- The generator, `Starcounter.ErrorCodes.Generator`. Generates sourcefiles based on the xml-file.
- The managed project, `Starcounter.ErrorCodes`. Invokes the generator and compiles assemblies for multiple platforms for .net (currently 4.5, 4.6 and netstandard1.0).

## Add new errorcodes

- Clone the repo.
- Add new codes in `src/errorcodes.xml`
- Push/merge to `master` branch.
- A build should be automatically triggered on the buildserver (`StarcounterGeneric/StarcounterShared/Starcounter.Errorcodes`), and if succesful, push a new version.

## Bugfix (or other change)
- Clone the repo.
- Do the change
- Bump version in `Starcounter.ErrorCodes.csproj` 
- Push/merge to `master` branch.
- A build should be automatically triggered on the buildserver (`StarcounterGeneric/StarcounterShared/Starcounter.Errorcodes`), and if succesful, push a new version.

## Versioning
- Major and minor version are bumped manually by editing the `<VersionPrefix>` tag in `Starcounter.ErrorCodes.csproj` file. This needs to be done for all changes **except** adding new errorcodes. See point below for that.
- Patch version is the number of existing errorcodes in `errorcodes.xml`. This means that the versionnumber will automatically increase when new errorcodes are added.
- The built assemblies and the nuget package will have the same version.



## The projects

### Starcounter.ErrorCodes

The main library for errorcodes. Will, as part of building, call the generator to generate sourcecode files for both .Net (C#) and native parts. Also as part of versioning, the number of errorcodes is outputted to be used as patchnumber.

The generated code with c# will be included as a part of the compilation of this project, while the other files are only needed when a nuget package is created.

#### Build
To build locally call `dotnet build` from the same folder as where the project file is (`Starcounter.ErrorCodes.csproj`)

```
cd src\Starcounter.ErrorCodes
dotnet restore
dotnet build
```

The generated files can be found under the `obj\Generated` folder.

#### Pack
The builtin functionality in `dotnet` is used to create nuget package. 

```
cd src\Starcounter.ErrorCodes
dotnet restore
dotnet pack
```

The created package will end up in `\artifacts` in the root.

### Starcounter.ErrorCodes.Generator

A runnable program used to generate sourcecode based on the entries in `errorcodes.xml`.

To see a list of available options simply execute `dotnet run` in the same folder as the project (`src\Starcounter.ErrorCodes.Generator`)

```
cd src\Starcounter.ErrorCodes.Generator
dotnet run
```

```
Usage:  [arguments] [options]

Arguments:
  sourcefile  Path to the xml-sourcefile to read errorcodes from.

Options:
  -cs | --csharp <csharpfile>      Path to write generated C# code to.
  -c | --c <cfile>                 Path to write generated C code to.
  -header | --header <headerfile>  Path to write generated header to.
  -v | --verbose                   Verbose mode.
  -cnt | --count <file>            Path to write the number of errorcodes to.
  -? | -h | --help                 Show help information
```

