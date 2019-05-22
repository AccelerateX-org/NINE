function initDayCalendar(idCal) {

    if (idCal == null || idCal === "")
        idCal = "calendar";

    var height = 640;

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
}



function initWeekCalendar(idCal, defaultDate) {

    if (idCal == null || idCal === "")
        idCal = "calendar";

    var height = 630;


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
}





/*
function initCalendar(isInteractive, isMoFr, isSmall, idCal) {

    if (idCal == null || idCal === "")
        idCal = "calendar";

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

    var slotDuration = "00:15:00";
    var contentHeight = 1088;
    if (isSmall) {
        slotDuration = "00:30:00";
        contentHeight = 500;
    }


    $('#' + idCal).fullCalendar('destroy'); // destroy the calendar
    $('#' + idCal).fullCalendar({
        weekNumbers: true,
        header: {
            left: 'title',
            center: '',
            right: 'prev,next,today,agendaWeek,agendaDay'
        },
        defaultView: smallScreen ? 'agendaDay' : 'agendaWeek',
        allDaySlot: false,
        slotDuration: slotDuration,
        minTime: "08:00:00",
        maxTime: "21:00:00",
        slotEventOverlap: false,
        hiddenDays: isMoFr ? [ 0, 6 ] : null,
        firstDay: 1,
        displayEventTime: false,
        contentHeight: contentHeight,
        eventRender: function (event, element, view) {
            
            if (event.htmlToolbarInfo != null || (event.htmlToolbar != null && isInteractive)) {

                var htmlToolBar = "";

                if (event.htmlToolbarInfo != null) {
                    htmlToolBar += event.htmlToolbarInfo;
                }

                if (event.htmlToolbar != null && isInteractive) {
                    htmlToolBar += event.htmlToolbar;
                }

                htmlToolBar = htmlToolBar.trim();

                if (htmlToolBar.length > 0) {
                    var html = "<span  class=\"btn-group nine-fc-event-subscription\">" + htmlToolBar + "</span>";
                    element.find('.fc-content').prepend(html);
                }
            }
            
            if (event.htmlContent != null) element.find('.fc-content').append(event.htmlContent);
        },
        eventClick: function (calEvent, jsEvent, view) {

            onShowEventInfo(calEvent.courseId);

            // change the border color just for fun
            // $(this).css('border-color', 'red');

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

function initPrintCalendar(height, isMoSa, defaultDate) {


    $('#loading').hide();

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
        /*
        eventRender: function (event, element, view) {
            if (event.htmlContent != null) element.find('.fc-content').append(event.htmlContent);
        },
        */
        loading: function (bool) {
            if (bool)
                $('#loading').show();
            else
                $('#loading').hide();
        },
    });
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

