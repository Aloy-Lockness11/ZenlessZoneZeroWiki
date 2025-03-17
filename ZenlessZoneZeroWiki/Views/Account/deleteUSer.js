import { auth } from './firebaseConfig';

export const deleteUserAccount = async () => {
    if (!auth.currentUser) {
        alert("User not logged in");
        return;
    }

    const confirmDelete = window.confirm("Are you sure you want to delete your account? This action cannot be undone.");
    if (!confirmDelete) return;

    try {
        // Obtain Firebase authentication token
        const token = await auth.currentUser.getIdToken();

        // Call backend to delete user details from database
        const response = await fetch('/Account/DeleteUser', {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (response.ok) {
            // Delete user from Firebase Authentication
            await auth.currentUser.delete();

            alert('Account deleted successfully.');
            window.location.href = '/';
        } else {
            alert('Server-side deletion failed.');
        }
    } catch (error) {
        console.error("Error deleting account:", error.message);
    }
};
