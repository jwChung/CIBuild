#!/bin/sh

function vsvers()
{
	if [ "$VS120COMNTOOLS" ]; then
		echo " /property:VisualStudioVersion=12.0"
	else
		echo ""
	fi
}

C:/Program\ Files\ \(x86\)/MSBuild/14.0/Bin/MSBuild.exe `dirname $0`/Build.Proj `vsvers` -v:minimal -maxcpucount -nodeReuse:false $@

