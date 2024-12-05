document.addEventListener("DOMContentLoaded", function () {
    const dialog = document.getElementById("success-dialog");
    if (dialog) {
        setTimeout(() => {
            if (typeof dialog.close === "function") {
                dialog.close(); // Close the dialog after 3 seconds 
            }
            else {
                dialog.removeAttribute("open"); // Fallback if close() is not supported 
            }
        }, 3000);
    }
});

const close = document.getElementById("close-btn");

close.addEventListener("click", () => {
    console.log("hello");
});