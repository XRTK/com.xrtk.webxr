# Copyright (c) XRTK. All rights reserved.
# Licensed under the MIT License. See LICENSE in the project root for license information.

$InputName = Read-Host "Enter a name for your new platform or project"
$PlatformName = "PlatformName"

Write-Host "Your new $InputName project is being updated..."

$excludes = @('*com.xrtk.core*', '*Library*', '*Obj*','*InitializeTemplate*')

# Rename any directories before we crawl the folders
Rename-Item -Path ".\XRTK.PlatformName" -NewName ".\XRTK.$InputName"
Rename-Item -Path ".\XRTK.$InputName\Packages\com.xrtk.platformname" -NewName "com.xrtk.$($InputName.ToLower())"

#TODO Rename any individual files with updated name
Get-ChildItem -Path "*"-File -Recurse -Exclude $excludes | ForEach-Object -Process {
  $isValid = $true

  foreach ($exclude in $excludes) {
    if ((Split-Path -Path $_.FullName -Parent) -ilike $exclude) {
      $isValid = $false
      break
    }
  }

  if ($isValid) {
    Get-ChildItem -Path $_ -File | ForEach-Object -Process {
      $updated = $false;

      $fileContent = Get-Content $($_.FullName) -Raw

      # Rename all PascalCase instances
      if ($fileContent -cmatch $PlatformName) {
        $fileContent -creplace $PlatformName, $InputName | Set-Content $($_.FullName) -NoNewline
        $updated = $true
      }

      $fileContent = Get-Content $($_.FullName) -Raw

      # Rename all lowercase instances
      if ($fileContent -cmatch $PlatformName.ToLower()) {
        $fileContent -creplace $PlatformName.ToLower(), $InputName.ToLower() | Set-Content $($_.FullName) -NoNewline
        $updated = $true
      }

      # Update guids
      if($fileContent -match "#INSERT_GUID_HERE#") {
        $fileContent -replace "#INSERT_GUID_HERE#", [guid]::NewGuid() | Set-Content $($_.FullName) -NoNewline
        $updated = $true
      }

      # Rename files
      if ($_.Name -match $PlatformName) {
        Rename-Item -LiteralPath $_.FullName -NewName ($_.Name -replace ($PlatformName, $InputName))
        $updated = $true
      }

      if ($updated) {
        Write-Host $_.Name
      }
    }
  }
}

Remove-Item -Path "InitializeTemplate.ps1"
