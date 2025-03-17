import { signOut } from "firebase/auth";
import { auth } from "./firebaseConfig";

export const logoutUser = async () => {
    try {
        await signOut(auth); // Firebase logs out user

        // Notify backend and get redirect URL
        const response = await fetch("/Account/Logout", {
            method: "POST",
            headers: { "Content-Type": "application/json" }
        });

        const data = await response.json();

        if (response.ok) {
            window.location.href = data.redirectUrl; // Use redirect URL from backend
        } else {
            alert(data.message || "Logout failed.");
        }
    } catch (error) {
        console.error("Logout failed:", error);
    }
};
