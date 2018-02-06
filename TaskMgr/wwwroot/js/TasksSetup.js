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
        [{
            "targets": [0],
            "visible": false,
            "searchable": false
        }],
        "columns": [
            { "data": "taskId", "name": "TaskId", "autoWidth": true },
            { "data": "name", "name": "Name", "autoWidth": true },
            {
                "data": null, "autoWidth": true, render: function (data, type, row) {
                    return '<p ' + (row.validStr !== 'Yes' ? 'class="invalidColor"' : '') + '>' + row.validStr + '</p>';
                }
            },
            { "data": "emailsOnStepStartStr", "name": "EmailsOnStepStartStr", "autoWidth": true },
            { "data": "emailsOnStepCompleteStr", "name": "EmailsOnStepCompleteStr", "autoWidth": true },
            { "data": "created", "name": "Created", "autoWidth": true },
            { "data": "modified", "name": "Modified", "autoWidth": true },
            {
                data: null, "orderable": false, render: function (data, type, row) {
                    return "<a href='#' class='btn-sm btn-success' onclick=AddEditData('" + row.taskId + "') >Edit</a>&nbsp;&nbsp;<a href='#' class='btn-sm btn-primary' onclick=SetSteps('" + row.taskId + "')>Set Steps</a>&nbsp;&nbsp;<a href='#' class='btn-sm btn-danger' onclick=DeleteData('" + row.taskId + "')>Delete</a> ";
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
            $("form").each(function () { $.data($(this)[0], 'validator', false); });
            $.validator.unobtrusive.parse("form");

            $('#listPanel').hide();
            // call this when need to reload datatable
//            $('#stepsTable').DataTable().ajax.reload();
        }
    });
}

function SetSteps(id) {
    var url = $("#taskStepsPageUrl").val() + "/Index/";
    window.location.href = url + id;
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

//function Save(e) {

//    var vm = $("form").serialize();

//    // somehow POST was not sending viewmodel to controller so used GET
//    $.ajax({
//        type: "POST",
//        url: "/Tasks/Save",
//        contentType: "application/json; charset=utf-8",
//        data: vm,
//        dataType: "json",
//        success: function (data) {
//            $("#editPanel").html("");
//            $('#listPanel').show();
//            $('#mainTable').DataTable().ajax.reload();
//        }
//    });

//}

function Cancel(e) {

    $("#editPanel").html("");
    $('#listPanel').show();
}
