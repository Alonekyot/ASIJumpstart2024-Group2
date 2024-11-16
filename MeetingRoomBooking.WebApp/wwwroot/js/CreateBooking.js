const toggle = document.getElementById("flexCheckDefault");
const form = document.getElementById("recurring-form");

console.log('Hello');

toggle.addEventListener('change',() => {
    form.classList.toggle("show", toggle.checked);
});


document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        events: '/BookingManagement/GetEvents',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        aspectRatio: 1.3
    });

    calendar.render();
});