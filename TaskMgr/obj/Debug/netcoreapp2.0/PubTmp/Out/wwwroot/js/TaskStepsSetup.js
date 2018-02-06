$(function () {
    $.ajaxSetup({ cache: false });

    var vm = {
        id: $("#TaskId").val()
    };

    var url = $("#pageUrl").val() + "/LoadData";

    $("#mainTable").DataTable({
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": false, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "paging": false,
        "bInfo": false,
        "ajax": {
            "url": url,
            "data": vm,
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs":
        [
            {
                "targets": [0],
                "visible": false
            },
            {
                "targets": [1],
                "visible": false
            }
        ],
        "columns": [
            { "data": "taskId", "name": "TaskStepId", "autoWidth": true },
            { "data": "taskId", "name": "TaskId", "autoWidth": true },
            { "data": "seq", "name": "Seq", "autoWidth": true },
            { "data": "step", "name": "Step", "autoWidth": true },
            { "data": "on", "name": "On", "autoWidth": true },
            { "data": "gotoSeq", "name": "GotoSeq", "autoWidth": true },
            {
                data: null, "orderable": false, render: function (data, type, row) {
                    return "<a href='#' class='btn-sm btn-success' onclick=AddEditData('" + row.taskStepId + "') >Edit</a>&nbsp;&nbsp;&nbsp;"
                        + "<a href='#' class='btn-sm btn-success' onclick=MoveUp('" + row.taskStepId + "') >Up</a>&nbsp;&nbsp;&nbsp;"
                        + "<a href='#' class='btn-sm btn-success' onclick=MoveDown('" + row.taskStepId + "') >Down</a>&nbsp;&nbsp;&nbsp;"
                        + "<a href='#' class='btn-sm btn-danger' onclick=DeleteData(" + row.taskStepId + ")>Delete</a>";
                }
            }
        ]
    });
});

function AddEditData(id, taskId) {

    var vm = {
        id: id,
        taskId : taskId
    };

    //var url = $(location).attr('href');
    //url = url.substr(0, url.indexOf("Index")) + "AddEdit";

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

function MoveUp(id) {
    var url = $("#pageUrl").val() + "/MoveUp/";
    window.location.href = url + id;
}

function MoveDown(id) {
    var url = $("#pageUrl").val() + "/MoveDown/";
    window.location.href = url + id;
}

function DeleteData(id) {
    var url = $("#pageUrl").val() + "/Delete/";
    window.location.href = url + id;
}

//function Save(e) {
//    var url = $(location).attr('href');
//    url = url.substr(0, url.indexOf("Index")) + "Save";

//    var vm = $("form").serialize();

//    // somehow POST was not sending viewmodel to controller so used GET
//    $.ajax({
//        type: "POST",
//        url: url,
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
