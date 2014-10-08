using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany("Jin-Wook Chung")]
[assembly: AssemblyCopyright("Copyright (c) 2014, Jin-Wook Chung")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyVersion("0.3.0")]
[assembly: AssemblyInformationalVersion("0.3.0")]

/*
 * Version 0.3.0
 * 
 * - [Major] Removed code-analysis rule sets as rule sets are specified by
 *   projects. This change is breaking-change but is treated as non-major
 *   version release because this release is based on major version zero.
 * 
 * - [Major] Made build properties as project and the old build project as
 *   targets. to enable a target solution to have only the build project file.
 *   This change is breaking-change but is treated as non-major version release
 *   because this release is based on major version zero.
 */