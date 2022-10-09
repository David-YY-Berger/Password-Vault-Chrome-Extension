# Connecting A Password Vault (in a TEE) with a Chrome Extension
### Project of David Berger and Eyal Seckbach - at Lev Academic Institute (Jerusalem)
(TEE = Trusted Execution Environment)  
  
## Overview
Our project has 3 parts:

![image](https://user-images.githubusercontent.com/91850832/194752126-617292f6-c7e7-489a-9211-c0cee0fe8495.png)
(1) The Chrome Extension (**JavaScript, HTML** - independently learned) -- the UI  
(2) The Host/Server Application (**C#**)   
(3) The Applet that runs in a **TEE** (Trusted Execution Environment) using Intel’s DAL (**java**) -- holds the Data  
  
The extension communicates with the Server via a **Websocket**, and the Host communicates with the Applet with **Byte arrays** (Intel’s built-in system).   
The extensions pulls the URL from the browser, and sends to the Host (and to the Applet). The Applet returns a randomly generated password (using **Sha1 algorithm**, based on a **time seed** given by Host - much stronger that _Security through Obscurity_). The pairs of URL’s and Password are stored in the Applet’s secure **Flashstorage**. All communication is echoed in the console opened by the a Host (helpful for debugging).  
For **2-point security**, the user must use his computer (“_what you have_”) and enter a master password (“_what you know_”) before accessing any information.   
The extension pulls the URL from the browser and **injects a script** into the browser (using **Chrome’s ActiveTab**), automatically locating and entering the password in the appropriate text box on the website.  
The application is versatile, and can be run in chrome, or in any JS IDE (just uncheck the checkbox)  
  
## How to run our Program (after downloading the Source Code):
You must download Intel's DAL: https://www.intel.com/content/www/us/en/developer/tools/dal/sdk-get-started.html  
(1) Ensure that Intel’s DAL is working, and compile the program into an **".dalp"** file. **Increase the flash storage size** in the Manifest:  
  
![image](https://user-images.githubusercontent.com/91850832/172611795-ca981c72-9d3f-472c-ab55-574a56447c5a.png)

(2) Set up the Host program, ensure that the **applet file path** (must be for local computer!) and **applet Id** are correct  
  
![image](https://user-images.githubusercontent.com/91850832/172613221-b14106a2-af67-431a-86ad-4abf03b36eaa.png)

(3) Run the Server (from the Host application)  
  
![image](https://user-images.githubusercontent.com/91850832/172613513-5f651bc3-110a-4d58-94ed-a304810cb468.png)

(4) Upload the extension’s folder to your google account (**"load unpacked"**)  
  
![image](https://user-images.githubusercontent.com/91850832/172614958-61e22d60-9a5d-403f-a14e-b06fc05de656.png)

(5) Enjoy  
  
![image](https://user-images.githubusercontent.com/91850832/172630405-0717f43d-e0cf-4458-98f8-cb95239d9ff1.png)
![image](https://user-images.githubusercontent.com/91850832/173538560-db7d6646-3cff-4ab4-a356-3f6e8c48f173.png)
![image](https://user-images.githubusercontent.com/91850832/172663317-8cfb3861-324e-464f-8976-763b6ccadcc9.png)

  

### Points about the source code:
*  Relevant **Exceptions** in the Host
*  Clear, **Universal Emuns**
*  **Organized API’s**, black box (Extension users “websocketAPI”, Host users “AppletAPI”, Applet uses “FlashStorageAPI”)

### Future ideas:
- [ ] Enabling Multiple users (on one computer) to use the TEE
- [ ] A better random seed (ex physical world generated instead of time...)
- [ ] Allowing User to hide the Console’s Echo
- [ ] Storing both the password and username with the URL
- [ ] Running a background script, offering User to save password in the Vault whenever User enters a password on a website
- [ ] Encrypting or Signing (or both) communication (Host <-> Applet, and Host <-> Chrome Extension)



## Motivation of Product:

#### What’s on the market now:
There are many different programs (_software_) which store a User’s passwords (and websites), often storing them in a cloud, theoretically accessible from any location. 

#### Our idea - A Local Password Vault in a TEE:
Instead of storing this sensitive data in a cloud/software, we have a _hardware_ solution - we will store passwords in a **local TEE (Trusted Execution Environment)**. 
A Host app on the computer can communicate with the TEE, and open a WebSocket (with local host) which will communicate with a Google Chrome Extension, allow the User to quickly save pairs of URLs and passwords

#### General Strengths of a local Password Vault (vs software):
 * Hardware is often more secure than software (objectively); less bugs and backdoors, less attack surfaces (attacker must run on the local machine, and listen to sockets)
 * Implementation, management is much simpler

#### General Weaknesses of a local Pwd Vault (vs software):
 * Inaccessible if user is not near computer 
 * Not easily portable to another computer (even to switch permanently)
 * Not as scalable
 * If something happens to the Hardware, the information is lost 

However - in our implementation - Our passworld vault's hardware is secured - see Full Security Analysis.
  
    
      
      
## Full Security Analysis:

### The Attacker:
A Password Vault would be attacked by anyone wanted to access the data inside. The Attacker probably will not care about changing the data ("**integrity**) because even if the password vault returns false passwords, the website will not function for the user. It is possible that he will want to generate his own passwords and supply them to the user, but at that point he would have to be running on the user's local machine. Furthermore - he could not access any previous passwords, rather he could just compromise future passwords or new websites that the User visits (it is likely that by then, if he is running on the local machine, he will have already done other damage....).    
**Availability** is also an unlikely target...A user would never pay a high ransom for his passwords back, he would just create new passwords with the website. Again, the Attacker cannot actually access the passwords within the fault.   
The only real motivation here would be to attack the Vault's **Confidentiality**. To protect against this attack, the information is secured in the TEE, and protected by 2-point security (as mentioned in the Overview).  
  
### Assets:
Our top priority is the (1) **passwords in the Vault**. This is protected well by the TEE's structure.    
Our second priority is the (2) **algorithm to generate the passwords**. If the attacker understands our algorithm - he can easily produce identical passwords, and has effectively broken into our vault. We understand that _Security through Obscurity_ is foolish, and therefore use a **Sha1 Algorithm** and a **time seed** (mentioned in Overview). This is not an ideal solution; the random seed is based on time, which is not a genuinely random variable... and therefore before marketing this product, we would generate the seed based on a _truly random variable_ (like the User's move of a mouse, or temperature that day, or the combination of different factors).   
  
### Attack Surfaces:
**User input** - The current version does not protect against malicious user inputs (for example code injections).However, because the chrome extension programmatically takes the URL, there is very little user input in general (except for signing in, and register) - most operations are done thru buttons. And therefore our program is relatively user to protect.  
**Sniffing** - This is potentially a huge problem - because if the Attacker simply **audits** the communication over the websocket, he has effectively breached any password retrieved by the User. However - because all communication runs on the **localhost** (127.0.0.0), in order to sniff this channel, the attacker must physically be on this computer (which is unlikely). And even if the attacker takes the machine - everything is protected by 2 point security.  
The only concern is if the attacker manages to run a malicious code on the user's computer (auditing the messages) which can replay them to the attacker from afar. We assume that the user has a safe PC.   
**Hardware Attack** - quote from Intel's website: 
> "Intel Trusted Execution Technology provides hardware-based mechanisms that help protect against software-based attacks and protects the confidentiality and integrity of data stored or created on the client PC."  
And therefore, we are not concerned about a hardware breach.
  
### Response to a Breach:
We would recommend that the User the "GetAllPasswords" functionality to see the websites saved by our application, and then immediately resets the data in the Vault. Then, the User must change his personal password of every website. The Attacker would be immediately stopped. We would then require the User to notify us, and we would track (from the User's computer's **Ledger**) when the attacker broke in, understand the attack surface breached, and protect another breach in the future. Lastly, we would change our random seed, and continue to market our product.









