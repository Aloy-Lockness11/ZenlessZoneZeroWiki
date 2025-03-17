import { initializeApp } from "firebase/app";
import { getAuth } from "firebase/auth";

const firebaseConfig = {
    apiKey: "AIzaSyCeKhaKHjOdbKB7clvTiz5ypgEth51WjCw",
    authDomain: "zenlesszonezerowikiauth.firebaseapp.com",
    projectId: "zenlesszonezerowikiauth",
    storageBucket: "zenlesszonezerowikiauth.appspot.com",
    messagingSenderId: "288709986341",
    appId: "1:288709986341:web:aa5fbd80b5d08b7dd097cc",
    measurementId: "G-9DEZE39Z5B"
};

const app = initializeApp(firebaseConfig);
export const auth = getAuth(app);
