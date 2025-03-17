import { auth } from './firebaseConfig';

export const updateUserDetails = async (firstName, lastName, username) => {
    try {
        const token = await auth.currentUser.getIdToken();

        const response = await fetch('/Account/UpdateUserDetails', {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ firstName, lastName, username })
        });

        if (response.ok) {
            alert('Details updated successfully!');
        } else {
            console.error("Failed to update user details.");
        }
    } catch (error) {
        console.error(error.message);
    }
};
