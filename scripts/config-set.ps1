param(
    [string]$Command = "help"
)

switch ($Command) {
    "copy" {
        Copy-Item -Path "src/AstraID.Api/appsettings.Local.example.json" -Destination "src/AstraID.Api/appsettings.Local.json" -ErrorAction SilentlyContinue
        Write-Host "Local config created at src/AstraID.Api/appsettings.Local.json"
    }
    "validate" {
        Invoke-RestMethod -Uri "http://localhost:5000/_diag/config/validate" -UseBasicParsing
    }
    default {
        Write-Host "Usage: ./config-set.ps1 -Command <copy|validate>"
        Write-Host "Example:`n$env:AstraId__Issuer='https://id.example.com'"
    }
}
