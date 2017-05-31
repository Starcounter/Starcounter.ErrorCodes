# Starcounter.ErrorCodes

Shared project for all Starcounter versions (including BlueStar and unmanaged projects). Contains a set of errorcodes and utilites for creating adorned exceptions, as well as some other helpers around errorcodes.

Includes assemblies for C#, header files for c/c++, and native dlls for formatting messages.

Starcounter.ErrorCodes contains of 3 parts:
- The source, `errorcodes.xml`. All codes are defined in this file.
- The generator, `Starcounter.ErrorCodes.Generator`. Generates sourcefiles based on the xml-file.
- The managed project, `Starcounter.ErrorCodes`. Invokes the generator and compiles assemblies for multiple platforms for .net (currenty 4.5, 4.6 and netstandard1.6). 

# Adding new errorcodes

- Clone the repo.
- Add new codes in `src/errorcodes.xml`
- Push/merge to `master` branch.
- A build should be automatically triggered on the buildserver (`StarcounterGeneric/StarcounterShared/Starcounter.Errorcodes`), and if succesful, push a new version.
- Version of the package is automatically calculated using commit count to get minor version.
- Updatolder 
:```
cd package for errorcodes in the wanted places.

# Build and pack (manually)

To get the calculated version of package and assemblies, use the script in the `build` folder:

```
cd build
build_and_pack.bat
```

Created packages will end up in `artifacts` folder.


To build and pack manually:

```
cd src\Starcounter.ErrorCodes
dotnet restore
dotnet build
dotnet pack --no-build
```

# Push (manually)

TODO
