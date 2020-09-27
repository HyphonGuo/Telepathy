param([string] $resourceGroup, [string] $aksClusterName, [string] $redisCacheName)

# Connect to Azure Kubernetes cluster and install kubectl 
Import-AzAksCredential -Force -ResourceGroupName $resourceGroup -Name $aksClusterName
Install-AzAksKubectl -Force
chmod 777 "/root/.azure-kubectl/kubectl"
if ($env:PATH) {
    $env:PATH += ":/root/.azure-kubectl/"
} else {
    $env:PATH = "/root/.azure-kubectl/"
}

$redisAccessKeys = Get-AzRedisCacheKey -ResourceGroupName $resourceGroup -Name $redisCacheName
[System.Environment]::SetEnvironmentVariable("REDIS_CACHE_NAME", $redisCacheName)
[System.Environment]::SetEnvironmentVariable("REDIS_CACHE_ACCESS_KEY", $redisAccessKeys.PrimaryKey)


curl "https://codeload.github.com/HyphonGuo/Telepathy/tar.gz/dev-integration" --output "archive.tar.gz"
tar -xzf "archive.tar.gz"

kubectl apply -f "Telepathy-dev-integration/deploy/manifests/dispather.yaml"

$dispatcherIpAddress = [System.Net.IPAddress]::None

while (True) {
    $serviceInfo = kubectl get service
    $isValidIpAddress = $false
    foreach ($service in $serviceInfo) {
        $serviceProps = $service.Split(' ', [System.StringSplitOptions]::RemoveEmptyEntries)
        $serviceName = $serviceProps[0]
        $serviceExternalIp = $serviceProps[3]
        if ([string]::Equals($serviceName, "dispatcher")) {
            $isValidIpAddress = [System.Net.IPAddress]::TryParse($serviceExternalIp, [ref] $dispatcherIpAddress)
        }
    }

    if ($isValidIpAddress) {
        break;
    }

    Start-Sleep -Seconds 5
}

[System.Environment]::SetEnvironmentVariable("DISPATCHER_IP_ADDRESS", $dispatcherIpAddress.ToString())
kubectl apply -f "Telepathy-dev-integration/deploy/manifests/session.yaml"
kubectl apply -f "Telepathy-dev-integration/deploy/manifests/frontend.yaml"
