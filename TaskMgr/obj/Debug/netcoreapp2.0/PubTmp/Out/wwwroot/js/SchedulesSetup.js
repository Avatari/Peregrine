$(function () {
    $.ajaxSetup({ cache: false });

    var url = $("#pageUrl").val() + "/LoadData";

    $("#mainTable").DataTable({
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "ajax": {
            "url": url,
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs":
        [
            {
                "targets": [0],
                "visible": false,
                "searchable": false
            }],
        "columns": [
            { "data": "scheduleId", "name": "ScheduleId", "autoWidth": true },
            { "data": "name", "name": "Name", "autoWidth": true },
            { "data": "taskName", "name": "TaskName", "autoWidth": true },
            { "data": "freq", "name": "Freq", "autoWidth": true },
            { "data": "identifier", "name": "Identifier", "autoWidth": true },
            { "data": "start", "name": "Start", "autoWidth": true },
            { "data": "end", "name": "End", "autoWidth": true },
            { "data": "disabled", "name": "Disabled", "autoWidth": true },
            {
                data: null, "orderable": false, render: function (data, type, row) {

                    // allow edit only for non completed schedules
                    var editLink = "<a href='#' class='btn-sm btn-success' onclick=AddEditData('" + row.scheduleId + "') >Edit</a>&nbsp;&nbsp;&nbsp;";

                    if (row.Completed === "1") {
                        editLink = "";
                    }

                    return editLink + "<a href='#' class='btn-sm btn-danger' onclick=DeleteData('" + row.scheduleId + "')>Delete</a>";
                }
            }]
    });
});

function AddEditData(id) {

    var vm = {
        id: id
    };

    var url = $("#pageUrl").val() + "/AddEdit";
    
    $('#editPanel').load(url, vm, function (response, status, xhr) {
        if (status === "success") {
            $('#startDateControl').datetimepicker();
            $('#endDateControl').datetimepicker();
            EnableDisablePeriodInterval($('#Freq').val());
            $("form").each(function () { $.data($(this)[0], 'validator', false); });
            $.validator.unobtrusive.parse("form");

            $('#listPanel').hide();
            // call this when need to reload datatable
//            $('#stepsTable').DataTable().ajax.reload();
        }
    });
}


function DeleteData(id) {
    var vm = {
        id: id
    };

    var url = $("#pageUrl").val() + "/Delete";

    $.ajax({
        type: "GET",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: vm,
        dataType: "json",
        success: function (data) {
            if (data === "Success") {
                $("#editPanel").html("");
                $('#listPanel').show();
                $('#mainTable').DataTable().ajax.reload();
            }
            else {
                alert("Could not delete. " + data);
            }
        }
    });
}

function Cancel(e) {

    $("#editPanel").html("");
    $('#listPanel').show();
}


function FreqChanged(freq) {
    EnableDisablePeriodInterval(freq);
    $('#PeriodInterval').val('');
}

function EnableDisablePeriodInterval(freq) {
    var shouldDisabled = ($.inArray(freq, ['', 'DAY', 'FDOM', 'FBDOM', 'LDOM', 'LBDOM', 'ONCE']) >= 0);
    $("#PeriodInterval").prop('disabled', shouldDisabled);
}