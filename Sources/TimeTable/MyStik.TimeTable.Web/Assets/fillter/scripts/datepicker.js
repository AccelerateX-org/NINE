function initDatePicker(name, format, language) {
    $("." + name).datepicker({
        orientation: "left",
        autoclose: true,
        format: format,
        weekStart: 1,
        calendarWeeks: true,
        language: language
    });
}