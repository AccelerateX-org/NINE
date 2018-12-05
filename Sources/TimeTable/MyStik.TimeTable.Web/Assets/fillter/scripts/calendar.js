
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

function initPrintCalendar(height, isMoSa, defaultDate) {

    $('#loading').hide();

    // Den Kalender "druckbar" machen
    // Gefunden http://code.google.com/p/fullcalendar/issues/detail?id=35 #42
    /*
    var w = $('#calendar').css('width');
    var beforePrint = function () {
        // prepare calendar for printing
        //$('#calendar').css('width', '21.5cm');
        $('#calendar').fullCalendar('render');
    };
    var afterPrint = function () {
        $('#calendar').css('width', w);
        $('#calendar').fullCalendar('render');
    };
    if (window.matchMedia) {
        var mediaQueryList = window.matchMedia('print');
        mediaQueryList.addListener(function (mql) {
            if (mql.matches) {
                beforePrint();
            } else {
                afterPrint();
            }
        });
    }
    window.onbeforeprint = beforePrint;
    window.onafterprint = afterPrint;
    */
    

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



function initAdminCalendar2() {
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




function initAdminCalendar() {
    $('body').popover({
        selector: '[data-toggle="popover"]'
    });

    $('body').tooltip({
        selector: 'a[rel="tooltip"], [data-toggle="tooltip"]'
    });

    $('#loading').hide();


    ////
    // Den Kalender "druckbar" machen
    // Gefunden http://code.google.com/p/fullcalendar/issues/detail?id=35 #42
    var w = $('#CourseList').css('width');
    var beforePrint = function () {
        // prepare calendar for printing
        $('#CourseList').css('width', '7.5in');
        $('#CourseList').fullCalendar('render');
    };
    var afterPrint = function () {
        $('#CourseList').css('width', w);
        $('#CourseList').fullCalendar('render');
    };
    if (window.matchMedia) {
        var mediaQueryList = window.matchMedia('print');
        mediaQueryList.addListener(function (mql) {
            if (mql.matches) {
                beforePrint();
            } else {
                afterPrint();
            }
        });
    }
    window.onbeforeprint = beforePrint;
    window.onafterprint = afterPrint;


    ////

    $('#CourseList').html('');

    $('#CourseList').fullCalendar({
        monthNames: ['Januar', 'Februar', 'M\u00e4rz', 'April', 'Mai', 'Juni', 'Juli', 'August', 'September', 'Oktober', 'November', 'Dezember'],
        monthNamesShort: ['Jan', 'Feb', 'M\u00e4r', 'Apr', 'Mai', 'Jun', 'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Dez'],
        dayNames: ['Sonntag', 'Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag'],
        dayNamesShort: ['So', 'Mo', 'Di', 'Mi', 'Do', 'Fr', 'Sa', 'So'],
        columnFormat: {
            month: 'ddd',
            week: 'ddd dd.MM',
            day: 'dddd dd.MM'
        },
        buttonText: {
            today: 'heute',
            week: 'Woche',
            day: 'Tag'
        },
        timeFormat: "HH:mm{ - HH:mm}",
        titleFormat: { month: 'MMMM yyyy', week: "dd. { '&#8722;' dd. MMMM yyyy}", day: 'dddd, d. MMMM yyyy' },
        weekNumbers: true,
        weekNumberTitle: 'KW',
        header: {
            left: 'prev,next today',
            center: 'title',
            right: ''
        },
        defaultView: 'agendaDay',
        allDaySlot: false,
        slotMinutes: 30,
        slotEventOverlap: false,
        //hiddenDays: [ 0 ], es finden Kurse an Sonntagen statt
        minTime: '8:00',
        maxTime: '22:00',
        axisFormat: "HH:mm",
        firstDay: 1,
        aspectRatio: 1,         // beeinflusst Größe des Kalenders als Ganzes, kleinere Werte machen den Bereich höher
        eventRender: function (event, element, view) {
            if (event.htmlToolbar != null) element.find('.fc-event-time').before(event.htmlToolbar);
            if (event.htmlContent != null) element.find('.fc-event-inner').append(event.htmlContent);
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

