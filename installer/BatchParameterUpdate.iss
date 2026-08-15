; ============================================================================
;  Batch Parameter Update, Inno Setup script
;
;  Produces one self contained setup executable that deploys the add-in to the
;  installed Revit versions the user selects, per machine, under ProgramData.
;
;  The version comes from build-installer.ps1:  ISCC /DMyAppVersion=1.0.0
; ============================================================================

#ifndef MyAppVersion
  #define MyAppVersion "1.0.0"
#endif

#define MyAppName "Batch Parameter Update"
#define MyAppPublisher "Mateo Lopez"
#define MyAddinFolder "BatchParameterUpdate"

[Setup]
; Fixed across versions, so an upgrade replaces the previous install instead of
; sitting beside it.
AppId={{4F2B7C91-8D63-4A15-9E0C-2A7B6F3D5E48}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
VersionInfoVersion={#MyAppVersion}

; Per machine install, so every user of the workstation gets the add-in. That
; needs elevation.
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible

; Files go to fixed ProgramData locations, so a destination folder page would be
; meaningless. Inno still requires a DefaultDirName.
DefaultDirName={commonappdata}\Autodesk\Revit\Addins
DisableDirPage=yes
DisableProgramGroupPage=yes
UninstallDisplayName={#MyAppName}

; Revit holds the assembly open while it runs, so offer to close it first.
CloseApplications=yes
CloseApplicationsFilter=Revit.exe

OutputDir=Output
OutputBaseFilename=BatchParameterUpdate-Setup-{#MyAppVersion}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"; LicenseFile: "LICENSE.txt"

[Components]
Name: "r2023"; Description: "Revit 2023"; Types: full
Name: "r2024"; Description: "Revit 2024"; Types: full
Name: "r2025"; Description: "Revit 2025"; Types: full
Name: "r2026"; Description: "Revit 2026"; Types: full

[Files]
; Revit 2023 and 2024 run on .NET Framework 4.8, 2025 and 2026 on .NET 8. Each
; component ships the build made against its own Revit API.
Source: "..\src\BatchParameterUpdate\bin\Release.R23\*"; DestDir: "{commonappdata}\Autodesk\Revit\Addins\2023\{#MyAddinFolder}"; Excludes: "*.pdb"; Flags: recursesubdirs ignoreversion; Components: r2023
Source: "..\src\BatchParameterUpdate\BatchParameterUpdate.addin"; DestDir: "{commonappdata}\Autodesk\Revit\Addins\2023"; Flags: ignoreversion; Components: r2023

Source: "..\src\BatchParameterUpdate\bin\Release.R24\*"; DestDir: "{commonappdata}\Autodesk\Revit\Addins\2024\{#MyAddinFolder}"; Excludes: "*.pdb"; Flags: recursesubdirs ignoreversion; Components: r2024
Source: "..\src\BatchParameterUpdate\BatchParameterUpdate.addin"; DestDir: "{commonappdata}\Autodesk\Revit\Addins\2024"; Flags: ignoreversion; Components: r2024

Source: "..\src\BatchParameterUpdate\bin\Release.R25\*"; DestDir: "{commonappdata}\Autodesk\Revit\Addins\2025\{#MyAddinFolder}"; Excludes: "*.pdb"; Flags: recursesubdirs ignoreversion; Components: r2025
Source: "..\src\BatchParameterUpdate\BatchParameterUpdate.addin"; DestDir: "{commonappdata}\Autodesk\Revit\Addins\2025"; Flags: ignoreversion; Components: r2025

Source: "..\src\BatchParameterUpdate\bin\Release.R26\*"; DestDir: "{commonappdata}\Autodesk\Revit\Addins\2026\{#MyAddinFolder}"; Excludes: "*.pdb"; Flags: recursesubdirs ignoreversion; Components: r2026
Source: "..\src\BatchParameterUpdate\BatchParameterUpdate.addin"; DestDir: "{commonappdata}\Autodesk\Revit\Addins\2026"; Flags: ignoreversion; Components: r2026

[UninstallDelete]
Type: filesandordirs; Name: "{commonappdata}\Autodesk\Revit\Addins\2023\{#MyAddinFolder}"
Type: filesandordirs; Name: "{commonappdata}\Autodesk\Revit\Addins\2024\{#MyAddinFolder}"
Type: filesandordirs; Name: "{commonappdata}\Autodesk\Revit\Addins\2025\{#MyAddinFolder}"
Type: filesandordirs; Name: "{commonappdata}\Autodesk\Revit\Addins\2026\{#MyAddinFolder}"

[Code]
{ ------------------------------------------------------------------------- }
{  Preselect the Revit versions that are actually installed, so the user is   }
{  not offered add-ins for software they do not have.                        }
{ ------------------------------------------------------------------------- }

function IsRevitInstalled(Version: String): Boolean;
begin
  { Revit creates its per machine add-ins folder on install, and registers
    itself under HKLM. Either is enough evidence that the version is present. }
  Result := DirExists(ExpandConstant('{commonappdata}\Autodesk\Revit\Addins\') + Version)
         or RegKeyExists(HKLM, 'SOFTWARE\Autodesk\Revit\' + Version)
         or RegKeyExists(HKLM, 'SOFTWARE\Autodesk\Revit\Autodesk Revit ' + Version);
end;

procedure SelectInstalledVersions;
var
  Versions: array[0..3] of String;
  Index: Integer;
  AnyFound: Boolean;
begin
  Versions[0] := '2023';
  Versions[1] := '2024';
  Versions[2] := '2025';
  Versions[3] := '2026';

  AnyFound := False;

  { Start from nothing checked, then tick what is detected. The order of the
    components list matches the order of the array above. }
  for Index := 0 to WizardForm.ComponentsList.Items.Count - 1 do
    WizardForm.ComponentsList.Checked[Index] := False;

  for Index := 0 to 3 do
  begin
    if IsRevitInstalled(Versions[Index]) then
    begin
      if Index < WizardForm.ComponentsList.Items.Count then
        WizardForm.ComponentsList.Checked[Index] := True;
      AnyFound := True;
    end;
  end;

  if not AnyFound then
    MsgBox('No installation of Revit 2023 to 2026 was detected on this machine.' + #13#10 +
           'You can still tick versions by hand, but the add-in will only load once ' +
           'the matching Revit is installed.', mbInformation, MB_OK);
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpSelectComponents then
    SelectInstalledVersions;
end;
