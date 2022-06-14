// ENUM FORMAT:
export class ToServerOperation {       // client ->server

    static REGISTER = new ToServerOperation("0");  //client sets up his account password
    static TRY_TO_SIGN_IN = new ToServerOperation("2"); //when client sends server a password to get into vault
    static GET_PASSWORD = new ToServerOperation("3"); //client wants password (from vault) for this URL
    static GET_ALL_PASSWORDS = new ToServerOperation("4"); //client wants all urls and passwords from vault
    static RESET_MEMORY = new ToServerOperation("5");

    constructor(name) {
        this.name = name
    }

}

