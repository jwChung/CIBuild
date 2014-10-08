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
[assembly: AssemblyVersion("0.2.0")]
[assembly: AssemblyInformationalVersion("0.2.0")]

/*
 * Version 0.2.0
 * 
 * - [Patch] Improved descriptions about build properties.
 * 
 * - [Major] Renamed 'Run' bulid-script to 'Build', which is treated as
 *   non-major version release because this release is based on major version
 *   zero.
 * 
 * - [Minor] Defined 'CI' bulid-constant to enable to skip some slow tests on
 *   local-machine and to run the tests only on CI server.
 */