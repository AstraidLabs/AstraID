# Collects quick repository stats for AstraID
$patterns = @{
    'UseOpenIddict' = 'UseOpenIddict'
    'IAuthorizedRequest' = 'IAuthorizedRequest'
    'OwnsMany' = 'OwnsMany'
    'Migrate\(' = 'Migrate\('
}

foreach ($p in $patterns.GetEnumerator()) {
    $count = (Get-ChildItem -Recurse -Include *.cs | Select-String -Pattern $p.Value | Measure-Object).Count
    Write-Output ("{0}: {1}" -f $p.Key, $count)
}
