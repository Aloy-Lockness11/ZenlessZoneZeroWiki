import { signInWithEmailAndPassword } from "firebase/auth";
import { auth } from './firebaseConfig';

const loginUser = async (email, password) => {
    try {
        const userCredential = await signInWithEmailAndPassword(auth, email, password);
        console.log(userCredential.user);
    } catch (error) {
        console.error(error);
    }
};