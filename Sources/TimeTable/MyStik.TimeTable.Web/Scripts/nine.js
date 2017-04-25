
function initCalendar() {
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

    var smallScreen = false;
    if (window.innerWidth < 768)
        smallScreen = true;

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
        titleFormat: smallScreen ? {month: '', week: "", day: '' } : { month: 'MMMM yyyy', week: "dd.MM.yyyy { '&#8722;' dd.MM.yyyy}", day: 'dddd, dd.MM.yyyy' },
        weekNumbers: true,
        weekNumberTitle: 'KW',
        header: {
            left: 'prev,next today agendaWeek,agendaDay',
            center: 'title',
            right: ''
        },
        defaultView: smallScreen ? 'agendaDay' : 'agendaWeek',
        allDaySlot: false,
        slotMinutes: 15,
        slotEventOverlap: false,
        //hiddenDays: [ 0, 6 ],
        minTime: '8:00',
        maxTime: '21:00',
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
    $('#CourseList').fullCalendar('refetchEvents');
}

