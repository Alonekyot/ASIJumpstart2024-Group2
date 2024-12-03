const toggle = document.getElementById("flexCheckDefault");
const form = document.getElementById("recurring-form");

toggle.addEventListener('change', () => {
    form.classList.toggle("show", toggle.checked);
});
document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');
    var roomId = document.getElementById('roomId').value; // Dynamically get Room ID

    

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        events: function (fetchInfo, successCallback, failureCallback) {
            fetch('/BookingManagement/GetEvents?roomId=' + roomId)
                .then(response => response.json())
                .then(events => successCallback(events))
                .catch(error => failureCallback(error));
        },
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        eventClick: function (info) {
            document.getElementById('modalEventTitle').textContent = info.event.title;
            document.getElementById('modalEventLocation').textContent = info.event.extendedProps.description;
            document.getElementById('modalEventStart').textContent = info.event.start.toLocaleString();
            document.getElementById('modalEventEnd').textContent = info.event.end ? info.event.end.toLocaleString() : 'N/A';

            var eventModal = new bootstrap.Modal(document.getElementById('eventModal'));
            eventModal.show();
        }
    });


    calendar.render();
});
