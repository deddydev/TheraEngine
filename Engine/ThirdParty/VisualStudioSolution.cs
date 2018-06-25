using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.ThirdParty
{
    class VisualStudioSolution
    {
        //Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Engine", "Engine\Engine.csproj", "{3BA74071-A8FB-473A-9388-0C8F75872F41}"
        //EndProject
        //Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Thera", "Thera\Thera.csproj", "{8953C1AF-EC68-49D0-A62D-4D3DB1D2E8CE}"
        //EndProject
        //Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Forms Editor", "Forms Editor\Forms Editor.csproj", "{87E68E70-16FA-4807-B5DB-04AB2468A8EF}"
        //EndProject
        //Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "ChromaCrossfire", "ChromaCrossfire\ChromaCrossfire.csproj", "{30E11483-260E-4AC4-AAE9-C7FBF94D7CE0}"
        //EndProject
        //Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Updater", "Updater\Updater.csproj", "{CF1E25A4-E55E-4C0E-A518-8565A4C4F86B}"
        //EndProject
        //Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "GameProjectTemplate", "GameProjectTemplate\GameProjectTemplate.csproj", "{98783021-4704-4548-902A-CFB771E211C6}"
        //EndProject
        //Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "GameProjectTemplateInstaller", "GameProjectTemplateInstaller\GameProjectTemplateInstaller.csproj", "{0A38B131-72DD-498E-A496-8B435DDA956B}"
        //EndProject

        public readonly string Begin = @"
        Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 15
VisualStudioVersion = 15.0.27130.2027
MinimumVisualStudioVersion = 10.0.40219.1 
{0}
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		DebugEditor|Any CPU     = DebugEditor|Any CPU
		DebugEditor|x64         = DebugEditor|x64
		DebugEditor|x86         = DebugEditor|x86
		DebugGame|Any CPU       = DebugGame|Any CPU
		DebugGame|x64           = DebugGame|x64
		DebugGame|x86           = DebugGame|x86
		ReleaseEditor|Any CPU   = ReleaseEditor|Any CPU
		ReleaseEditor|x64       = ReleaseEditor|x64
		ReleaseEditor|x86       = ReleaseEditor|x86
		ReleaseGame|Any CPU     = ReleaseGame|Any CPU
		ReleaseGame|x64         = ReleaseGame|x64
		ReleaseGame|x86         = ReleaseGame|x86
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.Debug|Any CPU.ActiveCfg = ReleaseGameObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.Debug|Any CPU.Build.0 = ReleaseGameObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.Debug|x64.ActiveCfg = DebugGame|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.Debug|x64.Build.0 = DebugGame|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.Debug|x86.ActiveCfg = DebugGame|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.Debug|x86.Build.0 = DebugGame|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugEditor|Any CPU.ActiveCfg = DebugEditor|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugEditor|x64.ActiveCfg = DebugEditor|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugEditor|x64.Build.0 = DebugEditor|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugEditor|x86.ActiveCfg = DebugEditor|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugEditor|x86.Build.0 = DebugEditor|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugEditorObfuscated|Any CPU.ActiveCfg = DebugEditorObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugEditorObfuscated|x64.ActiveCfg = DebugEditorObfuscated|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugEditorObfuscated|x64.Build.0 = DebugEditorObfuscated|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugEditorObfuscated|x86.ActiveCfg = DebugEditorObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugEditorObfuscated|x86.Build.0 = DebugEditorObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugGame|Any CPU.ActiveCfg = DebugGame|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugGame|x64.ActiveCfg = DebugGame|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugGame|x64.Build.0 = DebugGame|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugGame|x86.ActiveCfg = DebugGame|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugGame|x86.Build.0 = DebugGame|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugGameObfuscated|Any CPU.ActiveCfg = DebugGameObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugGameObfuscated|x64.ActiveCfg = DebugGameObfuscated|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugGameObfuscated|x64.Build.0 = DebugGameObfuscated|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugGameObfuscated|x86.ActiveCfg = DebugGameObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.DebugGameObfuscated|x86.Build.0 = DebugGameObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.Release|Any CPU.ActiveCfg = ReleaseGameObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.Release|Any CPU.Build.0 = ReleaseGameObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.Release|x64.ActiveCfg = ReleaseGame|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.Release|x64.Build.0 = ReleaseGame|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.Release|x86.ActiveCfg = ReleaseGame|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.Release|x86.Build.0 = ReleaseGame|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseEditor|Any CPU.ActiveCfg = ReleaseEditor|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseEditor|x64.ActiveCfg = ReleaseEditor|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseEditor|x64.Build.0 = ReleaseEditor|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseEditor|x86.ActiveCfg = ReleaseEditor|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseEditor|x86.Build.0 = ReleaseEditor|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseEditorObfuscated|Any CPU.ActiveCfg = ReleaseEditorObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseEditorObfuscated|x64.ActiveCfg = ReleaseEditorObfuscated|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseEditorObfuscated|x64.Build.0 = ReleaseEditorObfuscated|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseEditorObfuscated|x86.ActiveCfg = ReleaseEditorObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseEditorObfuscated|x86.Build.0 = ReleaseEditorObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseGame|Any CPU.ActiveCfg = ReleaseGame|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseGame|x64.ActiveCfg = ReleaseGame|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseGame|x64.Build.0 = ReleaseGame|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseGame|x86.ActiveCfg = ReleaseGame|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseGame|x86.Build.0 = ReleaseGame|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseGameObfuscated|Any CPU.ActiveCfg = ReleaseGameObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseGameObfuscated|x64.ActiveCfg = ReleaseGameObfuscated|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseGameObfuscated|x64.Build.0 = ReleaseGameObfuscated|x64
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseGameObfuscated|x86.ActiveCfg = ReleaseGameObfuscated|x86
		{3BA74071-A8FB-473A-9388-0C8F75872F41}.ReleaseGameObfuscated|x86.Build.0 = ReleaseGameObfuscated|x86
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {000ED08C-68F0-4934-A99E-E864CC0DAA75}
	EndGlobalSection
EndGlobal";
    }
}
