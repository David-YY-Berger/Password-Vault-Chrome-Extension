export class DisplayScreen {

    constructor(urlScreenTextElement, passwordScreenTextElement, msgScreenTextElement) {

        this.urlScreenTextElement = urlScreenTextElement
        this.passwordScreenTextElement = passwordScreenTextElement
        this.msgScreenTextElement = msgScreenTextElement
        this.clear()
    }

    displayText(msg, password = "", url = "") {
        this.urlScreenTextElement.innerText = url
        this.passwordScreenTextElement.innerText = password
        this.msgScreenTextElement.innerText = msg
    }

    clear() {
        this.urlScreen = ''
        this.passwordScreen = ''
        this.msgScreen = ''
    }


}