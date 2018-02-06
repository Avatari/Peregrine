
$(function () {
    $.ajaxSetup({ cache: false });

    $('#startDateControl').datetimepicker();
    $('#endDateControl').datetimepicker();

//    $('#queueStepsPanel').hide();

    $('#startDateControl').on('keypress', function (e) {
        e.preventDefault(); // Don't allow direct editing
    });

    $('#endDateControl').on('keypress', function (e) {
        e.preventDefault(); // Don't allow direct editing
    });

    var vm = {};

    SetQueueTable(vm);

    // start timer
});

function SetQueueTable(vm) {
    $("#mainTable").DataTable().destroy();  // clear old data table

    var url = $("#pageUrl").val() + "/LoadData";

    $("#mainTable").DataTable({
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "ajax": {
            "url": url,
            "type": "POST",
            "datatype": "json",
            "data" : vm
        },
        "columnDefs":
        [
            {
                "targets": [0],
                "visible": false,
                "searchable": false
            }],
        "columns": [
            { "data": "queueId", "name": "QueueId", "autoWidth": true },
            {
                data: null, "orderable": false, render: function (data, type, row) {
                    return "<a href='#' class='btn-sm btn-success' onclick=ShowSteps('" + row.queueId + "') >" + row.scheduleName + "</a>";
                }
            },
            { "data": "taskName", "name": "TaskName", "autoWidth": true },
            { "data": "scheduledStart", "name": "ScheduledStart", "autoWidth": true },
            { "data": "completed", "name": "Completed", "autoWidth": true },
            { "data": "status", "name": "Status", "autoWidth": true },
            { "data": "suspended", "name": "Suspended", "autoWidth": true },
            { "data": "cancelled", "name": "Cancelled", "autoWidth": true },
            {
                data: null, "orderable": false, render: function (data, type, row) {
                    var actionButtonLinks = "";
                    if (row.status !== "Completed") {
                        actionButtonLinks = "<a href='#' class='btn-sm btn-primary' onclick=SuspendQueue('" + row.queueId + "') > Suspend</a>&nbsp;&nbsp;&nbsp;"
                            + "<a href='#' class='btn-sm btn-danger' onclick=CancelQueue('" + row.queueId + "') > Cancel</a>";
                    }

                    return actionButtonLinks;
                }
            }]
    });
}

function ShowSteps(id) {
    var vm = {
        id: id
    };

    var url = $("#pageUrl").val() + "/QueueSteps";

    $('#queueStepsPanel').load(url, vm, function (response, status, xhr) {
        if (status === "success") {
        }
    });
}

function FilterQueues() {
    // populate model
    var vm = {
        "ScheduleId": $("#ScheduleId").val(),
        "TaskId": $("#TaskId").val(),
        "ScheduleStartFrom": $("#ScheduleStartFrom").val(),
        "ScheduleStartTo": $("#ScheduleStartTo").val()
    };

    $('#queueStepsPanel').hide();

    SetQueueTable(vm);
}


function ShowSteps(id) {
    $('#queueStepsPanel').show();
    $("#stepsTable tbody").unbind("click");
    $("#stepsTable").DataTable().destroy();  // clear old data table

    var vm = { "id" : id };
    var url = $("#pageUrl").val() + "/QueueSteps";

    $("#stepsTable").DataTable({
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": false, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "paging": false,
        "bInfo": false,
        "ajax": {
            "url": url,
            "type": "POST",
            "datatype": "json",
            "data": vm
        },
        "columnDefs":
        [
            {
                "targets": [0],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [1],
                "searchable": false
            }
        ],
        "columns": [
            { "data": "queueStepId", "name": "QueueStepId", "autoWidth": true },
            {
                "className": 'details-control',
                "orderable": false,
                "data": null,
                "defaultContent": ''
            },
            { "data": "seq", "name": "Seq", "autoWidth": true },
            { "data": "stepName", "name": "StepName", "autoWidth": true },
            { "data": "status", "name": "Status", "autoWidth": true },
            { "data": "added", "name": "Added", "autoWidth": true },
            { "data": "executionStated", "name": "ExecutionStated", "autoWidth": true },
            { "data": "onCompletionStatus", "name": "OnCompletionStatus", "autoWidth": true }
        ]
    });

    $('#stepsTable tbody').on('click', 'td.details-control', function () {
        var tr = $(this).closest('tr');
        var table = $("#stepsTable").DataTable();
        var row = table.row(tr);

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            // Open this row
            row.child(format(row.data())).show();
            tr.addClass('shown');
        }
    });
}

function format(d) {
    // `d` is the original data object for the row
    return '<div class="row">'
        + '<div class="col-md-6"><div class="form-group"><label class="control-label">Execution Completed</label><br/>' + d.executionCompleted + '</div></div>'
        + '<div class="col-md-6"><div class="form-group"><label class="control-label">Last Execution Idled</label><br/>' + d.lastExecutionSuspended + '</div></div>'
        + '</div>'
        + '<div class="row">'
        + '<div class="col-md-6"><div class="form-group"><label class="control-label">Return Values</label><br/>' + d.returnValues + '</div></div>'
        + '<div class="col-md-6"><div class="form-group"><label class="control-label">Failure Information</label><br/>' + d.failureInfo + '</div></div>'
        + '</div>';
}

function SuspendQueue(id) {
    var vm = {
        id: id
    };

    var url = $("#pageUrl").val() + "/ToggleSuspend";

    $.ajax({
        type: "GET",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: vm,
        dataType: "json",
        success: function (data) {
            // call filter click to get updated data based on filter criterias
            $("#filterButton").click();
        }
    });
}


function CancelQueue(id) {
    var vm = {
        id: id
    };

    var url = $("#pageUrl").val() + "/ToggleCancel";

    $.ajax({
        type: "GET",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: vm,
        dataType: "json",
        success: function (data) {
            // call filter click to get updated data based on filter criterias
            $("#filterButton").click();
        }
    });
}
