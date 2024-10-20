function toggleClass(checkbox, labelId) {
    const label = document.getElementById(labelId); // Get the label element by ID
    if (checkbox.checked) {
        label.classList.add('toggle-active'); // Add active class if checked
    } else {
        label.classList.remove('toggle-active'); // Remove active class if unchecked
    }
}