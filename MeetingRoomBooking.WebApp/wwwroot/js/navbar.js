const navtoggle = document.getElementById("navbar-toggle");
const navbar = document.getElementById("navbar")
const navbarText = document.querySelectorAll(".navbar-item-text");

navtoggle.addEventListener("click", () => {
    
    navbar.classList.toggle("navbar-active");

    navbarText.forEach(navText => {
        navText.classList.toggle('navbar-show-text');
    });

});