//need to import enums here?

export class WebSocketAPI {

    constructor(displayScreenElement) {
        this.socketToServer;
        this.MsgFromServer = "";
        this.myDisplayScreen = displayScreenElement;
    }

    isInitialized(){
        return document.querySelector('#ckbox_connected-to-server').checked;
    }

    initialize(webSocketAddress) {
        this.socketToServer = new WebSocket(webSocketAddress);
        this.myDisplayScreen.displayText("trying to connect..")
        //add callback from receiving a message:
        this.socketToServer.onmessage = this.onMessageCallback;
        //isInitialized field will be set when we receive our first message from the Server...
    }

    //this function contains callbacks for receiving messages from the Server
    onMessageCallback(event) {
        class FromServerOperation {    //server -> client

            static MUST_LOG_IN = new FromServerOperation("0"); //when client wants to get a password from vault, but he's not signed in
            static SEND_ME_PASSWORD = new FromServerOperation("1");
            static SUCCESS = new FromServerOperation('2');
            static ER_WRONG_PASSWORD = new FromServerOperation("3"); //when client wants to enter his account
            static HERE_IS_PASSWORD = new FromServerOperation("4"); //when server sends password to client
            static ER_NOT_REGISTERED = new FromServerOperation("5");
            static ER_UNKOWN_ERROR = new FromServerOperation("6");
            static HERE_ARE_ALL_PASSWORDs = new FromServerOperation("7");

            constructor(name) {
                this.name = name
            }
        }

        const passwordLength = 20;
        const lengthOfCommandId = 1;
        const urlScreenTextElement = document.querySelector('[data-url-screen]')
        const passwordScreenTextElement = document.querySelector('[data-password-screen]')
        const msgScreenTextElement = document.querySelector('[data-msg-screen]')

        switch (event.data[0]) { //command id is at beginning of the string..
            case (FromServerOperation.SUCCESS.name):
                {
                    document.querySelector('#ckbox_connected-to-server').checked = true;
                    msgScreenTextElement.innerText = event.data.substring(lengthOfCommandId, (event.data).length);
                    passwordScreenTextElement.innerText = "";
                    urlScreenTextElement.innerText = "";
                }
                break;
            case FromServerOperation.MUST_LOG_IN.name:
                {
                    msgScreenTextElement.innerText = event.data.substring(lengthOfCommandId, (event.data).length);
                    passwordScreenTextElement.innerText = "";
                    urlScreenTextElement.innerText = "";
                }
                break; 
            case FromServerOperation.ER_WRONG_PASSWORD.name:
                {
                    msgScreenTextElement.innerText = event.data.substring(lengthOfCommandId, (event.data).length);
                    passwordScreenTextElement.innerText = "";
                    urlScreenTextElement.innerText = "";
                }
                break;
            case FromServerOperation.HERE_IS_PASSWORD.name:
                {
                    //(1) display password and message on extension's screen
                    const passwordFromServer = event.data.substring(lengthOfCommandId, passwordLength+1);
                    passwordScreenTextElement.innerText = passwordFromServer;
                    //urlScreenTextElement.innerText = "";
                    msgScreenTextElement.innerText = event.data.substring(passwordLength+1);

                    //(2) if running in chrome browser - inject password to appropriate text box
                    const checkBox = document.querySelector('#ckbox_running_chrome');
                    if(checkBox.checked === true) 
                    {
                        // alert("checked");
                        chrome.tabs.query({'active': true, 'lastFocusedWindow': true}, function (tabs) {
                            // chrome.tabs.executeScript(null, {
                            //     code: `alert("inside query");`
                            // });      
    
                            const msg613 = passwordFromServer;
    
                            chrome.tabs.executeScript(null, {
                                code:
                                "var inputs = document.getElementsByTagName('input'); "
                                + "for (var i=0; i<inputs.length; i++) { " 
                                + " if (inputs[i].type.toLowerCase() === 'password') { "
                                + "  inputs[i].value = '"+ msg613 +"'"
                                +       "}"
                                + " }"
                            });
    
                        });
                    }
                    
                }
                break;
            case FromServerOperation.HERE_ARE_ALL_PASSWORDs.name:
                {
                    passwordScreenTextElement.innerText = event.data.substring(lengthOfCommandId)
                    msgScreenTextElement.innerText = "here are all passwords!";
                }
                break;
            case FromServerOperation.ER_NOT_REGISTERED.name:
                {
                    passwordScreenTextElement.innerText = "";
                    msgScreenTextElement.innerText = "You must first register!";
                }
                break;
            case FromServerOperation.ER_UNKOWN_ERROR.name:
                {
                    msgScreenTextElement.innerText = "unknown error!!";
                }
                break;
            
                default:
                break;
        }
    }

    

    sendMsgToServer(msg){
         this.socketToServer.send(msg)
    }


}