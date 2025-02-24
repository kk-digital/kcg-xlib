# Create timestamp for log file
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$logFile = "rename_uuid_to_uid_extended_$timestamp.log"

function Write-Log {
    param($Message)
    $logMessage = "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss'): $Message"
    Add-Content -Path $logFile -Value $logMessage
    Write-Host $logMessage
}

Write-Log "Starting enhanced Uuid to Uid rename operation"

# Define paths to exclude
$excludePaths = @(
    '*/bin/*',
    '*/obj/*',
    '*/packages/*',
    '*/.git/*',
    '*/.vs/*',
    '*/Library/*'
)

# Define replacements - ordered from most specific to most general
$replacements = @(
    # File content replacements
    @{
        'Find' = 'using lib.Uuid;'
        'Replace' = 'using lib.Uid;'
    },
    @{
        'Find' = 'lib-Uuid'
        'Replace' = 'lib-Uid'
    },
    @{
        'Find' = 'lib_Uuid'
        'Replace' = 'lib_Uid'
    },
    @{
        'Find' = 'UtilityUuid'
        'Replace' = 'UtilityUid'
    },
    @{
        'Find' = 'namespace lib.Uuid'
        'Replace' = 'namespace lib.Uid'
    },
    @{
        'Find' = '\bUuid64\b'
        'Replace' = 'Uid64'
    },
    @{
        'Find' = '\bUuidJsonConverter\b'
        'Replace' = 'UidJsonConverter'
    },
    @{
        'Find' = '\bUuid\b'
        'Replace' = 'Uid'
    },
    # Project reference replacements
    @{
        'Find' = 'lib-uuid.csproj'
        'Replace' = 'lib-uid.csproj'
    },
    @{
        'Find' = 'lib-uuid\\lib-uuid.csproj'
        'Replace' = 'lib-uid\\lib-uid.csproj'
    },
    @{
        'Find' = 'libUuid'
        'Replace' = 'libUid'
    },
    # Additional namespace and using statements
    @{
        'Find' = 'using static lib.Uuid.'
        'Replace' = 'using static lib.Uid.'
    },
    @{
        'Find' = 'lib.Uuid.'
        'Replace' = 'lib.Uid.'
    }
)

# First rename all .csproj files
Write-Log "Searching for .csproj files containing 'uuid'..."
Get-ChildItem -Path . -Recurse -Filter "*uuid*.csproj" | 
    Where-Object { 
        $path = $_.FullName
        -not ($excludePaths | Where-Object { $path -like $_ })
    } | ForEach-Object {
        $newName = $_.Name -replace 'uuid', 'uid'
        $newPath = Join-Path $_.Directory.FullName $newName
        if (Test-Path $newPath) {
            Write-Log "Warning: Target .csproj already exists: $newPath"
        } else {
            Rename-Item -Path $_.FullName -NewName $newName
            Write-Log "Renamed .csproj file: $($_.FullName) to $newName"
        }
    }

# Then get all files for content replacement
$files = Get-ChildItem -Path . -Recurse -File | 
    Where-Object { 
        $path = $_.FullName
        -not ($excludePaths | Where-Object { $path -like $_ })
    }

$extensions = @(".cs", ".csproj", ".sln", ".json", ".config", ".xaml")
$changedFiles = 0
$totalReplacements = 0

foreach ($file in $files) {
    if ($extensions -contains $file.Extension) {
        $content = Get-Content $file.FullName -Raw
        $originalContent = $content
        $fileReplacements = 0
        
        foreach ($replacement in $replacements) {
            $newContent = $content -replace $replacement.Find, $replacement.Replace
            if ($content -ne $newContent) {
                $matches = ([regex]::Matches($content, $replacement.Find)).Count
                $fileReplacements += $matches
                $content = $newContent
            }
        }
        
        if ($originalContent -ne $content) {
            Set-Content -Path $file.FullName -Value $content -NoNewline
            Write-Log "Modified file: $($file.FullName) ($fileReplacements replacements)"
            $changedFiles++
            $totalReplacements += $fileReplacements
        }
    }
}

# Rename directories
$dirsToRename = @()
Get-ChildItem -Path . -Recurse -Directory | 
    Where-Object { 
        $path = $_.FullName
        $containsUuid = $_.Name -like "*Uuid*"
        $notExcluded = -not ($excludePaths | Where-Object { $path -like $_ })
        $containsUuid -and $notExcluded
    } | ForEach-Object {
        $dirsToRename += $_.FullName
    }

# Sort directories by depth (deepest first) to handle nested directories correctly
$dirsToRename = $dirsToRename | Sort-Object { ($_ -split '\\').Count } -Descending

foreach ($dir in $dirsToRename) {
    $dirItem = Get-Item $dir
    $newName = $dirItem.Name -replace "Uuid", "Uid"
    $newPath = Join-Path (Split-Path $dir) $newName
    
    if (Test-Path $newPath) {
        Write-Log "Warning: Target directory already exists: $newPath"
        Write-Log "Attempting to merge contents..."
        Copy-Item -Path "$dir\*" -Destination $newPath -Recurse -Force
        Remove-Item -Path $dir -Recurse -Force
        Write-Log "Merged and removed original directory: $dir"
    } else {
        Rename-Item -Path $dir -NewName $newName
        Write-Log "Renamed directory: $dir to $newName"
    }
}

# Finally, rename any remaining files with Uuid in their names
Get-ChildItem -Path . -Recurse -File | 
    Where-Object { 
        $path = $_.FullName
        $containsUuid = $_.Name -like "*Uuid*"
        $notExcluded = -not ($excludePaths | Where-Object { $path -like $_ })
        $containsUuid -and $notExcluded
    } | ForEach-Object {
        $newName = $_.Name -replace "Uuid", "Uid"
        $newPath = Join-Path $_.Directory.FullName $newName
        if (Test-Path $newPath) {
            Write-Log "Warning: Target file already exists: $newPath"
        } else {
            Rename-Item -Path $_.FullName -NewName $newName
            Write-Log "Renamed file: $($_.FullName) to $newName"
        }
    }

Write-Log "Operation completed"
Write-Log "----------------------------------------"
Write-Log "Summary:"
Write-Log "- Total files processed: $($files.Count)"
Write-Log "- Files modified: $changedFiles"
Write-Log "- Total replacements: $totalReplacements"
Write-Log "- Log file location: $($logFile)"

Write-Log "`nPlease rebuild your solution now with 'dotnet build'"