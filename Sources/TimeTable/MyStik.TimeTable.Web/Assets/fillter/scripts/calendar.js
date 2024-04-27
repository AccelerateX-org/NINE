function initDayCalendar(idCal) {

    if (idCal == null || idCal === "")
        idCal = "calendar";

    var calendarEl = document.getElementById(idCal);

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'timeGridWeek',
        headerToolbar: {
            start: 'title',
            center: '',
            end: 'prev,next,today,dayGridMonth,timeGridWeek,dayGridDay'
        },
        timeZone: 'local',
        weekNumbers: true,
        allDaySlot: false,
        locale: 'de',
        dayHeaderFormat: {
            weekday: 'short',
            month: 'short',
            day: 'numeric',
            omitCommas: true
        },
        slotDuration: "00:45:00",
        slotMinTime: "08:00:00",
        slotMaxTime: "22:00:00",
        slotEventOverlap: false,
        contentHeight: "auto",
        /*
        eventContent: function (arg) {
            var italicEl = document.createElement('div');

            if (arg.event.extendedProps.htmlContent) {
                italicEl.innerHTML = "<div>" + arg.timeText + "</div>" + arg.event.extendedProps.htmlContent;
            } else {
                italicEl.innerHTML = "";
            }

            var arrayOfDomNodes = [italicEl];
            return { domNodes: arrayOfDomNodes };
        },
        */
        eventClick: function (calEvent, jsEvent, view) {
            calEvent.jsEvent.preventDefault(); // don't let the browser navigate
            //calEvent.el.style.borderColor = 'red';
            onShowEventInfo(calEvent.event.extendedProps.courseId, calEvent.event.id);
        },
    });
    calendar.render();
    return calendar;


    /*
    $('#' + idCal).fullCalendar('destroy'); // destroy the calendar
    $('#' + idCal).fullCalendar({
        weekNumbers: true,
        header: {
            left: 'title',
            center: '',
            right: 'prev,next,today,month,agendaWeek,agendaDay'
        },
        defaultView: 'agendaWeek',
        allDaySlot: false,
        slotDuration: "00:30:00",
        minTime: "08:00:00",
        maxTime: "22:00:00",
        slotEventOverlap: false,
        hiddenDays: null,
        firstDay: 1,
        displayEventTime: false,
        height: height,
        eventRender: function (event, element, view) {
            if (event.htmlContent != null) element.find('.fc-content').append(event.htmlContent);
        },
        eventClick: function (calEvent, jsEvent, view) {
            onShowEventInfo(calEvent.courseId);
        },
        loading: function (bool) {
            if (bool)
                $('#loading').show();
            else
                $('#loading').hide();
        },
    });
    */
}



function initWeekCalendar(idCal, defaultDate) {

    if (idCal == null || idCal === "")
        idCal = "calendar";

    var calendarEl = document.getElementById(idCal);

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: "timeGridWeek",
        headerToolbar: {
            start: '',
            center: '',
            end: ''
        },
        timeZone: 'local',
        allDaySlot: false,
        locale: 'de',
        dayHeaderFormat: { weekday: 'short' },
        slotDuration: "00:45:00",
        slotMinTime: "08:00:00",
        slotMaxTime: "22:00:00",
        slotEventOverlap: false,
        contentHeight: "auto",
        displayEventTime: true,
        /*
        eventContent: function (arg) {
            var italicEl = document.createElement('div');

            if (arg.event.extendedProps.htmlContent) {
                italicEl.innerHTML = "<div>"+arg.timeText+"</div>" + arg.event.extendedProps.htmlContent;
            } else {
                italicEl.innerHTML = "";
            }

            var arrayOfDomNodes = [italicEl];
            return { domNodes: arrayOfDomNodes };
        },
        */
        eventClick: function (calEvent, jsEvent, view) {
            calEvent.jsEvent.preventDefault(); // don't let the browser navigate
            onShowEventInfo(calEvent.event.extendedProps.courseId, calEvent.event.id);
        },
    });
    calendar.render();
    return calendar;


    /*
    $('#' + idCal).fullCalendar('destroy'); // destroy the calendar
    $('#' + idCal).fullCalendar({
        weekNumbers: false,
        header: {
            left: '',
            center: '',
            right: ''
        },
        defaultView: 'agendaWeek',
        allDaySlot: false,
        slotDuration: "00:30:00",
        minTime: "08:00:00",
        maxTime: "22:00:00",
        columnFormat: defaultDate ? "dd DD.MM.YYYY" : "dddd",
        slotEventOverlap: false,
        hiddenDays: null,
        firstDay: 1,
        height: height,
        defaultDate: defaultDate,
        eventRender: function (event, element, view) {
            if (event.htmlContent != null) element.find('.fc-content').append(event.htmlContent);
        },
        eventClick: function (calEvent, jsEvent, view) {
            onShowEventInfo(calEvent.courseId);
        },
        loading: function (bool) {
            if (bool)
                $('#loading').show();
            else
                $('#loading').hide();
        },
    });
    */
}


