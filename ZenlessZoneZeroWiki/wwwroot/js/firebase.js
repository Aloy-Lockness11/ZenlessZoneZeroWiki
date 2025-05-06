// firebase.js
// Ensure Firebase is loaded
if (typeof firebase === 'undefined') {
    console.error("Firebase SDK not loaded. Ensure firebase-app-compat.js and firebase-auth-compat.js are included.");
    window.firebaseAuth = {
        auth: null,
        signInWithGoogle: () => {
            window.location.href = '/Account/Login?message=Firebase%20SDK%20not%20loaded';
        }
    };
    throw new Error("Firebase SDK not loaded");
}

console.log("Firebase SDK loaded successfully");

// Firebase configuration
const firebaseConfig = {
    apiKey: "AIzaSyCeKhaKHjOdbKB7clvTiz5ypgEth51WjCw",
    authDomain: "project-287709986341.firebaseapp.com",
    projectId: "project-287709986341",
    storageBucket: "project-287709986341.appspot.com",
    messagingSenderId: "287709986341",
    appId: "1:287709986341:web:your-correct-app-id", // Replace with the correct appId from Firebase Console
    clientId: "287709986341-v9f3naqrr4jfvlvpq82pd4561i0sh7r.apps.googleusercontent.com"
};

// Initialize Firebase
let app, auth, provider;
try {
    console.log("Attempting to initialize Firebase with config:", firebaseConfig);
    app = firebase.initializeApp(firebaseConfig);
    auth = firebase.auth();
    provider = new firebase.auth.GoogleAuthProvider();
    console.log("Firebase initialized successfully");
} catch (error) {
    console.error("Firebase initialization failed:", error);
    console.error("Error code:", error.code);
    console.error("Error message:", error.message);
    window.firebaseAuth = {
        auth: null,
        signInWithGoogle: () => {
            window.location.href = '/Account/Login?message=Firebase%20initialization%20failed:%20' + encodeURIComponent(error.message);
        }
    };
}

// Function to handle Google Sign-In
async function signInWithGoogle() {
    if (!auth || !provider) {
        console.error("Firebase auth not initialized");
        window.location.href = '/Account/Login?message=Firebase%20not%20initialized%20properly';
        return;
    }

    // Show loading state
    const button = document.getElementById('googleSignInButton');
    const text = document.getElementById('googleSignInText');
    const loading = document.getElementById('googleSignInLoading');
    if (button && text && loading) {
        button.disabled = true;
        text.textContent = 'Redirecting...';
        loading.classList.remove('d-none');
    } else {
        console.error("DOM elements for Google Sign-In button not found");
    }

    try {
        // Use signInWithRedirect instead of signInWithPopup
        await auth.signInWithRedirect(provider);
        // Note: The redirect will take the user away, and the result will be handled on redirect back
    } catch (error) {
        console.error("Error during Google Sign-In:", error);
        alert("Google Sign-In failed: " + error.message);
        if (button && text && loading) {
            button.disabled = false;
            text.textContent = 'Sign in with Google';
            loading.classList.add('d-none');
        }
    }
}

// Handle the redirect result when the user returns
auth.getRedirectResult()
    .then(async (result) => {
        if (result.user) {
            const user = result.user;
            const idToken = await user.getIdToken();
            await sendTokenToBackend(idToken);
        }
    })
    .catch((error) => {
        console.error("Error during Google Sign-In redirect:", error);
        const button = document.getElementById('googleSignInButton');
        const text = document.getElementById('googleSignInText');
        const loading = document.getElementById('googleSignInLoading');
        if (button && text && loading) {
            button.disabled = false;
            text.textContent = 'Sign in with Google';
            loading.classList.add('d-none');
        }
        window.location.href = '/Account/Login?message=' + encodeURIComponent("Google Sign-In failed: " + error.message);
    });

async function sendTokenToBackend(idToken) {
    const button = document.getElementById('googleSignInButton');
    const text = document.getElementById('googleSignInText');
    const loading = document.getElementById('googleSignInLoading');

    try {
        const response = await fetch('/Account/GoogleLogin', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify({ idToken })
        });
        const data = await response.json();
        if (data.success) {
            window.location.href = '/Home/Index'; // Redirect to dashboard
        } else {
            alert("Backend authentication failed: " + data.error);
            if (button && text && loading) {
                button.disabled = false;
                text.textContent = 'Sign in with Google';
                loading.classList.add('d-none');
            }
        }
    } catch (error) {
        console.error("Error during Google Sign-In:", error);
        window.location.href = '/Account/Login?message=' + encodeURIComponent("Google Sign-In failed: " + error.message);
        if (button && text && loading) {
            button.disabled = false;
            text.textContent = 'Sign in with Google';
            loading.classList.add('d-none');
        }
    }
}

// Expose auth and signInWithGoogle for use in other scripts
window.firebaseAuth = { auth, signInWithGoogle };
console.log("window.firebaseAuth set:", window.firebaseAuth);