# Start both GLMS API and UI
Write-Host "Starting GLMS API..." -ForegroundColor Cyan
$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", "..\GlobalLogisticsManagementSystemAPI\GlobalLogisticsManagementSystemAPI\GlobalLogisticsManagementSystemAPI.csproj" -PassThru -NoNewWindow

Write-Host "Waiting for API to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

Write-Host "Starting GLMS UI..." -ForegroundColor Cyan
$uiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", "GlobalLogisticsManagementUI\GlobalLogisticsManagementUI.csproj" -PassThru -NoNewWindow

Write-Host ""
Write-Host "Both services are starting:" -ForegroundColor Green
Write-Host "  API: http://localhost:5110" -ForegroundColor White
Write-Host "  UI:  http://localhost:5053" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop both services." -ForegroundColor Yellow

try {
    Wait-Process -Id $apiProcess.Id, $uiProcess.Id
} finally {
    Write-Host "Shutting down..." -ForegroundColor Red
    Stop-Process -Id $apiProcess.Id -Force -ErrorAction SilentlyContinue
    Stop-Process -Id $uiProcess.Id -Force -ErrorAction SilentlyContinue
}
