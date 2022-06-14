package com;

import com.intel.langutil.Iterator;
import com.intel.langutil.LinkedList;
//import com.intel.langutil.List;
//import com.intel.util.DebugPrint;
import com.intel.util.FlashStorage;

public class FlashStorageAPI {
    // This is a singleton pattern.
    private static FlashStorageAPI instance = null;

    //CONSTANT VALUES FOR CHAR AND BYTES:
    final static int BYTE_CHAR_RATIO = 1;

    final static int NUM_CHAR_IN_PASSWORD = 20;
    final static int NUM_BYTE_IN_PASSWORD = BYTE_CHAR_RATIO * NUM_CHAR_IN_PASSWORD;

    final static int NUM_CHAR_IN_URL = 20;
    final static int NUM_BYTE_IN_URL = BYTE_CHAR_RATIO * NUM_CHAR_IN_URL;

    final int SLOT_SIZE = NUM_BYTE_IN_PASSWORD + NUM_BYTE_IN_URL;

    // the code for the each flashstorage documents
    final static int DATA_CODE = 0;
    private final int PASSWORD_CODE = 1;  

    LinkedList<Byte[]> passwords = LinkedList.create();
    LinkedList<Byte[]> urls = LinkedList.create();
    int LENGTH = 0; //num pairs of urls adn passwords


    /**
     * This function is used to get the singleton instance of the FlashStorageService
     *
     * @return The instance of the FlashStorageService.
     */
    public static FlashStorageAPI getInstance() {
        if (instance == null)
            instance = new FlashStorageAPI();

        // This is a singleton pattern. It is returning the instance of the class.
        return instance;
    }

    private FlashStorageAPI() {
        //private CTOR:

        if(existsData()) loadData();
    }

    public void resetData() {
        if(existsData()) {
            FlashStorage.eraseFlashData(DATA_CODE);
        }
        if(isRegistered()) {
            FlashStorage.eraseFlashData(PASSWORD_CODE);
        }
        LENGTH = 0;
        passwords = LinkedList.create();
        urls = LinkedList.create();
    }

    /**
     * Check if there is data stored in the flash memory
     *
     * @return A boolean value.
     */
    public boolean existsData() {
        return 0 != FlashStorage.getFlashDataSize(DATA_CODE);
    }


    /**
     * Check if user registered aka password is stored in the flash memory
     *
     * @return A boolean value.
     */
    public boolean isRegistered() {
        return 0 != FlashStorage.getFlashDataSize(PASSWORD_CODE);
    }
    
    public void loadData() {
        byte[] data = new byte[FlashStorage.getFlashDataSize(DATA_CODE)];
        FlashStorage.readFlashData(DATA_CODE, data, 0);

        byte[] pass;
        byte[] url;

        final int slot_size = NUM_BYTE_IN_PASSWORD + NUM_BYTE_IN_PASSWORD;
        for (int i = 0; i < data.length; i += slot_size) {
            // for every pair of: password, url
            pass = Utils.sliceArray(data, i, i + NUM_BYTE_IN_PASSWORD);
            url = Utils.sliceArray(data, i + NUM_BYTE_IN_PASSWORD, i + slot_size);

            passwords.add(Utils.convertByte(pass));
            urls.add(Utils.convertByte(url));
            LENGTH++;
        }

    }

    public void saveData() {
        Iterator<Byte[]> passIter = passwords.getIterator();
        Iterator<Byte[]> urlIter = urls.getIterator();

        Byte[] dataToSave = new Byte[LENGTH * SLOT_SIZE];

        int i = 0;

        while (passIter.hasNext()) {
            Utils.place(dataToSave, passIter.getNext(), i * SLOT_SIZE);
            Utils.place(dataToSave, urlIter.getNext(), i * SLOT_SIZE + NUM_BYTE_IN_PASSWORD);
            i++;
        }

        FlashStorage.writeFlashData(DATA_CODE, Utils.convertByte(dataToSave), 0, dataToSave.length);

    }

    public void addData(byte[] password, byte[] url) {
        byte[] fixedPassword = Utils.padZeros(password, NUM_BYTE_IN_PASSWORD);
        byte[] fixedUrl = Utils.padZeros(url, NUM_BYTE_IN_URL);

        passwords.add(Utils.convertByte(fixedPassword));
        urls.add(Utils.convertByte(fixedUrl));
        LENGTH++;

        saveData();
    }

    public byte[] getPassword(byte[] currentUrl) {

        Byte[] fixedUrl = Utils.convertByte(Utils.padZeros(currentUrl, NUM_BYTE_IN_URL));

        Iterator<Byte[]> passIter = passwords.getIterator();
        Iterator<Byte[]> urlIter = urls.getIterator();

        Byte[] pass;
        Byte[] url;
        while (passIter.hasNext()) {
            url = urlIter.getNext();
            pass = passIter.getNext();
            if(Utils.equals(url, fixedUrl))
                return Utils.convertByte(pass);
        }

        return null;  // if not found
    }

    public LinkedList<Byte[]> getPasswords(){
        return passwords;
    }
    public int getNumOfPasswords(){
        return passwords.size();
    }
    
    
    /**
     * @return byte[] of all pairs (url, then password)
     */
    public byte[] getAllPasswords() {

        byte[] res = new byte[(LENGTH) /*num passwords */
                * SLOT_SIZE /*num bytes in (url+password) */ ];

        Iterator<Byte[]> passIter = passwords.getIterator();
        Iterator<Byte[]> urlIter = urls.getIterator();

        byte[] passwords;
        byte[] url;
        int counter = 0;
        while (passIter.hasNext()) {

            url = Utils.convertByte(urlIter.getNext());
            passwords = Utils.convertByte(passIter.getNext());
            //add url
            for (int i = 0; i < NUM_BYTE_IN_URL; i++) {
                res[counter++] = url[i];
            }
            //add passwords
            for (int i = 0; i < NUM_BYTE_IN_PASSWORD; i++) {
                res[counter++] = passwords[i];
            }

        }
        return res;
    }

    /**
     * @return boolean if password correct
     */
    public boolean testPassword(byte[] p1) {
        byte[] p2 = new byte[FlashStorage.getFlashDataSize(PASSWORD_CODE)];
        FlashStorage.readFlashData(PASSWORD_CODE, p2, 0);
        return Utils.equals(p1, p2);
    }
    
    /**
     * It sets the password
     *
     * @param p The password to be written to the flash storage.
     */
    public void setPassword(byte[] p) {

        // write to the FlaseStorage
        FlashStorage.writeFlashData(PASSWORD_CODE, p, 0, p.length);
    }
    
}
