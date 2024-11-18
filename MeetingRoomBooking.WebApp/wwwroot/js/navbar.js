const navtoggle = document.getElementById("navbar-toggle");
const navbar = document.getElementById("navbar")
const navbarText = document.querySelectorAll(".navbar-item-text");
const roomGrid = documewnt.getElementById("card-grid");

navtoggle.addEventListener("click", () => {
    
    navbar.classList.toggle("navbar-active");
    roomGrid.classList.toggle("grid-2-cols");

    navbarText.forEach(navText => {
        navText.classList.toggle('navbar-show-text');
    });

});