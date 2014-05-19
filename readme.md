Cross platform way to migrate PCL Resources over to Android Resources

Put all of your resources in a PCL *.resx file, and run this with your build. It will automatically 
generate your *.xml resource file for android

Future Plans: to generate CustomUIColors for iOS.

note: start your file name with the type of resource you're generating

IE:
 
    bools.resx       // will generate bool resources
    dimensions.resx  // will generate dimen resources
    integers.resx    // will generate int resources
    colors.resx      // will generate color resources
    items.resx       // will generate item resources
    foo-bar.resx     // will generate string resources

Usage:

    ResourceMigrator.exe /path/to/pcl/resource/dir /my-android-app/resources/values/

