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
[assembly: AssemblyVersion("0.0.2")]
[assembly: AssemblyInformationalVersion("0.0.3-pre01")]

/*
 * Version 0.0.2
 * 
 * - [Patch] The GitRefNameToTag property should not be defined with built-in
 *   appveyor property, because appveyor can set it with non-exist referene
 *   (sha).
 */