CustomSSISComponents
====================

Custom SSIS components built to extend and improve on default functionality.

Custom SSIS components should be unzipped to your SQL Server installation's DTS folder, where there is a folder for all custom component types.  In my version of SSIS 2012, this is found at the following location: [important]C:Program Files (x86)Microsoft SQL Server110DTS[/important]

Currently available SSIS Component downloads:

* PicnicerrorDotNet.ForEachFileInteger: Custom ForEach File Enumerator (Conditional integer expression) as mentioned in Extending the ForEach File Enumerator in SSIS.  (SSIS 2012 Only).
* PicnicerrorDotNet.ForEachFileName: Custom ForEach File Enumerator (returns file names only).  Very basic, built for performance testing against standard ForEach File Enumerator.  (SSIS 2012 Only).
