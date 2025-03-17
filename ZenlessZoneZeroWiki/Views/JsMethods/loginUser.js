import { signInWithEmailAndPassword } from "firebase/auth";
import { auth } from "./firebaseConfig";

export const loginUser = async (email, password) => {
    try {
        const userCredential = await signInWithEmailAndPassword(auth, email, password);
        const token = await userCredential.user.getIdToken(); // Get Firebase Token

        const response = await fetch('/Account/Login', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();

        if (response.ok) {
            window.location.href = data.redirectUrl; 
        } else {
            alert(data.message || "Loggin Error");
        }

    } catch (error) {
        console.error("Login error:", error);
    }
};
