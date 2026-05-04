# 3rdPartySignOn
collection of 3rd party (single) sign on authentication implementations (C# .Net)

**Curremtly availible**:

## Saml2 Demo

- saml2 provider: https://sustainsys.com/
- sustainsys git: https://github.com/Sustainsys/Saml2
- try online: https://test.cqrxs.eu/SingleSignOn/Saml

<img width="1024" alt="https://test.cqrxs.eu/SingleSignOn/Saml" src="https://github.com/user-attachments/assets/b09d8ff1-5bc9-4efe-b4cb-3ff00cb705cb" />


## AzureAd with OpenId

- test azureAD: heinrichelsiganlive355.onmicrosoft.com
- try online: https://test.cqrxs.eu/SingleSignOn/MSIdentity/
- with credentials
```
user: guest@heinrichelsiganlive355.onmicrosoft.com
pass: $2y$04$Lha1LRWzLxK1LBOuLhe0LO5a1xBCySXJV2bei6Xwp.XzR1jGOgQtG
```
<img width="1024" alt="https://test.cqrxs.eu/SingleSignOn/MSIdentity/" src="https://github.com/user-attachments/assets/6ef3ed92-2df5-42f1-b4c9-d2832859c4b0" />

### ms azure portal configuration

<img width="800"  alt="EntraID for heinrichelsiganlive355.onmicrosoft.com" src="https://github.com/user-attachments/assets/2f30b159-7ab7-4f4e-87c1-bbe833e7077a" />
https://portal.azure.com/#view/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/~/RegisteredApps
<img width="800" alt="https://portal.azure.com/#view/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/~/RegisteredApps" src="https://github.com/user-attachments/assets/84966e30-9478-406c-bf5f-e8c909a336eb" />
<img width="800"  alt="AzureAD App MSIdentity" src="https://github.com/user-attachments/assets/b5d4a233-e150-4ea5-be3a-7e191e7b7c30" />

### **External Identities | All identity providers**: add google openid as provider service to azure ad 
<img width="1024" alt="image" src="https://github.com/user-attachments/assets/ce71d0c8-0c5e-4474-8f0e-e08193df43ff" />

### configure google [openauth](https://console.cloud.google.com/auth/branding) branding
<img width="1157" height="1079" alt="image" src="https://github.com/user-attachments/assets/a5845d7a-a9c1-497b-8f36-c8aaad3a0548" />
