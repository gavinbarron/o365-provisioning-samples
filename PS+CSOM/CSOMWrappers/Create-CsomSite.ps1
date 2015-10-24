
function Import-Libraries ([System.String]$CsomWrapperPath){
	Add-Type -Path "c:\Program Files\Common Files\microsoft shared\Web Server Extensions\16\ISAPI\Microsoft.SharePoint.Client.dll" 
	Add-Type -Path "c:\Program Files\Common Files\microsoft shared\Web Server Extensions\16\ISAPI\Microsoft.SharePoint.Client.Runtime.dll" 
	Add-Type -Path "c:\Program Files\Common Files\microsoft shared\Web Server Extensions\16\ISAPI\Microsoft.SharePoint.Client.Search.dll" 
	Add-Type -Path "C:\Program Files\SharePoint Client Components\16.0\Assemblies\Microsoft.Online.SharePoint.Client.Tenant.dll" 
	# note that you might need some other references (depending on what your script does) for example:
	#Add-Type -Path "c:\Program Files\Common Files\microsoft shared\Web Server Extensions\16\ISAPI\Microsoft.SharePoint.Client.Taxonomy.dll" 
	if($CsomWrapperPath){
		Write-Host "Adding $CsomWrapperPath"
		Add-Type -Path "$CsomWrapperPath"
	}
}


function Create-SiteProvisioner([System.String] $adminSite, [System.String] $owner, [System.String] $password){
	$adminUri = New-Object System.Uri($adminSite)
	$provisioner =  [CSOMWrappers.ManagerFactory]::CreateSiteProvisioner($adminUri, $owner, $password)
	return $provisioner 
}

function Create-SpoSite([CSOMWrappers.SiteProvisioner] $provisioner, [System.String] $siteUrl, [System.String] $title, [System.String] $owner, [System.String] $template){
	$uri = New-Object System.Uri($siteUrl)
	$provisioner.ProvisionSite($uri, $title, $owner, $template)
}


$scriptPath = Resolve-Path .
if(!$configFile){
	$configFile = "config.xml"
}
$configFilePath = Join-Path $scriptPath $configFile
$configuration =[xml] (gc $configFilePath)
$root = "setup-config"
$siteUrl = $configuration.$root.siteUrl
$siteName = $configuration.$root.siteName
$siteTemplate = $configuration.$root.siteTemplate

$tennantUrl = $configuration.$root.tennantUrl
$tennantAdmin = $configuration.$root.tennantAdmin
$tennantPassword = $configuration.$root.tennantPassword
$csomToolsProjectName = "CSOMWrappers"

$csomToolsFilePath = ".\bin\Debug\$csomToolsProjectName.dll"


Write-Host "Importing CSOM Libraries" -F Green
Import-Libraries $csomToolsFilePath

$provisioner = Create-SiteProvisioner $tennantUrl  $tennantAdmin  $tennantPassword

Create-SpoSite $provisioner $siteUrl $siteName $tennantAdmin $siteTemplate