let notificationPlayed = false; // Ensure the notification message is displayed only once
let soundPlayed = false; // Ensure the notification sound is played only once

window.onload = function () {
    const notification = document.getElementById('notification');
    const sound = document.getElementById('notificationSound');
    const notiSwitch = document.getElementById('notiSwitch'); // Get the switch element

    // Handle the notification message
    if (notification && !notificationPlayed) {
        notificationPlayed = true; // Mark the notification as displayed

        // Fade out the notification after a delay
        setTimeout(() => {
            notification.style.opacity = '0'; // Start fade-out
            setTimeout(() => {
                if (notification) {
                    notification.remove(); // Remove notification from DOM after fade-out
                }
            }, 500); // Wait for fade-out animation to complete
        }, 2000); // Delay before fade-out starts
    }

    // Handle the notification sound
    if (sound && notiSwitch.checked && !soundPlayed) {
        sound.play().catch(error => {
            console.error("Notification sound failed to play:", error);
        });

        soundPlayed = true; // Mark the sound as played
    }
};

// Event listener for the switch to dynamically control the sound
document.getElementById('notiSwitch').addEventListener('change', function () {
    if (!this.checked) {
        console.log("Sound is disabled."); // Optional log for debugging
        soundPlayed = false; // Reset soundPlayed so the sound can be enabled again if turned on
    } else {
        console.log("Sound is enabled."); // Optional log for debugging
    }
});