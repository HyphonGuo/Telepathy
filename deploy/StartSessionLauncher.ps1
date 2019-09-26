param (
    [string]$DestinationPath,
    [string]$DesStorageConnectionString,
    [string]$BatchAccountName,
    [string]$BatchPoolName,
    [string]$BatchAccountKey,
    [string]$BatchAccountServiceUrl
)

function Write-Log 
{ 
    [CmdletBinding()] 
    Param 
    ( 
        [Parameter(Mandatory=$true, 
                   ValueFromPipelineByPropertyName=$true)] 
        [ValidateNotNullOrEmpty()] 
        [Alias("LogContent")] 
        [string]$Message, 
 
        [Parameter(Mandatory=$false)] 
        [Alias('LogPath')] 
        [string]$Path='C:\Logs\telepathy.log', 
         
        [Parameter(Mandatory=$false)] 
        [ValidateSet("Error","Warn","Info")] 
        [string]$Level="Info", 
         
        [Parameter(Mandatory=$false)] 
        [switch]$NoClobber 
    ) 
 
    Begin 
    { 
        # Set VerbosePreference to Continue so that verbose messages are displayed. 
        $VerbosePreference = 'Continue' 
    } 
    Process 
    { 
         
        # If the file already exists and NoClobber was specified, do not write to the log. 
        if ((Test-Path $Path) -AND $NoClobber) { 
            Write-Error "Log file $Path already exists, and you specified NoClobber. Either delete the file or specify a different name." 
            Return 
            } 
 
        # If attempting to write to a log file in a folder/path that doesn't exist create the file including the path. 
        elseif (!(Test-Path $Path)) { 
            Write-Verbose "Creating $Path." 
            $NewLogFile = New-Item $Path -Force -ItemType File 
            } 
 
        else { 
            # Nothing to see here yet. 
            } 
 
        # Format Date for our Log File 
        $FormattedDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss" 
 
        # Write message to error, warning, or verbose pipeline and specify $LevelText 
        switch ($Level) { 
            'Error' { 
                Write-Error $Message 
                $LevelText = 'ERROR:' 
                } 
            'Warn' { 
                Write-Warning $Message 
                $LevelText = 'WARNING:' 
                } 
            'Info' { 
                Write-Verbose $Message 
                $LevelText = 'INFO:' 
                } 
            } 
         
        # Write log entry to $Path 
        "$FormattedDate $LevelText $Message" | Out-File -FilePath $Path -Append 
    } 
    End 
    { 
    } 
}

Write-Log -Message "DestinationPath to find resource : $DestinationPath"
Write-Log -Message "DesStorageConnectionString : $DesStorageConnectionString"
Write-Log -Message "BatchAccountName : $BatchAccountName"
Write-Log -Message "BatchAccountKey : $BatchAccountKey"
Write-Log -Message "BatchAccountServiceUrl : $BatchAccountServiceUrl"

Write-Log -Message "Start open NetTCPPortSharing & enable StrongName"
cmd /c "sc.exe config NetTcpPortSharing start=demand & reg ADD "HKLM\Software\Microsoft\StrongName\Verification\*,*" /f & reg ADD "HKLM\Software\Wow6432Node\Microsoft\StrongName\Verification\*,*^" /f"

Write-Log -Message "Open tcp port"
New-NetFirewallRule -DisplayName "Open TCP port for telepathy" -Direction Inbound -LocalPort 9087, 9090, 9091, 9092, 9093 -Protocol TCP -Action Allow

$sessionLauncher = "$DestinationPath\SessionLauncher\HpcSession.exe"
$broker = "$DestinationPath\BrokerOutput\HpcBroker.exe"
$serviceName = "TelepathySessionLauncher"

Try {
    Write-Log -Message "Start to new session launcher windows service"
    New-Service -Name $serviceName `
    -BinaryPathName "$sessionLauncher --AzureBatchServiceUrl $BatchAccountServiceUrl --AzureBatchAccountName $BatchAccountName --AzureBatchAccountKey $BatchAccountkey --AzureBatchPoolName $BatchPoolName --AzureBatchBrokerStorageConnectionString $DesStorageConnectionString --BrokerLauncherExePath $broker" `
    -DisplayName "Telepathy Session Launcher Service" `
    -StartupType Automatic `
    -Description "Telepathy Session Launcher service." 
} Catch {
    Write-Log -Message "Error happens when new session launcher windows service" -Level Error
    Write-Log -Message $_ -Level Error
}

Try {
    Write-Log -Message "Start session launcher windows service"
    Start-Service -Name $serviceName
} Catch {
    Write-Log -Message "Fail to start session launcher windows service" -Level Error
    Write-Log -Message $_ -Level Error
}