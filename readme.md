Cross platform way to migrate PCL Resources over to Android and iOS. I built this simply to help me keep all my Resources in one spot while building Xamarin projects.

-----

Put all of your resources in `*.resx` files in your Portable Class Libraries (PCL's), and run this with your build.  

It will automatically 
generate your `*.xml` resource file for Android, and a `CustomUIColor.cs` class for iOS (more iOS 
support to come).

*note: if you start your file name with the type of resource you're generating, it will generate the appropriate resource for Android automatically*

example:
 
    bools.resx       // will generate bool resources
    dimensions.resx  // will generate dimen resources
    integers.resx    // will generate int resources
    colors.resx      // will generate color resources
    items.resx       // will generate item resources
    strings.resx     // will generate string resources
    foo-bar.resx     // will ALSO generate string resources

usage:

    ResourceMigrator.exe /path/to/solution

todo:

 - Automatically update csproj file with any newly created files if it's not already in there.
 - Automatically create the appropriate directory if it doesn't exist.
 - Add more iOS support as the need arises.
