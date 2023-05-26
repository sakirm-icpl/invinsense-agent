<div align="center">
  <h1>Invinsense Single Agent4.0</h1>
  <br>
  <img alt="Invinsense Single Agent3.0" src="https://user-images.githubusercontent.com/103485015/184890149-3bfa14b3-2443-4a0b-960f-582ddc39f07a.png" width="300px">
</div>

## 📙 Documentation

- Single agent checks different tools (Wazuh, Deceptive Bytes, Microsoft Defender, OsQuery, Microsoft Sysmon and Lateral Moment Protection) status such like whether it is         Active, Connecting and Disconnected which will notify current status of agents with notification.
- If tools services are active it will notify that services are running else  it will display service has been stopped and turn status dot as green.
- When you restart the service, will give notification saying that services has been started and the red dot will turn green.
- Agent shows status by monitoring tools' services.

<p align="center">
    <img src="https://user-images.githubusercontent.com/121154130/220250038-0bcb276a-8f5c-436b-931b-36668dc6df2f.png" alt="CodeQL" style="max-width: 100%;">
  </a>
</p>
   
  - **What is wazuh? 🤔**
  [Check out Website](https://wazuh.com/)
  [Wazuh Installation Parameters](https://documentation.wazuh.com/current/user-manual/deployment-variables/deployment-variables-windows.html)

   - **What is DeceptiveBytes? 🤔**
  [Check out Website](https://deceptivebytes.com/)
  
   - **What is Microsoft sysmon? 🤔**
  [Check out Website](https://docs.microsoft.com/en-us/sysinternals/downloads/sysmon/)
  
   - **What is Microsoft osquery? 🤔**
  [Check out Website](https://osquery.readthedocs.io/en/latest/)
  

  ## 📖 Prerequisites
  ### System Requirements (Invinsense 4.0)
| Component | README |
| ------ | ------ |
| .NET Framework 4.8 | Must be pre-installed in computer in order to run the Single agent |
| Operating system | Windows 10 and later [64 bit] |
| CPU | Core 4 CPU or more |
| Memory | 8 GB RAM [16GB Recommended] |

## 🖥️ Run agent using Commandline/Userinterface

- Download `.msi` file 
- [text]https://github.com/Infopercept/invinsense-agent/blob/main/InvinSetup_23_02_21.msi
- copy file path and open on cmd(Administration)
- run following command
- <b>With Deceptivebytes:</b>
msiexec.exe /i InvinSetup_23_02_21.msi /l*v C:\ProgramData\install.log WAZUH_MANAGER="34.100.141.147" WAZUH_REGISTRATION_SERVER="34.100.141.147" 
WAZUH_AGENT_GROUP="default" WAZUH_REGISTRATION_PASSWORD="password" DBYTES_SERVER="172.17.14.76" 
DBYTES_APIKEY="9c208321ad917ef07680f485f6597e37b29ae53d23a62bc7e9ca4af97e0ad85b"<br/>
<b>Skiping Deceptivebytes:</b>
msiexec.exe /i InvinSetup_23_02_21.msi /l*v C:\ProgramData\install.log WAZUH_MANAGER="34.100.141.147" 
WAZUH_REGISTRATION_SERVER="34.100.141.147" WAZUH_AGENT_GROUP="default" WAZUH_REGISTRATION_PASSWORD="password" SKIP_ENDPOINT_DECEPTION="y"

## 🚧 Features

![image](https://user-images.githubusercontent.com/103485015/184895267-8c8fd0af-c923-44b1-a470-b79012bdd45a.png)
- System tray icon will turn green if all the services are running properly, if not then it will turn red.
- Status icon turns Red, Green, Gray and Yellow depending on its status.
- User will be prompted with notification depending on status of that specific tool.
- Log details for Invinsense single agent.
- Display service Invinsense3.0 on serices and also diplay process under name of single agent
- Create Window user name with single agent.

## ✅ How to perform quick virus scan with Microsoft Defender using Powershell
To complete a quick scan using PowerShell, use these steps:
<ul>
  <li>Open Start.</li>
<li>Search for PowerShell, right-click the top result, and select the Run as administrator option.</li>
<li>Type the following command to start a quick virus scan and press Enter:Start-MpScan -ScanType QuickScan</li>
</ul>

## ✅ How to perform full scan with Microsoft Defender using Powershell
To complete a full scan using commands on Windows 10, use these steps:
<ul>
  <li>Open Start.</li>
<li>Search for PowerShell, right-click the top result, and select the Run as administrator option.</li>
<li>Type the following command to start a full virus scan and press Enter:Start-MpScan -ScanType FullScan</li>
</ul>

## ✅ How to install Single Installer..(install wazuh,deceptiveBytes,Sysmon,SingleAgent)

- run the installation .msi file.
- Once the installation wizard has opened, click on `install`.
- Then User Access Control dialog box will appear, click on `YES` to proceed.
- Wazuh installation wizard will popup, continue installtation of Wazuh.
- Once the Wazuh installation proccess has been completed, check the `Run Agent configuration interface` and click on `Finish` button.
- Now, Wazuh Agent manager will open up. There you'll have to input Manage IP and provide Authentication key then click on `save`. Go to `Manage tab` and click on `restart` to restart the Wazuh Agent Service.
- Close Wazuh Agent manager wizard and DeceptiveBytes wizard will appear.
- Continue will installation and input Server Address and API key.
- Click on `next` and installation proccess will start.
- Once finished, Single Agent 3.0 setup will appear.
- Continue installation process.
- Once finished, locate Single Agent icon in System Tray and click on it to open.

## ✅Install wazuh agent using commandline

-download `.msi` file https://packages.wazuh.com/4.x/windows/wazuh-agent-4.2.5-1.msi
- run following command 
- wazuh-agent-4.2.5.msi; ./wazuh-agent-4.2.5.msi /q WAZUH_MANAGER='172.17.14.101' WAZUH_REGISTRATION_SERVER='172.17.14.101'

## ✅Install deceptive bytes agent using commandline

-download `.msi` file https://github.com/Infopercept/invinsense-agent/blob/main/DeceptiveBytes.EPS.x64%20(1).msi
- run following command 
- msiexec /i "DeceptiveBytes.EPS.x64.msi" ALLUSERS=1 /qn /norestart ServerAddress="3.18.202.216" ApiKey="9c208321ad917ef07680f485f6597e37b29ae53d23a62bc7e9ca4af97e0ad85b"

## ✅Install Sysmon64
- Download `.msi` file https://download.sysinternals.com/files/Sysmon.zip
- Open command prompt run as administration.
- run following command to install sysmon64 `Sysmon64 -i`

## ⚖️License
Licensed under the (https://www.infopercept.com/) License, Version 3.0.
Copyright 2022 Infopercept. [Copy of the license](LICENSE.txt).

## 🤝Contributors 

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="https://github.com/sunnym-icpl"><img src="https://avatars.githubusercontent.com/u/68695557?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Sunny rajwadi</b></sub></a><br /><a href="https://github.com/codenameone/CodenameOne/commits?author=Sunny rajwadir" title="Code">💻</a></td>
    <td align="center"><a href="https://github.com/vaibhavipinfopercept"><img src="https://avatars.githubusercontent.com/u/67953602?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Vaibhavi Pandya</b></sub></a><br /><a href="https://github.com/codenameone/CodenameOne/commits?author=Vaibhavi Pandya" title="Code">💻</a></td>
    <td align="center"><a href="https://github.com/Dhara-tech"><img src="https://avatars.githubusercontent.com/u/69102702?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Dhara patel</b></sub></a><br /><a href="https://github.com/codenameone/CodenameOne/commits?author=Dhara patel" title="Code">💻</a></td>


