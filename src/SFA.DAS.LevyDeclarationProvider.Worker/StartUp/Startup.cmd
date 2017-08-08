@ECHO OFF
REM Install named certificates from LocalMachine\My to LocalMahcine\Root 
ECHO Installing trusted certificates
PowerShell -ExecutionPolicy Unrestricted .\Startup\Copy-Certificate.ps1 -Thumbprint %TokenServiceCertificateThumbprint% 2>nul
EXIT /B %errorlevel%

