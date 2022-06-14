package com;

//import com.intel.langutil.Iterator;
import com.intel.util.*;


public class Applet_Package extends IntelApplet {

    //OPERATION CODES, HOST -> APPLET:
    final int RESET_MEMORY = 0;     //Applet erases any existing memory
    final int RETRIEVE_PASSWORD = 1; //Host sends URL, if Applet has no password saved,
    //generates password and returns
    final int GET_ALL_PASSWORDS = 2; //Applet prints all passwords
    final int SIGN_IN = 3;
    final int REGISTER = 4;


    //OPERATION CODES, APPLET -> HOST:
    final int RES_FAIL = 0;
    final int RES_SUCCESS = 1;
    final int RES_PSWD_GENERATED = 2;
    final int RES_PSWD_RETRIEVED = 3;
    final int RES_NOT_SIGNED_IN = 4;
    final int RES_WRONG_PASSWORD = 5;
    final int RES_NOT_REGISTERED = 6;

    FlashStorageAPI fsInstance = FlashStorageAPI.getInstance();

    boolean loggedIn = false;


    public int onInit(byte[] request) {
        DebugPrint.printString("Hello, DAL!");
        return APPLET_SUCCESS;
    }


    public int invokeCommand(int commandId, byte[] request) {

        //byte[] response;

        switch (commandId) {
            case RESET_MEMORY:
            {
                // reset the flash storage, not require logging
                fsInstance.resetData();
                sendEmptyResponse(RES_SUCCESS);
                break;
            }
            case RETRIEVE_PASSWORD:
            {
                if(!loggedIn)	
                {
                    sendEmptyResponse(RES_NOT_SIGNED_IN);
                    break;
                }
                byte[] password = getPassword(request);
                if(password == null)
                    sendResponse(RES_PSWD_GENERATED, generatePassword(request));
                else //if password is already generated
                    sendResponse(RES_PSWD_RETRIEVED, password);
                break;
            }
            case GET_ALL_PASSWORDS:
            {
                if(!loggedIn)
                {
                    sendEmptyResponse(RES_NOT_SIGNED_IN);
                    break;
                }
                sendResponse(RES_SUCCESS, fsInstance.getAllPasswords());
                break;
            }
            case SIGN_IN:
            {
            	if (!fsInstance.isRegistered()) {
                    sendEmptyResponse(RES_NOT_REGISTERED);
            	}
                else if(fsInstance.testPassword(request)){
                    loggedIn = true;
                    sendEmptyResponse(RES_SUCCESS);
                } else {
                    sendEmptyResponse(RES_WRONG_PASSWORD);
                }
                break;

            }
            case REGISTER:
            {
            	
                fsInstance.setPassword(request);
                loggedIn = true;          
                sendEmptyResponse(RES_SUCCESS);

                break;

            }
            default:
            {
                sendEmptyResponse(RES_FAIL); //no proper command received..
            }
            break;
        }


        return APPLET_SUCCESS;
    }


    //sends response code, with empty byte[]
    public void sendEmptyResponse(int responseCode) {
        byte[] response = new byte[0];
        sendResponse(responseCode, response);
    }

    public void sendResponse(int code, byte[] response) {
        DebugPrint.printString("Sending code: " + code + ", message: ");
        DebugPrint.printBuffer(response);

        /*
         * To return the response data to the command, call the setResponse
         * method before returning from this method.
         * Note that calling this method more than once will
         * reset the response data previously set.
         */
        setResponse(response, 0, response.length);

        /*
         * In order to provide a return value for the command, which will be
         * delivered to the SW application communicating with the Trusted Application,
         * setResponseCode method should be called.
         * Note that calling this method more than once will reset the code previously set.
         * If not set, the default response code that will be returned to SW application is 0.
         */
        setResponseCode(code);
    }


    /*
     * getPassword() - function might return null! caller must check..
     */
    public byte[] getPassword(byte[] websiteName) /*returns byte[] this website's passwords*/{
        return fsInstance.getPassword(websiteName);

    }
    public byte[] generatePassword(byte[] websiteName) {
    	// in order to get more randomness
    	for(int i = 0; i < fsInstance.getNumOfPasswords(); i++) 
    		Utils.randomBytes(FlashStorageAPI.NUM_BYTE_IN_PASSWORD);
    	
        // if there is no password for this url, we generate one and returns it
        byte[] password = Utils.randomBytes(FlashStorageAPI.NUM_BYTE_IN_PASSWORD);
        
        // every byte must be in range 0x21 to 0x7e to valid letters
        for(int i = 0; i < password.length; i++)
        	password[i] = (byte)((int)(password[i] % 94) + 33);
        	
        fsInstance.addData(password, websiteName);
        return password;
    }

    public int onClose() {
        DebugPrint.printString("Goodbye, DAL!");
        return APPLET_SUCCESS;
    }
}



