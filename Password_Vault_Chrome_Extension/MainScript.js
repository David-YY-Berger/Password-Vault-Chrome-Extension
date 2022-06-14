import {WebSocketAPI} from './WebSocketAPI.js'
import {DisplayScreen} from './DisplayScreen.js'
import {ToServerOperation} from './Enums.js'

//buttons:
const registerButton = document.querySelector('[data-register]')
const signInButton = document.querySelector('[data-sign-in]')
const getAllPasswordsButton = document.querySelector('[data-get-all-passwords]')
const getPasswordButton = document.querySelector('[data-get-password]')
//see websockethandler file for button's callbacks
const connectToSocketButton = document.querySelector('[data-connect-to-socket]')
const resetMemButton = document.querySelector('[data-reset-memory]')

const checkBoxRunningChrome =  document.querySelector('#ckbox_running_chrome'); //checks the checkbox by default
checkBoxRunningChrome.checked = true;
const checkBoxConnectedServer = document.querySelector('#ckbox_connected-to-server'); 
//if connected, this is checked...but only by back code, never by the user
checkBoxConnectedServer.checked = false;
checkBoxConnectedServer.disabled = true;

//"screen"
const urlScreenTextElement = document.querySelector('[data-url-screen]')
const passwordScreenTextElement = document.querySelector('[data-password-screen]')
const msgScreenTextElement = document.querySelector('[data-msg-screen]')

//objects:
const myDisplayScreen = new DisplayScreen(urlScreenTextElement, passwordScreenTextElement, msgScreenTextElement)
const websockethandler = new WebSocketAPI(myDisplayScreen);
const socketName = "ws://127.0.0.1:6130/WSBehav_Applet";
 
//string messages:
const alreadyConnected = "already connected to the Server!";
const mustconnect = "you must first initialize the websocket!";

websockethandler.initialize(socketName); //you can comment this out, and manually connect

//ADD EVENTHANDLERS FOR BUTTONS/CHECKBOXES::

registerButton.addEventListener('click', button => {
    if (websockethandler.isInitialized() === false) {
        alert(mustconnect);
        return;
    }
    
    if (confirm("Registering will erase any memory. Are you sure that you want to Register anyway?")) {
        const textFromScreen = prompt("Please Enter a Password:");
        websockethandler.sendMsgToServer(ToServerOperation.REGISTER.name + textFromScreen)
    }
})

signInButton.addEventListener('click', button => {
    if (websockethandler.isInitialized() === false) {
        alert(mustconnect);
        return;
    }
    //must get text from screen!
    const textFromScreen = prompt("Please enter Password:");
    websockethandler.sendMsgToServer(ToServerOperation.TRY_TO_SIGN_IN.name + textFromScreen)
})

getAllPasswordsButton.addEventListener('click', button => {
    if (websockethandler.isInitialized() === false) {
        alert(mustconnect);
        return;
    }
    websockethandler.sendMsgToServer(ToServerOperation.GET_ALL_PASSWORDS.name)
})

getPasswordButton.addEventListener('click', button => {
    if (websockethandler.isInitialized() === false) {
        alert(mustconnect);
        return;
    }

    const checkBox = document.querySelector('#ckbox_running_chrome');
    if (checkBox.checked === true) //then get the URL directly from the browser
    {
        chrome.tabs.query({'active': true, 'lastFocusedWindow': true}, function (tabs) {
            const url = tabs[0].url;
            //alert("in function" + url);
            let domain_name = (new URL(url)).hostname;
            if (domain_name.substring(0, 4) === "www.") {
                domain_name = domain_name.substring(4);
            }

            myDisplayScreen.displayText("", "", domain_name);
            websockethandler.sendMsgToServer(ToServerOperation.GET_PASSWORD.name + domain_name)
        });
    } else if (checkBox.checked === false) { //if program run from debugger
        const url = prompt("pls enter url:");
        let domain_name = (new URL(url)).hostname;
        if (domain_name.substring(0, 4) === "www.") {
            domain_name = domain_name.substring(4);
        }

        myDisplayScreen.displayText("", "", domain_name);
        websockethandler.sendMsgToServer(ToServerOperation.GET_PASSWORD.name + domain_name)
    }
})

resetMemButton.addEventListener('click', button => {
    if (websockethandler.isInitialized() === false) {
        alert(mustconnect);
        return;
    }
    websockethandler.sendMsgToServer(ToServerOperation.RESET_MEMORY.name)
})

connectToSocketButton.addEventListener('click', button => {
    if(websockethandler.isInitialized() === true){
        alert(alreadyConnected);
        return;
    }
    websockethandler.initialize(socketName);
})


// const tryThisButton = document.querySelector('[data-try-this]')
// tryThisButton.addEventListener('click', button => {

//     const checkBox = document.querySelector('#ckbox_running_chrome');
//     if (checkBox.checked === true) {
//         alert("checked")
//     } else if (checkBox.checked === false) {
//         alert("not checked")
//     }


//     //end of function
// });