function initPrintCalendar(height, isMoSa, defaultDate) {

    var idCal = "calendar";
    var calendarEl = document.getElementById(idCal);

    $("#loading").hide();

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: "timeGridWeek",
        headerToolbar: {
            start: '',
            center: '',
            end: ''
        },
        timeZone: 'local',
        allDaySlot: false,
        locale: 'de',
        dayHeaderFormat: { weekday: 'long' },
        slotDuration: "00:30:00",
        slotMinTime: "08:00:00",
        slotMaxTime: "22:00:00",
        columnFormat: defaultDate ? "dd DD.MM.YYYY" : "dddd",
        slotEventOverlap: false,
        contentHeight: "auto",
        displayEventTime: true,
        height: height,
        hiddenDays: isMoSa ? [0] : [0, 6],

        /*
        eventContent: function (arg) {
            var italicEl = document.createElement('div');

            if (arg.event.extendedProps.htmlContent) {
                italicEl.innerHTML = "<div>" + arg.timeText + "</div>" + arg.event.extendedProps.htmlContent;
            } else {
                italicEl.innerHTML = "";
            }

            var arrayOfDomNodes = [italicEl];
            return { domNodes: arrayOfDomNodes };
        },
        */
    });
    calendar.render();
    return calendar;

    /*


    $('#calendar').fullCalendar('destroy'); // destroy the calendar
    $('#calendar').fullCalendar({
        weekNumbers: false,
        header: {
            left: '',
            center: '',
            right: ''
        },
        defaultView: 'agendaWeek',
        allDaySlot: false,
        slotDuration: "00:30:00",
        minTime: "08:00:00",
        maxTime: "22:00:00",
        columnFormat: defaultDate ? "dd DD.MM.YYYY" : "dddd",
        slotEventOverlap: false,
        hiddenDays: isMoSa ? [0] : [0, 6],
        firstDay: 1,
        height: height,
        defaultDate: defaultDate,
        loading: function (bool) {
            if (bool)
                $('#loading').show();
            else
                $('#loading').hide();
        },
        });
    */

}


/*
function initWeeklyCalendar(height, isMoSa, defaultDate, idCal) {


    if (idCal == null || idCal === "")
        idCal = "calendar";

    $('#loading').hide();


    $('#' + idCal).fullCalendar('destroy'); // destroy the calendar
    $('#' + idCal).fullCalendar({
        weekNumbers: false,
        header: {
            left: '',
            center: '',
            right: ''
        },
        defaultView: 'agendaWeek',
        allDaySlot: false,
        slotDuration: "00:30:00",
        minTime: "08:00:00",
        maxTime: "22:00:00",
        columnFormat: defaultDate ? "dd DD.MM.YYYY" : "dddd",
        slotEventOverlap: false,
        hiddenDays: isMoSa ? [0] : [0, 6],
        firstDay: 1,
        height: height,
        defaultDate: defaultDate,
        loading: function (bool) {
            if (bool)
                $('#loading').show();
            else
                $('#loading').hide();
        },
    });
}



function initMobileCalendar(isInteractive, isMoFr) {
    $('body').popover({
        selector: '[data-toggle="popover"]'
    });

    $('body').tooltip({
        selector: 'a[rel="tooltip"], [data-toggle="tooltip"]'
    });

    $('#loading').hide();


    var smallScreen = false;
    if (window.innerWidth < 768)
        smallScreen = true;
    ////


    $('#calendar').fullCalendar('destroy'); // destroy the calendar
    $('#calendar').fullCalendar({
        weekNumbers: true,
        header: {
            
            left: 'prev',
            center: 'title',
            right: 'next'
        },
        defaultView: 'agendaDay',
        allDaySlot: false,
        slotDuration: "00:30:00",
        minTime: "08:00:00",
        maxTime: "21:00:00",
        slotEventOverlap: false,
        hiddenDays: isMoFr ? [0, 6] : null,
        firstDay: 1,
        contentHeight: 555,
        eventRender: function (event, element, view) {

            if (event.htmlToolbarInfo != null || (event.htmlToolbar != null && isInteractive)) {

                var htmlToolBar = "<span  class=\"btn-group nine-fc-event-subscription\">";

                if (event.htmlToolbarInfo != null) {
                    htmlToolBar += event.htmlToolbarInfo;
                }

                if (event.htmlToolbar != null && isInteractive) {
                    htmlToolBar += event.htmlToolbar;
                }
                htmlToolBar += "</span>";

                element.find('.fc-content').prepend(htmlToolBar);
            }

            if (event.htmlContent != null) element.find('.fc-content').append(event.htmlContent);
        },
        loading: function (bool) {
            if (bool)
                $('#loading').show();
            else
                $('#loading').hide();
        },
    });
}
*/


function initAdminCalendar() {
    $('body').popover({
        selector: '[data-toggle="popover"]'
    });

    $('body').tooltip({
        selector: 'a[rel="tooltip"], [data-toggle="tooltip"]'
    });

    $('#loading').hide();

    var smallScreen = false;
    if (window.innerWidth < 768)
        smallScreen = true;


    $('#calendar').fullCalendar('destroy'); // destroy the calendar
    $('#calendar').fullCalendar({
        weekNumbers: true,
        header: {
            left: 'title',
            center: '',
            right: 'prev,next,today,agendaWeek,agendaDay'
        },
        defaultView: 'agendaDay',
        allDaySlot: false,
        slotDuration: "00:15:00",
        minTime: "08:00:00",
        maxTime: "21:00:00",
        slotEventOverlap: false,
        firstDay: 1,
        contentHeight: 1088,
        eventRender: function (event, element, view) {
            if (event.htmlContent != null) element.find('.fc-content').append(event.htmlContent);
        },
        loading: function (bool) {
            if (bool)
                $('#loading').show();
            else
                $('#loading').hide();
        },
    });
}



function UpdateCalendar() {
    $('#calendar').fullCalendar('refetchEvents');
}

