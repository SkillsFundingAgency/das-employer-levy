<#
.SYNOPSIS
Copy self signed certificates from LocalMachine\My to LocalMahcine\Root

.DESCRIPTION
Copy self signed certificates from LocalMachine\My to LocalMahcine\Root

The script logs to windows event log with the following parameters:
- LogName = Application
- Source = Copy-Certificate
- EventId = 2788
The logs are picked up by WAD and can be viewed in the storage account table WADWindowsEventLogsTable associated with the cloud service. 

.PARAMETER SubjectName
One or more subject names to search for

.PARAMETER Destination
The destination certificate store. The default is Cert:\LocalMachine\Root

.EXAMPLE
.\Copy-Certificate.ps1 -SubjectName "cert1.test.com","cert2.test.com"

.EXAMPLE
Execution from Statup.cmd:

@ECHO OFF
REM Install named certificates from LocalMachine\My to LocalMahcine\Root
ECHO Installing trusted certificates
PowerShell -ExecutionPolicy Unrestricted .\Startup\Copy-Certificate.ps1 -Thumbprint %Certificate1Thumbprint%,%Certificate2Thumbprint% >> ".\Startup\StartupLog.txt" 2>&1
EXIT /B %errorlevel%

.NOTES
Reference: https://github.com/Azure-Samples/cloud-services-dotnet-import-set-certificate

#>
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$true, Position=0)]
    [ValidateNotNull()]
    [String[]]$Thumbprint,
    [Parameter(Mandatory=$false, Position=1)]
    [ValidateScript({Test-Path -Path $_})]
    [ValidateNotNull()]
    [String]$Destination = "Cert:\LocalMachine\Root"
)

try {
	# --- Set up logging
	New-EventLog -LogName Application -Source "Copy-Certificate" #-ErrorAction SilentlyContinue
	$LoggingParameters = @{
		LogName = "Application"
		Source = "Copy-Certificate"
		EventId = 2788
	}

    # --- Open the destination store once
	Write-EventLog @LoggingParameters -EntryType Information -Message "Opening destination store: $Destination"
    $DestinationStore = Get-Item -Path $Destination -ErrorAction Stop
    $DestinationStore.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)

    foreach ($Item in $Thumbprint) {
        # --- Retrieve the source certificate object from the personal cert store
		Write-EventLog @LoggingParameters -EntryType Information -Message  "Retrieving source certificate with thumbprint $Item"
        $X509Certificate = Get-ChildItem -Path Cert:\LocalMachine\My\ | Where-Object {$_.Thumbprint -eq "$Item"}
 
		if (!$X509Certificate) {
			Write-EventLog @LoggingParameters -EntryType Warning -Message "Could not find certificate with thumbprint $Item in store LocalMachine\My"
			continue
		}

		# --- Run a test to see if the certificate already exists in the destination cert store
		$DestinationCertificatePath = "Cert:\$($DestinationStore.Location)\$($DestinationStore.Name)\$($X509Certificate.Thumbprint)"
		if ((Get-Item -Path $DestinationCertificatePath -ErrorAction SilentlyContinue)) {
			Write-EventLog @LoggingParameters -EntryType Warning -Message "A certificate with the thumbprint $Item already exists in the destination store $Destination"
			continue
		}

        # --- Add the certificate to to the destination store
		Write-EventLog @LoggingParameters -EntryType Information -Message "Copying certificate to $DestinationCertificatePath"
        $DestinationStore.Add($X509Certificate)

        # --- Return the new certificate to the user
        Get-Item -Path $DestinationCertificatePath
		Write-EventLog @LoggingParameters -EntryType Information -Message "Complete"
    }
} catch {
	Write-EventLog @LoggingParameters -EntryType Error -Message "$_"
    $PSCmdlet.ThrowTerminatingError($_)
}