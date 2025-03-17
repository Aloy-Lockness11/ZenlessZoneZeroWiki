import { createUserWithEmailAndPassword } from "firebase/auth";
import { auth } from "./firebaseConfig";

export const registerUser = async (email, password, firstName, lastName, username) => {
    try {
        const userCredential = await createUserWithEmailAndPassword(auth, email, password);
        const token = await userCredential.user.getIdToken(); // Get Firebase Token

        // Send token & user details to backend
        const response = await fetch('/Account/Register', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, email, firstName, lastName })
        });

        const data = await response.json();

        if (response.ok) {
            window.location.href = data.redirectUrl;
        } else {
            alert(data.message || "Registration Error");
        }
    } catch (error) {
        console.error("Registration error:", error);
    }
};
