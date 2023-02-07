#!/bin/bash
#$1 Solution File
#$2 Configuration
#$3 Platform
#$4 Provisioning Profile UUID
/Library/Frameworks/Mono.framework/Versions/Current/Commands/mono "/Applications/Visual Studio.app/Contents/MonoBundle/MSBuild/Current/bin/MSBuild.dll" /t:Build "$1" /p:Configuration="$2" /p:Platform="$3" /p:buildForSimulator=false /p:BuildIpa=true /p:CodesignProvision="$4" /bl