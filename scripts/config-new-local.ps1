$Template = "src/AstraID.Api/appsettings.Local.example.json"
$Target = "src/AstraID.Api/appsettings.Local.json"
if (Test-Path $Target) {
    Write-Error "$Target already exists"
    exit 1
}
Copy-Item $Template $Target
Write-Host "Created $Target. Replace placeholder values (e.g., CHANGE_ME) before running the app."
