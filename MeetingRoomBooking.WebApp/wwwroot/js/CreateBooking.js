const toggle = document.getElementById("flexCheckDefault");
const form = document.getElementById("recurring-form");

toggle.addEventListener('change',() => {
    form.classList.toggle("show", toggle.checked);
});

document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');
    var roomId = document.getElementById('roomId').value; // Ensure roomId is correct

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        events: '/BookingManagement/GetEvents',
        data: { roomId: roomId },
        dayMaxEvents: true, // will apply the max number of events 
        datMaxEventsRows: 3,
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
  
        aspectRatio: 2
    });

    calendar.render();
});