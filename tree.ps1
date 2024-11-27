function Show-Tree($path = ".", $prefix = "", $level = 1, $maxLevel = 3) {
    if ($level -gt $maxLevel) {
        return
    }

    $items = Get-ChildItem -Path $path -Force
    $folders = @()
    $files = @()

    foreach ($item in $items) {
        if ($item.PSIsContainer -and $item.Name -ne "bin" -and $item.Name -ne ".git" -and $item.Name -ne "obj") {
            $folders += $item
        } elseif (-not $item.PSIsContainer) {
            $files += $item
        }
    }

    foreach ($folder in $folders) {
        Write-Output "$prefix+--- $($folder.Name)"
        Show-Tree "$($folder.FullName)" "$prefix|   " ($level + 1) $maxLevel
    }

    foreach ($file in $files) {
        Write-Output "$prefix+--- $($file.Name)"
    }
}

# Call the function to display the tree structure, limited to 3 levels and ignoring 'bin', '.git', and 'obj' folders
Show-Tree ""
