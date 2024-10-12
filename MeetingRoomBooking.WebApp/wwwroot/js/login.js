document.getElementById('toggle-password').addEventListener('click', function () {
    // Toggle the icon and word
    const icon = document.getElementById('toggle-icon');
    const word = document.getElementById('word-toggle');
    const passwordInput = document.getElementById('password');

    if (icon.classList.contains('bi-eye')) {
        // Change the icon to 'eye-slash' and the word to 'Hide'
        icon.classList.replace('bi-eye', 'bi-eye-slash');
        word.textContent = 'Hide';

        // Show the password
        passwordInput.type = 'text';
    } else {
        // Change the icon back to 'eye' and the word to 'Show'
        icon.classList.replace('bi-eye-slash', 'bi-eye');
        word.textContent = 'Show';

        // Hide the password
        passwordInput.type = 'password';
    }
});

document.getElementById('confirm-toggle-password').addEventListener('click', function () {
    // Toggle the icon and word
    const confirmIcon = document.getElementById('confirm-toggle-icon');
    const confirmWord = document.getElementById('confirm-word-toggle');
    const confirmPasswordInput = document.getElementById('confirm-password');

    if (confirmIcon.classList.contains('bi-eye')) {
        // Change the icon to 'eye-slash' and the word to 'Hide'
        confirmIcon.classList.replace('bi-eye', 'bi-eye-slash');
        confirmWord.textContent = 'Hide';

        // Show the password
        confirmPasswordInput.type = 'text';
    } else {
        // Change the icon back to 'eye' and the word to 'Show'
        confirmIcon.classList.replace('bi-eye-slash', 'bi-eye');
        confirmWord.textContent = 'Show';

        // Hide the password
        confirmPasswordInput.type = 'password';
    }
});
