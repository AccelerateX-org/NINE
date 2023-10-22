function initDatePicker(name, format, language) {
    $("." + name).datepicker({
        orientation: "top",
        autoclose: true,
        format: format,
        weekStart: 1,
        calendarWeeks: true,
        language: language
    });
}

function initDatePickerTopLeft(name, format, language) {
    $("." + name).datepicker({
        orientation: "top",
        autoclose: true,
        format: format,
        weekStart: 1,
        calendarWeeks: true,
        language: language
    });
}
