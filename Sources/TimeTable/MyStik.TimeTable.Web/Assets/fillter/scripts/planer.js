//var myCalendar = null;
var showWeekend = false;
var showWeekDay = true;

function initCalendar() {
    if (showWeekDay) {
        try {

            $('#loading').show();

            $('#calendar').html('');
            myCalendar = initWeekCalendar("calendar");
            myCalendar.setOption("headerToolbar", false);

            $("#btnPrev").addClass("disabled");
            $("#btnNext").addClass("disabled");

            showWeekendDays();
            showLabelDates();

        } catch (err) {
            var strMessage =
                "<div class=\"alert alert-danger\"><h4>Fehler beim Laden der Daten</h4>";
            strMessage += "<p>Grund: " + err + "</p>";
            strMessage += "<p>Abhilfe: Den Brwoser Cache leeren, z.B. mit der Tastenkombination CTRL + F5 bzw. Strg + F5</p>";
            strMessage += "</div>";

            $('#calendar').html(strMessage);
            $('#loading').hide();
        }
    } else {
        try {
            $('#loading').show();

            $('#calendar').html('');
            myCalendar = initDayCalendar("calendar");
            myCalendar.setOption("headerToolbar", false);

            $("#btnPrev").removeClass("disabled");
            $("#btnNext").removeClass("disabled");


            showWeekendDays();
            showLabelDates();

        } catch (err) {
            var strMessage =
                "<div class=\"alert alert-danger\"><h4>Fehler beim Laden der Daten</h4>";
            strMessage += "<p>Grund: " + err + "</p>";
            strMessage += "<p>Abhilfe: Den Brwoser Cache leeren, z.B. mit der Tastenkombination CTRL + F5 bzw. Strg + F5</p>";
            strMessage += "</div>";

            $('#calendar').html(strMessage);
            $('#loading').hide();
        }
    }

}


function onPrev() {
    myCalendar.prev();
}

function onNext() {
    myCalendar.next();
}

function toggleWeekend() {
    showWeekend = !showWeekend;
    showWeekendDays();
}


function toggleWeekDay() {
    showWeekDay = !showWeekDay;

    if (showWeekDay) {
        initCalendar();
        $("#btnWeekDay").addClass("active");
    } else {
        initCalendar();
        $("#btnWeekDay").removeClass("active");
    }
}

function showWeekendDays() {
    myCalendar.setOption("weekends", showWeekend);
    if (showWeekend) {
        $("#btnWeekend").addClass("active");
    } else {
        $("#btnWeekend").removeClass("active");
    }
}


function onShowEventInfo(courseId, dateId) {
    if (dateId != null) {
        $.ajax(
            {
                type: "POST",
                url: '@Url.Action("Date")',
                data: {
                    dateId: dateId,
                },
                success: function (data, success, xhr) {
                    $('#conflictArea').html(data);
                    $("#loading").hide();
                }
            });
    }
}


function onShowLecturer(memberId) {

    var seriesId = "c-" + memberId;
    var source = myCalendar.getEventSourceById(seriesId);
    if (source != null) {
        $("#" + seriesId).removeClass("active");
        source.remove();
        return;
    }
    var semId = $("#Semester_Id").val();

    $("#" + seriesId).addClass("active");

    if (showWeekDay) {
        myCalendar.addEventSource(
            {
                id: seriesId,
                url: '@Url.Action("MemberPlanWeekly", "Calendar")',
                method: 'POST',
                extraParams: {
                    semId: semId,
                    memberId: memberId,
                },
                success: function (data, success, xhr) {
                    $('#calendarList').html(data);
                    $('#calendarList').show();
                    $('#loading').hide();
                },
                failure: function (data, type, ex) {
                    alert("Fehler beim laden der Daten." + ex);
                    $('#loading').hide();
                }
            }
        );
    } else {
        myCalendar.addEventSource(
            {
                id: seriesId,
                url: '@Url.Action("MemberActivityPlan", "Calendar")',
                method: 'POST',
                extraParams: {
                    memberId: memberId
                },
                success: function (data, success, xhr) {
                    $('#loading').hide();
                },
                failure: function (data, type, ex) {
                    alert("Fehler beim laden der Daten." + ex);
                    $('#loading').hide();
                }
            }
        );
    }

}

function onShowAvailability(memberId) {

    var seriesId = "a-" + memberId;
    var source = myCalendar.getEventSourceById(seriesId);
    if (source != null) {
        $("#" + seriesId).removeClass("active");
        source.remove();
        return;
    }

    $("#" + seriesId).addClass("active");

    var orgId = $("#Organiser_Id").val();
    var semId = $("#Semester_Id").val();
    var segId = $("#Segment_Id").val();
    var currId = $("#Curriculum_Id").val();
    var labelId = $("#Label_Id").val();


    if (showWeekDay) {
        myCalendar.addEventSource(
            {
                id: seriesId,
                url: '@Url.Action("MemberAvailabilityWeekly", "Calendar")',
                method: 'POST',
                extraParams: {
                    semId: semId,
                    segId: segId,
                    memberId: memberId,
                },
                success: function (data, success, xhr) {
                    $('#calendarList').html(data);
                    $('#calendarList').show();
                    $('#loading').hide();
                },
                failure: function (data, type, ex) {
                    alert("Fehler beim laden der Daten." + ex);
                    $('#loading').hide();
                }
            }
        );
    } else {
        myCalendar.addEventSource(
            {
                id: seriesId,
                url: '@Url.Action("MemberAvailability", "Calendar")',
                method: 'POST',
                extraParams: {
                    memberId: memberId
                },
                success: function (data, success, xhr) {
                    $('#loading').hide();
                },
                failure: function (data, type, ex) {
                    alert("Fehler beim laden der Daten." + ex);
                    $('#loading').hide();
                }
            }
        );
    }
}
