# DeepBlueHash

Detective safelisting using Sysmon event logs.

Parses the Sysmon event logs, grabbing the SHA256 hashes from process creation (event 1), driver load (event 6, sys), and image load (event 7, DLL) events. 

## VirusTotal and Safelisting setup

Setting up VirusTotal hash submissions and safelisting:

The hash checker requires Post-VirusTotal:

 - https://github.com/darkoperator/Posh-VirusTotal

It also requires a VirusTotal API key: 

 - https://www.virustotal.com/en/documentation/public-api/

Then configure your VirusTotal API key:
```powershell
set-VTAPIKey -APIKey <API Key>
```
The script assumes a personal API key, and waits 15 seconds between submissions.

## Sysmon setup

Sysmon is required: https://docs.microsoft.com/en-us/sysinternals/downloads/sysmon

Must log the SHA256 hash, DeepBlueHash will ignore the others.

This minimal Sysmon 6.0 config will log the proper events/hashes. Note that image (DLL) logging may create performance issues. This config ignores DLLs signed by Microsoft (which should lighten the load), but please test!

```xml
<Sysmon schemaversion="3.3">
  <!-- Capture SHA256 hashes only -->
  <HashAlgorithms>SHA256</HashAlgorithms>
  <EventFiltering>
    <!-- Log all drivers (.sys) except if the signature contains Microsoft or Windows -->
    <DriverLoad onmatch="exclude">
      <Signature condition="contains">microsoft</Signature>
      <Signature condition="contains">windows</Signature>
    </DriverLoad>
    <!-- Log all images (.dll) except if the signature contains Microsoft or Windows -->
    <!-- Note: this may create a performance issue, please test -->
    <ImageLoad onmatch="exclude">
      <Signature condition="contains">microsoft</Signature>
      <Signature condition="contains">windows</Signature>
    </ImageLoad>
    <!-- Do not log process termination -->
    <ProcessTerminate onmatch="include" />
    <!-- Log process creation  -->
    <ProcessCreate onmatch="exclude" />
  </EventFiltering>
</Sysmon>
```
These are the events used by DeepBlueCLI and DeepBlueHash.

You can go *much* further than this with Sysmon. The Sysinternals Sysmon page has a good basic configuration: https://docs.microsoft.com/en-us/sysinternals/downloads/sysmon

Also see @swiftonsecurity's awesome Sysmon config here: https://github.com/SwiftOnSecurity/sysmon-config

## Generating a Safelist

Generate a custom safelist on Windows (note: this is optional):

```
PS C:\> Get-ChildItem c:\windows\system32 -Include '*.exe','*.dll','*.sys','*.com' -Recurse | Get-FileHash| Export-Csv -Path safelist.csv
```
Note: this will generate (harmless) 'PermissionDenied' warnings for locked files, etc. They may be ignored.
